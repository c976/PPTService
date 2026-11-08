using System.Runtime.Serialization;

namespace Cvte.WebService.Feedback
{
	[DataContract]
	internal class UploadFileResponseBody : ResponseBody
	{
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}
}
