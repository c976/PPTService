using System;
using System.Windows;
using System.Windows.Input;
using PPTService.Ink.Core.Windows.Input;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal class InputGestureInfo
	{
		private Rect _lastClickRect;

		private int _lastClickTime = int.MaxValue;

		private Point _lastPosition;

		internal int ClickCount { get; private set; }

		internal void RecordDown(IInputElement sender, MouseButtonEventArgs e)
		{
			uint doubleClickTime = NativeMethod.GetDoubleClickTime();
			Size size = new Size(NativeMethod.GetSystemMetrics(NativeMethod.SystemMetric.SM_CXDOUBLECLK), NativeMethod.GetSystemMetrics(NativeMethod.SystemMetric.SM_CYDOUBLECLK));
			Point point = TextHelperInner.CorrectPosition(e, (UIElement)sender);
			int timestamp = e.Timestamp;
			if (!_lastClickRect.Contains(point) || timestamp - _lastClickTime > doubleClickTime)
			{
				ClickCount = 0;
			}
			ClickCount++;
			_lastClickTime = timestamp;
			_lastClickRect = new Rect(point, default(Size));
			_lastClickRect.Inflate(size.Width / 2.0, size.Height / 2.0);
		}

		public void RecordDown(Point position, DeviceType type)
		{
			int thresholdSquared = GetThresholdSquared(type);
			int tickCount = Environment.TickCount;
			if ((double)(tickCount - _lastClickTime) > 500.0 || (_lastPosition - position).LengthSquared > (double)thresholdSquared)
			{
				ClickCount = 0;
			}
			ClickCount++;
			_lastClickTime = tickCount;
			_lastPosition = position;
		}

		private int GetThresholdSquared(DeviceType type)
		{
			switch (type)
			{
			case DeviceType.Mouse:
				return 1;
			case DeviceType.Stylus:
				return 16;
			case DeviceType.Touch:
				return 100;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
		}
	}
}
