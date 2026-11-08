using System.Runtime.CompilerServices;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Feedback
{
	
	
	internal class FeedbackUploadFileRequest : UploadFileRequest
	{
		public virtual string FileKey => "file";

		public string FilePath { get; set; }
	}
}
