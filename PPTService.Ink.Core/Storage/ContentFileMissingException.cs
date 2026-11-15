using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class ContentFileMissingException : DocumentException
	{
		public override string Message => MissingName + "文件未找到";

		public string MissingName { get; set; }

		public ContentFileMissingException(string missingName)
		{
			MissingName = missingName;
		}
	}
}
