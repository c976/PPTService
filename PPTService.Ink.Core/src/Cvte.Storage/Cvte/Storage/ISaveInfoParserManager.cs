using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	public interface ISaveInfoParserManager
	{
		string CurrentVersion { get; }

		ISaveInfoParser CurrentParser { get; }

		void AddParser(string fileFormat, string version, ISaveInfoParser parser);

		void AddCurrentParser(string fileFormat, string version, ISaveInfoParser parser);

		bool TryGetParser(string version, out ISaveInfoParser parser);
	}
}
