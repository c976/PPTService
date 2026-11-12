using System.Windows;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CircleErasing
{
	internal class 圆
	{
		public Point 圆心 { get; }

		public double 半径 { get; }

		public Rect Bounds
		{
			get
			{
				Point point = 圆心;
				double num = 半径;
				return new Rect(new Point(point.X - num, point.Y - num), new Size(num * 2.0, num * 2.0));
			}
		}

		public 圆(Point 圆心, double 半径)
		{
			this.圆心 = 圆心;
			this.半径 = 半径;
		}

		public bool Contains(Point point)
		{
			double num = 半径 * 半径;
			return (point - 圆心).LengthSquared <= num;
		}

		public 两圆轨迹 求两圆轨迹线(圆 另一个圆)
		{
			Point point = 圆心;
			Point point2 = 另一个圆.圆心;
			double num = 半径;
			double num2 = 另一个圆.半径;
			Vector vector = point - point2;
			double cos = vector.GetCos();
			double sin = vector.GetSin();
			double x = point.X + num * cos;
			double y = point.Y - num * sin;
			double x2 = point.X - num * cos;
			double y2 = point.Y + num * sin;
			double x3 = point2.X - num2 * cos;
			double y3 = point2.Y + num2 * sin;
			double x4 = point2.X + num2 * cos;
			double y4 = point2.Y - num2 * sin;
			Point point3 = new Point(x, y);
			Point point4 = new Point(x2, y2);
			Point point5 = new Point(x3, y3);
			Point point6 = new Point(x4, y4);
			线段 线段2 = new 线段(point3, point6);
			线段 线段3 = new 线段(point4, point5);
			return new 两圆轨迹(this, 另一个圆, 线段2, 线段3, new Point[4] { point3, point4, point5, point6 });
		}
	}
}
