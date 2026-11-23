using System;
using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows
{
	internal static class MathExtensions
	{
		public static Vector GetRotateVector(this Vector vector, double angleInDegrees)
		{
			Matrix identity = Matrix.Identity;
			identity.Rotate(angleInDegrees);
			return Vector.Multiply(vector, identity);
		}

		internal static Rect MoveToRound(this Rect bounds, bool shouldRoundSize = false)
		{
			double x = Math.Round(bounds.X);
			double y = Math.Round(bounds.Y);
			double width = (shouldRoundSize ? Math.Round(bounds.Width) : bounds.Width);
			double height = (shouldRoundSize ? Math.Round(bounds.Height) : bounds.Height);
			return new Rect(x, y, width, height);
		}
	}
}
