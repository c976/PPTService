using System;
using System.ComponentModel;
using System.Windows;
using PPTService.Paint.Framework.GeometryMath;

namespace PPTService.Paint.Framework
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("请使用 GeometryCalculatorExtensions 类代替")]
	public static class ShapeElementHelper
	{
		[Obsolete("请使用 GeometryCalculatorExtensions.Rotate 代替")]
		public static Point Rotate(this Point point, Point origin, double angle)
		{
			return GeometryCalculatorExtensions.Rotate(point, origin, angle);
		}

		[Obsolete("请使用 GeometryCalculatorExtensions.GetRotateVector 代替")]
		public static Vector GetRotateVector(this Vector v, double angle)
		{
			return GeometryCalculatorExtensions.GetRotateVector(v, angle);
		}
	}
}
