using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Settings;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework
{
	public abstract class InputProcessor
	{
		private bool _isActive;

		private ModeInputEventArgs _currentInputEventArgs;

		private int _firstMouseDownId;

		private readonly Dictionary<int, ModeInputEventArgs> _devices = new Dictionary<int, ModeInputEventArgs>();

		private bool _isCanceled;

		private readonly DownMoveUpToDragTapEventConverter _eventConverter;

		private Lazy<DeviceInputDoubleDifferentialValue> _dragTolerance;

		private Lazy<DeviceInputDoubleDifferentialValue> _hitTolerance;

		private DateTime _inputStartedTime;

		private bool _isMultiTouchEnabled = true;

		private List<IEventConverter> EventConverterList { get; set; }

		protected bool IsInputLocked => false;

		public Slide CurrentSlide { get; internal set; }

		protected bool IsOperating => _devices.Count > 0;

		protected TimeSpan OperatingDuration
		{
			get
			{
				if (IsOperating)
				{
					return DateTime.Now - _inputStartedTime;
				}
				return TimeSpan.Zero;
			}
		}

		public bool IsMultiTouchEnabled
		{
			get
			{
				return _isMultiTouchEnabled;
			}
			protected set
			{
				if (_isActive)
				{
					throw new InvalidOperationException("不能在InputProcesser被SwitchOn时设置此属性。");
				}
				_isMultiTouchEnabled = value;
			}
		}

		protected static bool WithShift => Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

		protected static bool WithCtrl => Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

		protected static bool WithAlt => Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);

		protected InputProcessor()
		{
			_eventConverter = new DownMoveUpToDragTapEventConverter(OnNormalTap, OnNormalDragStarted, OnNormalDragDelta, OnNormalDragCompleted);
		}

		public void AddEventConverter(IEventConverter eventConverter)
		{
			if (eventConverter == null)
			{
				throw new ArgumentNullException("eventConverter");
			}
			EventConverterList = EventConverterList ?? new List<IEventConverter>();
			if (EventConverterList.Contains(eventConverter))
			{
				throw new ArgumentException("已经添加了事件转换器，请不要重复添加");
			}
			EventConverterList.Add(eventConverter);
		}

		public void RaiseDeviceDown(ModeInputEventArgs e)
		{
			if (IsInputLocked)
			{
				return;
			}
			_currentInputEventArgs = e;
			if (!_devices.Any())
			{
				OnInputStarting(ModeInputEventArgs.FromDeviceInput(e));
				_inputStartedTime = DateTime.Now;
				_firstMouseDownId = e.Id;
				EventConverterList?.ForEach((IEventConverter converter) =>
				{
					converter.InputStarting(e);
				});
			}
			_devices[e.Id] = e;
			if (!IsMultiTouchEnabled && _firstMouseDownId != e.Id)
			{
				return;
			}
			PaintAggregateExceptionBuilder.DoActionsWithException("Mode.OnDeviceDown 出错。", () =>
			{
				OnDeviceDown(e);
			}, () =>
			{
				if (_isActive)
				{
					if (e.Type == DeviceType.Mouse || (e.Type == DeviceType.Stylus && e.Button != DeviceButton.Eraser))
					{
						if (_firstMouseDownId == e.Id)
						{
							double num = ((e.Button == DeviceButton.LeftButton) ? _dragTolerance.Value.Mouse : _dragTolerance.Value.Stylus);
							num /= Board.GetContentScale(CurrentSlide);
							_eventConverter.Down(e.Position, num);
						}
					}
					else if (e.Type == DeviceType.Touch && _firstMouseDownId == e.Id)
					{
						double tolerance = _dragTolerance.Value.Touch / Board.GetContentScale(CurrentSlide);
						_eventConverter.Down(e.Position, tolerance);
					}
				}
			}, () =>
			{
				EventConverterList?.ForEach((IEventConverter converter) =>
				{
					converter.DeviceDown(e);
				});
			});
		}

		public void RaiseDeviceMove(ModeInputEventArgs e)
		{
			if (IsInputLocked)
			{
				return;
			}
			_currentInputEventArgs = e;
			if (!IsMultiTouchEnabled && _firstMouseDownId != e.Id)
			{
				return;
			}
			PaintAggregateExceptionBuilder.DoActionsWithException("Mode.OnDeviceMove 或 Mode.OnElementDragStarted 或 Mode.OnElementDragDelta 发生异常，异常的详细信息如下：", () =>
			{
				OnDeviceMove(e);
			}, () =>
			{
				if (_isActive)
				{
					if (e.Type == DeviceType.Mouse || (e.Type == DeviceType.Stylus && e.Button != DeviceButton.Eraser))
					{
						if (_firstMouseDownId == e.Id)
						{
							_eventConverter.Move(e.Position);
						}
					}
					else if (e.Type == DeviceType.Touch && _firstMouseDownId == e.Id)
					{
						_eventConverter.Move(e.Position);
					}
				}
			}, () =>
			{
				EventConverterList?.ForEach((IEventConverter converter) =>
				{
					converter.DeviceMove(e);
				});
			});
		}

		public void RaiseDeviceHover(ModeInputEventArgs e)
		{
			OnDeviceHover(e);
		}

		public void RaiseDeviceUp(ModeInputEventArgs e)
		{
			if (IsInputLocked)
			{
				return;
			}
			if (e.VirtualType == VirtualDeviceType.LostCapture)
			{
				_isCanceled = true;
			}
			_currentInputEventArgs = e;
			_devices.Remove(e.Id);
			if (!IsMultiTouchEnabled && _firstMouseDownId != e.Id)
			{
				return;
			}
			PaintAggregateExceptionBuilder.DoActionsWithException("Mode.OnDeviceUp 或 Mode.OnElementClicked 或 Mode.OnInputCompleted 或 Mode.OnElementDragCompleted 或 Mode.OnElementDragCanceled 发生异常，异常的详细信息如下：", () =>
			{
				OnDeviceUp(e);
			}, () =>
			{
				if (_isActive)
				{
					if (e.Type == DeviceType.Mouse || (e.Type == DeviceType.Stylus && e.Button != DeviceButton.Eraser))
					{
						if (_firstMouseDownId == e.Id)
						{
							_firstMouseDownId = 0;
							_eventConverter.Up(e.Position);
						}
					}
					else if (e.Type == DeviceType.Touch && _firstMouseDownId == e.Id)
					{
						_firstMouseDownId = 0;
						_eventConverter.Up(e.Position);
					}
				}
			}, () =>
			{
				EventConverterList?.ForEach((IEventConverter converter) =>
				{
					converter.DeviceUp(e);
				});
			}, () =>
			{
				if (!_devices.Any())
				{
					OnInputCompleted(e);
					_isCanceled = false;
					EventConverterList?.ForEach((IEventConverter converter) =>
					{
						converter.InputCompleted(e);
					});
				}
			});
		}

		public virtual void SwitchOn()
		{
			if (CurrentSlide != null)
			{
				_dragTolerance = new Lazy<DeviceInputDoubleDifferentialValue>(() => CurrentSlide.GetSettings<ThresholdBoardSettings>().DragTolerance);
				_hitTolerance = new Lazy<DeviceInputDoubleDifferentialValue>(() => CurrentSlide.GetSettings<ThresholdBoardSettings>().HitTestTolerance);
				CurrentSlide.KeyDown += CurrentSlide_KeyDown;
				try
				{
					OnSwitchOn();
				}
				finally
				{
					_isActive = true;
				}
				EventConverterList?.ForEach((IEventConverter converter) =>
				{
					converter.CurrentSlide = CurrentSlide;
					converter.SwitchOn();
				});
			}
		}

		public virtual void SwitchOff()
		{
			if (CurrentSlide != null)
			{
				Slide currentSlide = CurrentSlide;
				_isActive = false;
				try
				{
					OnSwitchOff();
				}
				finally
				{
					_dragTolerance = null;
					_hitTolerance = null;
					_devices.Clear();
					currentSlide.KeyDown -= CurrentSlide_KeyDown;
				}
				EventConverterList?.ForEach((IEventConverter converter) =>
				{
					converter.SwitchOff();
					converter.CurrentSlide = null;
				});
			}
		}

		protected int GetDeviceCount()
		{
			return _devices.Count;
		}

		protected int? GetMainDeviceId()
		{
			if (_devices.Any())
			{
				return _devices.First().Key;
			}
			return null;
		}

		protected bool ContainsDevice(int id)
		{
			return _devices.ContainsKey(id);
		}

		[CanBeNull]
		public Board GetBoard()
		{
			return CurrentSlide?.Parent as Board;
		}

		protected virtual void OnDeviceDown(ModeInputEventArgs e)
		{
		}

		protected virtual void OnDeviceMove(ModeInputEventArgs e)
		{
		}

		protected virtual void OnDeviceHover(ModeInputEventArgs e)
		{
		}

		protected virtual void OnDeviceUp(ModeInputEventArgs e)
		{
		}

		protected virtual void OnInputStarting(ModeInputEventArgs e)
		{
		}

		protected virtual void OnInputCompleted(ModeInputEventArgs e)
		{
		}

		protected virtual void OnElementClicked(ModeInputEventArgs e)
		{
		}

		protected virtual void OnElementDragStarted(ModeInputDragEventArgs e)
		{
		}

		protected virtual void OnElementDragDelta(ModeInputDragEventArgs e)
		{
		}

		protected virtual void OnElementDragCompleted(ModeInputDragEventArgs e)
		{
		}

		protected virtual void OnElementDragCanceled(ModeInputDragEventArgs e)
		{
		}

		protected virtual void OnSwitchOn()
		{
		}

		protected virtual void OnSwitchOff()
		{
		}

		private void CurrentSlide_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
			{
				Mouse.Capture(null);
				Stylus.Capture(null);
			}
		}

		private void OnNormalTap(Point originPoint, Point currentPoint)
		{
			if (!IsInputLocked)
			{
				OnElementClicked(ModeInputEventArgs.FromDeviceInput(_currentInputEventArgs));
			}
		}

		private void OnNormalDragStarted(Point originPoint, Point currentPoint, Vector offsetToOrigin, Vector offsetToLast)
		{
			if (!IsInputLocked)
			{
				OnElementDragStarted(ModeInputDragEventArgs.FromDeviceInput(_currentInputEventArgs, originPoint, currentPoint, offsetToOrigin, offsetToLast));
			}
		}

		private void OnNormalDragDelta(Point originPoint, Point currentPoint, Vector offsetToOrigin, Vector offsetToLast)
		{
			if (!IsInputLocked)
			{
				OnElementDragDelta(ModeInputDragEventArgs.FromDeviceInput(_currentInputEventArgs, originPoint, currentPoint, offsetToOrigin, offsetToLast));
			}
		}

		private void OnNormalDragCompleted(Point originPoint, Point currentPoint, Vector offsetToOrigin, Vector offsetToLast)
		{
			if (!IsInputLocked)
			{
				ModeInputDragEventArgs e = ModeInputDragEventArgs.FromDeviceInput(_currentInputEventArgs, originPoint, currentPoint, offsetToOrigin, offsetToLast);
				if (_isCanceled)
				{
					OnElementDragCanceled(e);
				}
				else
				{
					OnElementDragCompleted(e);
				}
			}
		}

		protected double GetAllowance(DeviceType type, DeviceInputDoubleDifferentialValue allowance = null)
		{
			DeviceInputDoubleDifferentialValue deviceInputDoubleDifferentialValue = allowance ?? _hitTolerance?.Value;
			if (deviceInputDoubleDifferentialValue == null)
			{
				return 1.0;
			}
			switch (type)
			{
			case DeviceType.Mouse:
				return deviceInputDoubleDifferentialValue.Mouse;
			case DeviceType.Touch:
				return deviceInputDoubleDifferentialValue.Touch;
			case DeviceType.Stylus:
				return deviceInputDoubleDifferentialValue.Stylus;
			default:
				return 0.0;
			}
		}
	}
}
