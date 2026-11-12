using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Threading;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;
using Cvte.Windows.Input;
using Cvte.Windows.Threading;

namespace Cvte.Paint.Inks
{
	public class WritingMode : WritingPenMode, IPenMode
	{
		private const int CacheDynamicRenderer = 20;

		private InkPresenter _inkPresenter;

		private readonly List<InkRecorder> _shortTimeInks;

		private Board _currentBoard;

		private double _boardScale = 1.0;

		private List<WritingDynamicRenderer> _inkDynamicRenderers;

		private DependencyPropertyDescriptor _boardContentScaleProperty;

		private DependencyPropertyDescriptor _boardContentOffsetProperty;

		public double BezierInterpolationThreshold = 3.0;

		protected InkBoardSettings CurrentInkBoardSettings { get; private set; }

		protected Board CurrentBoard => _currentBoard ?? (_currentBoard = GetBoard());

		public double MinDrawingTimeInMilliseconds { get; set; } = 100.0;

		public Canvas InkPanel { get; set; } = new Canvas();

		internal bool ShouldIgnoreOnce { get; set; }

		protected event EventHandler<ModeInputEventArgs> DeviceDown;

		protected event EventHandler<ModeInputEventArgs> DeviceMove;

		protected event EventHandler<ModeInputEventArgs> DeviceUp;

		public WritingMode()
		{
			_shortTimeInks = new List<InkRecorder>();
			InitialInkPresenter();
		}

		protected override void OnSwitchOn()
		{
			if (!CurrentBoard.ContainsBoardUtilitiesElement(InkPanel))
			{
				CurrentBoard.AddBoardUtilitiesElement(InkPanel);
			}
			base.OnSwitchOn();
			UpdateSettings();
			AttachInkPresenter();
			_boardContentScaleProperty = DependencyPropertyDescriptor.FromProperty(Board.ContentScaleProperty, typeof(Board));
			_boardContentScaleProperty.AddValueChanged(CurrentBoard, UpdateBoardConentLocation);
			_boardContentOffsetProperty = DependencyPropertyDescriptor.FromProperty(Board.ContentOffsetProperty, typeof(Board));
			_boardContentOffsetProperty.AddValueChanged(CurrentBoard, UpdateBoardConentLocation);
			CurrentBoard.IsEnabledChanged += CurrentBoard_IsEnabledChanged;
		}

		protected override void OnSwitchOff()
		{
			base.OnSwitchOff();
			CurrentInkBoardSettings.PropertyChanged -= State_PropertyChanged;
			_inkDynamicRenderers.ForEach((WritingDynamicRenderer dy) =>
			{
				dy.Enabled = false;
			});
			foreach (WritingDynamicRenderer item in _inkDynamicRenderers.Where((WritingDynamicRenderer x) => x.WritingContext != null))
			{
				OnWritingCanceled(item.WritingContext.DeviceId, null);
			}
			_inkDynamicRenderers.ForEach((WritingDynamicRenderer dy) =>
			{
				dy.Reset();
			});
			DetachInkPresenter();
			RemoveGestureInk();
			_boardContentOffsetProperty.RemoveValueChanged(CurrentBoard, UpdateBoardConentLocation);
			_boardContentScaleProperty.RemoveValueChanged(CurrentBoard, UpdateBoardConentLocation);
			CurrentBoard.IsEnabledChanged -= CurrentBoard_IsEnabledChanged;
		}

		protected sealed override void OnDeviceDown(ModeInputEventArgs e)
		{
			WritingContext writingContext = GetWritingContext(e.Id, true);
			if (writingContext != null)
			{
				writingContext.IsProcessedByUI = true;
				writingContext.StylusDevice = (e.OriginEventArgs as StylusEventArgs)?.StylusDevice;
			}
			DeviceDown?.Invoke(this, e);
		}

		protected sealed override void OnDeviceMove(ModeInputEventArgs e)
		{
			WritingContext writingContext = GetWritingContext(e.Id, true);
			if (writingContext != null)
			{
				writingContext.IsProcessedByUI = true;
				writingContext.StylusDevice = (e.OriginEventArgs as StylusEventArgs)?.StylusDevice;
			}
			DeviceMove?.Invoke(this, e);
		}

