using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Linq;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal class SaveInfoMetadataManager
	{
		private readonly Dictionary<string, MetadataSaveInfo> _metadataDictionary = new Dictionary<string, MetadataSaveInfo>();

		public void Add(MetadataSaveInfo metadata)
		{
			_metadataDictionary[metadata.Name] = metadata;
		}

		public List<MetadataSaveInfo> GetSaveInfoMetadata()
		{
			return _metadataDictionary.Values.ToList();
		}

		public void AddRange(IList<MetadataSaveInfo> metadata)
		{
			metadata.ForEach(Add);
		}

		public void Reset()
		{
			_metadataDictionary.Clear();
		}

		public bool TryGetMetadataSaveInfo(string name, out MetadataSaveInfo saveInfo)
		{
			return _metadataDictionary.TryGetValue(name, out saveInfo);
		}
	}
}
