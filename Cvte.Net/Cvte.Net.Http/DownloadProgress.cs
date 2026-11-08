namespace Cvte.Net.Http
{
	public class DownloadProgress
	{
		public long CurrentProgress { get; set; }

		public long TotalProgress { get; set; }

		public DownloadProgress(long current, long total)
		{
			CurrentProgress = current;
			TotalProgress = total;
		}
	}
}
