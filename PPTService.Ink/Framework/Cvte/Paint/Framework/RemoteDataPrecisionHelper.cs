using System;
using System.Windows;

namespace Cvte.Paint.Framework
{
	internal static class RemoteDataPrecisionHelper
	{
		public static double ToRemote(this double dValue)
		{
			return Math.Round(dValue, 3);
		}

		public static Rect ToRemote(this Rect value)
		{
			return new Rect(value.Left.ToRemote(), value.Top.ToRemote(), value.Width.ToRemote(), value.Height.ToRemote());
		}
	}
}
