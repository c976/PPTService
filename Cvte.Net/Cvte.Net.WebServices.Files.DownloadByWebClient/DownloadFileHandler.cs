using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.WebServices.Files.Utils;

namespace Cvte.Net.WebServices.Files.DownloadByWebClient
{
	
	
	internal class DownloadFileHandler
	{
		private sealed class _003C_003Ec__DisplayClass1_0
		{
			public CancellationToken cancellationToken;

			
			public IProgress<DownloadProgress> progress;

			public long totalSize;

			public long finishedSize;

			internal void _003CDownloadFileListAsync_003Eg__OnClientOnDownloadProgressChanged_007C1(object s, DownloadProgressChangedEventArgs e)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					((WebClient)s).CancelAsync();
				}
				else
				{
					progress.Report((totalSize == 0L) ? new DownloadProgress(e.BytesReceived, e.TotalBytesToReceive) : new DownloadProgress(e.BytesReceived + finishedSize, totalSize));
				}
			}
		}

		private sealed class _003C_003Ec__DisplayClass1_1
		{
			
			public AsyncCompletedEventArgs result;

			internal void _003CDownloadFileListAsync_003Eg__OnClientOnDownloadFileCompleted_007C2(object s, AsyncCompletedEventArgs e)
			{
				result = e;
			}
		}

		[Obsolete]
		public static async Task<AsyncCompletedEventArgs> DownloadFilesAsync(IList<DownloadFileEntry> entries, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
		{
			long totalSize = entries.Sum( (DownloadFileEntry downloadFileEntry) => downloadFileEntry.Size);
			long finishedSize = 0L;
			foreach (DownloadFileEntry entry in entries)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return new AsyncCompletedEventArgs(null, true, null);
				}
				string filePath = entry.FilePath;
				DirectoryInfo parent = new DirectoryInfo(filePath).Parent;
				if (parent != null && !parent.Exists)
				{
					Directory.CreateDirectory(parent.FullName);
				}
				string tempPath = filePath + Guid.NewGuid().ToString() + ".tmp";
				AsyncCompletedEventArgs e = await Task.Run( () =>
				{
					WebClient webClient = new WebClient();
					ManualResetEvent manualResetEvent = new ManualResetEvent(true);
					try
					{
						AsyncCompletedEventArgs result = null;
						webClient.DownloadProgressChanged +=  (object s, DownloadProgressChangedEventArgs e2) =>
						{
							if (cancellationToken.IsCancellationRequested)
							{
								manualResetEvent?.Set();
							}
							else
							{
								progress.Report((totalSize == 0L) ? new DownloadProgress(e2.BytesReceived, e2.TotalBytesToReceive) : new DownloadProgress(e2.BytesReceived + finishedSize, totalSize));
							}
						};
						webClient.DownloadFileCompleted +=  (object s, AsyncCompletedEventArgs e2) =>
						{
							if (e2.Error == null && !e2.Cancelled)
							{
								finishedSize += (e2.UserState as FileEntry)?.Size ?? 0;
							}
							result = e2;
							manualResetEvent?.Set();
						};
						manualResetEvent.Reset();
						webClient.DownloadFileAsync(entry.DownloadUri, tempPath, entry);
						manualResetEvent.WaitOne();
						if (cancellationToken.IsCancellationRequested)
						{
							CommonFileHelper.DeleteTempFile(tempPath);
							return new AsyncCompletedEventArgs(null, true, null);
						}
						if (result == null)
						{
							return new AsyncCompletedEventArgs(new Exception("result can not be null."), false, null);
						}
						if (result.Error != null)
						{
							CommonFileHelper.DeleteTempFile(tempPath);
							return new AsyncCompletedEventArgs(result.Error, false, null);
						}
						try
						{
							if (File.Exists(tempPath))
							{
								if (!string.IsNullOrEmpty(entry.Hash) && SecurityUtility.GetMd5HashFromFile(tempPath) != entry.Hash)
								{
									result = new AsyncCompletedEventArgs(new Exception("file hash does not match."), false, null);
								}
								else
								{
									File.Copy(tempPath, filePath, true);
								}
								File.Delete(tempPath);
							}
							else
							{
								result = new AsyncCompletedEventArgs(new TimeoutException(), false, null);
							}
						}
						catch (Exception error)
						{
							result = new AsyncCompletedEventArgs(error, false, null);
						}
						return result;
					}
					finally
					{
						webClient.CancelAsync();
						webClient.Dispose();
						manualResetEvent.Dispose();
						manualResetEvent = null;
					}
				}, cancellationToken).ConfigureAwait(false);
				if (e.Error != null || e.Cancelled)
				{
					return new AsyncCompletedEventArgs(e.Error, e.Cancelled, null);
				}
			}
			return new AsyncCompletedEventArgs(null, false, null);
		}

		public static async Task<AsyncCompletedEventArgs> DownloadFileListAsync(IList<DownloadFileEntry> entries, IProgress<DownloadProgress> progress, CancellationToken cancellationToken, bool resume = false)
		{
			_003C_003Ec__DisplayClass1_0 obj = new _003C_003Ec__DisplayClass1_0();
			obj.cancellationToken = cancellationToken;
			obj.progress = progress;
			obj.totalSize = entries.Sum( (DownloadFileEntry e) => e.Size);
			obj.finishedSize = 0L;
			using (WebClientForResume client = new WebClientForResume(0L))
			{
				_003C_003Ec__DisplayClass1_1 obj2 = new _003C_003Ec__DisplayClass1_1();
				obj2.result = new AsyncCompletedEventArgs(null, true, null);
				client.DownloadProgressChanged += obj._003CDownloadFileListAsync_003Eg__OnClientOnDownloadProgressChanged_007C1;
				client.DownloadFileCompleted += obj2._003CDownloadFileListAsync_003Eg__OnClientOnDownloadFileCompleted_007C2;
				foreach (DownloadFileEntry entry in entries)
				{
					string filePath = entry.FilePath;
					DirectoryInfo directoryInfo = CommonFileHelper.GetTempPathDirectory(filePath);
					string tempPath = filePath + Guid.NewGuid().ToString() + ".tmp";
					if (resume)
					{
						obj.finishedSize = await SpliceFileHelper.GetDownloadedSize(directoryInfo, filePath).ConfigureAwait(false);
						client.SetHeaderRange(obj.finishedSize);
					}
					try
					{
						await client.DownloadFileTaskAsync(entry.DownloadUri, tempPath).ConfigureAwait(false);
						if (resume)
						{
							tempPath = await SpliceFileHelper.SpliceFiles(directoryInfo, filePath).ConfigureAwait(false);
						}
						CommonFileHelper.FileCheck(tempPath, filePath, entry.Hash);
					}
					catch (Exception error)
					{
						if (!resume)
						{
							CommonFileHelper.DeleteTempFile(tempPath);
						}
						return new AsyncCompletedEventArgs(error, obj2.result.Cancelled, null);
					}
					CommonFileHelper.DeleteTempFile(tempPath);
				}
				return obj2.result;
			}
		}
	}
}
