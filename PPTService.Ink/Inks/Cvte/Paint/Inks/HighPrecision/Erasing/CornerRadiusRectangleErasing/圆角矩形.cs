using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing
{
	public class 圆角矩形
	{
		private Point[] _所有圆角的圆心;

		private 圆角矩形HitTester _hitTester;

		public double Width { get; }

		public double Height { get; }

		public double Radius { get; }

		public Point Position { get; }

		public double Angle { get; }

		public Point[] 所有圆角的圆心 => _所有圆角的圆心 ?? (_所有圆角的圆心 = 求所有圆角的圆心());

		public 圆角矩形(double width, double height, double radius, Point position, double angle)
		{
			if (width < 0.0)
			{
				throw new ArgumentOutOfRangeException("width", "圆角矩形的宽度必须大于 0。");
			}
			if (height < 0.0)
			{
				throw new ArgumentOutOfRangeException("height", "圆角矩形的高度必须大于 0。");
			}
			if (radius < 0.0)
			{
				throw new ArgumentOutOfRangeException("radius", "圆角矩形的圆角半径必须大于 0。");
			}
			if (width < radius + radius)
			{
				throw new ArgumentOutOfRangeException("width", "圆角矩形的宽度必须大于两倍的圆角半径。");
			}
			if (height < radius + radius)
			{
				throw new ArgumentOutOfRangeException("height", "圆角矩形的高度必须大于两倍的圆角半径。");
			}
			Width = width;
			Height = height;
			Radius = radius;
			Position = position;
			Angle = angle;
		}

		public DebuggablePathInfo 与另一圆角矩形相连(圆角矩形 to)
		{
			DebuggablePathInfo debuggablePathInfo = new DebuggablePathInfo();
			直线或线段 直线或线段2 = (debuggablePathInfo.位置连线 = new 直线或线段(Position, to.Position));
			ValueTuple<Point, Point> valueTuple = 求最远的圆角圆心(直线或线段2);
			ValueTuple<Point, Point> valueTuple2 = to.求最远的圆角圆心(直线或线段2);
			直线或线段 路径圆心连线 = new 直线或线段(valueTuple.Item1, valueTuple2.Item1);
			直线或线段 路径圆心连线2 = new 直线或线段(valueTuple.Item2, valueTuple2.Item2);
			直线或线段 路径圆心连线3 = new 直线或线段(valueTuple.Item1, valueTuple2.Item2);
			直线或线段 路径圆心连线4 = new 直线或线段(valueTuple.Item2, valueTuple2.Item1);
			if (路径圆心连线.AngleBetween(直线或线段2) < 路径圆心连线3.AngleBetween(直线或线段2))
			{
				debuggablePathInfo.路径圆心连线1 = 路径圆心连线;
				debuggablePathInfo.路径圆心连线2 = 路径圆心连线2;
			}
			else
			{
				debuggablePathInfo.路径圆心连线1 = 路径圆心连线3;
				debuggablePathInfo.路径圆心连线2 = 路径圆心连线4;
			}
			直线或线段 路径圆心连线5 = debuggablePathInfo.路径圆心连线1;
			直线或线段 路径圆心连线6 = debuggablePathInfo.路径圆心连线2;
			Point point = 求最远的圆角圆心(to.Position, 路径圆心连线5.Point1, 路径圆心连线6.Point1);
			Point point2 = to.求最远的圆角圆心(Position, 路径圆心连线5.Point2, 路径圆心连线6.Point2);
			直线或线段 直线或线段4 = 路径圆心连线5.求远离后的新直线(直线或线段2, Radius);
			直线或线段 直线或线段5 = 路径圆心连线6.求远离后的新直线(直线或线段2, Radius);
			直线或线段 直线或线段6 = new 直线或线段(point, 路径圆心连线5.Point1);
			直线或线段 直线或线段7 = new 直线或线段(point, 路径圆心连线6.Point1);
			直线或线段 直线或线段8 = new 直线或线段(路径圆心连线5.Point2, point2);
			直线或线段 直线或线段9 = new 直线或线段(point2, 路径圆心连线6.Point2);
			Point point3 = 路径圆心连线5.Point1;
			Point point4 = 直线或线段8.Point1;
			Point point5 = 直线或线段9.Point2;
			Point point6 = 直线或线段7.Point2;
			直线或线段 直线或线段10 = 直线或线段6.求远离后的新直线(Position, Radius);
			直线或线段 直线或线段11 = 直线或线段7.求远离后的新直线(Position, Radius);
			直线或线段 直线或线段12 = 直线或线段8.求远离后的新直线(to.Position, Radius);
			直线或线段 直线或线段13 = 直线或线段9.求远离后的新直线(to.Position, Radius);
			debuggablePathInfo.十二边形顶点 = new Point[12]
			{
				直线或线段10.Point1, 直线或线段10.Point2, 直线或线段4.Point1, 直线或线段4.Point2, 直线或线段12.Point1, 直线或线段12.Point2, 直线或线段13.Point1, 直线或线段13.Point2, 直线或线段5.Point2, 直线或线段5.Point1,
				直线或线段11.Point2, 直线或线段11.Point1
			};
			debuggablePathInfo.补齐十二边形顶点空隙的六个圆的圆心 = new Point[6] { point, point3, point4, point2, point5, point6 };
			if (IsClockwise(point, point3, point4))
			{
				debuggablePathInfo.十二边形顶点 = debuggablePathInfo.十二边形顶点.Reverse().ToArray();
				debuggablePathInfo.补齐十二边形顶点空隙的六个圆的圆心 = debuggablePathInfo.补齐十二边形顶点空隙的六个圆的圆心.Reverse().ToArray();
			}
			debuggablePathInfo.圆角半径 = Radius;
			return debuggablePathInfo;
		}

		private Point 求最远的圆角圆心(Point point, params Point[] 不参与计算的圆角圆心集合)
		{
			return (from p in 所有圆角的圆心
				where !不参与计算的圆角圆心集合.Contains(p)
				select new ValueTuple<double, Point>(Math.Sqrt((p.X - point.X) * (p.X - point.X) + (p.Y - point.Y) * (p.Y - point.Y)), p)).OrderByDescending((ValueTuple<double, Point> x) => x.Item1).First().Item2;
		}

		private ValueTuple<Point, Point> 求最远的圆角圆心(直线或线段 line)
		{
			Point[] array = 所有圆角的圆心;
			ValueTuple<double, Point>[] array2 = array.Select((Point 圆心) => new ValueTuple<double, Point>(line.GetDistance(圆心), 圆心)).OrderByDescending((ValueTuple<double, Point> x) => x.Item1).ToArray();
			if (判定长度近似相等(array2.Select((ValueTuple<double, Point> p) => p.Item1).ToArray()))
			{
				return new ValueTuple<Point, Point>(array[0], array[2]);
			}
			return new ValueTuple<Point, Point>(array2[0].Item2, array2[1].Item2);
		}

		private Point[] 求所有圆角的圆心()
		{
			Point point = 已知未旋转的圆角圆心坐标求旋转后的圆角圆心坐标(Width / 2.0 - Radius, Height / 2.0 - Radius);
			Point point2 = 已知未旋转的圆角圆心坐标求旋转后的圆角圆心坐标((0.0 - Width) / 2.0 + Radius, Height / 2.0 - Radius);
			Point point3 = 已知未旋转的圆角圆心坐标求旋转后的圆角圆心坐标((0.0 - Width) / 2.0 + Radius, (0.0 - Height) / 2.0 + Radius);
			Point point4 = 已知未旋转的圆角圆心坐标求旋转后的圆角圆心坐标(Width / 2.0 - Radius, (0.0 - Height) / 2.0 + Radius);
			return new Point[4] { point, point2, point3, point4 };
		}

		private Point 已知未旋转的圆角圆心坐标求旋转后的圆角圆心坐标(double x, double y)
		{
			double x2 = Position.X;
			double y2 = Position.Y;
			double num = Angle * Math.PI / 180.0;
			return new Point(x * Math.Cos(num) - y * Math.Sin(num) + x2, x * Math.Sin(num) + y * Math.Cos(num) + y2);
		}

		private static bool 判定长度近似相等(params double[] values)
		{
			if (values.Length == 0)
			{
				return true;
			}
			double value = values[0];
			return values.All((double x) => Math.Abs(x - value) < 0.125);
		}

		private static bool IsClockwise(Point pa, Point pb, Point pc)
		{
			return (pa.X - pb.X) * (pc.Y - pb.Y) - (pa.Y - pb.Y) * (pc.X - pb.X) >= 0.0;
		}

		public bool Contains(Point point)
		{
			return GetHitTester().Contains(point);
		}

		public Rect GetBounds()
		{
			return GetHitTester().GetBounds();
		}

		private 圆角矩形HitTester GetHitTester()
		{
			return _hitTester ?? (_hitTester = new 圆角矩形HitTester(this));
		}
	}
}
