using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Core.Net.FileExtension.MimeType
{
	
	public interface IFileMimeTypeBuilder
	{
		byte PopularLevel { get; }

		bool TryBuildFileMimeType(string filePath, out string mimeType);
	}
}
