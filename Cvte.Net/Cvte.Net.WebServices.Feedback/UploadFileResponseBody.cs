using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Feedback
{
	
	
	[DataContract]
	internal class UploadFileResponseBody : ResponseBody
	{
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}
}
