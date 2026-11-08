using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.WebServices.Files.Utils;

namespace Cvte.Net.WebServices.Files.DownloadByWebClient
{
	internal class DownloadFileWithResumeHandler
	{
		
		[Obsolete]
		public static async Task<AsyncCompletedEventArgs> DownloadFileWithResume(IList<DownloadFileEntry> entries, IProgress<DownloadProgress> progress, CancellationToken cancellationToken, bool resume)
		{
			long totalSize = entries.Sum((DownloadFileEntry downloadFileEntry) => downloadFileEntry.Size);
			long finishedSize = 0L;
			foreach (DownloadFileEntry entry in entries)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return new AsyncCompletedEventArgs(null, true, null);
				}
				string filePath = entry.FilePath;
				DirectoryInfo dir = new DirectoryInfo(filePath).Parent;
				if (dir != null && !dir.Exists)
				{
					Directory.CreateDirectory(dir.FullName);
				}
				else if (dir == null)
				{
					dir = new DirectoryInfo(filePath);
				}
				string tempPath = filePath + Guid.NewGuid().ToString() + ".tmp";
				string pattern = CommonFileHelper.GetFileNamePattern(filePath);
				long num = 0L;
				if (resume)
				{
					IEnumerable<FileInfo> source = from x in dir.GetFiles()
						where Regex.IsMatch(x.Name, pattern)
						select x;
					if (source.Any())
					{
						string text = ((source.Count() <= 1) ? source.ElementAt(0).FullName : (await SpliceFileHelper.SpliceFiles(dir, filePath)));
						string text2 = text;
						num = new FileInfo(text2).Length;
						finishedSize = num;
						if (SecurityUtility.GetMd5HashFromFile(text2) == entry.Hash)
						{
							try
							{
								File.Copy(text2, filePath, true);
								File.Delete(text2);
								return new AsyncCompletedEventArgs(null, false, null);
							}
							catch (Exception error)
							{
								return new AsyncCompletedEventArgs(error, false, null);
							}
						}
					}
				}
				WebClientForResume client = new WebClientForResume(num);
				ManualResetEvent manualResetEvent = new ManualResetEvent(true);
				try
				{
					AsyncCompletedEventArgs e = await Task.Run(async () =>
					{
						AsyncCompletedEventArgs result = null;
						client.DownloadProgressChanged += async (object s, DownloadProgressChangedEventArgs e2) =>
						{
							if (cancellationToken.IsCancellationRequested)
							{
								client.CancelAsync();
								client.Dispose();
								await Task.Delay(1000);
								manualResetEvent?.Set();
							}
							else
							{
								progress.Report((totalSize == 0L) ? new DownloadProgress(e2.BytesReceived, e2.TotalBytesToReceive) : new DownloadProgress(e2.BytesReceived + finishedSize, totalSize));
							}
						};
						client.DownloadFileCompleted += (object s, AsyncCompletedEventArgs e2) =>
						{
							if (e2.Error == null && !e2.Cancelled)
							{
								finishedSize += (e2.UserState as FileEntry)?.Size ?? 0;
							}
							result = e2;
							manualResetEvent?.Set();
							client.Dispose();
						};
						manualResetEvent.Reset();
						client.DownloadFileAsync(entry.DownloadUri, tempPath, entry);
						manualResetEvent.WaitOne();
						if (cancellationToken.IsCancellationRequested)
						{
							if (!resume)
							{
								CommonFileHelper.DeleteTempFile(tempPath);
							}
							return new AsyncCompletedEventArgs(null, true, null);
						}
						if (result == null || result.Error != null)
						{
							CommonFileHelper.DeleteTempFile(tempPath);
							return new AsyncCompletedEventArgs((result == null) ? new Exception("返回结果为空！") : result.Error, false, null);
						}
						try
						{
							if (resume)
							{
								tempPath = await SpliceFileHelper.SpliceFiles(dir, filePath);
							}
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
						catch (Exception error3)
						{
							result = new AsyncCompletedEventArgs(error3, false, null);
						}
						return result;
					}, cancellationToken).ConfigureAwait(false);
					if (e.Error != null || e.Cancelled)
					{
						return new AsyncCompletedEventArgs(e.Error, e.Cancelled, null);
					}
				}
				catch (Exception error2)
				{
					return new AsyncCompletedEventArgs(error2, false, null);
				}
				finally
				{
					client.Dispose();
					manualResetEvent.Dispose();
					manualResetEvent = null;
				}
			}
			return new AsyncCompletedEventArgs(null, false, null);
		}
	}
}
