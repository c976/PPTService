using System;
using System.Collections.Generic;
using System.Windows;

namespace PPTService.Ink.Core.Windows.Mathematics.Geometries
{
	public static class Geometry2D
	{
		public static Vector Rotate(Vector vector, double angleInDegree)
		{
			double length = vector.Length;
			if (length.Equals(0.0))
			{
				return vector;
			}
			double angleInDegrees = Vector.AngleBetween(vector, new Vector(1.0, 0.0)) + angleInDegree;
			return length * UnitVecor(angleInDegrees);
		}

		public static double AngleInDegreesToRadians(double angleInDegrees)
		{
			return angleInDegrees / 180.0 * Math.PI;
		}

		public static double AngleInRadiansToDegrees(double angleInDegrees)
		{
			return angleInDegrees / Math.PI * 180.0;
		}

		public static Vector UnitVecor(double angleInDegrees)
		{
			double num = AngleInDegreesToRadians(angleInDegrees);
			return new Vector(Math.Cos(num), 0.0 - Math.Sin(num));
		}

		public static Rect UnionRect(IEnumerable<Point> points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Rect empty = Rect.Empty;
			foreach (Point point in points)
			{
				empty.Union(point);
			}
			return empty;
		}

		public static Rect UnionRect(IEnumerable<Rect> rects)
		{
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			Rect empty = Rect.Empty;
			foreach (Rect rect in rects)
			{
				empty.Union(rect);
			}
			return empty;
		}

		public static Point Midpoint(Point point1, Point point2)
		{
			return new Point((point1.X + point2.X) / 2.0, (point1.Y + point2.Y) / 2.0);
		}

		public static Point Center(this Rect rect)
		{
			return Midpoint(rect.TopLeft, rect.BottomRight);
		}

		public static Rect FillInto(this Rect innerRect, Rect outerRect, RectStretch stretch = RectStretch.Uniform)
		{
			if (!Finite.IsFinite(innerRect.Size))
			{
				throw new ArgumentException("innerRect 包含无效的宽高值", "innerRect");
			}
			if (innerRect.Width.Equals(0.0) && innerRect.Height.Equals(0.0))
			{
				throw new ArgumentException("innerRect 宽高不能都为0", "innerRect");
			}
			if (!Finite.IsFinite(outerRect))
			{
				throw new ArgumentException("outerRect 包含无效值", "outerRect");
			}
			switch (stretch)
			{
			case RectStretch.Uniform:
				return innerRect.FillIntoByUniform(outerRect);
			case RectStretch.UniformToFill:
				return innerRect.FillIntoByUniformToFill(outerRect);
			default:
				throw new NotImplementedException();
			}
		}

		public static Rect AlignTo(this Rect rect, Rect alignedRect, RectHorizontalAlignment horizontalAlignment)
		{
			if (!Finite.IsFinite(rect.Size))
			{
				throw new ArgumentException("innerRect 包含无效值", "rect");
			}
			if (!Finite.IsFinite(alignedRect))
			{
				throw new ArgumentException("outerRect 包含无效值", "alignedRect");
			}
			switch (horizontalAlignment)
			{
			case RectHorizontalAlignment.Left:
				rect.X = alignedRect.X;
				return rect;
			case RectHorizontalAlignment.Center:
				rect.X = alignedRect.X + (alignedRect.Width - rect.Width) / 2.0;
				return rect;
			case RectHorizontalAlignment.Right:
				rect.X = alignedRect.X + (alignedRect.Width - rect.Width);
				return rect;
			default:
				throw new NotImplementedException();
			}
		}

		public static Rect AlignTo(this Rect rect, Rect alignedRect, RectVerticalAlignment verticalAlignment)
		{
			if (!Finite.IsFinite(rect.Size))
			{
				throw new ArgumentException("innerRect 包含无效值", "rect");
			}
			if (!Finite.IsFinite(alignedRect))
			{
				throw new ArgumentException("outerRect 包含无效值", "alignedRect");
			}
			switch (verticalAlignment)
			{
			case RectVerticalAlignment.Top:
				rect.Y = alignedRect.Y;
				return rect;
			case RectVerticalAlignment.Center:
				rect.Y = alignedRect.Y + (alignedRect.Height - rect.Height) / 2.0;
				return rect;
			case RectVerticalAlignment.Bottom:
				rect.Y = alignedRect.Y + (alignedRect.Height - rect.Height);
				return rect;
			default:
				throw new NotImplementedException();
			}
		}

