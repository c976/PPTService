using System;
using System.Windows;

namespace PPTService.Ink.Core.Windows.Media
{
	internal static class MathScalingExtensions
	{
		internal static Size ToScaledSize(this Size size, Size scalingFactor)
		{
			return size.ToScaledSize(scalingFactor.Width, scalingFactor.Height);
		}

		internal static Size ToScaledSize(this Size size, double scaleX, double scaleY)
		{
			return new Size(size.Width * scaleX, size.Height * scaleY);
		}

		internal static Rect ToScaledRect(this Rect rect, Size scalingFactor)
		{
			return rect.ToScaledRect(scalingFactor.Width, scalingFactor.Height);
		}

		internal static Rect ToScaledRect(this Rect rect, double scaleX, double scaleY)
		{
			return new Rect(rect.Left * scaleX, rect.Top * scaleY, rect.Width * scaleX, rect.Height * scaleY);
		}

		internal static Rect ToAbsoluteViewbox(this Rect relativeViewbox, Size originSize)
		{
			return new Rect(relativeViewbox.Left * originSize.Width, relativeViewbox.Top * originSize.Height, relativeViewbox.Width * originSize.Width, relativeViewbox.Height * originSize.Height);
		}

		internal static Int32Rect ToInt32Rect(this Rect rect)
		{
			return new Int32Rect((int)Math.Round(rect.Left), (int)Math.Round(rect.Top), (int)Math.Round(rect.Width), (int)Math.Round(rect.Height));
		}
	}
}
