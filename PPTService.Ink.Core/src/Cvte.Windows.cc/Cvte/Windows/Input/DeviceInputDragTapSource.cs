using System.Windows;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceInputDragTapSource : DeviceInputSource
	{
		private DeviceInputEventHandler _attachedTapHandler;

		private DeviceInputDragEventHandler _attachedDragStartedHandler;

		private DeviceInputDragEventHandler _attachedDragDeltaHandler;

		private DeviceInputDragEventHandler _attachedDragCompletedHandler;

		private readonly DeviceInputDoubleDifferentialValue _tolerance;

		private readonly DownMoveUpToDragTapEventConverter _eventConverter;

		private int? _deviceId;

		private DeviceInputEventArgs _currentArgs;

		private bool _isCanceled;

		private IInputElement _currentCapture;

		public bool SuppressCompleteWhenCancel { get; set; }

		public event DeviceInputEventHandler Tap;

		public event DeviceInputDragEventHandler DragStarted;

		public event DeviceInputDragEventHandler DragDelta;

		public event DeviceInputDragEventHandler DragCompleted;

		public DeviceInputDragTapSource(UIElement target, DeviceInputDoubleDifferentialValue tolerance, bool preview)
			: base(target, preview)
		{
			_tolerance = tolerance;
			_eventConverter = new DownMoveUpToDragTapEventConverter(OnTap, OnDragStarted, OnDragDelta, OnDragCompleted);
			InputStarted += OnInputStarted;
			InputCompleted += OnInputCompleted;
		}

		public void Attach(DeviceInputEventHandler tapHandler)
		{
			Attach((DeviceInputEventHandler)OnInputDown, (DeviceInputEventHandler)OnInputMove, (DeviceInputEventHandler)OnInputUp);
			if (_attachedTapHandler != null)
			{
				Tap -= _attachedTapHandler;
			}
			_attachedTapHandler = tapHandler;
			Tap += _attachedTapHandler;
		}

		public void Attach(DeviceInputDragEventHandler dragStartedHandler, DeviceInputDragEventHandler dragDeltaHandler, DeviceInputDragEventHandler dragCompletedHandler)
		{
			Attach((DeviceInputEventHandler)OnInputDown, (DeviceInputEventHandler)OnInputMove, (DeviceInputEventHandler)OnInputUp);
			if (_attachedDragStartedHandler != null)
			{
				DragStarted -= _attachedDragStartedHandler;
			}
			if (_attachedDragDeltaHandler != null)
			{
				DragDelta -= _attachedDragDeltaHandler;
			}
			if (_attachedDragCompletedHandler != null)
			{
				DragCompleted -= _attachedDragCompletedHandler;
			}
			_attachedDragStartedHandler = dragStartedHandler;
			_attachedDragDeltaHandler = dragDeltaHandler;
			_attachedDragCompletedHandler = dragCompletedHandler;
			DragStarted += _attachedDragStartedHandler;
			DragDelta += _attachedDragDeltaHandler;
			DragCompleted += _attachedDragCompletedHandler;
		}

		public override void Detach()
		{
			base.Detach();
			if (_attachedTapHandler != null)
			{
				Tap -= _attachedTapHandler;
			}
			if (_attachedDragStartedHandler != null)
			{
				DragStarted -= _attachedDragStartedHandler;
			}
			if (_attachedDragDeltaHandler != null)
			{
				DragDelta -= _attachedDragDeltaHandler;
			}
			if (_attachedDragCompletedHandler != null)
			{
				DragCompleted -= _attachedDragCompletedHandler;
			}
		}

		private void OnInputStarted(object sender, DeviceInputStartedEventArgs e)
		{
			_isCanceled = false;
			IInputElement captureTarget = CaptureTarget;
			_currentCapture = captureTarget ?? CurrentCaptureTarget;
		}

		private void OnInputCompleted(object sender, DeviceInputCompletedEventArgs e)
		{
			_currentCapture = null;
			_deviceId = null;
		}

		private void OnInputDown(object sender, DeviceInputEventArgs e)
		{
			if (!_deviceId.HasValue || _deviceId == e.Id)
			{
				_currentArgs = e;
				_deviceId = e.Id;
				double tolerance = _tolerance.Mouse;
				switch (e.Type)
				{
				case DeviceType.Touch:
					tolerance = _tolerance.Touch;
					break;
				case DeviceType.Stylus:
					tolerance = _tolerance.Stylus;
					break;
				}
				_eventConverter.Down(e.Position, tolerance);
			}
		}

		private void OnInputMove(object sender, DeviceInputEventArgs e)
		{
			if (_deviceId == e.Id)
			{
				_currentArgs = e;
				_eventConverter.Move(e.Position);
			}
		}

		private void OnInputUp(object sender, DeviceInputEventArgs e)
		{
			if (_deviceId == e.Id)
			{
				if (e.VirtualType == VirtualDeviceType.LostCapture)
				{
					_isCanceled = true;
				}
				_currentArgs = e;
				_eventConverter.Up(e.Position);
			}
		}

		private void OnTap(Point originPoint, Point currentPoint)
		{
			RaiseTap(new DeviceInputEventArgs(_currentArgs.Id, currentPoint, _currentArgs.Type, _currentArgs.Button, null));
		}

		private void OnDragStarted(Point originPoint, Point currentPoint, Vector offsetToOrigin, Vector offsetToLast)
		{
			RaiseDragStarted(new DeviceInputDragEventArgs(_currentArgs.Id, _currentArgs.Type, _currentArgs.Button, originPoint, currentPoint, offsetToOrigin, offsetToLast));
		}

		private void OnDragDelta(Point originPoint, Point currentPoint, Vector offsetToOrigin, Vector offsetToLast)
		{
			RaiseDragDelta(new DeviceInputDragEventArgs(_currentArgs.Id, _currentArgs.Type, _currentArgs.Button, originPoint, currentPoint, offsetToOrigin, offsetToLast));
		}

		private void OnDragCompleted(Point originPoint, Point currentPoint, Vector offsetToOrigin, Vector offsetToLast)
		{
			if (!_isCanceled || !SuppressCompleteWhenCancel)
			{
				RaiseDragCompleted(new DeviceInputDragEventArgs(_currentArgs.Id, _currentArgs.Type, _currentArgs.Button, originPoint, currentPoint, offsetToOrigin, offsetToLast)
				{
					VirtualType = (_isCanceled ? VirtualDeviceType.LostCapture : VirtualDeviceType.Device)
				});
			}
		}

		private void RaiseTap(DeviceInputEventArgs e)
		{
			DeviceInputEventHandler deviceInputEventHandler = Tap;
			OnRaising(e);
			deviceInputEventHandler?.Invoke(this, e);
		}

		private void RaiseDragStarted(DeviceInputDragEventArgs e)
		{
			DeviceInputDragEventHandler deviceInputDragEventHandler = DragStarted;
			OnRaising(e);
			deviceInputDragEventHandler?.Invoke(this, e);
		}

		private void RaiseDragDelta(DeviceInputDragEventArgs e)
		{
			DeviceInputDragEventHandler deviceInputDragEventHandler = DragDelta;
			OnRaising(e);
			deviceInputDragEventHandler?.Invoke(this, e);
		}

		private void RaiseDragCompleted(DeviceInputDragEventArgs e)
		{
			DeviceInputDragEventHandler deviceInputDragEventHandler = DragCompleted;
			OnRaising(e);
			deviceInputDragEventHandler?.Invoke(this, e);
		}

		public void Cancel()
		{
			IInputElement currentCapture = _currentCapture;
			if (currentCapture != null)
			{
				currentCapture.ReleaseMouseCapture();
				currentCapture.ReleaseStylusCapture();
			}
		}
	}
}
