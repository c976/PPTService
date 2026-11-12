using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Inks
{
	internal static class MathMatrix
	{
		public static Matrix GetDeltaTransform(Rect newBounds, Rect oldBounds)
		{
			Matrix result = default(Matrix);
			double num = newBounds.Width / oldBounds.Width;
			if (double.IsNaN(num))
			{
				num = 1.0;
			}
			double num2 = newBounds.Height / oldBounds.Height;
			if (double.IsNaN(num2))
			{
				num2 = 1.0;
			}
			result.Scale(num, num2);
			result.Translate(newBounds.X - oldBounds.X * num, newBounds.Y - oldBounds.Y * num2);
			return result;
		}
	}
}
