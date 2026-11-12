using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using PPTService.Core.Net.Http;

namespace PPTService.Core.Net.WebServices.Lychee
{
	
	
	[DataContract]
	internal class UploadFileLycheeRequest : UploadFileRequest
	{
		[DataMember(Name = "token")]
		public string Key { get; set; }

		[DataMember(Name = "x:token")]
		public string Token { get; set; }

		[DataMember(Name = "key")]
		public string PageName { get; set; }

		[DataMember(Name = "x:appid")]
		public string AppId { get; set; }
	}
}
