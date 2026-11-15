using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class Rect : SplitStringEncodedType<double>
	{
		public double Left { get; set; }

		public double Top { get; set; }

		public double Width { get; set; }

		public double Height { get; set; }

		public static implicit operator Rect(System.Windows.Rect rect)
		{
			return Convert(rect);
		}

		public static implicit operator System.Windows.Rect(Rect rect)
		{
			return ConvertBack(rect);
		}

		private static Rect Convert(System.Windows.Rect value)
		{
			return new Rect
			{
				Left = value.Left,
				Top = value.Top,
				Width = value.Width,
				Height = value.Height
			};
		}

		private static System.Windows.Rect ConvertBack(Rect value)
		{
			if (value == null)
			{
				return default(System.Windows.Rect);
			}
			return new System.Windows.Rect(value.Left, value.Top, value.Width, value.Height);
		}

		protected override double[] GetParams()
		{
			return new double[4] { Left, Top, Width, Height };
		}

		protected override void SetParams(params double[] values)
		{
			Left = values[0];
			Top = values[1];
			Width = values[2];
			Height = values[3];
		}
	}
}
