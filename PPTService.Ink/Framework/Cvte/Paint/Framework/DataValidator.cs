using System.Windows;

namespace Cvte.Paint.Framework
{
	internal class DataValidator
	{
		internal static bool IsRectValid(Rect bounds)
		{
			if (IsDoubleValid(bounds.Width) && IsDoubleValid(bounds.Height) && IsDoubleValid(bounds.X) && IsDoubleValid(bounds.Y))
			{
				return true;
			}
			return false;
		}

		internal static bool IsDoubleValid(double value)
		{
			if (double.IsNaN(value) || double.IsInfinity(value))
			{
				return false;
			}
			return true;
		}
	}
}
