using System.Windows;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CircleErasing
{
	internal static class Geometry2DHelper
	{
		public static bool 求点是否在任意凸多边形内部算法(Point 点, Point[] 多边形的顶点集)
		{
			bool? flag = null;
			for (int i = 0; i < 多边形的顶点集.Length; i++)
			{
				int num = i + 1;
				if (i == 多边形的顶点集.Length - 1)
				{
					num = 0;
				}
				Point point = 多边形的顶点集[i];
				Vector vector = 多边形的顶点集[num] - point;
				Vector vector2 = 点 - point;
				double num2 = Vector.CrossProduct(vector, vector2);
				if (num2 > 0.0)
				{
					if (!flag.HasValue)
					{
						flag = true;
					}
					else if (flag == false)
					{
						return false;
					}
				}
				else if (num2 < 0.0)
				{
					if (!flag.HasValue)
					{
						flag = false;
					}
					else if (flag == true)
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
