using System.Windows;
using System.Windows.Input;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceInputStartedEventArgs : DeviceInputEventArgs
	{
		private new Point Position { get; }

		private new InputEventArgs OriginEventArgs { get; }

		public DeviceInputStartedEventArgs(int id, DeviceType type, DeviceButton button)
			: base(id, type, button)
		{
		}
	}
}
