using System;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class Point3D : SplitStringEncodedType<double>, IEquatable<Point3D>
	{
		public double X { get; set; }

		public double Y { get; set; }

		public double Z { get; set; }

		public static implicit operator Point3D(System.Windows.Media.Media3D.Point3D point)
		{
			return Convert(point);
		}

		public static implicit operator System.Windows.Media.Media3D.Point3D(Point3D point)
		{
			return ConvertBack(point);
		}

		private static Point3D Convert(System.Windows.Media.Media3D.Point3D value)
		{
			return new Point3D
			{
				X = value.X,
				Y = value.Y,
				Z = value.Z
			};
		}

		private static System.Windows.Media.Media3D.Point3D ConvertBack(Point3D value)
		{
			if (value == null)
			{
				return default(System.Windows.Media.Media3D.Point3D);
			}
			return new System.Windows.Media.Media3D.Point3D(value.X, value.Y, value.Z);
		}

		protected override double[] GetParams()
		{
			return new double[3] { X, Y, Z };
		}

		protected override void SetParams(params double[] values)
		{
			X = values[0];
			Y = values[1];
			Z = values[2];
		}

		public bool Equals(Point3D other)
		{
			if (other.X.Equals(X) && other.Y.Equals(Y))
			{
				return other.Z.Equals(Z);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			Point3D point3D = obj as Point3D;
			if (point3D != null)
			{
				return Equals(point3D);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return $"{X},{Y},{Z}".GetHashCode();
		}
	}
}
