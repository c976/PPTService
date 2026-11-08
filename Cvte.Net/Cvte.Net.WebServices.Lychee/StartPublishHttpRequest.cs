using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Lychee
{
	
	
	[Request("/cgi/api/pre/start")]
	[DataContract]
	internal class StartPublishHttpRequest : LycheeHttpRequest
	{
		[DataMember(Name = "pid")]
		public string Pid { get; set; }
	}
}
