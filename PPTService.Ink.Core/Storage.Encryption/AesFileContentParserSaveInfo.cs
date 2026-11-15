using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Encryption
{
	
	
	[SaveInfoContract("AesFileContentParser")]
	public class AesFileContentParserSaveInfo : FileContentParserSaveInfo
	{
		[SaveInfoMember("Key")]
		public string Key { get; set; }

		[Obsolete("默认构造函数，仅供存储反射使用。")]
		public AesFileContentParserSaveInfo()
		{
		}

		public AesFileContentParserSaveInfo(string parserName)
		{
			ParserName = parserName;
		}
	}
}
