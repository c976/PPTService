using System.Windows;
using System.Windows.Input;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceEventArgs : RoutedEventArgs
	{
		private const int MouseDeviceId = -1;

		public VirtualDeviceType VirtualType { get; set; }

		public DeviceType DeviceType { get; private set; }

		public int DeviceId { get; private set; }

		public InputDevice Device { get; private set; }

		public InputEventArgs OrignalArgs { get; private set; }

		public DeviceState DeviceState { get; private set; }

		internal DeviceEventArgs(InputEventArgs originalArgs)
		{
			MouseEventArgs e = originalArgs as MouseEventArgs;
			if (e != null)
			{
				DeviceId = -1;
				Device = e.MouseDevice;
				DeviceType = DeviceType.Mouse;
				DeviceState = ((e.LeftButton == MouseButtonState.Pressed) ? DeviceState.Pressed : DeviceState.Released);
			}
			else
			{
				StylusEventArgs e2 = originalArgs as StylusEventArgs;
				if (e2 != null)
				{
					DeviceId = e2.StylusDevice.Id;
					Device = e2.StylusDevice;
					DeviceType = DeviceType.Stylus;
					DeviceState = DeviceState.Pressed;
				}
				else
				{
					TouchEventArgs e3 = originalArgs as TouchEventArgs;
					if (e3 != null)
					{
						DeviceId = e3.TouchDevice.Id;
						Device = e3.TouchDevice;
						DeviceType = DeviceType.Touch;
						DeviceState = DeviceState.Pressed;
					}
				}
			}
			OrignalArgs = originalArgs;
		}

		public Point GetPosition(IInputElement relativElement)
		{
			if (DeviceType == DeviceType.Mouse)
			{
				return ((MouseEventArgs)OrignalArgs).GetPosition(relativElement);
			}
			if (DeviceType == DeviceType.Stylus)
			{
				return ((StylusEventArgs)OrignalArgs).GetPosition(relativElement);
			}
			return ((TouchEventArgs)OrignalArgs).GetTouchPoint(relativElement).Position;
		}

		public static explicit operator DeviceEventArgs(InputEventArgs e)
		{
			return new DeviceEventArgs(e);
		}
	}
}
