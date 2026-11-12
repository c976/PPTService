using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using PPTService.Core.Net.Exceptions;
using PPTService.Core.Net.Http;

namespace PPTService.Core.Net.WebServices.Files.Download
{
	
	
	internal static class UnitFileDownloadUtility
	{
		public static async Task<HttpWebResponseWrapper> DownloadFile(string url, string host, string filePath, int timeOut = 30000)
		{
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			try
			{
				CookieContainer cookieContainer = new CookieContainer();
				HttpsUtility.SetSecurity(url);
				request = (HttpWebRequest)WebRequest.Create(url);
				request.Headers = new WebHeaderCollection();
				request.ServicePoint.Expect100Continue = false;
				request.AllowAutoRedirect = true;
				request.Timeout = timeOut;
				request.ReadWriteTimeout = timeOut;
				request.Method = "GET";
				request.CookieContainer = cookieContainer;
				if (!string.IsNullOrWhiteSpace(host))
				{
					request.Host = host;
				}
				response = (HttpWebResponse)(await request.GetResponseAsync().ConfigureAwait(false));
				if (response.Cookies == null)
				{
					response.Cookies = new CookieCollection();
				}
				foreach (Cookie cookie in request.CookieContainer.GetCookies(request.RequestUri))
				{
					response.Cookies.Add(cookie);
				}
				long fileSize = response.ContentLength;
				string html = string.Empty;
				using (Stream stream = response.GetResponseStream())
				{
					if (stream != null)
					{
						byte[] buffer = new byte[4096];
						DirectoryInfo parent = Directory.GetParent(filePath);
						if (parent == null)
						{
							return new HttpWebResponseWrapper(response, new DirectoryNotFoundException("download path directory is null"));
						}
						string fullName = parent.FullName;
						if (!Directory.Exists(fullName))
						{
							Directory.CreateDirectory(fullName);
						}
						using (FileStream writer = new FileStream(filePath, FileMode.Create))
						{
							long receiveSize = 0L;
							int num;
							while ((num = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
							{
								writer.Write(buffer, 0, num);
								receiveSize += num;
							}
							if (fileSize != -1 && fileSize != receiveSize)
							{
								return new HttpWebResponseWrapper(response, new FileNotConsistentException("File Not Consistent"));
							}
						}
					}
				}
				return new HttpWebResponseWrapper(response, html);
			}
			catch (Exception exception)
			{
				return new HttpWebResponseWrapper(response, exception);
			}
			finally
			{
				request?.Abort();
				response?.Close();
			}
		}

		public static async Task<HttpWebResponseWrapper> DownloadFile(string url, string host, string filePath, long rangeFrom, IProgress<DownloadProgress> downloadProgress, CancellationToken cancellationToken, int timeOut = 30000)
		{
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			try
			{
				request = HttpWebRequestInit(url, rangeFrom, timeOut);
				if (!string.IsNullOrWhiteSpace(host))
				{
					request.Host = host;
				}
				response = (HttpWebResponse)(await request.GetResponseAsync().ConfigureAwait(false));
				if (response.Cookies == null)
				{
					response.Cookies = new CookieCollection();
				}
				foreach (Cookie cookie in request.CookieContainer.GetCookies(request.RequestUri))
				{
					response.Cookies.Add(cookie);
				}
				return ProcessResponse(response, filePath, rangeFrom, downloadProgress, cancellationToken);
			}
			catch (Exception exception)
			{
				return new HttpWebResponseWrapper(response, exception);
			}
			finally
			{
				request?.Abort();
				response?.Close();
			}
		}

		private static HttpWebRequest HttpWebRequestInit(string url, long range, int timeOut)
		{
			CookieContainer cookieContainer = new CookieContainer();
			HttpsUtility.SetSecurity(url);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.AddRange(range);
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.AllowAutoRedirect = true;
			httpWebRequest.Timeout = timeOut;
			httpWebRequest.ReadWriteTimeout = timeOut;
			httpWebRequest.Method = "GET";
			httpWebRequest.CookieContainer = cookieContainer;
			return httpWebRequest;
		}

		private static HttpWebResponseWrapper ProcessResponse(HttpWebResponse response, string filePath, long rangeFrom, IProgress<DownloadProgress> downloadProgress, CancellationToken cancellationToken)
		{
			string text = filePath + ".tmp";
			long num = response.ContentLength + rangeFrom;
			string empty = string.Empty;
			using (Stream stream = response.GetResponseStream())
			{
				if (stream != null)
				{
					byte[] array = new byte[4096];
					DirectoryInfo parent = Directory.GetParent(text);
					if (parent == null)
					{
						return new HttpWebResponseWrapper(response, new DirectoryNotFoundException("download path directory is null"));
					}
					string fullName = parent.FullName;
					if (!Directory.Exists(fullName))
					{
						Directory.CreateDirectory(fullName);
					}
					using (FileStream fileStream = new FileStream(text, FileMode.Create))
					{
						long num2 = rangeFrom;
						DownloadProgress downloadProgress2 = new DownloadProgress(num2, num);
						int num3;
						while ((num3 = stream.Read(array, 0, array.Length)) > 0)
						{
							cancellationToken.ThrowIfCancellationRequested();
							fileStream.Write(array, 0, num3);
							num2 = (downloadProgress2.CurrentProgress = num2 + num3);
							downloadProgress.Report(downloadProgress2);
						}
						if (num != -1 && num != num2)
						{
							try
							{
								throw new FileNotConsistentException("File Not Consistent");
							}
							catch (FileNotConsistentException exception)
							{
								return new HttpWebResponseWrapper(response, exception);
							}
						}
					}
				}
			}
			File.Copy(text, filePath, true);
			File.Delete(text);
			return new HttpWebResponseWrapper(response, empty);
		}
	}
}
