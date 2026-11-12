using System.Windows;
using System.Windows.Input;

namespace Cvte.Paint.Framework
{
	public class MouseDeviceEventArgs : DeviceEventArgs
	{
		public override bool IsTouch => false;

		public MouseDeviceEventArgs(Point position, InputEventArgs args)
			: base(position, args, int.MaxValue)
		{
		}

		public MouseDeviceEventArgs(Point position, InputEventArgs args, int deviceId)
			: base(position, args, deviceId)
		{
		}
	}
}
