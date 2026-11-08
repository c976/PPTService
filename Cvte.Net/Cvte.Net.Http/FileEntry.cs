using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.Http
{
	[Serializable]
	
	
	[DataContract]
	public class FileEntry
	{
		[DataMember(Name = "FilePath")]
		public string FilePath { get; set; }

		[DataMember(Name = "Size")]
		public long Size { get; set; }

		[DataMember(Name = "Hash")]
		public string Hash { get; set; }
	}
}
