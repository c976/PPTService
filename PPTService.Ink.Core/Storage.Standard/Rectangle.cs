using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class Rectangle : SplitStringEncodedType<double>
	{
		public double Left { get; set; }

		public double Top { get; set; }

		public double Height { get; set; }

		public double Width { get; set; }

		public static implicit operator Rectangle(System.Windows.Rect rect)
		{
			return Convert(rect);
		}

		private static Rectangle Convert(System.Windows.Rect value)
		{
			return new Rectangle
			{
				Left = value.Left,
				Top = value.Top,
				Width = value.Width,
				Height = value.Height
			};
		}

		private static System.Windows.Rect ConvertBack(Rectangle value)
		{
			if (value == null)
			{
				return default(System.Windows.Rect);
			}
			return new System.Windows.Rect(value.Left, value.Top, value.Width, value.Height);
		}

		protected override double[] GetParams()
		{
			return new double[4] { Left, Top, Height, Width };
		}

		protected override void SetParams(params double[] values)
		{
			Left = values[0];
			Top = values[1];
			Height = values[2];
			Width = values[3];
		}

		public static Rectangle Parse(string value)
		{
			Rectangle rectangle = new Rectangle();
			rectangle.TryDecode(value);
			return rectangle;
		}
	}
}
