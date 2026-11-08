using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.Http
{
	[Serializable]
	
	
	[DataContract]
	public class DownloadFileEntry : FileEntry
	{
		[DataMember(Name = "Uri")]
		public Uri DownloadUri { get; set; }
	}
}
