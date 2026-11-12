using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;
using Cvte.Windows.Input;
using Cvte.Windows.Mathematics;
using Cvte.Windows.Threading;

namespace Cvte.Paint.Inks
{
	public class EraserMode : Mode, IEraserMode
	{
		private const double CompleteErasingSessionDelayedMilliseconds = 70.0;

		private int? _trackingDeviceId;

		private DateTime _erasingStartedTime;

		private bool _isErasing;

		private readonly DispatcherTask _completeErasingSessionTask;

		private readonly List<Point> _queueingPoints = new List<Point>();

		private IList<EraserHandler> _handlers;

		private static readonly IList<EraserHandler> DumbHandlers = new List<EraserHandler>();

		private FrameworkElement _eraserVisual;

		private Size _currentEraserSize;

		private LegacyDevice _legacyDevice;

		private double _inkScale = 1.0;

		[CanBeNull]
		private EraserSettings EraserSettings { get; set; }

		private EraserGestureSettings EraserGestureSettings { get; set; }

		private TraceSettings TraceSettings { get; set; }

		private InkBoardSettings InkBoardSettings { get; set; }

		[Obsolete("这个属性不再使用")]
		public double SizeScale { get; set; } = 1.0;

		public event EventHandler<ErasingCompletedEventArgs> ErasingCompleted;

		public EraserMode()
			: base(new ModeSettings(false, false))
		{
			_completeErasingSessionTask = new DispatcherTask(70.0, CompleteErasingSession);
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			if (!_trackingDeviceId.HasValue && _legacyDevice != null && _isErasing)
			{
				double length = (e.Position - _legacyDevice.DeathPosition).Length;
				length *= Board.GetContentScale(CurrentSlide);
				if (GetDeviceCount() == 1 && length < 400.0 && _legacyDevice.DeathTime.IsWithin(100.0))
				{
					_erasingStartedTime = default(DateTime);
					_trackingDeviceId = e.Id;
					_completeErasingSessionTask.Stop();
				}
			}
			_legacyDevice = null;
			OnDeviceMove(e);
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			if (!_trackingDeviceId.HasValue)
			{
				_inkScale = ((InkBoardSettings.InkScale > 0.0) ? InkBoardSettings.InkScale : 1.0);
				if (!_isErasing)
				{
					StartErasing(e);
				}
				else
				{
					RestartErasing(e);
				}
			}
			else if (_trackingDeviceId == e.Id)
			{
				ContinueErasing(e);
			}
		}

		protected override void OnDeviceUp(ModeInputEventArgs e)
		{
			if (_trackingDeviceId == e.Id)
			{
				ContinueErasing(e);
				_legacyDevice = new LegacyDevice
				{
					Id = e.Id,
					DeathPosition = e.Position,
					DeathTime = DateTime.Now
				};
				_trackingDeviceId = null;
			}
			if (!GetMainDeviceId().HasValue)
			{
				_completeErasingSessionTask.Restart();
			}
		}

		protected override void OnSwitchOn()
		{
			InitSettings();
			_trackingDeviceId = null;
			_erasingStartedTime = default(DateTime);
			_isErasing = false;
			_queueingPoints.Clear();
			_handlers = DumbHandlers;
			_legacyDevice = null;
		}

		protected override void OnSwitchOff()
		{
			base.OnSwitchOff();
			if (_isErasing)
			{
				_isErasing = false;
				CancelErasingSession();
				_completeErasingSessionTask.Stop();
			}
			RemoveEraserVisual();
		}

		protected virtual EraserHandler[] CreateEraserHandlers()
		{
			return new EraserHandler[2]
			{
				new InkEraserHandler(CurrentSlide),
				new MaskEraserHandler(CurrentSlide)
			};
		}

		private void InitSettings()
		{
			Board board = GetBoard();
			if (board != null)
			{
				EraserSettings = board.GetSettings<EraserSettings>();
				EraserGestureSettings = board.GetSettings<EraserGestureSettings>();
				TraceSettings = board.GetSettings<TraceSettings>();
				InkBoardSettings = board.GetSettings<InkBoardSettings>();
			}
		}

