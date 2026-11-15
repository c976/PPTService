using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cli.Compatibility
{
	
	
	internal static class LegacyFrameworkCompatibility
	{
		public static int IndexOf(this string @string, char value, StringComparison stringComparison)
		{
			return @string.IndexOf(value.ToString(), stringComparison);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] EmptyArray< T>()
		{
			return new T[0];
		}
	}
}
