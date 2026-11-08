using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Lychee
{
	
	
	[Request("/cgi/api/pre/complete")]
	[DataContract]
	internal class CompletePublishRequest : LycheeHttpRequest
	{
		[DataMember(Name = "key")]
		public string Key { get; set; }

		[DataMember(Name = "uid")]
		public string Uid { get; set; }

		[DataMember(Name = "pid")]
		public string Pid { get; set; }
	}
}
