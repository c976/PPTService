using System.Windows;
using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceDragEventArgs : DeviceEventArgs
	{
		public double HorizontalChange { get; private set; }

		public double VerticalChange { get; private set; }

		public Point OldPosition { get; internal set; }

		public Point NewPosition { get; internal set; }

		public DeviceDragEventArgs(InputEventArgs originalArgs, double horizontalChange, double verticalChange)
			: base(originalArgs)
		{
			HorizontalChange = horizontalChange;
			VerticalChange = verticalChange;
		}
	}
}
