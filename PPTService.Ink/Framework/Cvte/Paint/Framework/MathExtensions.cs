using System;
using System.Windows;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Framework
{
	public static class MathExtensions
	{
		internal static Rect MoveToRound(this Rect bounds, bool shouldRoundSize = false)
		{
			double x = Math.Round(bounds.X);
			double y = Math.Round(bounds.Y);
			double width = (shouldRoundSize ? Math.Round(bounds.Width) : bounds.Width);
			double height = (shouldRoundSize ? Math.Round(bounds.Height) : bounds.Height);
			return new Rect(x, y, width, height);
		}

		public static Rect Expand(this Rect rect, double left, double top, double right, double bottom)
		{
			return new Rect(rect.Left - left, rect.Top - top, rect.Width + left + right, rect.Height + top + bottom);
		}

		public static Rect Rotate(this Rect bounds, double angle)
		{
			return bounds.RotateAt(angle, bounds.Center());
		}

		internal static Rect RotateAt(this Rect bounds, double angle, Point center)
		{
			Rect empty = Rect.Empty;
			empty.Union(Geometry2D.Rotate(bounds.TopLeft - center, 0.0 - angle) + center);
			empty.Union(Geometry2D.Rotate(bounds.TopRight - center, 0.0 - angle) + center);
			empty.Union(Geometry2D.Rotate(bounds.BottomRight - center, 0.0 - angle) + center);
			empty.Union(Geometry2D.Rotate(bounds.BottomLeft - center, 0.0 - angle) + center);
			return empty;
		}
	}
}
