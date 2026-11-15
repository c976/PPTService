using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class Circle : SplitStringEncodedType<double>
	{
		public double CenterX { get; set; }

		public double CenterY { get; set; }

		public double Radius { get; set; }

		protected override double[] GetParams()
		{
			return new double[3] { CenterX, CenterY, Radius };
		}

		protected override void SetParams(params double[] values)
		{
			CenterX = values[0];
			CenterY = values[1];
			Radius = values[2];
		}

		public static Circle Parse(string value)
		{
			Circle circle = new Circle();
			circle.TryDecode(value);
			return circle;
		}
	}
}
