using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.FileExtension;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Files.Upload
{
	internal class UnitFileUploadUtility2
	{
		private const int DefaultTimeout = 30000;

		
		public static async Task<HttpWebResponseWrapper> UploadFile(string url, string host, string fileName, Stream fileStream, IDictionary<string, string> withParams, WebHeaderCollection headerCollection, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken, string fileKey = "file", int timeOut = 100000)
		{
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			Stream requestStream = null;
			try
			{
				HttpsUtility.SetSecurity(url);
				request = (HttpWebRequest)WebRequest.Create(url);
				string boundary = "-----------" + DateTime.Now.Ticks.ToString("x");
				request.Headers = headerCollection ?? new WebHeaderCollection();
				request.ContentType = "multipart/form-data; boundary=" + boundary;
				request.ServicePoint.Expect100Continue = false;
				request.KeepAlive = false;
				request.AllowAutoRedirect = true;
				request.AllowWriteStreamBuffering = false;
				request.Method = "POST";
				request.Headers.Add("Charsert", "UTF-8");
				request.Headers.Add("Cache-Control", "no-cache");
				request.Headers.Add("Pragma", "no-cache");
				request.Timeout = timeOut;
				if (!string.IsNullOrWhiteSpace(host))
				{
					request.Host = host;
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item in withParams.Select((KeyValuePair<string, string> withParam) => "--" + boundary + "\r\nContent-Disposition: form-data; name=\"" + withParam.Key + "\"\r\n\r\n" + withParam.Value + "\r\n"))
				{
					stringBuilder.Append(item);
				}
				if (fileStream != null)
				{
					string text = FileExtensionFactory.Default.MimeTypeFactory.BuildFileMimeType(fileName);
					string value = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"" + fileKey + "\"; filename=\"" + fileName + "\"\r\nContent-Type: " + text + "\r\n\r\n";
					stringBuilder.Append(value);
					byte[] postHeaderBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					string s = "\r\n--" + boundary + "--\r\n";
					byte[] boundaryBytes = Encoding.UTF8.GetBytes(s);
					fileStream.Seek(0L, SeekOrigin.Begin);
					request.ContentLength = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;
					request.ReadWriteTimeout = 30000;
					requestStream = await request.GetRequestStreamAsync().WithTimeout(TimeSpan.FromMilliseconds(30000.0)).ConfigureAwait(false);
					await requestStream.WriteAsync(postHeaderBytes, 0, postHeaderBytes.Length, cancellationToken).ConfigureAwait(false);
					byte[] buffer = new byte[32768];
					long total = fileStream.Length;
					long done = 0L;
					while (true)
					{
						int num;
						int length = (num = await fileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false));
						if (num == 0)
						{
							break;
						}
						if (cancellationToken.IsCancellationRequested)
						{
							throw new OperationCanceledException("Operation is cancelled");
						}
						await requestStream.WriteAsync(buffer, 0, length, cancellationToken).ConfigureAwait(false);
						done += length;
						uploadProgress?.Report(new UploadProgress(done, total));
					}
					await requestStream.WriteAsync(boundaryBytes, 0, boundaryBytes.Length, cancellationToken).ConfigureAwait(false);
				}
				else
				{
					byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					string s2 = "\r\n--" + boundary + "--\r\n";
					byte[] buffer = Encoding.UTF8.GetBytes(s2);
					request.ContentLength = bytes.Length + buffer.Length;
					requestStream = request.GetRequestStream();
					await requestStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false);
					await requestStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
				}
				response = (HttpWebResponse)(await request.GetResponseAsync().ConfigureAwait(false));
				string html = string.Empty;
				using (Stream responseStream = response.GetResponseStream())
				{
					if (responseStream != null)
					{
						using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
						{
							html = await reader.ReadToEndAsync().ConfigureAwait(false);
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
				requestStream?.Close();
				request?.Abort();
				response?.Close();
			}
		}
	}
}
