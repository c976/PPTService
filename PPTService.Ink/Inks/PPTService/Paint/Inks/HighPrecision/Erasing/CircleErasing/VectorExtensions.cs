using System.Windows;

namespace PPTService.Paint.Inks.HighPrecision.Erasing.CircleErasing
{
	internal static class VectorExtensions
	{
		public static double GetCos(this Vector vector)
		{
			return vector.Y / vector.Length;
		}

		public static double GetSin(this Vector vector)
		{
			return vector.X / vector.Length;
		}
	}
}
