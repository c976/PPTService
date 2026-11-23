using System;
using System.Collections.ObjectModel;

namespace PPTServiceUltra.Ink.Core.Windows.Mathematics
{
	public class QuadraticEquation
	{
		public double A { get; private set; }

		public double B { get; private set; }

		public double C { get; private set; }

		public ReadOnlyCollection<double> Roots { get; private set; }

		public QuadraticEquation(double a, double b, double c)
		{
			if (!Finite.IsFinite(a))
			{
				throw new ArgumentException("a 数值无效。", "a");
			}
			if (!Finite.IsFinite(b))
			{
				throw new ArgumentException("b 数值无效。", "b");
			}
			if (!Finite.IsFinite(c))
			{
				throw new ArgumentException("c 数值无效。", "c");
			}
			if (Nearly.Equals(a, 0.0) && Nearly.Equals(b, 0.0))
			{
				throw new ArgumentException("a和b不能全部为0。", "a");
			}
			A = a;
			B = b;
			C = c;
			Solve();
		}

		private void Solve()
		{
			if (Nearly.Equals(A, 0.0))
			{
				Roots = new ReadOnlyCollection<double>(new double[1] { (0.0 - C) / B });
				return;
			}
			double num = B * B - 4.0 * A * C;
			if (Nearly.Equals(num, 0.0))
			{
				double num2 = (0.0 - B) / (2.0 * A);
				Roots = new ReadOnlyCollection<double>(new double[1] { num2 });
			}
			else if (num > 0.0)
			{
				double num3 = (0.0 - B + Math.Sqrt(num)) / (2.0 * A);
				double num4 = (0.0 - B - Math.Sqrt(num)) / (2.0 * A);
				double[] list = ((!(num3 < num4)) ? new double[2] { num4, num3 } : new double[2] { num3, num4 });
				Roots = new ReadOnlyCollection<double>(list);
			}
			else
			{
				Roots = new ReadOnlyCollection<double>(new double[0]);
			}
		}
	}
}
