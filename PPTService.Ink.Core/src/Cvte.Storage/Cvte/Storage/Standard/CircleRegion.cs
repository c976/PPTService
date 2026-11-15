using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	[ComplexType(typeof(Region), "Circle")]
	public class CircleRegion : Region
	{
		private Circle _circle;

		public double CenterX => _circle.CenterX;

		public double CenterY => _circle.CenterY;

		public double Radius => _circle.Radius;

		public CircleRegion()
		{
		}

		public CircleRegion(double centerX, double centerY, double radius)
		{
			_circle = new Circle
			{
				CenterX = centerX,
				CenterY = centerY,
				Radius = radius
			};
		}

		public override StorableNode Deparse()
		{
			return new StorableNode("Circle")
			{
				Value = _circle.Encode()
			};
		}

		public override void Parse(StorableNode node)
		{
			_circle = Circle.Parse(node.StringValue);
		}
	}
}
