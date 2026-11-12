using System;
using System.Windows;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing
{
	internal class 圆角矩形HitTester
	{
		public 旋转矩形 矩形1 { get; }

		public 旋转矩形 矩形2 { get; }

		public Point[] 所有圆角的圆心 { get; }

		public double 圆角半径 { get; }

		public Point Position { get; }

		public double 旋转角度弧度 { get; }

		public 圆角矩形HitTester(圆角矩形 圆角矩形)
		{
			double width = 圆角矩形.Width;
			double height = 圆角矩形.Height;
			double radius = 圆角矩形.Radius;
			Position = 圆角矩形.Position;
			旋转角度弧度 = 圆角矩形.Angle * Math.PI / 180.0;
			double num = radius - width / 2.0;
			double num2 = (0.0 - height) / 2.0;
			double num3 = width / 2.0 - radius;
			double num4 = num2;
			double cx = num;
			double num5 = height / 2.0;
			double dx = num3;
			double dy = num5;
			矩形1 = 创建旋转之后的矩形(num, num2, num3, num4, cx, num5, dx, dy);
			double num6 = (0.0 - width) / 2.0;
			double num7 = 0.0 - radius - height / 2.0;
			double num8 = width / 2.0;
			double num9 = num7;
			double cx2 = num6;
			double num10 = height / 2.0 - radius;
			double dx2 = num8;
			double dy2 = num10;
			矩形2 = 创建旋转之后的矩形(num6, num7, num8, num9, cx2, num10, dx2, dy2);
			所有圆角的圆心 = 圆角矩形.所有圆角的圆心;
			圆角半径 = 圆角矩形.Radius;
		}

		public bool Contains(Point point)
		{
			if (!矩形1.Contains(point) && !矩形2.Contains(point))
			{
				return Geometry2DHelper.ContainsInCircles(point, 所有圆角的圆心, 圆角半径);
			}
			return true;
		}

		private 旋转矩形 创建旋转之后的矩形(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
		{
			Point a = 已知未旋转的相对圆角矩形中心点的坐标求旋转后的相对于零点的坐标(ax, ay);
			Point b = 已知未旋转的相对圆角矩形中心点的坐标求旋转后的相对于零点的坐标(bx, by);
			Point c = 已知未旋转的相对圆角矩形中心点的坐标求旋转后的相对于零点的坐标(cx, cy);
			Point d = 已知未旋转的相对圆角矩形中心点的坐标求旋转后的相对于零点的坐标(dx, dy);
			return new 旋转矩形(a, b, c, d);
		}

		private Point 已知未旋转的相对圆角矩形中心点的坐标求旋转后的相对于零点的坐标(double x, double y)
		{
			double x2 = Position.X;
			double y2 = Position.Y;
			double num = 旋转角度弧度;
			return new Point(x * Math.Cos(num) - y * Math.Sin(num) + x2, x * Math.Sin(num) + y * Math.Cos(num) + y2);
		}

		public Rect GetBounds()
		{
			Rect result = new Rect(矩形1.A, new Size(0.0, 0.0));
			Point[] array = new Point[8] { 矩形1.A, 矩形1.B, 矩形1.C, 矩形1.D, 矩形2.A, 矩形2.B, 矩形2.C, 矩形2.D };
			foreach (Point point in array)
			{
				result.Union(point);
			}
			return result;
		}
	}
}
