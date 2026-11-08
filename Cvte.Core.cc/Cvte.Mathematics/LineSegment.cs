using System;
using System.Windows;

namespace Cvte.Mathematics
{
	public struct LineSegment
	{
		private readonly Point _point1;

		private readonly Point _point2;

		public Line Line
		{
			get
			{
				if (!IsValid)
				{
					throw new InvalidOperationException("此线段不是有意义的线段。");
				}
				return new Line(_point1, _point2);
			}
		}

		public bool IsValid => !Nearly.Equals(Point1, Point2);

		public Point Point1 => _point1;

		public Point Point2 => _point2;

		public Rect Bounds => new Rect(Point1, Point2);

		public LineSegment(Point point1, Point point2)
		{
			_point1 = point1;
			_point2 = point2;
		}

		public bool Contains(Point point)
		{
			double num = 1E-05;
			do
			{
				if (!Line.Contains(point))
				{
					continue;
				}
				double num2 = point.X;
				double val = Point1.X;
				while (true)
				{
					Point point2 = Point2;
					if (6 == 0)
					{
						continue;
					}
					if (!(num2 <= Math.Max(val, point2.X) + num))
					{
						break;
					}
					num2 = point.X;
					if (true)
					{
						double num3 = Point1.X;
						if (5 != 0)
						{
							double num4 = Point2.X;
							if (0 == 0)
							{
								if (!(num2 >= Math.Min(num3, num4) - num))
								{
									break;
								}
								num2 = point.Y;
								val = Point1.Y;
								if (3 == 0)
								{
									continue;
								}
								num3 = Math.Max(val, Point2.Y);
								num4 = num;
							}
							num3 += num4;
						}
						if (!(num2 <= num3))
						{
							break;
						}
						num2 = point.Y;
					}
					return num2 >= Math.Min(Point1.Y, Point2.Y) - num;
				}
			}
			while (false);
			return false;
		}

		public static double AngleBetween(LineSegment lineSegment1, LineSegment lineSegment2)
		{
			if (0 == 0)
			{
				if (false || !lineSegment1.IsValid)
				{
					goto IL_000c;
				}
				if (!lineSegment2.IsValid)
				{
					goto IL_0022;
				}
			}
			return Line.AngleBetween(lineSegment1.Line, lineSegment2.Line);
			IL_0022:
			if (0 == 0)
			{
				throw new ArgumentException("lineSegment2不是有意义的线段。", "lineSegment2");
			}
			goto IL_000c;
			IL_000c:
			if (7 != 0)
			{
				throw new ArgumentException("lineSegment1不是有意义的线段。", "lineSegment1");
			}
			goto IL_0022;
		}

		public static Point? Intersect(LineSegment lineSegment1, LineSegment lineSegment2)
		{
			Point? result2;
			while (0 == 0)
			{
				if (!lineSegment1.IsValid)
				{
					if (0 == 0)
					{
						throw new ArgumentException("lineSegment1不是有意义的线段。", "lineSegment1");
					}
				}
				else if (lineSegment2.IsValid)
				{
					goto IL_003e;
				}
				if (0 == 0)
				{
					throw new ArgumentException("lineSegment2不是有意义的线段。", "lineSegment2");
				}
				goto IL_003e;
				IL_003e:
				while (5 != 0)
				{
					Point? result = Line.Intersect(lineSegment1.Line, lineSegment2.Line);
					if (result.HasValue && lineSegment1.Contains(result.Value))
					{
						if (3 == 0)
						{
							continue;
						}
						if (lineSegment2.Contains(result.Value))
						{
							return result;
						}
					}
					goto IL_0084;
				}
				continue;
				IL_0084:
				result2 = null;
				break;
			}
			return result2;
		}

		public bool AreParrallel(LineSegment lineSegment1, LineSegment lineSegment2)
		{
			bool flag;
			if (0 == 0)
			{
				if (false || !lineSegment1.IsValid)
				{
					if (7 != 0)
					{
						throw new ArgumentException("lineSegment1不是有意义的线段。", "lineSegment1");
					}
					goto IL_0022;
				}
				flag = lineSegment2.IsValid;
				goto IL_0020;
			}
			goto IL_002f;
			IL_0020:
			if (!flag)
			{
				goto IL_0022;
			}
			goto IL_002f;
			IL_0022:
			throw new ArgumentException("lineSegment2不是有意义的线段。", "lineSegment2");
			IL_002f:
			flag = Line.AreParallel(lineSegment1.Line, lineSegment2.Line);
			if (0 == 0)
			{
				return flag;
			}
			goto IL_0020;
		}

		public bool Equals(LineSegment segment)
		{
			while (true)
			{
				int num;
				if (7 != 0)
				{
					if (false)
					{
						goto IL_003f;
					}
					if (segment._point1 == _point1)
					{
						num = ((segment._point2 == _point2) ? 1 : 0);
						goto IL_0028;
					}
				}
				goto IL_007b;
				IL_007b:
				num = ((segment._point1 == _point2) ? 1 : 0);
				if (0 == 0)
				{
					if (num != 0)
					{
						goto IL_003f;
					}
					if (4 == 0)
					{
						continue;
					}
					num = 0;
					if (num == 0)
					{
						return (byte)num != 0;
					}
				}
				goto IL_0028;
				IL_0028:
				if (num != 0)
				{
					break;
				}
				goto IL_007b;
				IL_003f:
				return segment._point2 == _point1;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is LineSegment))
			{
				return false;
			}
			return Equals((LineSegment)obj);
		}

		public override int GetHashCode()
		{
			Point point = default(Point);
			do
			{
				if (4 != 0)
				{
					point = _point1;
				}
			}
			while (6 == 0);
			int num = point.GetHashCode();
			while (0 == 0)
			{
				point = _point2;
				if (6 != 0)
				{
					num ^= point.GetHashCode();
					break;
				}
			}
			return num;
		}
	}
}
