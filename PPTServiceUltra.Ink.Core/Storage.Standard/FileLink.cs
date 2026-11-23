using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	[ActionType("File")]
	public class FileLink : Hyperlink
	{
		[ActionParameter("filePath")]
		public string FilePath { get; set; }

		public FileLink()
		{
		}

		public FileLink(string filePath)
		{
			FilePath = filePath;
		}
	}
}
