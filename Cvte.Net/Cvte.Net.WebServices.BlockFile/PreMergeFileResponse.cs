using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	[DataContract]
	public class PreMergeFileResponse : JsonResponse
	{
		[DataMember(Name = "data")]
		public HeaderFileInfo HeaderFileInfo { get; set; }
	}
}
