using System.Windows;

namespace Cvte.Paint.Inks.Hardware.RawTouchEngine
{
	public class RawTouchEventArgs : RoutedEventArgs
	{
		internal Point Position { get; set; }

		public int Id { get; internal set; }

		public int Width { get; internal set; }

		public int Height { get; internal set; }

		public RawTouchEventArgs()
		{
		}

		public RawTouchEventArgs(RoutedEvent routedEvent, UIElement element)
			: base(routedEvent, element)
		{
		}

		public Point GetPosition(UIElement relativeTo)
		{
			return relativeTo.PointFromScreen(Position);
		}

		public override string ToString()
		{
			return $"RawTouchEventArgs{{ Id={Id}, Position={Position:f0}, Size=({Width}, {Height}) }}";
		}
	}
}
