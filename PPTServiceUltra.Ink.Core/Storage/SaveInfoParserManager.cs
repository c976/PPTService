using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class SaveInfoParserManager : ISaveInfoParserManager
	{
		private readonly IDictionary<string, ISaveInfoParser> _versionParserMapping = new Dictionary<string, ISaveInfoParser>();

		public string CurrentVersion { get; private set; }

		public ISaveInfoParser CurrentParser { get; private set; }

		public void AddParser(string fileFormat, string version, ISaveInfoParser parser)
		{
			if (!_versionParserMapping.ContainsKey(version))
			{
				_versionParserMapping.Add(version, parser);
			}
		}

		public void AddCurrentParser(string fileFormat, string version, ISaveInfoParser parser)
		{
			AddParser(fileFormat, version, parser);
			CurrentVersion = version;
			CurrentParser = parser;
		}

		public bool TryGetParser(string version, out ISaveInfoParser parser)
		{
			return _versionParserMapping.TryGetValue(version, out parser);
		}
	}
}
