using System;

namespace PPTService.Paint.Framework
{
	public static class DateTimeExtensions
	{
		public static bool IsWithin(this DateTime dateTime, TimeSpan timeSpan)
		{
			return DateTime.Now - dateTime < timeSpan;
		}

		public static bool IsWithin(this DateTime dateTime, double milliseconds)
		{
			return dateTime.IsWithin(TimeSpan.FromMilliseconds(milliseconds));
		}
	}
}
