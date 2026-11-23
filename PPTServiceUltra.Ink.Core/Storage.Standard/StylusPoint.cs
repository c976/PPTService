using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class StylusPoint : SplitStringEncodedType<double>
	{
		public double X { get; set; }

		public double Y { get; set; }

		public double P { get; set; }

		public static implicit operator StylusPoint(System.Windows.Input.StylusPoint point)
		{
			return Convert(point);
		}

		public static implicit operator System.Windows.Input.StylusPoint(StylusPoint point)
		{
			return ConvertBack(point);
		}

		private static StylusPoint Convert(System.Windows.Input.StylusPoint value)
		{
			return new StylusPoint
			{
				X = value.X,
				Y = value.Y,
				P = value.PressureFactor
			};
		}

		private static System.Windows.Input.StylusPoint ConvertBack(StylusPoint value)
		{
			if (value == null)
			{
				return default(System.Windows.Input.StylusPoint);
			}
			return new System.Windows.Input.StylusPoint(value.X, value.Y, (float)value.P);
		}

		protected override double[] GetParams()
		{
			return new double[3] { X, Y, P };
		}

		protected override void SetParams(params double[] values)
		{
			X = values[0];
			Y = values[1];
			P = values[2];
		}

		public bool Equals(StylusPoint other)
		{
			if (other.X.Equals(X) && other.Y.Equals(Y))
			{
				return other.P.Equals(P);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			StylusPoint stylusPoint = obj as StylusPoint;
			if (stylusPoint != null)
			{
				return Equals(stylusPoint);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return $"{X},{Y},{P}".GetHashCode();
		}
	}
}
