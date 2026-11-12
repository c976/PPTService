using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks.HighPrecision.Erasing.CircleErasing;
using Cvte.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing;
using Cvte.Paint.Settings;
using Cvte.Windows;

namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	public class HighPrecisionEraserModeOperation : IModeDownMoveUpOperation, IModeOperation, IModePrepareOperation, IModeInputPrepareOperation
	{
		protected struct InputContext
		{
			private StylusPoint? _firstStylusPoint;

			public ModeInputEventArgs ModeInputEventArgs
			{
				get;
			}

			public InputInfo InputInfo
			{
				get;
			}

			public StylusPoint FirstStylusPoint
			{
				get
				{
					StylusPoint valueOrDefault = _firstStylusPoint.GetValueOrDefault();
					if (!_firstStylusPoint.HasValue)
					{
						valueOrDefault = ModeInputEventArgs.GetStylusPoints().FirstOrDefault();
						_firstStylusPoint = valueOrDefault;
						return valueOrDefault;
					}
					return valueOrDefault;
				}
			}

			public InputContext(ModeInputEventArgs modeInputEventArgs, InputInfo inputInfo)
			{
				this = default(InputContext);
				ModeInputEventArgs = modeInputEventArgs;
				InputInfo = inputInfo;
			}
		}

		protected enum InputInfo : short
		{
			Started,
			Delta,
			Completed,
			Canceled
		}

		public Slide CurrentSlide { get; set; }

		protected Dictionary<int, IHighPrecisionEraserHandler> EraserInkHandlerList { get; } = new Dictionary<int, IHighPrecisionEraserHandler>();

		private double Rotation { get; set; }

		protected EraserSettings EraserSettings { get; set; }

		protected HighPrecisionEraserBoardSettings HighPrecisionEraserBoardSettings { get; set; }

		protected Size MonitorSize { get; private set; }

		protected Size SlideMonitorRadio { get; private set; }

		protected ErasingSynchronizer ErasingSynchronizer { get; set; }

		public event EventHandler<ErasingCompletedEventArgs> ErasingCompleted;

		[CanBeNull]
		protected Board GetBoard()
		{
			return CurrentSlide?.Parent as Board;
		}

		protected virtual IHighPrecisionEraserHandler GetOrCreatorEraserInkHandler(ModeInputEventArgs e)
		{
			IHighPrecisionEraserHandler value;
			if (!EraserInkHandlerList.TryGetValue(e.Id, out value))
			{
				if (HighPrecisionEraserBoardSettings.HighPrecisionEraserInkHandlerCreator != null)
				{
					value = HighPrecisionEraserBoardSettings.HighPrecisionEraserInkHandlerCreator(e);
				}
				else
				{
					value = ((HighPrecisionEraserBoardSettings.HighPrecisionEraserInkHandlerType != HighPrecisionEraserInkHandlerType.CircleEraser) ? ((HighPrecisionEraserHandler)new CornerRadiusRectangleEraserHandler(e.Id, CurrentSlide, EraserSettings, GetBoard(), ErasingSynchronizer)) : ((HighPrecisionEraserHandler)new CircleEraserHandler(e.Id, CurrentSlide, EraserSettings, GetBoard(), ErasingSynchronizer)));
				}
				EraserInkHandlerList[e.Id] = value;
			}
			return value;
		}

		protected virtual EraserTouchEventArgs GetEraserTouchEventArgs(InputContext context)
		{
			Rotation = GetRotation(context);
			Size touchSize = GetTouchSize(context);
			double scaleX = 1.0;
			double scaleY = 1.0;
			Board board = CurrentSlide.Parent as Board;
			if (board != null)
			{
				scaleX = board.ContentScaleX;
				scaleY = board.ContentScaleY;
			}
			ModeInputEventArgs modeInputEventArgs = context.ModeInputEventArgs;
			return new EraserTouchEventArgs(modeInputEventArgs.Id, modeInputEventArgs.Position.X, modeInputEventArgs.Position.Y, touchSize.Height, touchSize.Width, Rotation + 90.0, scaleX, scaleY);
		}

		protected virtual Size GetTouchSize(InputContext context)
		{
			StylusPoint firstStylusPoint = context.FirstStylusPoint;
			double num = 1.0;
			double num2 = 1.0;
			Board board = CurrentSlide.Parent as Board;
			if (board != null)
			{
				num = board.ContentScaleX;
				num2 = board.ContentScaleY;
			}
			double num3 = EraserSettings.DefaultEraserSize.Width / num;
			if (firstStylusPoint.HasProperty(StylusPointProperties.Width))
			{
				num3 = firstStylusPoint.GetPropertyValue(StylusPointProperties.Width);
				StylusPointPropertyInfo propertyInfo = firstStylusPoint.Description.GetPropertyInfo(StylusPointProperties.Width);
				if (propertyInfo != null)
				{
					num3 = num3 * MonitorSize.Width / (double)propertyInfo.Maximum / SlideMonitorRadio.Width;
				}
			}
			double num4 = EraserSettings.DefaultEraserSize.Height / num2;
			if (firstStylusPoint.HasProperty(StylusPointProperties.Height))
			{
				num4 = firstStylusPoint.GetPropertyValue(StylusPointProperties.Height);
				StylusPointPropertyInfo propertyInfo2 = firstStylusPoint.Description.GetPropertyInfo(StylusPointProperties.Height);
				if (propertyInfo2 != null)
				{
					num4 = num4 * MonitorSize.Height / (double)propertyInfo2.Maximum / SlideMonitorRadio.Height;
				}
			}
			double num5 = 30.0 / num;
			if (num3 < num5)
			{
				num3 = num5;
			}
			double num6 = 30.0 / num2;
			if (num4 < num6)
			{
				num4 = num6;
			}
			return new Size(num3, num4);
		}

		protected virtual double GetRotation(InputContext context)
		{
			double result = 0.0;
			StylusPoint firstStylusPoint = context.FirstStylusPoint;
			if (firstStylusPoint.HasProperty(StylusPointProperties.AzimuthOrientation))
			{
				int propertyValue = firstStylusPoint.GetPropertyValue(StylusPointProperties.AzimuthOrientation);
				StylusPointPropertyInfo propertyInfo = firstStylusPoint.Description.GetPropertyInfo(StylusPointProperties.AzimuthOrientation);
				if (propertyInfo != null)
				{
					result = ((propertyInfo.Unit != StylusPointPropertyUnit.Radians) ? ((double)((float)propertyValue / propertyInfo.Resolution)) : ((double)((float)(-propertyValue) / propertyInfo.Resolution * 180f) / Math.PI));
				}
			}
			else
			{
				result = Rotation;
			}
			return result;
		}

		protected virtual void AddUndoRedo(IEnumerable<IOperation> operations)
		{
			if (operations.Any())
			{
				Board.GetUndoRedoProvider(CurrentSlide)?.Insert(new ElementCombinationOperation(CurrentSlide, operations.ToArray()));
			}
		}

		public void OnSwitchOn()
		{
			Board board = GetBoard();
			EraserSettings = board.GetSettings<EraserSettings>();
			HighPrecisionEraserBoardSettings = board.GetSettings<HighPrecisionEraserBoardSettings>();
			ErasingSynchronizer = new ErasingSynchronizer(CurrentSlide);
		}

		public void OnSwitchOff()
		{
			CurrentSlide.MouseWheel -= CurrentSlide_MouseWheel;
		}

		public void OnInputStarting(ModeInputEventArgs e)
		{
			MonitorSize = CurrentSlide.GetMonitor().PixelBounds.Size;
			SlideMonitorRadio = CurrentSlide.GetScalingRatioToDevice();
			ErasingSynchronizer.Reset();
		}

		public void OnInputCompleted(ModeInputEventArgs e)
		{
			List<KeyValuePair<int, IHighPrecisionEraserHandler>> eraserInkHandlerList = EraserInkHandlerList.ToList();
			CurrentSlide.Dispatcher.InvokeAsync(() =>
			{
				foreach (KeyValuePair<int, IHighPrecisionEraserHandler> item in eraserInkHandlerList)
				{
					item.Value.CompleteEraser();
				}
			});
			EraserInkHandlerList.Clear();
			IList<IOperation> first = ErasingSynchronizer.FinishEraseElements();
			IList<IOperation> second = ErasingSynchronizer.FinishEraseInks();
			AddUndoRedo(first.Concat(second));
			ErasingCompleted?.Invoke(this, new ErasingCompletedEventArgs(true));
		}

		public void OnDeviceDown(ModeInputEventArgs e)
		{
			GetOrCreatorEraserInkHandler(e).StartEraser(GetEraserTouchEventArgs(new InputContext(e, InputInfo.Started)));
		}

		public void OnDeviceMove(ModeInputEventArgs e)
		{
			GetOrCreatorEraserInkHandler(e).ContinueEraser(GetEraserTouchEventArgs(new InputContext(e, InputInfo.Delta)));
		}

		public void OnDeviceUp(ModeInputEventArgs e)
		{
			GetOrCreatorEraserInkHandler(e).FinishEraser(GetEraserTouchEventArgs(new InputContext(e, InputInfo.Completed)));
		}

		private void CurrentSlide_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Rotation += (double)e.Delta / 20.0;
		}
	}
}
