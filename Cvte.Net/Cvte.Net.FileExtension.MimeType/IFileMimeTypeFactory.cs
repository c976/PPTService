using System.Runtime.CompilerServices;

namespace Cvte.Net.FileExtension.MimeType
{
	
	public interface IFileMimeTypeFactory
	{
		void SetFileMimeTypeBuilder(IFileMimeTypeBuilder mimeTypeBuilder);

		string BuildFileMimeType(string filePath);
	}
}
