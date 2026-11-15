using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class Size : SplitStringEncodedType<double>
	{
		public double Width { get; set; }

		public double Height { get; set; }

		public static implicit operator Size(System.Windows.Size size)
		{
			return Convert(size);
		}

		public static implicit operator System.Windows.Size(Size size)
		{
			return ConvertBack(size);
		}

		private static Size Convert(System.Windows.Size value)
		{
			return new Size
			{
				Width = value.Width,
				Height = value.Height
			};
		}

		private static System.Windows.Size ConvertBack(Size value)
		{
			if (value == null)
			{
				return default(System.Windows.Size);
			}
			return new System.Windows.Size(value.Width, value.Height);
		}

		protected override double[] GetParams()
		{
			return new double[2] { Width, Height };
		}

		protected override void SetParams(params double[] values)
		{
			Width = values[0];
			Height = values[1];
		}
	}
}
