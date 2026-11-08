using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace Cvte.Windows
{
	[DebuggerDisplay("X = {X} ({FactorX}), Y = {Y} ({FactorY})")]
	public struct Dpi
	{
		private static readonly Lazy<Dpi> SystemDpiLazy = new Lazy<Dpi>(() =>
		{
			PropertyInfo property = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.Static | BindingFlags.NonPublic);
			PropertyInfo property2 = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.Static | BindingFlags.NonPublic);
			PropertyInfo propertyInfo = default(PropertyInfo);
			if (0 == 0)
			{
				propertyInfo = property2;
			}
			int num;
			if (!(property != null))
			{
				num = 96;
				goto IL_0057;
			}
			goto IL_00c2;
			IL_0057:
			int x = num;
			if (-1 != 0)
			{
				int y = ((propertyInfo != null) ? ((int)propertyInfo.GetValue(null, null)) : 96);
				return new Dpi(x, y);
			}
			goto IL_00c2;
			IL_00c2:
			num = (int)property.GetValue(null, null);
			goto IL_0057;
		}, LazyThreadSafetyMode.ExecutionAndPublication);

		public static readonly Dpi ScreenStandard = new Dpi(96, 96);

		public static Dpi System => SystemDpiLazy.Value;

		public int X { get; private set; }

		public int Y { get; private set; }

		public double FactorX => (double)X / (double)ScreenStandard.X;

		public double FactorY => (double)Y / (double)ScreenStandard.Y;

		public Dpi(int x, int y)
		{
			this = default(Dpi);
			X = x;
			Y = y;
		}

		public static bool operator ==(Dpi dpi1, Dpi dpi2)
		{
			while (true)
			{
				int num = dpi1.X;
				int num2 = dpi2.X;
				while (true)
				{
					if (0 == 0)
					{
						if (num != num2)
						{
							if (false)
							{
								break;
							}
							if (2 != 0)
							{
								return false;
							}
						}
						num = dpi1.Y;
						num2 = dpi2.Y;
					}
					if (-1 != 0)
					{
						return num == num2;
					}
				}
			}
		}

		public static bool operator !=(Dpi dpi1, Dpi dpi2)
		{
			return !(dpi1 == dpi2);
		}

		public bool Equals(Dpi other)
		{
			while (true)
			{
				int num = X;
				int num2 = other.X;
				while (true)
				{
					if (0 == 0)
					{
						if (num != num2)
						{
							if (false)
							{
								break;
							}
							if (2 != 0)
							{
								return false;
							}
						}
						num = Y;
						num2 = other.Y;
					}
					if (-1 != 0)
					{
						return num == num2;
					}
				}
			}
		}

		public override bool Equals(object obj)
		{
			int num = (object.ReferenceEquals(null, obj) ? 1 : 0);
			while (true)
			{
				if (num != 0)
				{
					num = 0;
					goto IL_0008;
				}
				if (0 == 0 && !(obj is Dpi))
				{
					num = 0;
					if (num == 0)
					{
						if (num != 0)
						{
							continue;
						}
						if (num == 0)
						{
							break;
						}
						goto IL_0008;
					}
				}
				else
				{
					num = (Equals((Dpi)obj) ? 1 : 0);
				}
				return (byte)num != 0;
				IL_0008:
				return (byte)num != 0;
			}
			return (byte)num != 0;
		}

		public override int GetHashCode()
		{
			return (X * 397) ^ Y;
		}

		public override string ToString()
		{
			return $"{X}({FactorX:P0}),{Y}({FactorY:P0})";
		}
	}
}
