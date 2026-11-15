using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class Point : SplitStringEncodedType<double>, IEquatable<Point>
	{
		public double X { get; set; }

		public double Y { get; set; }

		public static implicit operator Point(System.Windows.Point point)
		{
			return Convert(point);
		}

		public static implicit operator System.Windows.Point(Point point)
		{
			return ConvertBack(point);
		}

		private static Point Convert(System.Windows.Point value)
		{
			return new Point
			{
				X = value.X,
				Y = value.Y
			};
		}

		private static System.Windows.Point ConvertBack(Point value)
		{
			if (value == null)
			{
				return default(System.Windows.Point);
			}
			return new System.Windows.Point(value.X, value.Y);
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

		public bool Equals(Point other)
		{
			if (other.X.Equals(X))
			{
				return other.Y.Equals(Y);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			Point point = obj as Point;
			if (point != null)
			{
				return Equals(point);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return $"{X},{Y}".GetHashCode();
		}
	}
}
