using System.Windows;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing
{
	public class 两个圆角矩形相连后所得的包含途径范围的图形信息
	{
		public Point[] 十二边形顶点 { get; internal set; }

		public Point[] 补齐十二边形顶点空隙的六个圆的圆心 { get; internal set; }

		public double 圆角半径 { get; internal set; }

		public bool Contains(Point point)
		{
			if (!ContainsInPolygon(point))
			{
				return ContainsInCircles(point);
			}
			return true;
		}

		public Rect GetBounds()
		{
			Rect result = new Rect(十二边形顶点[0], new Size(0.0, 0.0));
			for (int i = 1; i < 十二边形顶点.Length; i++)
			{
				Point point = 十二边形顶点[i];
				result.Union(point);
			}
			return result;
		}

		private bool ContainsInCircles(Point point)
		{
			return Geometry2DHelper.ContainsInCircles(point, 补齐十二边形顶点空隙的六个圆的圆心, 圆角半径);
		}

		private bool ContainsInPolygon(Point point)
		{
			return Geometry2DHelper.ContainsInPolygon(point, 十二边形顶点);
		}
	}
}
