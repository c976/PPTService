using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class Vector : SplitStringEncodedType<double>
	{
		public double X { get; set; }

		public double Y { get; set; }

		public static implicit operator Vector(System.Windows.Vector vector)
		{
			return Convert(vector);
		}

		public static implicit operator System.Windows.Vector(Vector vector)
		{
			return ConvetBack(vector);
		}

		private static Vector Convert(System.Windows.Vector value)
		{
			return new Vector
			{
				X = value.X,
				Y = value.Y
			};
		}

		private static System.Windows.Vector ConvetBack(Vector value)
		{
			if (value == null)
			{
				return default(System.Windows.Vector);
			}
			return new System.Windows.Vector(value.X, value.Y);
		}

		protected override double[] GetParams()
		{
			return new double[2] { X, Y };
		}

		protected override void SetParams(params double[] values)
		{
			X = values[0];
			Y = values[1];
		}
	}
}