		private void StartErasing(DeviceInputEventArgs args)
		{
			CurrentSlide.RaiseEvent(new ErasingStartingRoutedEventArgs(ErasingEvents.ErasingStartingEvent));
			_completeErasingSessionTask.Stop();
			_isErasing = true;
			_erasingStartedTime = DateTime.Now;
			_handlers = CreateEraserHandlers();
			_trackingDeviceId = GetMainDeviceId() ?? args.Id;
			_currentEraserSize = EraserSettings?.GetEraserSize() ?? EraserSettings.DefaultEraserSize;
			Delegate((EraserHandler h) =>
			{
				h.EraserSize = _currentEraserSize;
			});
			_queueingPoints.Clear();
			UpdateEraserSizeByTouchPoint(args);
			Delegate((EraserHandler h) =>
			{
				h.OnErasingStarted();
			});
			_queueingPoints.Add(args.Position);
			Delegate((EraserHandler h) =>
			{
				h.OnErasingSectionStarted(args.Position);
			});
			CurrentSlide.RaiseEvent(new ErasingStartedRoutedEventArgs(ErasingEvents.ErasingStartedEvent));
			Delegate((EraserHandler h) =>
			{
				h.EraseCore(_queueingPoints);
			});
		}

		private void RestartErasing(DeviceInputEventArgs args)
		{
			_completeErasingSessionTask.Stop();
			if (_queueingPoints.Any())
			{
				Delegate((EraserHandler h) =>
				{
					h.EraseCore(_queueingPoints);
				});
				_queueingPoints.Clear();
			}
			Delegate((EraserHandler h) =>
			{
				h.OnErasingSectionCompleted();
			});
			CurrentSlide.RaiseEvent(new ErasingStartingRoutedEventArgs(ErasingEvents.ErasingStartingEvent));
			_trackingDeviceId = GetMainDeviceId() ?? args.Id;
			_queueingPoints.Clear();
			_queueingPoints.Add(args.Position);
			MoveEraserVisual(args.Position);
			Delegate((EraserHandler h) =>
			{
				h.OnErasingSectionStarted(args.Position);
			});
			CurrentSlide.RaiseEvent(new ErasingStartedRoutedEventArgs(ErasingEvents.ErasingStartedEvent));
			UpdateEraserSizeByTouchPoint(args);
			Delegate((EraserHandler h) =>
			{
				h.EraseCore(_queueingPoints);
			});
		}

		private void ContinueErasing(ModeInputEventArgs e)
		{
			Point position = e.Position;
			MoveEraserVisual(position);
			if ((position - _queueingPoints.First()).Length > 3.0 * _inkScale)
			{
				_queueingPoints.Add(position);
				Point item = _queueingPoints.Last();
				Delegate((EraserHandler h) =>
				{
					h.EraseCore(_queueingPoints);
				});
				_queueingPoints.Clear();
				_queueingPoints.Add(item);
			}
			UpdateEraserSizeByTouchPoint(e);
		}

		private void SetEraserSize(Size size)
		{
			if (Nearly.Equals(_currentEraserSize, size))
			{
				return;
			}
			_currentEraserSize = size;
			Delegate((EraserHandler h) =>
			{
				h.EraserSize = size;
			});
			if (_queueingPoints.Any())
			{
				Point last = _queueingPoints.Last();
				Delegate((EraserHandler h) =>
				{
					h.EraseCore(_queueingPoints);
				});
				_queueingPoints.Clear();
				Delegate((EraserHandler h) =>
				{
					h.OnErasingSectionCompleted();
				});
				_queueingPoints.Add(last);
				Delegate((EraserHandler h) =>
				{
					h.EraserSize = size;
					h.OnErasingSectionStarted(last);
				});
			}
			else
			{
				Delegate((EraserHandler h) =>
				{
					h.EraserSize = size;
				});
			}
		}

