using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Input;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceInputSource
	{
		public static readonly DependencyProperty CanRespondProperty = DependencyProperty.RegisterAttached("CanRespond", typeof(bool), typeof(DeviceInputSource), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

		private readonly bool _preview;

		private DeviceInputEventHandler _attachedInputDown;

		private DeviceInputEventHandler _attachedInputMove;

		private DeviceInputEventHandler _attachedInputUp;

		private UIElement _currentContainer;

		private readonly MouseButtonEventHandler _previewMouseDownEventHandler;

		private readonly MouseEventHandler _previewMouseMoveEventHandler;

		private readonly MouseButtonEventHandler _previewMouseUpEventHandler;

		private readonly MouseButtonEventHandler _mouseDownEventHandler;

		private readonly MouseEventHandler _mouseMoveEventHandler;

		private readonly MouseButtonEventHandler _mouseUpEventHandler;

		private readonly MouseEventHandler _lostMouseCaptureEventHandler;

		private readonly StylusDownEventHandler _previewStylusDownEventHandler;

		private readonly StylusEventHandler _previewStylusMoveEventHandler;

		private readonly StylusEventHandler _previewStylusUpEventHandler;

		private readonly StylusDownEventHandler _stylusDownEventHandler;

		private readonly StylusEventHandler _stylusMoveEventHandler;

		private readonly StylusEventHandler _stylusUpEventHandler;

		private readonly StylusEventHandler _lostStylusCaptureEventHandler;

		private DeviceType? _currentDeviceType;

		private DeviceButton _currentDeviceButton;

		private bool _isCapturing;

		private readonly Dictionary<int, DeviceInputEventArgs> _currentDevices = new Dictionary<int, DeviceInputEventArgs>();

		private bool _isRunning;

		private readonly Queue<KeyValuePair<string, Action>> _actions = new Queue<KeyValuePair<string, Action>>();

		[NotNull]
		public UIElement Target { get; }

		[CanBeNull]
		public UIElement Container { get; set; }

		[CanBeNull]
		public UIElement CaptureTarget { get; set; }

		[NotNull]
		private UIElement InternalContainer => Container ?? Target;

		protected IInputElement CurrentCaptureTarget { get; private set; }

		private Point LastPoint { get; set; }

		public event DeviceInputStartingEventHandler InputStarting;

		public event DeviceInputStartedEventHandler InputStarted;

		public event DeviceInputCompletedEventHandler InputCompleted;

		public event DeviceInputEventHandler InputDown;

		public event DeviceInputEventHandler InputMove;

		public event DeviceInputEventHandler InputHover;

		public event DeviceInputEventHandler InputUp;

		public DeviceInputSource([NotNull] UIElement target, bool preview = false)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			Target = target;
			_preview = preview;
			_previewMouseDownEventHandler = OnMouseDown;
			_previewMouseMoveEventHandler = OnMouseMove;
			_previewMouseUpEventHandler = OnMouseUp;
			_mouseDownEventHandler = OnMouseDown;
			_mouseMoveEventHandler = OnMouseMove;
			_mouseUpEventHandler = OnMouseUp;
			_lostMouseCaptureEventHandler = OnLostMouseCapture;
			_previewStylusDownEventHandler = OnStylusDown;
			_previewStylusMoveEventHandler = OnStylusMove;
			_previewStylusUpEventHandler = OnStylusUp;
			_stylusDownEventHandler = OnStylusDown;
			_stylusMoveEventHandler = OnStylusMove;
			_stylusUpEventHandler = OnStylusUp;
			_lostStylusCaptureEventHandler = OnLostStylusCapture;
		}

		public void Attach(DeviceInputEventHandler inputDown, DeviceInputEventHandler inputMove, DeviceInputEventHandler inputUp)
		{
			if (_attachedInputDown != null)
			{
				InputDown -= _attachedInputDown;
			}
			if (_attachedInputMove != null)
			{
				InputMove -= _attachedInputMove;
			}
			if (_attachedInputUp != null)
			{
				InputUp -= _attachedInputUp;
			}
			_attachedInputDown = inputDown;
			_attachedInputMove = inputMove;
			_attachedInputUp = inputUp;
			InputDown += _attachedInputDown;
			InputMove += _attachedInputMove;
			InputUp += _attachedInputUp;
			if (_preview)
			{
				Target.RemoveHandler(Mouse.PreviewMouseDownEvent, _previewMouseDownEventHandler);
				Target.RemoveHandler(Mouse.PreviewMouseMoveEvent, _previewMouseMoveEventHandler);
				Target.RemoveHandler(Mouse.PreviewMouseUpEvent, _previewMouseUpEventHandler);
				Target.RemoveHandler(Mouse.LostMouseCaptureEvent, _lostMouseCaptureEventHandler);
				Target.RemoveHandler(Stylus.PreviewStylusDownEvent, _previewStylusDownEventHandler);
				Target.RemoveHandler(Stylus.PreviewStylusMoveEvent, _previewStylusMoveEventHandler);
				Target.RemoveHandler(Stylus.PreviewStylusUpEvent, _previewStylusUpEventHandler);
				Target.RemoveHandler(Stylus.LostStylusCaptureEvent, _lostStylusCaptureEventHandler);
				Target.AddHandler(Mouse.PreviewMouseDownEvent, _previewMouseDownEventHandler);
				Target.AddHandler(Mouse.PreviewMouseMoveEvent, _previewMouseMoveEventHandler);
				Target.AddHandler(Mouse.PreviewMouseUpEvent, _previewMouseUpEventHandler);
				Target.AddHandler(Mouse.LostMouseCaptureEvent, _lostMouseCaptureEventHandler);
				Target.AddHandler(Stylus.PreviewStylusDownEvent, _previewStylusDownEventHandler);
				Target.AddHandler(Stylus.PreviewStylusMoveEvent, _previewStylusMoveEventHandler);
				Target.AddHandler(Stylus.PreviewStylusUpEvent, _previewStylusUpEventHandler);
				Target.AddHandler(Stylus.LostStylusCaptureEvent, _lostStylusCaptureEventHandler);
			}
			else
			{
				Target.RemoveHandler(Mouse.MouseDownEvent, _mouseDownEventHandler);
				Target.RemoveHandler(Mouse.MouseMoveEvent, _mouseMoveEventHandler);
				Target.RemoveHandler(Mouse.MouseUpEvent, _mouseUpEventHandler);
				Target.RemoveHandler(Mouse.LostMouseCaptureEvent, _lostMouseCaptureEventHandler);
				Target.RemoveHandler(Stylus.StylusDownEvent, _stylusDownEventHandler);
				Target.RemoveHandler(Stylus.StylusMoveEvent, _stylusMoveEventHandler);
				Target.RemoveHandler(Stylus.StylusUpEvent, _stylusUpEventHandler);
				Target.RemoveHandler(Stylus.LostStylusCaptureEvent, _lostStylusCaptureEventHandler);
				Target.AddHandler(Mouse.MouseDownEvent, _mouseDownEventHandler);
				Target.AddHandler(Mouse.MouseMoveEvent, _mouseMoveEventHandler);
				Target.AddHandler(Mouse.MouseUpEvent, _mouseUpEventHandler);
				Target.AddHandler(Mouse.LostMouseCaptureEvent, _lostMouseCaptureEventHandler);
				Target.AddHandler(Stylus.StylusDownEvent, _stylusDownEventHandler);
				Target.AddHandler(Stylus.StylusMoveEvent, _stylusMoveEventHandler);
				Target.AddHandler(Stylus.StylusUpEvent, _stylusUpEventHandler);
				Target.AddHandler(Stylus.LostStylusCaptureEvent, _lostStylusCaptureEventHandler);
			}
		}

		public virtual void Detach()
		{
			if (_preview)
			{
				Target.PreviewMouseDown -= OnMouseDown;
				Target.PreviewMouseMove -= OnMouseMove;
				Target.PreviewMouseUp -= OnMouseUp;
				Target.PreviewStylusDown -= OnStylusDown;
				Target.PreviewStylusMove -= OnStylusMove;
				Target.PreviewStylusUp -= OnStylusUp;
			}
			else
			{
				Target.MouseDown -= OnMouseDown;
				Target.MouseMove -= OnMouseMove;
				Target.MouseUp -= OnMouseUp;
				Target.StylusDown -= OnStylusDown;
				Target.StylusMove -= OnStylusMove;
				Target.StylusUp -= OnStylusUp;
			}
			if (_attachedInputDown != null)
			{
				InputDown -= _attachedInputDown;
			}
			if (_attachedInputMove != null)
			{
				InputMove -= _attachedInputMove;
			}
			if (_attachedInputUp != null)
			{
				InputUp -= _attachedInputUp;
			}
		}

		private void OnStylusDown(object sender, StylusEventArgs e)
		{
			IInputElement captureTarget = CaptureTarget;
			if (!CanCapture(captureTarget ?? ((IInputElement)e.OriginalSource)))
			{
				return;
			}
			RunOneByOne(() =>
			{
				if (_currentDeviceType.HasValue && _currentDeviceType == DeviceType.Mouse)
				{
					ReleaseMouseCapture();
					RaiseInputUp(new DeviceInputEventArgs(-1, LastPoint, DeviceType.Mouse, _currentDeviceButton, e)
					{
						VirtualType = VirtualDeviceType.LostCapture
					});
					_currentDeviceType = null;
					if (_currentDevices.ContainsKey(-1))
					{
						_currentDevices.Remove(-1);
					}
				}
			}, "OnStylusDown");
			RunOneByOne(() =>
			{
				DeviceType? deviceType = GetDeviceType(e);
				if (deviceType.HasValue)
				{
					DeviceType value = deviceType.Value;
					DeviceButton deviceButton = GetDeviceButton(e);
					if (!_currentDevices.Any())
					{
						RaiseStarting(new DeviceInputStartingEventArgs(e.StylusDevice.Id, value, deviceButton));
						_currentContainer = InternalContainer;
						DeviceInputSource deviceInputSource = this;
						IInputElement captureTarget2 = CaptureTarget;
						deviceInputSource.CurrentCaptureTarget = captureTarget2 ?? ((IInputElement)e.OriginalSource);
						_currentDeviceButton = deviceButton;
						_currentDeviceType = value;
						RaiseStarted(new DeviceInputStartedEventArgs(e.StylusDevice.Id, value, deviceButton));
					}
					Point position = e.GetPosition(_currentContainer);
					LastPoint = position;
					DeviceInputEventArgs e2 = new DeviceInputEventArgs(e.StylusDevice.Id, position, value, _currentDeviceButton, e.GetStylusPoints(_currentContainer), e);
					if (!_currentDeviceType.HasValue || _currentDeviceType == value)
					{
						_currentDevices[e.StylusDevice.Id] = e2;
						try
						{
							RaiseInputDown(e2);
						}
						finally
						{
							_isCapturing = true;
							CurrentCaptureTarget.CaptureStylus();
							_isCapturing = false;
							if (_preview)
							{
								e.Handled = true;
							}
						}
					}
				}
			}, "OnStylusDown");
		}

		private void OnStylusMove(object sender, StylusEventArgs e)
		{
			RunOneByOne(() =>
			{
				if (_currentDeviceType != DeviceType.Mouse && !_isCapturing)
				{
					DeviceType? deviceType = GetDeviceType(e);
					if (deviceType.HasValue)
					{
						DeviceType value = deviceType.Value;
						try
						{
							if (_currentDeviceType == value && _currentDevices.ContainsKey(e.StylusDevice.Id))
							{
								try
								{
									Point position = e.GetPosition(_currentContainer);
									LastPoint = position;
									RaiseInputMove(new DeviceInputEventArgs(e.StylusDevice.Id, position, value, _currentDeviceButton, e.GetStylusPoints(_currentContainer), e));
									return;
								}
								catch (Exception)
								{
									ReleaseStylusCapture();
									throw;
								}
							}
							RaiseInputHover(new DeviceInputEventArgs(e.StylusDevice.Id, e.GetPosition(InternalContainer), value, DeviceButton.None, e.GetStylusPoints(_currentContainer), e));
						}
						finally
						{
							if (_preview)
							{
								e.Handled = true;
							}
						}
					}
				}
			}, "OnStylusMove");
		}

		private void OnStylusUp(object sender, StylusEventArgs e)
		{
			RunOneByOne(() =>
			{
				if (_currentDeviceType != DeviceType.Mouse && e.StylusDevice != null && _currentDevices.ContainsKey(e.StylusDevice.Id))
				{
					DeviceType? deviceType = GetDeviceType(e);
					if (deviceType.HasValue)
					{
						DeviceType value = deviceType.Value;
						if (_currentDeviceType == value)
						{
							_currentDevices.Remove(e.StylusDevice.Id);
							try
							{
								Point position = e.GetPosition(_currentContainer);
								LastPoint = position;
								RaiseInputUp(new DeviceInputEventArgs(e.StylusDevice.Id, position, value, _currentDeviceButton, e.GetStylusPoints(_currentContainer), e));
							}
							finally
							{
								if (!_currentDevices.Any())
								{
									ReleaseStylusCapture();
								}
							}
						}
					}
				}
			}, "OnStylusUp");
		}

		private void OnLostStylusCapture(object sender, StylusEventArgs e)
		{
			RunOneByOne(() =>
			{
				if (!_currentDeviceType.HasValue)
				{
					return;
				}
				DeviceType value = _currentDeviceType.Value;
				_currentDeviceType = null;
				try
				{
					DeviceInputEventArgs[] array = _currentDevices.Values.ToArray();
					foreach (DeviceInputEventArgs e2 in array)
					{
						e2.VirtualType = VirtualDeviceType.LostCapture;
						RaiseInputUp(e2);
					}
				}
				finally
				{
					VirtualDeviceType virtualType = (_currentDevices.Any() ? VirtualDeviceType.LostCapture : VirtualDeviceType.Device);
					_currentDevices.Clear();
					RaiseCompleted(new DeviceInputCompletedEventArgs(value, _currentDeviceButton, virtualType));
				}
			}, "OnLostStylusCapture");
		}

		private void ReleaseStylusCapture()
		{
			_isCapturing = true;
			if (CurrentCaptureTarget.IsStylusCaptured)
			{
				CurrentCaptureTarget.ReleaseStylusCapture();
			}
			else
			{
				OnLostStylusCapture(null, null);
			}
			_isCapturing = false;
		}

		private DeviceType? GetDeviceType(StylusEventArgs e)
		{
			switch (e.StylusDevice.TabletDevice.Type)
			{
			case TabletDeviceType.Touch:
				return DeviceType.Touch;
			case TabletDeviceType.Stylus:
				return DeviceType.Stylus;
			default:
				return null;
			}
		}

		private DeviceButton GetDeviceButton(StylusEventArgs e)
		{
			if (e.StylusDevice.Name == "Stylus")
			{
				StylusButton stylusButton = e.StylusDevice.StylusButtons?.FirstOrDefault((StylusButton x) => x.Name == "Barrel Switch");
				if (stylusButton != null && stylusButton.StylusButtonState == StylusButtonState.Down)
				{
					return DeviceButton.Context;
				}
				return DeviceButton.LeftButton;
			}
			if (e.StylusDevice.Name == "Eraser")
			{
				return DeviceButton.Eraser;
			}
			return DeviceButton.LeftButton;
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			IInputElement captureTarget = CaptureTarget;
			if (!CanCapture(captureTarget ?? ((IInputElement)e.OriginalSource)))
			{
				return;
			}
			RunOneByOne(() =>
			{
				if (e.StylusDevice != null || _currentDeviceType.HasValue)
				{
					return;
				}
				DeviceButton deviceButton = GetDeviceButton(e);
				if (!_currentDevices.Any())
				{
					RaiseStarting(new DeviceInputStartingEventArgs(-1, DeviceType.Mouse, deviceButton));
					_currentContainer = InternalContainer;
					DeviceInputSource deviceInputSource = this;
					IInputElement captureTarget2 = CaptureTarget;
					deviceInputSource.CurrentCaptureTarget = captureTarget2 ?? ((IInputElement)e.OriginalSource);
					_currentDeviceButton = deviceButton;
					_currentDeviceType = DeviceType.Mouse;
					RaiseStarted(new DeviceInputStartedEventArgs(-1, DeviceType.Mouse, deviceButton));
				}
				Point position = e.GetPosition(_currentContainer);
				LastPoint = position;
				DeviceInputEventArgs e2 = new DeviceInputEventArgs(-1, position, DeviceType.Mouse, _currentDeviceButton, e);
				_currentDevices[-1] = e2;
				try
				{
					RaiseInputDown(e2);
				}
				finally
				{
					_isCapturing = true;
					CurrentCaptureTarget.CaptureMouse();
					_isCapturing = false;
					if (_preview)
					{
						e.Handled = true;
					}
				}
			}, "OnMouseDown");
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			RunOneByOne(() =>
			{
				if (e.StylusDevice != null || _isCapturing)
				{
					return;
				}
				try
				{
					DeviceInputEventArgs e2;
					if (_currentDeviceType == DeviceType.Mouse)
					{
						Point position = e.GetPosition(_currentContainer);
						LastPoint = position;
						e2 = new DeviceInputEventArgs(-1, position, DeviceType.Mouse, _currentDeviceButton, e);
						_currentDevices[-1] = e2;
						try
						{
							RaiseInputMove(e2);
							return;
						}
						catch
						{
							ReleaseMouseCapture();
							throw;
						}
					}
					e2 = new DeviceInputEventArgs(-1, e.GetPosition(InternalContainer), DeviceType.Mouse, DeviceButton.None, e);
					RaiseInputHover(e2);
				}
				finally
				{
					if (_preview)
					{
						e.Handled = true;
					}
				}
			}, "OnMouseMove");
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			RunOneByOne(() =>
			{
				if (e.StylusDevice == null)
				{
					if (_currentDeviceType == DeviceType.Mouse)
					{
						Point position = e.GetPosition(_currentContainer);
						LastPoint = position;
						DeviceInputEventArgs e2 = new DeviceInputEventArgs(-1, position, DeviceType.Mouse, _currentDeviceButton, e);
						_currentDevices.Remove(-1);
						try
						{
							RaiseInputUp(e2);
						}
						finally
						{
							ReleaseMouseCapture();
						}
					}
					if (_preview)
					{
						e.Handled = true;
					}
				}
			}, "OnMouseUp");
		}

		private void OnLostMouseCapture(object sender, MouseEventArgs e)
		{
			RunOneByOne(() =>
			{
				if (!_currentDeviceType.HasValue)
				{
					return;
				}
				DeviceType value = _currentDeviceType.Value;
				_currentDeviceType = null;
				try
				{
					DeviceInputEventArgs[] array = _currentDevices.Values.ToArray();
					foreach (DeviceInputEventArgs e2 in array)
					{
						e2.VirtualType = VirtualDeviceType.LostCapture;
						RaiseInputUp(e2);
					}
				}
				finally
				{
					VirtualDeviceType virtualType = (_currentDevices.Any() ? VirtualDeviceType.LostCapture : VirtualDeviceType.Device);
					_currentDevices.Clear();
					RaiseCompleted(new DeviceInputCompletedEventArgs(value, _currentDeviceButton, virtualType));
				}
			}, "OnLostMouseCapture");
		}

		private void ReleaseMouseCapture()
		{
			_isCapturing = true;
			if (CurrentCaptureTarget.IsMouseCaptured)
			{
				CurrentCaptureTarget.ReleaseMouseCapture();
			}
			else
			{
				OnLostMouseCapture(null, null);
			}
			_isCapturing = false;
		}

		private DeviceButton GetDeviceButton(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				return DeviceButton.LeftButton;
			}
			if (e.RightButton == MouseButtonState.Pressed)
			{
				return DeviceButton.Context;
			}
			return DeviceButton.LeftButton;
		}

		private void RunOneByOne([NotNull] Action action, [CallerMemberName] string callerName = null)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			_actions.Enqueue(new KeyValuePair<string, Action>(callerName, action));
			if (_isRunning)
			{
				return;
			}
			_isRunning = true;
			try
			{
				Dictionary<Exception, string> dictionary = new Dictionary<Exception, string>();
				while (_actions.Any())
				{
					KeyValuePair<string, Action> keyValuePair = _actions.Dequeue();
					try
					{
						keyValuePair.Value();
					}
					catch (Exception key)
					{
						dictionary.Add(key, keyValuePair.Key);
					}
				}
				if (dictionary.Count == 1)
				{
					ExceptionDispatchInfo.Capture(dictionary.First().Key).Throw();
				}
				else if (dictionary.Any())
				{
					throw new AggregateException("DeviceInputSource: Error occurred during raising DeviceInput event. The events are: " + string.Join(",", dictionary.Values) + ".", dictionary.Select((KeyValuePair<Exception, string> x) => x.Key));
				}
			}
			finally
			{
				_isRunning = false;
			}
		}

		[Pure]
		private static bool CanCapture(IInputElement element)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null)
			{
				if (uIElement.IsVisible)
				{
					return uIElement.IsEnabled;
				}
				return false;
			}
			ContentElement contentElement = element as ContentElement;
			if (contentElement != null)
			{
				return contentElement.IsEnabled;
			}
			UIElement3D uIElement3D = element as UIElement3D;
			if (uIElement3D != null)
			{
				if (uIElement3D.IsVisible)
				{
					return uIElement3D.IsEnabled;
				}
				return false;
			}
			return true;
		}

		private void RaiseStarting(DeviceInputStartingEventArgs e)
		{
			DeviceInputStartingEventHandler deviceInputStartingEventHandler = InputStarting;
			OnRaising(e);
			deviceInputStartingEventHandler?.Invoke(this, e);
		}

		private void RaiseStarted(DeviceInputStartedEventArgs e)
		{
			DeviceInputStartedEventHandler deviceInputStartedEventHandler = InputStarted;
			OnRaising(e);
			deviceInputStartedEventHandler?.Invoke(this, e);
		}

		private void RaiseCompleted(DeviceInputCompletedEventArgs e)
		{
			DeviceInputCompletedEventHandler deviceInputCompletedEventHandler = InputCompleted;
			OnRaising(e);
			deviceInputCompletedEventHandler?.Invoke(this, e);
		}

		private void RaiseInputDown(DeviceInputEventArgs e)
		{
			DeviceInputEventHandler deviceInputEventHandler = InputDown;
			OnRaising(e);
			deviceInputEventHandler?.Invoke(this, e);
		}

		private void RaiseInputMove(DeviceInputEventArgs e)
		{
			DeviceInputEventHandler deviceInputEventHandler = InputMove;
			OnRaising(e);
			deviceInputEventHandler?.Invoke(this, e);
		}

		private void RaiseInputHover(DeviceInputEventArgs e)
		{
			DeviceInputEventHandler deviceInputEventHandler = InputHover;
			OnRaising(e);
			deviceInputEventHandler?.Invoke(this, e);
		}

		private void RaiseInputUp(DeviceInputEventArgs e)
		{
			DeviceInputEventHandler deviceInputEventHandler = InputUp;
			OnRaising(e);
			deviceInputEventHandler?.Invoke(this, e);
		}

		protected virtual void OnRaising(DeviceInputEventArgs e)
		{
			object value = Target.GetValue(CanRespondProperty);
			if (value is bool && !(bool)value)
			{
				e.VirtualType = VirtualDeviceType.LostCapture;
			}
		}
	}
}
