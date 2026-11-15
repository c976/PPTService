using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Configurations.Utils
{
	internal static class OSUtils
	{
		
		internal static bool PathEquals(string path1, string path2)
		{
			return string.Equals(path1, path2, IsPathCaseSensitive() ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsPathCaseSensitive()
		{
			return false;
		}
	}
}
