using System;
using System.Collections.ObjectModel;

namespace Cvte.Mathematics
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
				throw new ArgumentException("a和b不能全部为0。");
			}
			A = a;
			B = b;
			C = c;
			Solve();
		}

		private unsafe void Solve()
		{
			void* ptr = stackalloc byte[32];
			if (Nearly.Equals(A, 0.0))
			{
				Roots = new ReadOnlyCollection<double>(new double[1] { (0.0 - C) / B });
				return;
			}
			*(double*)ptr = B * B - 4.0 * A * C;
			if (Nearly.Equals(*(double*)ptr, 0.0))
			{
				((double*)ptr)[1] = (0.0 - B) / (2.0 * A);
				Roots = new ReadOnlyCollection<double>(new double[1] { ((double*)ptr)[1] });
			}
			else if (*(double*)ptr > 0.0)
			{
				((double*)ptr)[2] = (0.0 - B + Math.Sqrt(*(double*)ptr)) / (2.0 * A);
				((double*)ptr)[3] = (0.0 - B - Math.Sqrt(*(double*)ptr)) / (2.0 * A);
				double[] list = ((!(((double*)ptr)[2] < ((double*)ptr)[3])) ? new double[2]
				{
					((double*)ptr)[3],
					((double*)ptr)[2]
				} : new double[2]
				{
					((double*)ptr)[2],
					((double*)ptr)[3]
				});
				Roots = new ReadOnlyCollection<double>(list);
			}
			else
			{
				Roots = new ReadOnlyCollection<double>(new double[0]);
			}
		}
	}
}
