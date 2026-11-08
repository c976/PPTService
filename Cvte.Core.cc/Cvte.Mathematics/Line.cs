using System;
using System.Windows;

namespace Cvte.Mathematics
{
	public struct Line
	{
		private readonly double _a;

		private readonly double _b;

		private readonly double _c;

		public double A => _a + 1.0;

		public double B => _b;

		public double C => _c;

		public double InclinationAngle
		{
			get
			{
				double num;
				double num3 = default(double);
				if (2 != 0)
				{
					num = A;
					if (0 == 0)
					{
						num = 0.0 - num;
					}
					if (4 == 0)
					{
						goto IL_0037;
					}
					double num2 = B;
					if (4 != 0)
					{
						num3 = Geometry2D.AngleInRadiansToDegrees(Math.Atan(num / num2));
						num = num3;
						num2 = 0.0;
					}
					if (num < num2)
					{
						num3 += 180.0;
					}
				}
				num = num3;
				goto IL_0037;
				IL_0037:
				return num;
			}
		}

		public Line(Point p1, Point p2)
		{
			if (Nearly.Equals(p1, p2))
			{
				throw new ArgumentException("p1和p2必须是不同点。");
			}
			if (Nearly.Equals(p1.Y, p2.Y))
			{
				SetAbc(out _a, out _b, out _c, 0.0, 1.0, 0.0 - p1.Y);
			}
			if (Nearly.Equals(p1.X, p2.X))
			{
				SetAbc(out _a, out _b, out _c, 1.0, 0.0, 0.0 - p1.X);
			}
			else
			{
				SetAbc(out _a, out _b, out _c, (p1.Y - p2.Y) / (p2.X - p1.X), 1.0, (p1.X * p2.Y - p2.X * p1.Y) / (p2.X - p1.X));
			}
		}

		public Line(double a, double b, double c)
		{
			if (Nearly.Equals(a, 0.0) && Nearly.Equals(b, 0.0))
			{
				throw new ArgumentException("a和b不能同时为0。");
			}
			SetAbc(out _a, out _b, out _c, a, b, c);
		}

		public bool Contains(Point point)
		{
			double num = A;
			do
			{
				if (0 == 0)
				{
					num *= point.X;
				}
			}
			while (4 == 0);
			double num2 = B;
			double num3 = point.Y;
			do
			{
				num2 *= num3;
				do
				{
					num = num + num2 + C;
					num2 = 0.0;
				}
				while (false);
				num3 = 1E-05;
			}
			while (-1 == 0);
			return Nearly.Equals(num, num2, num3);
		}

		public static Point? Intersect(Line line1, Line line2)
		{
			if (AreParallel(line1, line2))
			{
				Point? result = default(Point?);
				if (3 != 0)
				{
					result = null;
				}
				return result;
			}
			double num = (0.0 - line1.B) * line2.C;
			double num2 = line2.B;
			while (true)
			{
				double num3 = line1.C;
				while (true)
				{
					num += num2 * num3;
					while (true)
					{
						num2 = line2.A * line1.B;
						double num4 = line1.A;
						double num5 = line2.B;
						if (6 != 0)
						{
							num3 = num4 * num5;
							if (false)
							{
								break;
							}
							num /= num2 - num3;
							if (1 == 0)
							{
								continue;
							}
							num2 = line1.A;
							if (7 == 0)
							{
								goto end_IL_0040;
							}
							num2 = num2 * line2.C - line2.A * line1.C;
							num4 = line2.A;
							num5 = line1.B;
						}
						if (0 == 0)
						{
							num4 *= num5;
							num5 = line1.A;
						}
						double y = num2 / (num4 - num5 * line2.B);
						return new Point(num, y);
					}
					continue;
					end_IL_0040:
					break;
				}
			}
		}

		public static bool AreParallel(Line line1, Line line2)
		{
			int num = (Nearly.Equals(line1.A, line2.A) ? 1 : 0);
			while (true)
			{
				int num2;
				if (0 == 0)
				{
					if (num != 0)
					{
						num = (Nearly.Equals(line1.B, line2.B) ? 1 : 0);
						goto IL_002b;
					}
					num = 0;
				}
				else if (num2 != 0)
				{
					goto IL_002b;
				}
				if (num != 0)
				{
					continue;
				}
				if (num == 0)
				{
					break;
				}
				goto IL_002b;
				IL_002b:
				return (byte)num != 0;
			}
			return (byte)num != 0;
		}

		public static Line Perpendicular(Line line, Point point)
		{
			double num;
			double num2;
			double num3;
			while (true)
			{
				num = line.A;
				num2 = 0.0;
				if (5 != 0)
				{
					bool flag = Nearly.Equals(num, num2);
					if (0 == 0)
					{
						if (flag)
						{
							num = 1.0;
							num2 = 0.0;
							if (6 != 0)
							{
								return new Line(num, num2, 0.0 - point.X);
							}
							goto IL_007e;
						}
						flag = Nearly.Equals(line.B, 0.0);
					}
					if (flag)
					{
						num = 0.0;
						num2 = 1.0;
						goto IL_007e;
					}
					if (false)
					{
						continue;
					}
					num = (0.0 - line.B) / line.A;
					num2 = 1.0;
				}
				num3 = point.X / line.A - point.Y;
				if (0 == 0)
				{
					break;
				}
				goto IL_0085;
				IL_0085:
				return new Line(num, num2, 0.0 - num3);
				IL_007e:
				num3 = point.Y;
				goto IL_0085;
			}
			return new Line(num, num2, num3);
		}

