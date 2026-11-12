using System.Windows.Ink;

namespace Cvte.Internal.Ink
{
	public sealed class RectangleStylusShape : StylusShape
	{
		public RectangleStylusShape(double width, double height)
			: this(width, height, 0.0)
		{
		}

		public RectangleStylusShape(double width, double height, double rotation)
			: base(StylusTip.Rectangle, width, height, rotation)
		{
		}
	}
}
