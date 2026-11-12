using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PPTService.Core.Net.Http;

namespace PPTService.Core.Net
{
	
	
	internal static class HttpUtility
	{
		internal const int DefaultTimeout = 30000;

		public static async Task<HttpWebResponseWrapper> UploadFile(string url, string fileName, string requestMethod, IDictionary<string, string> withParams, WebHeaderCollection headerCollection, CookieCollection cookieCollection, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken)
		{
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			Stream requestStream = null;
			try
			{
				CookieContainer cookieContainer = new CookieContainer();
				if (cookieCollection != null)
				{
					foreach (Cookie item in cookieCollection)
					{
						if (item.Value != null)
						{
							cookieContainer.Add(new Uri(url), item);
						}
					}
				}
				request = (HttpWebRequest)WebRequest.Create(url);
				string boundary = $"-----------{DateTime.Now.Ticks:x}";
				request.Headers = headerCollection ?? new WebHeaderCollection();
				request.ContentType = "multipart/form-data; boundary=" + boundary;
				request.KeepAlive = false;
				request.AllowAutoRedirect = true;
				request.AllowWriteStreamBuffering = false;
				request.Method = requestMethod;
				request.CookieContainer = cookieContainer;
				request.Headers.Add("Charsert", "UTF-8");
				request.Headers.Add("Cache-Control", "no-cache");
				request.Headers.Add("Pragma", "no-cache");
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item2 in withParams.Select( (KeyValuePair<string, string> withParam) => "--" + boundary + "\r\nContent-Disposition: form-data; name=\"" + withParam.Key + "\"\r\n\r\n" + withParam.Value + "\r\n"))
				{
					stringBuilder.Append(item2);
				}
				if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
				{
					string value = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"file\"; filename=\"" + Path.GetFileName(fileName) + "\"\r\nContent-Type: application/octet-stream\r\n\r\n";
					stringBuilder.Append(value);
					byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					string s = "\r\n--" + boundary + "--\r\n";
					byte[] bytes2 = Encoding.UTF8.GetBytes(s);
					using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
					{
						request.ContentLength = bytes.Length + fileStream.Length + bytes2.Length;
						request.ReadWriteTimeout = 30000;
						requestStream = request.GetRequestStreamAsync().WithTimeout(TimeSpan.FromSeconds(30.0)).Result;
						requestStream.Write(bytes, 0, bytes.Length);
						byte[] array = new byte[32768];
						long length = fileStream.Length;
						long num = 0L;
						int num2;
						while ((num2 = fileStream.Read(array, 0, array.Length)) != 0)
						{
							if (!cancellationToken.IsCancellationRequested)
							{
								requestStream.Write(array, 0, num2);
								num += num2;
								uploadProgress?.Report(new UploadProgress(num, length));
								continue;
							}
							throw new OperationCanceledException("Operation is cancelled");
						}
						requestStream.Write(bytes2, 0, bytes2.Length);
					}
				}
				else
				{
					byte[] postHeaderBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					string s2 = "\r\n--" + boundary + "--\r\n";
					byte[] boundaryBytes = Encoding.UTF8.GetBytes(s2);
					request.ContentLength = postHeaderBytes.Length + boundaryBytes.Length;
					requestStream = await request.GetRequestStreamAsync().WithTimeout(TimeSpan.FromSeconds(30.0)).ConfigureAwait(false);
					requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
					requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
				}
				response = (HttpWebResponse)(await request.GetResponseAsync().WithTimeout(TimeSpan.FromSeconds(30.0)).ConfigureAwait(false));
				string html = string.Empty;
				using (Stream stream = response.GetResponseStream())
				{
					if (stream != null)
					{
						using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
						{
							html = streamReader.ReadToEnd();
						}
					}
				}
				return new HttpWebResponseWrapper(response, html);
			}
			finally
			{
				requestStream?.Close();
				request?.Abort();
				response?.Close();
			}
		}

