using System.Runtime.CompilerServices;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Feedback
{
	
	
	[Request("/file/pics")]
	internal class UploadPictueRequest : FeedbackUploadFileRequest
	{
		public override string FileKey => "picture";
	}
}
