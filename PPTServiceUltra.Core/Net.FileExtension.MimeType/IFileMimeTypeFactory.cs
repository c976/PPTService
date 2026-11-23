using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Core.Net.FileExtension.MimeType
{
	
	public interface IFileMimeTypeFactory
	{
		void SetFileMimeTypeBuilder(IFileMimeTypeBuilder mimeTypeBuilder);

		string BuildFileMimeType(string filePath);
	}
}
