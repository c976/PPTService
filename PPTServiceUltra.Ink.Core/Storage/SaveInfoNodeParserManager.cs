using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal class SaveInfoNodeParserManager
	{
		private readonly Dictionary<string, ISaveInfoNodeParser> _versionParserMapping = new Dictionary<string, ISaveInfoNodeParser>();

		public string CurrentVersion { get; private set; }

		public ISaveInfoNodeParser CurrentParser { get; private set; }

		public void AddParser(string fileFormat, string version, ISaveInfoNodeParser parser)
		{
			if (!_versionParserMapping.ContainsKey(version))
			{
				_versionParserMapping.Add(version, parser);
			}
		}

		public void AddCurrentParser(string fileFormat, string version, ISaveInfoNodeParser parser)
		{
			AddParser(fileFormat, version, parser);
			CurrentVersion = version;
			CurrentParser = parser;
		}

		public bool TryGetParser(string version, out ISaveInfoNodeParser parser)
		{
			return _versionParserMapping.TryGetValue(version, out parser);
		}
	}
}
