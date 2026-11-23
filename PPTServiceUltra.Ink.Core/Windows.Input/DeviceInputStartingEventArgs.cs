using System.Windows;
using System.Windows.Input;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceInputStartingEventArgs : DeviceInputEventArgs
	{
		private new Point Position { get; }

		private new InputEventArgs OriginEventArgs { get; }

		public DeviceInputStartingEventArgs(int id, DeviceType type, DeviceButton button)
			: base(id, type, button)
		{
		}
	}
}