		public static async Task<HttpWebResponseWrapper> UploadFile(string url, IDictionary<string, string> dicFile, string requestMethod, IDictionary<string, string> withParams, WebHeaderCollection headerCollection, CookieCollection cookieCollection, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken)
		{
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			Stream requestStream = null;
			try
			{
				CookieContainer cookieContainer = new CookieContainer();
				if (cookieCollection != null)
				{
					foreach (Cookie item in cookieCollection)
					{
						if (item.Value != null)
						{
							cookieContainer.Add(new Uri(url), item);
						}
					}
				}
				request = (HttpWebRequest)WebRequest.Create(url);
				string boundary = $"-----------{DateTime.Now.Ticks:x}";
				request.Headers = headerCollection ?? new WebHeaderCollection();
				request.ContentType = "multipart/form-data; boundary=" + boundary;
				request.KeepAlive = false;
				request.AllowAutoRedirect = true;
				request.AllowWriteStreamBuffering = false;
				request.Method = requestMethod;
				request.CookieContainer = cookieContainer;
				request.Headers.Add("Charsert", "UTF-8");
				request.Headers.Add("Cache-Control", "no-cache");
				request.Headers.Add("Pragma", "no-cache");
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item2 in withParams.Select( (KeyValuePair<string, string> withParam) => "--" + boundary + "\r\nContent-Disposition: form-data; name=\"" + withParam.Key + "\"\r\n\r\n" + withParam.Value + "\r\n"))
				{
					stringBuilder.Append(item2);
				}
				byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
				long num = 0L;
				num += bytes.Length;
				long num2 = 0L;
				long num3 = 0L;
				foreach (KeyValuePair<string, string> item3 in dicFile)
				{
					if (!string.IsNullOrEmpty(item3.Value) && File.Exists(item3.Value))
					{
						string s = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"" + item3.Key + "\"; filename=\"" + Path.GetFileName(item3.Value) + "\"\r\nContent-Type: application/octet-stream\r\n\r\n";
						num += new FileInfo(item3.Value).Length;
						int num4 = Encoding.UTF8.GetBytes(s).Length;
						num2 += num4;
						num += num4;
					}
				}
				string s2 = "\r\n--" + boundary + "--\r\n";
				byte[] bytes2 = Encoding.UTF8.GetBytes(s2);
				num += bytes2.Length;
				request.ContentLength = num;
				request.ReadWriteTimeout = 30000;
				requestStream = request.GetRequestStreamAsync().WithTimeout(TimeSpan.FromSeconds(30.0)).Result;
				requestStream.Write(bytes, 0, bytes.Length);
				foreach (KeyValuePair<string, string> item4 in dicFile)
				{
					if (string.IsNullOrEmpty(item4.Value) || !File.Exists(item4.Value))
					{
						continue;
					}
					string s = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"" + item4.Key + "\"; filename=\"" + Path.GetFileName(item4.Value) + "\"\r\nContent-Type: application/octet-stream\r\n\r\n";
					byte[] bytes3 = Encoding.UTF8.GetBytes(s);
					requestStream.Write(bytes3, 0, bytes3.Length);
					using (FileStream fileStream = new FileStream(item4.Value, FileMode.Open, FileAccess.Read))
					{
						byte[] array = new byte[32768];
						int num5;
						while ((num5 = fileStream.Read(array, 0, array.Length)) != 0)
						{
							if (!cancellationToken.IsCancellationRequested)
							{
								requestStream.Write(array, 0, num5);
								num3 += num5;
								uploadProgress?.Report(new UploadProgress(num3, num2));
								continue;
							}
							throw new OperationCanceledException("Operation is cancelled");
						}
					}
				}
				requestStream.Write(bytes2, 0, bytes2.Length);
				response = (HttpWebResponse)(await request.GetResponseAsync().WithTimeout(TimeSpan.FromSeconds(30.0)).ConfigureAwait(false));
				string html = string.Empty;
				using (Stream stream = response.GetResponseStream())
				{
					if (stream != null)
					{
						using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
						{
							html = streamReader.ReadToEnd();
						}
					}
				}
				return new HttpWebResponseWrapper(response, html);
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
