using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	[ComplexType(typeof(Region), "Ellipse")]
	public class EllipseRegion : Region
	{
		private Ellipse _ellipse;

		public double CenterX => _ellipse.CenterX;

		public double CenterY => _ellipse.CenterY;

		public double RadiusX => _ellipse.RadiusX;

		public double RadiusY => _ellipse.RadiusY;

		public EllipseRegion()
		{
		}

		public EllipseRegion(double centerX, double centerY, double radiusX, double radiusY)
		{
			_ellipse = new Ellipse
			{
				CenterX = centerX,
				CenterY = centerY,
				RadiusX = radiusX,
				RadiusY = radiusY
			};
		}

		public override StorableNode Deparse()
		{
			return new StorableNode("Ellipse")
			{
				Value = _ellipse.Encode()
			};
		}

		public override void Parse(StorableNode node)
		{
			_ellipse = Ellipse.Parse(node.StringValue);
		}
	}
}
