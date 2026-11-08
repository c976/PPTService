using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.WebServices.Files.DownloadByWebClient;

namespace Cvte.Net.WebServices.Files
{
	
	
	public class WebFileDownloadProvider
	{
		[Obsolete]
		public async Task<AsyncCompletedEventArgs> DownloadFileAsync(DownloadFileEntry downloadFileEntry, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
		{
			return await DownloadFileListAsync(new List<DownloadFileEntry> { downloadFileEntry }, progress, cancellationToken).ConfigureAwait(false);
		}

		[Obsolete]
		public Task<AsyncCompletedEventArgs> DownloadFilesAsync(IList<DownloadFileEntry> entries, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
		{
			return DownloadFileHandler.DownloadFilesAsync(entries, progress, cancellationToken);
		}

		public Task<AsyncCompletedEventArgs> DownloadFileListAsync(IList<DownloadFileEntry> entries, IProgress<DownloadProgress> progress, CancellationToken cancellationToken, bool resume = false)
		{
			return DownloadFileHandler.DownloadFileListAsync(entries, progress, cancellationToken, resume);
		}

		[Obsolete]
		public Task<AsyncCompletedEventArgs> DownloadFileWithResume(IList<DownloadFileEntry> entries, IProgress<DownloadProgress> progress, CancellationToken cancellationToken, bool resume)
		{
			return DownloadFileWithResumeHandler.DownloadFileWithResume(entries, progress, cancellationToken, resume);
		}
	}
}
