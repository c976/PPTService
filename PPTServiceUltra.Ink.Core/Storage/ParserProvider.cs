using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal class ParserProvider
	{
		internal ISaveInfoParserManager SaveInfoParserManager { get; }

		internal ParserProvider(ISaveInfoParserManager parserManager)
		{
			SaveInfoParserManager = parserManager;
		}

		internal ISaveInfoParser GetSaveInfoParser(string version)
		{
			if (string.IsNullOrEmpty(version))
			{
				return SaveInfoParserManager.CurrentParser;
			}
			ISaveInfoParser parser;
			if (!SaveInfoParserManager.TryGetParser(version, out parser))
			{
				throw new UnknowDocumentVersionException
				{
					Version = version,
					Info = "找不到对应版本的解析器，请调用AddParser添加。"
				};
			}
			return parser;
		}

		internal ISaveInfoNodeParser GetSaveInfoNodeParser(string version)
		{
			ISaveInfoNodeParser saveInfoNodeParser = GetSaveInfoParser(version) as ISaveInfoNodeParser;
			if (saveInfoNodeParser != null)
			{
				return saveInfoNodeParser;
			}
			throw new UnknowDocumentVersionException
			{
				Version = version,
				Info = "找不到对应版本的解析器，请调用AddParser添加。"
			};
		}
	}
}
