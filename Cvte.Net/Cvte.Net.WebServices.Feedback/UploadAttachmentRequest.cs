using System.Runtime.CompilerServices;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Feedback
{
	
	
	[Request("/file/attachments")]
	internal class UploadAttachmentRequest : FeedbackUploadFileRequest
	{
		public override string FileKey => "attachment";
	}
}
