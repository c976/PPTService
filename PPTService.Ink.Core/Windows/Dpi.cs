using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace PPTService.Ink.Core.Windows
{
	[DebuggerDisplay("X = {X} ({FactorX}), Y = {Y} ({FactorY})")]
	public struct Dpi
	{
		private static readonly Lazy<Dpi> SystemDpiLazy = new Lazy<Dpi>(() =>
		{
			PropertyInfo property = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.Static | BindingFlags.NonPublic);
			PropertyInfo property2 = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.Static | BindingFlags.NonPublic);
			int num = ((int?)property?.GetValue(null, null)) ?? 96;
			int num2 = ((int?)property2?.GetValue(null, null)) ?? 96;
			if (num != 0 && num2 != 0)
			{
				return new Dpi(num, num2);
			}
			if (num2 != 0)
			{
				return new Dpi(num2, num2);
			}
			return (num != 0) ? new Dpi(num, num) : ScreenStandard;
		}, LazyThreadSafetyMode.ExecutionAndPublication);

		public static readonly Dpi ScreenStandard = new Dpi(96, 96);

		public static Dpi System => SystemDpiLazy.Value;

		public int X
		{
			
			get;
		}

		public int Y
		{
			
			get;
		}

		public double FactorX => (double)X / (double)ScreenStandard.X;

		public double FactorY => (double)Y / (double)ScreenStandard.Y;

		public Dpi(int x, int y)
		{
			this = default(Dpi);
			X = x;
			Y = y;
		}

		[DllImport("Shcore.dll")]
		private static extern int GetProcessDpiAwareness(IntPtr hprocess, out DpiAwareness value);

		public static DpiAwareness? GetProcessDpiAwareness()
		{
			Version version = Environment.OSVersion.Version;
			Version version2 = new Version(6, 3);
			if (version >= version2)
			{
				DpiAwareness value;
				int processDpiAwareness = GetProcessDpiAwareness(IntPtr.Zero, out value);
				if (processDpiAwareness == 0)
				{
					return value;
				}
				throw new Win32Exception(processDpiAwareness);
			}
			return null;
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
			if (obj == null)
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
