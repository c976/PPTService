using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Cvte.Net
{
	internal static class DateTimeExtension
	{
		
		public static string GetTimeSpanString(this DateTime dateTime)
		{
			return ((long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString(CultureInfo.InvariantCulture);
		}
	}
}
