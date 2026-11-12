using System.Windows.Ink;

namespace Cvte.Internal.Ink
{
	public sealed class EllipseStylusShape : StylusShape
	{
		public EllipseStylusShape(double width, double height, double rotation = 0.0)
			: base(StylusTip.Ellipse, width, height, rotation)
		{
		}
	}
}
