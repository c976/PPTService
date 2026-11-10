using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Internal
{
	[Reqeust("/file/attachments", ReqeustType.Post)]
	internal class AttachmentUploadRequest : UploadFileRequest
	{
		// 原 IL 是 .override UploadFileRequest::get_FileKey()，ILSpy 误写为显式接口实现
		public override string FileKey => "attachment";
	}
}
