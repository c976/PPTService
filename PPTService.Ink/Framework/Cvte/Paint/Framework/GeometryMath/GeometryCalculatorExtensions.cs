using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Framework.GeometryMath
{
	public static class GeometryCalculatorExtensions
	{
		public static Point Rotate(this Point point, Point origin, double angle)
		{
			return (point - origin).GetRotateVector(angle) + origin;
		}

		public static Vector GetRotateVector(this Vector v, double angle)
		{
			Matrix identity = Matrix.Identity;
			identity.Rotate(angle);
			return Vector.Multiply(v, identity);
		}
	}
}
