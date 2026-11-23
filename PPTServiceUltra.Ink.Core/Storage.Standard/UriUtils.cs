using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	internal static class UriUtils
	{
		
		public static string RemovePrefix(string value, string prefix)
		{
			value = value.Trim();
			if (value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
			{
				return value.Substring(prefix.Length).Trim();
			}
			return value;
		}
	}
}
