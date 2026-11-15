using System;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class Size3D : SplitStringEncodedType<double>, IEquatable<Size3D>
	{
		public double X { get; set; }

		public double Y { get; set; }

		public double Z { get; set; }

		public static implicit operator Size3D(System.Windows.Media.Media3D.Size3D size)
		{
			return Convert(size);
		}

		public static implicit operator System.Windows.Media.Media3D.Size3D(Size3D size)
		{
			return ConvertBack(size);
		}

		private static Size3D Convert(System.Windows.Media.Media3D.Size3D value)
		{
			return new Size3D
			{
				X = value.X,
				Y = value.Y,
				Z = value.Z
			};
		}

		private static System.Windows.Media.Media3D.Size3D ConvertBack(Size3D value)
		{
			if (value == null)
			{
				return default(System.Windows.Media.Media3D.Size3D);
			}
			return new System.Windows.Media.Media3D.Size3D(value.X, value.Y, value.Z);
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

		public bool Equals(Size3D other)
		{
			if (other.X.Equals(X) && other.Y.Equals(Y))
			{
				return other.Z.Equals(Z);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			Size3D size3D = obj as Size3D;
			if (size3D != null)
			{
				return Equals(size3D);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return $"{X},{Y},{Z}".GetHashCode();
		}
	}
}
