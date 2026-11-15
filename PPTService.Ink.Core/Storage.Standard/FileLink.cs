using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
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
