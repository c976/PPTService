using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Encryption
{
	
	
	[SaveInfoContract("ContentParser", Description = "文档转换信息的描述")]
	public class FileContentParserSaveInfo : SaveInfo
	{
		public const string FileContentParserFileName = "ContentParser";

		[SaveInfoMember("ParserName")]
		public string ParserName { get; set; }
	}
}