		private void UpdateEraserSizeByTouchPoint(DeviceInputEventArgs args)
		{
			EraserGestureSettings eraserGestureSettings = EraserGestureSettings;
			Slide currentSlide = CurrentSlide;
			if (eraserGestureSettings == null || !eraserGestureSettings.AdjustEraserSizeByTouchArea)
			{
				return;
			}
			double? eraserSizeAdjustingDurationInMilliseconds = eraserGestureSettings.EraserSizeAdjustingDurationInMilliseconds;
			if (!eraserSizeAdjustingDurationInMilliseconds.HasValue || _erasingStartedTime.IsWithin(eraserSizeAdjustingDurationInMilliseconds.Value))
			{
				Size? size = eraserGestureSettings.GetEraserSizeAlgorithm(args, this);
				if (size.HasValue)
				{
					Size value = size.Value;
					SetEraserSize(value);
				}
			}
		}

		private void CompleteErasingSession()
		{
			Slide currentSlide = CurrentSlide;
			Delegate((EraserHandler h) =>
			{
				h.OnErasingSectionCompleted();
			});
			IOperation[] array = (from x in Delegate((EraserHandler h) => h.OnErasingCompleted())
				where x != null
				select x).ToArray();
			if (array.Any() && currentSlide != null)
			{
				Board.GetUndoRedoProvider(currentSlide)?.Insert(new ElementCombinationOperation(currentSlide, array));
			}
			RemoveEraserVisual();
			_isErasing = false;
			InkTracer.LogTracer.Debug("Erasing complete.", "[EraserMode]");
			ErasingCompleted?.Invoke(this, new ErasingCompletedEventArgs(false));
			currentSlide?.RaiseEvent(new ErasingCompletedRoutedEventArgs(ErasingEvents.ErasingCompletedEvent));
		}

		private void CancelErasingSession()
		{
			Delegate((EraserHandler h) =>
			{
				h.OnErasingCanceled();
			});
			RemoveEraserVisual();
			Slide currentSlide = CurrentSlide;
			ErasingCompleted?.Invoke(this, new ErasingCompletedEventArgs(true));
			currentSlide.RaiseEvent(new ErasingCanceledRoutedEventArgs(ErasingEvents.ErasingCanceledEvent));
			_isErasing = false;
		}

		private void MoveEraserVisual(Point point)
		{
			Size currentEraserSize = _currentEraserSize;
			if (_eraserVisual == null)
			{
				_eraserVisual = GetBoard()?.GetSettings<EraserSettings>()?.CreateEraserVisual(currentEraserSize) ?? new Border();
				_eraserVisual.Opacity = 0.0;
				_eraserVisual.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1.0, new Duration(TimeSpan.FromMilliseconds(200.0))));
				CurrentSlide.AddUtilities(_eraserVisual);
			}
			if (!object.Equals(_eraserVisual.Width, currentEraserSize.Width) || !object.Equals(_eraserVisual.Height, currentEraserSize.Height))
			{
				_eraserVisual.Width = currentEraserSize.Width;
				_eraserVisual.Height = currentEraserSize.Height;
			}
			TranslateTransform translateTransform = (TranslateTransform)_eraserVisual.RenderTransform;
			double num = (_eraserVisual.ActualWidth.Equals(0.0) ? _eraserVisual.Width : _eraserVisual.ActualWidth);
			double num2 = (_eraserVisual.ActualHeight.Equals(0.0) ? _eraserVisual.Height : _eraserVisual.ActualHeight);
			translateTransform.X = point.X - num / 2.0;
			translateTransform.Y = point.Y - num2 / 2.0;
		}

		private void RemoveEraserVisual()
		{
			if (_eraserVisual != null)
			{
				CurrentSlide?.RemoveUtilities(_eraserVisual);
			}
			_eraserVisual = null;
		}

		private IEnumerable<T> Delegate<T>(Func<EraserHandler, T> func)
		{
			foreach (EraserHandler handler in _handlers)
			{
				T val;
				try
				{
					val = func(handler);
				}
				catch (Exception exception)
				{
					InkTracer.LogTracer.Error(exception);
					continue;
				}
				yield return val;
			}
		}

		private void Delegate(Action<EraserHandler> func)
		{
			foreach (EraserHandler handler in _handlers)
			{
				try
				{
					handler.CanActivateEventHelper = true;
					func(handler);
				}
				catch (Exception exception)
				{
					InkTracer.LogTracer.Error(exception);
				}
			}
		}
	}
}
