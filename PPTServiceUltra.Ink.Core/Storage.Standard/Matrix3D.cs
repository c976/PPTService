using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class Matrix3D : SplitStringEncodedType<double>
	{
		public double M11 { get; set; }

		public double M12 { get; set; }

		public double M13 { get; set; }

		public double M14 { get; set; }

		public double M21 { get; set; }

		public double M22 { get; set; }

		public double M23 { get; set; }

		public double M24 { get; set; }

		public double M31 { get; set; }

		public double M32 { get; set; }

		public double M33 { get; set; }

		public double M34 { get; set; }

		public double M44 { get; set; }

		public double OffsetX { get; set; }

		public double OffsetY { get; set; }

		public double OffsetZ { get; set; }

		public static implicit operator Matrix3D(System.Windows.Media.Media3D.Matrix3D matrix)
		{
			return new Matrix3D
			{
				M11 = matrix.M11,
				M12 = matrix.M12,
				M13 = matrix.M13,
				M14 = matrix.M14,
				M21 = matrix.M21,
				M22 = matrix.M22,
				M23 = matrix.M23,
				M24 = matrix.M24,
				M31 = matrix.M31,
				M32 = matrix.M32,
				M33 = matrix.M33,
				M34 = matrix.M34,
				M44 = matrix.M44,
				OffsetX = matrix.OffsetX,
				OffsetY = matrix.OffsetY,
				OffsetZ = matrix.OffsetZ
			};
		}

		public static implicit operator System.Windows.Media.Media3D.Matrix3D(Matrix3D matrix)
		{
			if (matrix == null)
			{
				return default(System.Windows.Media.Media3D.Matrix3D);
			}
			return new System.Windows.Media.Media3D.Matrix3D(matrix.M11, matrix.M12, matrix.M13, matrix.M14, matrix.M21, matrix.M22, matrix.M23, matrix.M24, matrix.M31, matrix.M32, matrix.M33, matrix.M34, matrix.OffsetX, matrix.OffsetY, matrix.OffsetZ, matrix.M44);
		}

		protected override double[] GetParams()
		{
			return new double[16]
			{
				M11, M12, M13, M14, M21, M22, M23, M24, M31, M32,
				M33, M34, OffsetX, OffsetY, OffsetZ, M44
			};
		}

		protected override void SetParams(params double[] values)
		{
			M11 = values[0];
			M12 = values[1];
			M13 = values[2];
			M14 = values[3];
			M21 = values[4];
			M22 = values[5];
			M23 = values[6];
			M24 = values[7];
			M31 = values[8];
			M32 = values[9];
			M33 = values[10];
			M34 = values[11];
			OffsetX = values[12];
			OffsetY = values[13];
			OffsetZ = values[14];
			M44 = values[15];
		}
	}
}
