using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	[ComplexType(typeof(Region), "Rectangle")]
	public class RectangleRegion : Region
	{
		private Rectangle _rectangle;

		public double Left => _rectangle.Left;

		public double Top => _rectangle.Top;

		public double Width => _rectangle.Width;

		public double Height => _rectangle.Height;

		public RectangleRegion()
		{
		}

		public RectangleRegion(double left, double top, double width, double height)
		{
			_rectangle = new Rectangle
			{
				Left = left,
				Top = top,
				Width = width,
				Height = height
			};
		}

		public override StorableNode Deparse()
		{
			return new StorableNode("Rectangle")
			{
				Value = _rectangle.Encode()
			};
		}

		public override void Parse(StorableNode node)
		{
			_rectangle = Rectangle.Parse(node.StringValue);
		}
	}
}
