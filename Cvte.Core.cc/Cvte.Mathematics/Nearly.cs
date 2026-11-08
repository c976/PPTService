using System;
using System.Windows;

namespace Cvte.Mathematics
{
	public static class Nearly
	{
		internal const double AlmostZero = 1E-05;

		public static bool Equals(Point p1, Point p2, double tolerance = 1E-05)
		{
			int num = (Equals(p1.X, p2.X, tolerance) ? 1 : 0);
			while (true)
			{
				int num2;
				if (0 == 0)
				{
					if (num != 0)
					{
						num = (Equals(p1.Y, p2.Y, tolerance) ? 1 : 0);
						goto IL_001d;
					}
					num = 0;
				}
				else if (num2 != 0)
				{
					goto IL_001d;
				}
				if (num != 0)
				{
					continue;
				}
				if (num == 0)
				{
					break;
				}
				goto IL_001d;
				IL_001d:
				return (byte)num != 0;
			}
			return (byte)num != 0;
		}

		public static bool Equals(Line line1, Line line2, double tolerance = 1E-05)
		{
			double d = line1.A;
			double d2 = line2.A;
			if (6 == 0)
			{
				goto IL_0036;
			}
			double tolerance2 = tolerance;
			if (0 == 0)
			{
				if (false)
				{
					goto IL_0037;
				}
				if (!Equals(d, d2, tolerance2))
				{
					goto IL_003d;
				}
				d = line1.B;
				if (false)
				{
					goto IL_002f;
				}
				d2 = line2.B;
				tolerance2 = tolerance;
			}
			int num = (Equals(d, d2, tolerance2) ? 1 : 0);
			if (0 == 0)
			{
				if (num != 0)
				{
					d = line1.C;
					goto IL_002f;
				}
				goto IL_003d;
			}
			goto IL_003e;
			IL_0036:
			tolerance2 = tolerance;
			goto IL_0037;
			IL_003d:
			num = 0;
			goto IL_003e;
			IL_003e:
			return (byte)num != 0;
			IL_0037:
			return Equals(d, d2, tolerance2);
			IL_002f:
			d2 = line2.C;
			goto IL_0036;
		}

		public static bool Equals(Vector v1, Vector v2, double tolerance = 1E-05)
		{
			int num = (Equals(v1.X, v2.X, tolerance) ? 1 : 0);
			while (true)
			{
				int num2;
				if (0 == 0)
				{
					if (num != 0)
					{
						num = (Equals(v1.Y, v2.Y, tolerance) ? 1 : 0);
						goto IL_001d;
					}
					num = 0;
				}
				else if (num2 != 0)
				{
					goto IL_001d;
				}
				if (num != 0)
				{
					continue;
				}
				if (num == 0)
				{
					break;
				}
				goto IL_001d;
				IL_001d:
				return (byte)num != 0;
			}
			return (byte)num != 0;
		}

		public static bool Equals(Rect r1, Rect r2, double tolerance = 1E-05)
		{
			int num;
			if (Equals(r1.Width, r2.Width, tolerance))
			{
				double d = r1.Height;
				while (true)
				{
					double d2 = r2.Height;
					if (8 != 0)
					{
						if (false)
						{
							goto IL_005e;
						}
						if (!Equals(d, d2, tolerance))
						{
							break;
						}
						if (false)
						{
							goto IL_004d;
						}
						d = r1.X;
						d2 = r2.X;
					}
					if (!Equals(d, d2, tolerance))
					{
						break;
					}
					goto IL_004d;
					IL_005e:
					num = (Equals(d, d2, tolerance) ? 1 : 0);
					goto IL_0064;
					IL_004d:
					d = r1.Y;
					if (false)
					{
						continue;
					}
					d2 = r2.Y;
					goto IL_005e;
				}
			}
			num = 0;
			if (num == 0)
			{
				return (byte)num != 0;
			}
			goto IL_0064;
			IL_0064:
			return (byte)num != 0;
		}

		public static bool Equals(double d1, double d2, double tolerance = 1E-05)
		{
			return Math.Abs(d1 - d2) < tolerance;
		}

		public static bool Equals(Size size1, Size size2, double tolerance = 1E-05)
		{
			int num = (Equals(size1.Width, size2.Width, tolerance) ? 1 : 0);
			while (true)
			{
				int num2;
				if (0 == 0)
				{
					if (num != 0)
					{
						num = (Equals(size1.Height, size2.Height, tolerance) ? 1 : 0);
						goto IL_001d;
					}
					num = 0;
				}
				else if (num2 != 0)
				{
					goto IL_001d;
				}
				if (num != 0)
				{
					continue;
				}
				if (num == 0)
				{
					break;
				}
				goto IL_001d;
				IL_001d:
				return (byte)num != 0;
			}
			return (byte)num != 0;
		}

		[Obsolete("Nearly doesn't support comparision of these types.", true)]
		public new static bool Equals(object obj1, object obj2)
		{
			throw new NotSupportedException($"Nearly doesn't support comparision between {obj1?.GetType()} and {obj2?.GetType()}");
		}
	}
}
