using System.Runtime.CompilerServices;
using Cvte.Net.FileExtension.MimeType;

namespace Cvte.Net.FileExtension
{
	
	
	public class FileExtensionFactory
	{
		public static FileExtensionFactory Default { get; }

		public IFileMimeTypeFactory MimeTypeFactory { get; set; }

		static FileExtensionFactory()
		{
			Default = InitialDefaultFactory();
		}

		private static FileExtensionFactory InitialDefaultFactory()
		{
			FileExtensionFactory fileExtensionFactory = new FileExtensionFactory();
			fileExtensionFactory.MimeTypeFactory = new FileMimeTypeFactory();
			fileExtensionFactory.MimeTypeFactory.SetFileMimeTypeBuilder(new DefaultFileExtensionMimeTypeBuilder());
			fileExtensionFactory.MimeTypeFactory.SetFileMimeTypeBuilder(new FileMimeTypeBuilder());
			return fileExtensionFactory;
		}
	}
}
