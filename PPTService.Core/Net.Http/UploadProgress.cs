namespace PPTService.Core.Net.Http
{
	public class UploadProgress
	{
		public long CurrentProgress { get; set; }

		public long TotalProgress { get; set; }

		public UploadProgress(long current, long total)
		{
			CurrentProgress = current;
			TotalProgress = total;
		}
	}
}
