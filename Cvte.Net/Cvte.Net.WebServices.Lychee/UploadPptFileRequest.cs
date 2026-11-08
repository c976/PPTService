using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Lychee
{
	
	
	[DataContract]
	internal class UploadPptFileRequest : UploadFileRequest
	{
		[DataMember(Name = "app")]
		public string App { get; set; }

		[DataMember(Name = "ts")]
		public long TimeStamp { get; set; }

		[DataMember(Name = "sign")]
		public string Sign { get; set; }
	}
}
