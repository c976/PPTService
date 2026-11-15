using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Encryption
{
	[SaveInfoContract("Base64Parser")]
	internal class Base64FileContentParserSaveInfo : FileContentParserSaveInfo
	{
		[Obsolete("默认构造函数，仅供存储反射使用。")]
		public Base64FileContentParserSaveInfo()
		{
		}

		
		public Base64FileContentParserSaveInfo(string parserName)
		{
			ParserName = parserName;
		}
	}
}
