using System.Windows;
using System.Windows.Input;

namespace PPTServiceUltra.Paint.Framework
{
	public class TouchDeviceEventArgs : DeviceEventArgs
	{
		public override bool IsTouch => true;

		public TouchDeviceEventArgs(Point position, InputEventArgs args, int deviceId)
			: base(position, args, deviceId)
		{
		}
	}
}
