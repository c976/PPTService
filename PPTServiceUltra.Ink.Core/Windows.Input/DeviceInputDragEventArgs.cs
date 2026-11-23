using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceInputDragEventArgs : DeviceInputEventArgs
	{
		public Point OriginPosition { get; set; }

		public Vector OffsetToOrigin { get; set; }

		public Vector OffsetToLast { get; set; }

		public DeviceInputDragEventArgs(int id, DeviceType type, DeviceButton button, Point originPosition, Point currentPosition, Vector offsetToOrigin, Vector offsetToLast)
			: base(id, currentPosition, type, button, null)
		{
			OriginPosition = originPosition;
			OffsetToOrigin = offsetToOrigin;
			OffsetToLast = offsetToLast;
		}
	}
}
