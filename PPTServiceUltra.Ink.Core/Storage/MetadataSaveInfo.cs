using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	[SaveInfoContract("MetadataContract")]
	public class MetadataSaveInfo : SaveInfo
	{
		[SaveInfoMember("SaveInfoName")]
		public string Name { get; set; }

		[SaveInfoMember("SaveInfoFriendlyName")]
		public string FriendlyName { get; set; }

		[SaveInfoMember("SaveInfoType")]
		public SaveInfoType Type { get; set; }

		[SaveInfoMember("FallbackSaveInfo")]
		public string FallbackType { get; set; }

		public MetadataSaveInfo()
		{
		}

		public MetadataSaveInfo(string name)
		{
			Name = name;
		}
	}
}