		public static Rect AlignTo(this Rect rect, Rect alignedRect, AlignmentSettings alignment)
		{
			if (!Finite.IsFinite(rect.Size))
			{
				throw new ArgumentException("innerRect 包含无效值", "rect");
			}
			if (!Finite.IsFinite(alignedRect))
			{
				throw new ArgumentException("outerRect 包含无效值", "alignedRect");
			}
			return rect.AlignTo(alignedRect, alignment.HorizontalAlignment).AlignTo(alignedRect, alignment.VerticalAlignment);
		}

		public static Size UniformSetWidth(this Size size, double width)
		{
			if (!Finite.IsFinite(size))
			{
				throw new ArgumentException("size值无效。", "size");
			}
			if (!Finite.IsFinite(width))
			{
				throw new ArgumentException("width值无效。", "width");
			}
			double height = width / size.Width * size.Height;
			size.Width = width;
			size.Height = height;
			return size;
		}

		public static Rect UniformSetWidth(this Rect rect, double width)
		{
			if (!Finite.IsFinite(rect.Size))
			{
				throw new ArgumentException("rect值无效。", "rect");
			}
			if (!Finite.IsFinite(width))
			{
				throw new ArgumentException("width值无效。", "width");
			}
			double height = width / rect.Width * rect.Height;
			rect.Width = width;
			rect.Height = height;
			return rect;
		}

		public static Size UniformSetHeight(this Size size, double height)
		{
			if (!Finite.IsFinite(size))
			{
				throw new ArgumentException("size值无效。", "size");
			}
			if (!Finite.IsFinite(height))
			{
				throw new ArgumentException("height值无效。", "height");
			}
			double width = height / size.Height * size.Width;
			size.Width = width;
			size.Height = height;
			return size;
		}

		public static Rect UniformSetHeight(this Rect rect, double height)
		{
			if (!Finite.IsFinite(rect.Size))
			{
				throw new ArgumentException("rect值无效。", "rect");
			}
			if (!Finite.IsFinite(height))
			{
				throw new ArgumentException("height值无效。", "height");
			}
			double width = height / rect.Height * rect.Width;
			rect.Width = width;
			rect.Height = height;
			return rect;
		}

		private static Rect FillIntoByUniform(this Rect innerRect, Rect outerRect)
		{
			if (innerRect.Width / innerRect.Height > outerRect.Width / outerRect.Height)
			{
				innerRect.Height = innerRect.Height / innerRect.Width * outerRect.Width;
				innerRect.Width = outerRect.Width;
			}
			else
			{
				innerRect.Width = innerRect.Width / innerRect.Height * outerRect.Height;
				innerRect.Height = outerRect.Height;
			}
			innerRect.X = outerRect.X + outerRect.Width / 2.0 - innerRect.Width / 2.0;
			innerRect.Y = outerRect.Y + outerRect.Height / 2.0 - innerRect.Height / 2.0;
			return innerRect;
		}

		private static Rect FillIntoByUniformToFill(this Rect innerRect, Rect outerRect)
		{
			if (innerRect.Width / innerRect.Height > outerRect.Width / outerRect.Height)
			{
				innerRect.Width = innerRect.Width / innerRect.Height * outerRect.Height;
				innerRect.Height = outerRect.Height;
			}
			else
			{
				innerRect.Height = innerRect.Height / innerRect.Width * outerRect.Width;
				innerRect.Width = outerRect.Width;
			}
			innerRect.X = outerRect.X + outerRect.Width / 2.0 - innerRect.Width / 2.0;
			innerRect.Y = outerRect.Y + outerRect.Height / 2.0 - innerRect.Height / 2.0;
			return innerRect;
		}
	}
}