		protected sealed override void OnDeviceUp(ModeInputEventArgs e)
		{
			if (e.VirtualType == VirtualDeviceType.LostCapture)
			{
				ShouldIgnoreOnce = true;
			}
			WritingContext writingContext = GetWritingContext(e.Id, true);
			if (writingContext == null)
			{
				DeviceUp?.Invoke(this, e);
				return;
			}
			writingContext.WritingDynamicRenderer.Reset();
			if (writingContext.IsCanceled)
			{
				DeviceUp?.Invoke(this, e);
				return;
			}
			writingContext.IsProcessedByUI = true;
			writingContext.StylusDevice = (e.OriginEventArgs as StylusEventArgs)?.StylusDevice;
			Element ink = CreateStaticInk(new StylusPointCollection(writingContext.CollectedStylusPoints), writingContext);
			CurrentSlide.Add(ink);
			DateTime writingStartTime = writingContext.WritingStartTime;
			double totalMilliseconds = (DateTime.Now - writingStartTime).TotalMilliseconds;
			if (e.Type == DeviceType.Touch && totalMilliseconds < MinDrawingTimeInMilliseconds)
			{
				InkRecorder inkRecorder = new InkRecorder(ink, writingStartTime, e.Id);
				_shortTimeInks.Add(inkRecorder);
				DispatcherTask.Start(MinDrawingTimeInMilliseconds - totalMilliseconds, () =>
				{
					if (CurrentSlide != null && _shortTimeInks.Contains(inkRecorder))
					{
						_shortTimeInks.Remove(inkRecorder);
						OnWritingCompleted(e.Id, ink);
					}
				});
			}
			else
			{
				Dispatcher.CurrentDispatcher.InvokeAsync(() =>
				{
					OnWritingCompleted(e.Id, ink);
				});
			}
			DeviceUp?.Invoke(this, e);
		}

		protected virtual Element CreateStaticInk(StylusPointCollection points, WritingContext context)
		{
			Ink ink = new Ink(new HardPenStroke(points, CurrentInkBoardSettings)
			{
				DrawingAttributes = 
				{
					FitToCurve = context.DrawingAttributes.FitToCurve
				}
			});
			ink.SetId(Guid.NewGuid().ToString("N"));
			return ink;
		}

		protected internal virtual void OnWritingStarting(RawStylusInput rawStylusInput, WritingContext context)
		{
		}

		protected internal virtual void OnWritingStarted(RawStylusInput rawStylusInput, WritingContext context)
		{
		}

		protected internal virtual void OnWritingContinued(RawStylusInput rawStylusInput, WritingContext context)
		{
		}

		protected internal virtual void OnWritingCompleting(RawStylusInput rawStylusInput, WritingContext context)
		{
		}

		protected internal virtual void OnWritingCanceled(int deviceId, Element ink)
		{
		}

		protected internal virtual void OnWritingCompleted(int deviceId, Element ink)
		{
			InsertOperation(ink);
		}

		protected internal virtual StylusPointCollection FilterStylusPoints(StylusPointCollection points, WritingContext context)
		{
			double pointMinDistance = 0.5 / _boardScale;
			double bezierInterpolationThreshold = BezierInterpolationThreshold / _boardScale;
			return BezierInterpolation.Interpolate(points, context.CollectedStylusPoints, bezierInterpolationThreshold, pointMinDistance);
		}

		protected internal virtual void OnDynamicRenderering(WritingContext writingContext, DrawingContext drawingContext, StylusPointCollection stylusPoints, Geometry geometry, Brush fillBrush)
		{
			drawingContext.DrawGeometry(fillBrush, null, geometry);
		}

		private void InitialInkPresenter()
		{
			_inkPresenter = new InkPresenter();
			_inkDynamicRenderers = new List<WritingDynamicRenderer>();
			for (int i = 0; i < 20; i++)
			{
				AddDynamicRenderer();
			}
		}

		private void UpdateSettings()
		{
			CurrentInkBoardSettings = CurrentSlide.GetSettings<InkBoardSettings>();
			UpdateDynamicRendererDrawingAttributes();
			CurrentInkBoardSettings.PropertyChanged -= State_PropertyChanged;
			CurrentInkBoardSettings.PropertyChanged += State_PropertyChanged;
		}

		private void RemoveGestureInk()
		{
			foreach (InkRecorder shortTimeInk in _shortTimeInks)
			{
				if (shortTimeInk.CreationTime.IsWithin(MinDrawingTimeInMilliseconds))
				{
					CurrentSlide.Remove(shortTimeInk.Ink);
					OnWritingCanceled(shortTimeInk.DeviceId, shortTimeInk.Ink);
				}
			}
			_shortTimeInks.Clear();
		}

