using System.Runtime.Serialization;
using PPTService.Core.Pipe.Http.Request;

namespace PPTService.Core.WebService.Lychee
{
	[DataContract]
	internal class UploadFileLycheeRequest : UploadFileRequest
	{
		private readonly string _uploadUrl;

		[DataMember(Name = "token")]
		public string Key { get; set; }

		[DataMember(Name = "x:token")]
		public string Token { get; set; }

		[DataMember(Name = "key")]
		public string PageName { get; set; }

		[DataMember(Name = "x:appid")]
		public string AppId { get; set; }

		public UploadFileLycheeRequest(string uploadUrl)
		{
			_uploadUrl = uploadUrl;
		}

		public override string GetRequestUrl()
		{
			return _uploadUrl;
		}
	}
}
