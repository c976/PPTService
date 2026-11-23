using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using PPTServiceUltra.Core.Net.Http;

namespace PPTServiceUltra.Core.Net.WebServices.Lychee
{
	
	
	[DataContract]
	internal class LycheeHttpRequest : FormsRequest
	{
		[DataMember(Name = "sign")]
		public string Sign { get; set; }

		[DataMember(Name = "app")]
		public string App { get; set; }

		[DataMember(Name = "timestamp")]
		public long TimeStamp { get; set; }

		[DataMember(Name = "version")]
		public int Version { get; set; }
	}
}