		private void InsertOperation(Element element)
		{
			Board.GetUndoRedoProvider(CurrentSlide)?.Insert(new AddElementsOperation(CurrentSlide, element));
		}

		private void State_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateDynamicRendererDrawingAttributes();
		}

		private void CurrentBoard_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			foreach (WritingDynamicRenderer inkDynamicRenderer in _inkDynamicRenderers)
			{
				for (int i = -1; i < 100; i++)
				{
					inkDynamicRenderer.SetIsWaitingForDownToActivate(i, true);
				}
			}
		}

		private void AttachInkPresenter()
		{
			StylusPlugInCollection stylusPlugins = CurrentBoard.GetStylusPlugins();
			foreach (WritingDynamicRenderer inkDynamicRenderer in _inkDynamicRenderers)
			{
				inkDynamicRenderer.Enabled = true;
				if (!stylusPlugins.Contains(inkDynamicRenderer))
				{
					stylusPlugins.Add(inkDynamicRenderer);
				}
			}
			if (!InkPanel.Children.Contains(_inkPresenter))
			{
				InkPanel.Children.Add(_inkPresenter);
			}
		}

		private void DetachInkPresenter()
		{
			StylusPlugInCollection stylusPlugins = CurrentBoard.GetStylusPlugins();
			foreach (WritingDynamicRenderer inkDynamicRenderer in _inkDynamicRenderers)
			{
				if (stylusPlugins.Contains(inkDynamicRenderer))
				{
					inkDynamicRenderer.Enabled = false;
				}
			}
			if (InkPanel.Children.Contains(_inkPresenter))
			{
				InkPanel.Children.Remove(_inkPresenter);
			}
		}

		private void AddDynamicRenderer()
		{
			WritingDynamicRenderer writingDynamicRenderer = CreateDynamicRenderer();
			_inkPresenter.AttachVisuals(writingDynamicRenderer.RootVisual, writingDynamicRenderer.DrawingAttributes);
			_inkDynamicRenderers.Add(writingDynamicRenderer);
		}

		private WritingDynamicRenderer CreateDynamicRenderer()
		{
			return new WritingDynamicRenderer(this);
		}

		private void UpdateDynamicRendererDrawingAttributes()
		{
			double contentScale = Board.GetContentScale(CurrentSlide);
			Vector contentOffset = Board.GetContentOffset(CurrentSlide);
			Matrix matrix = default(Matrix);
			matrix.Translate(0.0 - contentOffset.X, 0.0 - contentOffset.Y);
			matrix.Scale(1.0 / contentScale, 1.0 / contentScale);
			_boardScale = contentScale;
			double inkThickness = CurrentInkBoardSettings.InkThickness * CurrentInkBoardSettings.InkScale * contentScale;
			Color color = CurrentInkBoardSettings.InkColor;
			_inkDynamicRenderers.ForEach((WritingDynamicRenderer dy) =>
			{
				dy.DrawingAttributes.Width = inkThickness;
				dy.DrawingAttributes.Height = inkThickness;
				dy.DrawingAttributes.Color = color;
				dy.TransformFromBoard = matrix;
			});
		}

		private void UpdateBoardConentLocation(object sender, EventArgs eventArgs)
		{
			UpdateDynamicRendererDrawingAttributes();
		}

		internal WritingContext GetWritingContext(int deviceId, bool onUIThread = false)
		{
			if (deviceId == -1)
			{
				deviceId = 0;
			}
			lock (_inkDynamicRenderers)
			{
				if (onUIThread)
				{
					return (from x in _inkDynamicRenderers.Where((WritingDynamicRenderer x) =>
						{
							WritingContext writingContext = x.WritingContext;
							return writingContext != null && writingContext.DeviceId == deviceId;
						})
						orderby x.WritingContext.FirstTimestamp
						select x).FirstOrDefault()?.WritingContext;
				}
				return _inkDynamicRenderers.FirstOrDefault((WritingDynamicRenderer x) =>
				{
					WritingContext writingContext = x.WritingContext;
					if (writingContext != null && writingContext.DeviceId == deviceId)
					{
						WritingContext writingContext2 = x.WritingContext;
						if (writingContext2 == null)
						{
							return false;
						}
						return !writingContext2.IsDynamicUpTriggered;
					}
					return false;
				})?.WritingContext;
			}
		}
	}
}
