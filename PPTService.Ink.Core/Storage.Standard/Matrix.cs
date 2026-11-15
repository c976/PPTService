using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class Matrix : SplitStringEncodedType<double>
	{
		public double M11 { get; set; }

		public double M12 { get; set; }

		public double M21 { get; set; }

		public double M22 { get; set; }

		public double OffsetX { get; set; }

		public double OffsetY { get; set; }

		public static implicit operator Matrix(System.Windows.Media.Matrix matrix)
		{
			return new Matrix
			{
				M11 = matrix.M11,
				M12 = matrix.M12,
				M21 = matrix.M21,
				M22 = matrix.M22,
				OffsetX = matrix.OffsetX,
				OffsetY = matrix.OffsetY
			};
		}

		public static implicit operator System.Windows.Media.Matrix(Matrix matrix)
		{
			if (matrix == null)
			{
				return default(System.Windows.Media.Matrix);
			}
			return new System.Windows.Media.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
		}

		protected override double[] GetParams()
		{
			return new double[6] { M11, M12, M21, M22, OffsetX, OffsetY };
		}

		protected override void SetParams(params double[] values)
		{
			M11 = values[0];
			M12 = values[1];
			M21 = values[2];
			M22 = values[3];
			OffsetX = values[4];
			OffsetY = values[5];
		}
	}
}
