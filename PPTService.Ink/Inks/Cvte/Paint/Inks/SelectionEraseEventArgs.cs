using System.Windows;
using System.Windows.Media;
using Cvte.Windows.Input;

namespace Cvte.Paint.Inks
{
	public class SelectionEraseEventArgs : OperationSynchronousEventArgs
	{
		public enum SelectionEraseType
		{
			Point,
			Rect,
			Path
		}

		public DeviceType DeviceType { get; internal set; }

		public Point TappedPosition { get; internal set; }

		public Rect ContainedBounds { get; internal set; }

		public Geometry ContainedGeometry { get; internal set; }

		public SelectionEraseType EraseType { get; }

		public SelectionEraseEventArgs(SelectionEraseType type)
		{
			EraseType = type;
		}
	}
}
