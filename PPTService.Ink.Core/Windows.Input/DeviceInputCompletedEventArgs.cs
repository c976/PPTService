using System.Windows;
using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceInputCompletedEventArgs : DeviceInputEventArgs
	{
		private new Point Position { get; }

		private new InputEventArgs OriginEventArgs { get; }

		private new int Id { get; }

		public DeviceInputCompletedEventArgs(DeviceType type, DeviceButton button, VirtualDeviceType virtualType)
			: base(-1, type, button)
		{
			VirtualType = virtualType;
		}
	}
}
