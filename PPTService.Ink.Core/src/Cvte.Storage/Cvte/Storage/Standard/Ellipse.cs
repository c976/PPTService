using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class Ellipse : SplitStringEncodedType<double>
	{
		public double CenterX { get; set; }

		public double CenterY { get; set; }

		public double RadiusX { get; set; }

		public double RadiusY { get; set; }

		protected override double[] GetParams()
		{
			return new double[4] { CenterX, CenterY, RadiusX, RadiusY };
		}

		protected override void SetParams(params double[] values)
		{
			CenterX = values[0];
			CenterY = values[1];
			RadiusX = values[2];
			RadiusY = values[3];
		}

		public static Ellipse Parse(string value)
		{
			Ellipse ellipse = new Ellipse();
			ellipse.TryDecode(value);
			return ellipse;
		}
	}
}
