using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class Thickness : SplitStringEncodedType<double>
	{
		public double Left { get; set; }

		public double Top { get; set; }

		public double Right { get; set; }

		public double Bottom { get; set; }

		public static implicit operator Thickness(System.Windows.Thickness thickness)
		{
			return Convert(thickness);
		}

		public static implicit operator System.Windows.Thickness(Thickness thickness)
		{
			return ConvertBack(thickness);
		}

		private static Thickness Convert(System.Windows.Thickness value)
		{
			return new Thickness
			{
				Left = value.Left,
				Right = value.Right,
				Bottom = value.Bottom,
				Top = value.Top
			};
		}

		private static System.Windows.Thickness ConvertBack(Thickness value)
		{
			if (value == null)
			{
				return default(System.Windows.Thickness);
			}
			return new System.Windows.Thickness(value.Left, value.Top, value.Right, value.Bottom);
		}

		protected override double[] GetParams()
		{
			return new double[4] { Left, Top, Right, Bottom };
		}

		protected override void SetParams(params double[] values)
		{
			Left = values[0];
			Top = values[1];
			Right = values[2];
			Bottom = values[3];
		}
	}
}
