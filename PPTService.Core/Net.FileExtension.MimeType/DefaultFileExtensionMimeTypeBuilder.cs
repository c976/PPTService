using System;
using System.Runtime.CompilerServices;
using System.Web;

namespace PPTService.Core.Net.FileExtension.MimeType
{
	internal class DefaultFileExtensionMimeTypeBuilder : IFileMimeTypeBuilder
	{
		
		private const string UnknownMimeType = "application/octet-stream";

		public byte PopularLevel => 100;

		
		public bool TryBuildFileMimeType(string filePath, out string mimeType)
		{
			mimeType = MimeMapping.GetMimeMapping(filePath);
			if (string.Equals(mimeType, "application/octet-stream", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			return true;
		}
	}
}