		public static Point Foot(Line line, Point point)
		{
			Line line2;
			if (true)
			{
				line2 = Perpendicular(line, point);
			}
			return Intersect(line, line2).Value;
		}

		public static double AngleBetween(Line line1, Line line2)
		{
			return Math.Abs(line1.InclinationAngle - line2.InclinationAngle);
		}

		public static double DistanceBetween(Line line, Point point)
		{
			return (point - Foot(line, point)).Length;
		}

		public override string ToString()
		{
			double d = A;
			double d2 = 0.0;
			if (5 != 0)
			{
				if (Nearly.Equals(d, d2))
				{
					return $"y={0.0 - C:0.##}";
				}
				d = B;
				d2 = 0.0;
			}
			bool flag = Nearly.Equals(d, d2);
			do
			{
				if (flag)
				{
					return $"x={0.0 - C:0.##}";
				}
				flag = Nearly.Equals(C, 0.0);
			}
			while (4 == 0);
			if (flag)
			{
				return $"y={FormatA(A)}x";
			}
			string arg = ((C > 0.0) ? (0.0 - C).ToString("0.##") : ("+" + (0.0 - C).ToString("0.##")));
			return $"y={FormatA(A)}x{arg}";
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Line))
			{
				return false;
			}
			return Equals((Line)obj);
		}

		public bool Equals(Line line)
		{
			double num = A;
			while (true)
			{
				double num2;
				if (0 == 0)
				{
					num2 = num;
					goto IL_0009;
				}
				goto IL_001a;
				IL_0009:
				if (num2.Equals(line.A))
				{
					if (-1 != 0)
					{
						num = B;
						goto IL_001a;
					}
					goto IL_001f;
				}
				goto IL_0045;
				IL_001a:
				if (false)
				{
					continue;
				}
				num2 = num;
				goto IL_001f;
				IL_0045:
				if (4 != 0)
				{
					break;
				}
				goto IL_0009;
				IL_001f:
				if (8 != 0)
				{
					if (!num2.Equals(line.B))
					{
						goto IL_0045;
					}
					num2 = C;
				}
				return num2.Equals(line.C);
			}
			return false;
		}

		public override int GetHashCode()
		{
			double num = A;
			int num2 = num.GetHashCode();
			if (0 == 0)
			{
				if (7 == 0)
				{
					goto IL_0038;
				}
				num2 *= 397;
			}
			double num3;
			while (true)
			{
				num3 = B;
				while (true)
				{
					num = num3;
					if (8 != 0)
					{
						num2 ^= num.GetHashCode();
						if (6 == 0)
						{
							break;
						}
						num2 *= 397;
					}
					num3 = C;
					if (0 == 0)
					{
						goto end_IL_0055;
					}
				}
				continue;
				end_IL_0055:
				break;
			}
			num = num3;
			goto IL_0038;
			IL_0038:
			return num2 ^ num.GetHashCode();
		}

		private static void SetAbc(out double a, out double b, out double c, double aValue, double bValue, double cValue)
		{
			while (true)
			{
				double num;
				double num2;
				double num3;
				if (2 != 0)
				{
					num = bValue;
					num2 = 0.0;
					while (!Nearly.Equals(num, num2))
					{
						cValue /= bValue;
						if (7 == 0)
						{
							goto end_IL_0000;
						}
						num = aValue;
						num2 = bValue;
						if (false)
						{
							continue;
						}
						goto IL_0045;
					}
					if (4 == 0)
					{
						continue;
					}
					num3 = cValue;
					if (false)
					{
						goto IL_0046;
					}
					cValue = num3 / aValue;
				}
				aValue = 1.0;
				goto IL_0053;
				IL_0053:
				a = aValue - 1.0;
				break;
				IL_0045:
				num3 = num / num2;
				goto IL_0046;
				IL_0046:
				aValue = num3;
				bValue = 1.0;
				goto IL_0053;
				continue;
				end_IL_0000:
				break;
			}
			b = bValue;
			c = cValue;
		}

		private static string FormatA(double a)
		{
			double num = a;
			bool flag;
			if (8 != 0)
			{
				double d = -1.0;
				double tolerance = 1E-05;
				while (true)
				{
					IL_0068:
					flag = Nearly.Equals(num, d, tolerance);
					while (!flag)
					{
						num = a;
						d = 1.0;
						tolerance = 1E-05;
						if (3 == 0)
						{
							goto IL_0068;
						}
						flag = Nearly.Equals(num, d, tolerance);
						if (false || 1 == 0)
						{
							continue;
						}
						goto IL_0043;
					}
					break;
				}
				goto IL_001b;
			}
			goto IL_0050;
			IL_001b:
			return "";
			IL_0043:
			if (flag)
			{
				return "-";
			}
			num = a;
			goto IL_0050;
			IL_0050:
			double num2 = 0.0 - num;
			if (0 == 0)
			{
				return num2.ToString("0.##");
			}
			goto IL_001b;
		}
	}
}
