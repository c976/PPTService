using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTService.Ink.Core.Mathematics
{
	[Obsolete("相关类型无法实现跨平台，因此请使用 Cvte.Windows 中的同名类型。")]
	public static class IntersectionCompute
	{
		
		public static Point[] LineAndCircle(Line line, Point center, double radius)
		{
			double a = line.A;
			double b = line.B;
			double c = line.C;
			if (_003CLineAndCircle_003Eg__AreZero_007C0_0(a) && _003CLineAndCircle_003Eg__AreZero_007C0_0(b))
			{
				return null;
			}
			if (_003CLineAndCircle_003Eg__AreZero_007C0_0(a) && !_003CLineAndCircle_003Eg__AreZero_007C0_0(b))
			{
				double num = (0.0 - c) / b;
				double x = center.X + Math.Sqrt(Math.Pow(radius, 2.0) - Math.Pow(num - center.Y, 2.0));
				double x2 = center.X - Math.Sqrt(Math.Pow(radius, 2.0) - Math.Pow(num - center.Y, 2.0));
				return new Point[2]
				{
					new Point(x, num),
					new Point(x2, num)
				};
			}
			if (_003CLineAndCircle_003Eg__AreZero_007C0_0(b) && !_003CLineAndCircle_003Eg__AreZero_007C0_0(a))
			{
				double num2 = (0.0 - c) / a;
				double y = center.Y + Math.Sqrt(Math.Pow(radius, 2.0) - Math.Pow(num2 - center.X, 2.0));
				double y2 = center.Y - Math.Sqrt(Math.Pow(radius, 2.0) - Math.Pow(num2 - center.X, 2.0));
				return new Point[2]
				{
					new Point(num2, y),
					new Point(num2, y2)
				};
			}
			double a2 = 1.0 + a * a / (b * b);
			double b2 = 2.0 * a * c / (b * b) + 2.0 * a * center.Y / b - 2.0 * center.X;
			double c2 = c * c / (b * b) + 2.0 * c * center.Y / b - radius * radius + center.X * center.X + center.Y * center.Y;
			ReadOnlyCollection<double> roots = new QuadraticEquation(a2, b2, c2).Roots;
			Point point = new Point(roots[0], (0.0 - c - a * roots[0]) / b);
			Point point2 = new Point(roots[1], (0.0 - c - a * roots[1]) / b);
			return new Point[2] { point, point2 };
		}

		[CompilerGenerated]
		internal static bool _003CLineAndCircle_003Eg__AreZero_007C0_0(double n)
		{
			return Nearly.Equals(n, 0.0);
		}
	}
}
