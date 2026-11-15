using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal static class PathComparisonHelper
	{
		public static bool IsSameRelativePath(string sourcePath, string targetPath)
		{
			if (string.IsNullOrWhiteSpace(sourcePath) && string.IsNullOrWhiteSpace(targetPath))
			{
				return true;
			}
			if (string.IsNullOrWhiteSpace(sourcePath) || string.IsNullOrWhiteSpace(targetPath))
			{
				return false;
			}
			sourcePath = sourcePath.Trim().Replace("/", "\\");
			targetPath = targetPath.Trim().Replace("/", "\\");
			return sourcePath.Equals(targetPath, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsStartWithRelativePath(string sourcePath, string relativeDir)
		{
			if (string.IsNullOrWhiteSpace(sourcePath))
			{
				throw new ArgumentNullException("sourcePath");
			}
			if (string.IsNullOrWhiteSpace(relativeDir))
			{
				throw new ArgumentNullException("relativeDir");
			}
			sourcePath = sourcePath.Trim().Replace("/", "\\");
			relativeDir = relativeDir.Trim().Replace("/", "\\");
			return sourcePath.StartsWith(relativeDir, StringComparison.OrdinalIgnoreCase);
		}
	}
}
