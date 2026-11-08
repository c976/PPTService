using System;
using System.Runtime.Serialization;

namespace Cvte.Pipe.Http.Request
{
	[DataContract]
	public class DownloadFileRequest : JsonRequest
	{
		public EventHandler<UploadProgressChangedEventArgs> DownloadProgressChanged;

		public EventHandler<UploadCompletedEventArgs> DownloadCompleted;

		public EventHandler<DownloadErrorEventArgs> DownloadError;

		public string SavePath { get; set; }

		public override string RequestMethod => "POST";

		internal void UpdateProgress(long current, long total)
		{
			if (current == total)
			{
				DownloadCompleted?.Invoke(this, new UploadCompletedEventArgs());
				return;
			}
			DownloadProgressChanged?.Invoke(this, new UploadProgressChangedEventArgs
			{
				CurrentProgress = current,
				TotalProgress = total
			});
		}

		internal void HandleError(Exception ex)
		{
			DownloadError?.Invoke(this, new DownloadErrorEventArgs
			{
				Error = ex
			});
		}
	}
}
