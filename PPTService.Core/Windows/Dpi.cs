using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace PPTService.Core.Windows
{
	[DebuggerDisplay("X = {X} ({FactorX}), Y = {Y} ({FactorY})")]
	public struct Dpi
	{
		private static readonly Lazy<Dpi> SystemDpiLazy = new Lazy<Dpi>(() =>
		{
			PropertyInfo property = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.Static | BindingFlags.NonPublic);
			PropertyInfo property2 = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.Static | BindingFlags.NonPublic);
			int x = ((property != null) ? ((int)property.GetValue(null, null)) : 96);
			int y = ((property2 != null) ? ((int)property2.GetValue(null, null)) : 96);
			return new Dpi(x, y);
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
			if (dpi1.X == dpi2.X)
			{
				return dpi1.Y == dpi2.Y;
			}
			return false;
		}

		public static bool operator !=(Dpi dpi1, Dpi dpi2)
		{
			return !(dpi1 == dpi2);
		}

		public bool Equals(Dpi other)
		{
			if (X == other.X)
			{
				return Y == other.Y;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			if (obj is Dpi)
			{
				return Equals((Dpi)obj);
			}
			return false;
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
