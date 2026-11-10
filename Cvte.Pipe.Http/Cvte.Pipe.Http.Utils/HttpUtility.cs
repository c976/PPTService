using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cvte.Pipe.Http.Utils
{
	internal static class HttpUtility
	{
		public delegate void Delegate0(string html);

		private sealed class Class8
		{
			public Stream stream_0;

			public byte[] byte_0;

			public HttpWebResponse httpWebResponse_0;

			public Delegate0 delegate0_0;

			public HttpWebRequest httpWebRequest_0;

			internal void method_0(IAsyncResult iasyncResult_0)
			{
				try
				{
					HttpWebRequest httpWebRequest = (HttpWebRequest)iasyncResult_0.AsyncState;
					stream_0 = httpWebRequest.EndGetRequestStream(iasyncResult_0);
					stream_0.Write(byte_0, 0, byte_0.Length);
					stream_0.Close();
					httpWebResponse_0 = (HttpWebResponse)httpWebRequest.GetResponse();
					string html = string.Empty;
					using (Stream stream = httpWebResponse_0.GetResponseStream())
					{
						if (stream != null)
						{
							using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
							{
								html = streamReader.ReadToEnd();
							}
						}
					}
					if (delegate0_0 != null)
					{
						delegate0_0(html);
					}
				}
				catch (Exception)
				{
					if (delegate0_0 != null)
					{
						delegate0_0(null);
					}
				}
				finally
				{
					if (stream_0 != null)
					{
						stream_0.Close();
					}
					httpWebRequest_0.Abort();
					if (httpWebResponse_0 != null)
					{
						httpWebResponse_0.Close();
					}
				}
			}
		}

		private sealed class Class9
		{
			public string string_0;

			public Func<KeyValuePair<string, string>, string> func_0;

			internal string method_0(KeyValuePair<string, string> keyValuePair_0)
			{
				return $"--{string_0}\r\nContent-Disposition: form-data; name=\"{keyValuePair_0.Key}\"\r\n\r\n{keyValuePair_0.Value}\r\n";
			}
		}

		private sealed class Class10
		{
			public string string_0;

			public Func<KeyValuePair<string, string>, string> func_0;

			internal string method_0(KeyValuePair<string, string> keyValuePair_0)
			{
				return $"--{string_0}\r\nContent-Disposition: form-data; name=\"{keyValuePair_0.Key}\"\r\n\r\n{keyValuePair_0.Value}\r\n";
			}
		}

		private const int int_0 = 30000;

		public static string smethod_0(string string_0, string string_1, string string_2, string string_3, WebHeaderCollection webHeaderCollection_0, CookieCollection cookieCollection_0, int int_1 = 30000)
		{
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			try
			{
				CookieContainer cookieContainer = new CookieContainer();
				if (cookieCollection_0 != null)
				{
					foreach (Cookie item in cookieCollection_0)
					{
						cookieContainer.Add(new Uri(string_0), item);
					}
				}
				httpWebRequest = (HttpWebRequest)WebRequest.Create(string_0);
				httpWebRequest.Headers = webHeaderCollection_0 ?? new WebHeaderCollection();
				httpWebRequest.KeepAlive = false;
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.Timeout = int_1;
				httpWebRequest.Method = string_1;
				httpWebRequest.CookieContainer = cookieContainer;
				if (!string.IsNullOrEmpty(string_3))
				{
					httpWebRequest.UserAgent = string_3;
				}
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				string result = string.Empty;
				using (Stream stream = httpWebResponse.GetResponseStream())
				{
					if (stream != null)
					{
						using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
						{
							result = streamReader.ReadToEnd();
						}
					}
				}
				return result;
			}
			finally
			{
				httpWebRequest?.Abort();
				httpWebResponse?.Close();
			}
		}

		public static string smethod_1(string string_0, string string_1, string string_2, string string_3, Dictionary<string, string> dictionary_0, WebHeaderCollection webHeaderCollection_0, CookieCollection cookieCollection_0, int int_1 = 30000)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in dictionary_0)
			{
				stringBuilder.AppendFormat((stringBuilder.Length == 0) ? "{0}={1}" : "&{0}={1}", item.Key, item.Value);
			}
			string string_4 = stringBuilder.ToString();
			return smethod_2(string_0, string_1, string_2, string_3, string_4, webHeaderCollection_0, cookieCollection_0, int_1);
		}

		public static string smethod_2(string string_0, string string_1, string string_2, string string_3, string string_4, WebHeaderCollection webHeaderCollection_0, CookieCollection cookieCollection_0, int int_1 = 30000)
		{
			return smethod_3(string_0, string_1, string_2, string_3, Encoding.UTF8.GetBytes(string_4), webHeaderCollection_0, cookieCollection_0, int_1);
		}

		public static string smethod_3(string string_0, string string_1, string string_2, string string_3, byte[] byte_0, WebHeaderCollection webHeaderCollection_0, CookieCollection cookieCollection_0, int int_1 = 30000)
		{
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			Stream stream = null;
			try
			{
				CookieContainer cookieContainer = new CookieContainer();
				if (cookieCollection_0 != null)
				{
					foreach (Cookie item in cookieCollection_0)
					{
						cookieContainer.Add(new Uri(string_0), item);
					}
				}
				httpWebRequest = (HttpWebRequest)WebRequest.Create(string_0);
				httpWebRequest.Headers = webHeaderCollection_0 ?? new WebHeaderCollection();
				httpWebRequest.KeepAlive = false;
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.ServicePoint.Expect100Continue = false;
				httpWebRequest.Timeout = int_1;
				httpWebRequest.ReadWriteTimeout = int_1;
				httpWebRequest.Method = string_1;
				httpWebRequest.ContentType = string_2;
				httpWebRequest.ContentLength = byte_0.Length;
				httpWebRequest.CookieContainer = cookieContainer;
				if (!string.IsNullOrEmpty(string_3))
				{
					httpWebRequest.UserAgent = string_3;
				}
				stream = httpWebRequest.GetRequestStream();
				stream.Write(byte_0, 0, byte_0.Length);
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				string result = string.Empty;
				using (Stream stream2 = httpWebResponse.GetResponseStream())
				{
					if (stream2 == null)
					{
						return result;
					}
					using (StreamReader streamReader = new StreamReader(stream2, Encoding.UTF8))
					{
						result = streamReader.ReadToEnd();
					}
				}
				return result;
			}
			finally
			{
				stream?.Close();
				httpWebRequest?.Abort();
				httpWebResponse?.Close();
			}
		}

		public static void smethod_4(string string_0, string string_1, string string_2, byte[] byte_0, CookieCollection cookieCollection_0, int int_1, Delegate0 delegate0_0)
		{
			HttpWebResponse httpWebResponse_0 = null;
			Stream stream_0 = null;
			CookieContainer cookieContainer = new CookieContainer();
			if (cookieCollection_0 != null)
			{
				foreach (Cookie item in cookieCollection_0)
				{
					cookieContainer.Add(new Uri(string_0), item);
				}
			}
			HttpWebRequest httpWebRequest_0 = (HttpWebRequest)WebRequest.Create(string_0);
			httpWebRequest_0.KeepAlive = false;
			httpWebRequest_0.AllowAutoRedirect = true;
			httpWebRequest_0.ServicePoint.Expect100Continue = false;
			httpWebRequest_0.Method = string_1;
			httpWebRequest_0.ContentType = string_2;
			httpWebRequest_0.ContentLength = byte_0.Length;
			httpWebRequest_0.CookieContainer = cookieContainer;
			try
			{
				httpWebRequest_0.BeginGetRequestStream((IAsyncResult iasyncResult_0) =>
				{
					try
					{
						HttpWebRequest httpWebRequest = (HttpWebRequest)iasyncResult_0.AsyncState;
						stream_0 = httpWebRequest.EndGetRequestStream(iasyncResult_0);
						stream_0.Write(byte_0, 0, byte_0.Length);
						stream_0.Close();
						httpWebResponse_0 = (HttpWebResponse)httpWebRequest.GetResponse();
						string html = string.Empty;
						using (Stream stream = httpWebResponse_0.GetResponseStream())
						{
							if (stream != null)
							{
								using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
								{
									html = streamReader.ReadToEnd();
								}
							}
						}
						if (delegate0_0 != null)
						{
							delegate0_0(html);
						}
					}
					catch (Exception)
					{
						if (delegate0_0 != null)
						{
							delegate0_0(null);
						}
					}
					finally
					{
						if (stream_0 != null)
						{
							stream_0.Close();
						}
						httpWebRequest_0.Abort();
						if (httpWebResponse_0 != null)
						{
							httpWebResponse_0.Close();
						}
					}
				}, httpWebRequest_0);
			}
			catch (Exception)
			{
				if (stream_0 != null)
				{
					stream_0.Close();
				}
				httpWebRequest_0.Abort();
				if (httpWebResponse_0 != null)
				{
					httpWebResponse_0.Close();
				}
			}
		}

		public static string smethod_5(string string_0, string string_1, string string_2, IDictionary<string, string> idictionary_0, WebHeaderCollection webHeaderCollection_0, Action<long, long> action_0)
		{
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			Stream stream = null;
			try
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(string_0);
				string string_3 = string.Format("-----------{0}", DateTime.Now.Ticks.ToString("x"));
				httpWebRequest.Headers = webHeaderCollection_0 ?? new WebHeaderCollection();
				httpWebRequest.ContentType = $"multipart/form-data; boundary={string_3}";
				httpWebRequest.KeepAlive = false;
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.AllowWriteStreamBuffering = false;
				httpWebRequest.Method = "POST";
				httpWebRequest.Headers.Add("Charsert", "UTF-8");
				httpWebRequest.Headers.Add("Cache-Control", "no-cache");
				httpWebRequest.Headers.Add("Pragma", "no-cache");
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item in idictionary_0.Select((KeyValuePair<string, string> keyValuePair_0) => $"--{string_3}\r\nContent-Disposition: form-data; name=\"{keyValuePair_0.Key}\"\r\n\r\n{keyValuePair_0.Value}\r\n"))
				{
					stringBuilder.Append(item);
				}
				if (!string.IsNullOrEmpty(string_2) && File.Exists(string_2))
				{
					string value = $"--{string_3}\r\nContent-Disposition: form-data; name=\"{string_1}\"; filename=\"{Path.GetFileName(string_2)}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
					stringBuilder.Append(value);
					byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					string s = $"\r\n--{string_3}--\r\n";
					byte[] bytes2 = Encoding.UTF8.GetBytes(s);
					using (FileStream fileStream = new FileStream(string_2, FileMode.Open, FileAccess.Read))
					{
						httpWebRequest.ContentLength = bytes.Length + fileStream.Length + bytes2.Length;
						int readWriteTimeout = (httpWebRequest.Timeout = ((fileStream.Length > 10485760L) ? 1000000 : 30000));
						httpWebRequest.ReadWriteTimeout = readWriteTimeout;
						stream = httpWebRequest.GetRequestStream();
						stream.Write(bytes, 0, bytes.Length);
						byte[] array = new byte[32768];
						long length = fileStream.Length;
						long num2 = 0L;
						int num3;
						while ((num3 = fileStream.Read(array, 0, array.Length)) != 0)
						{
							stream.Write(array, 0, num3);
							num2 += num3;
							action_0?.Invoke(num2, length);
						}
						stream.Write(bytes2, 0, bytes2.Length);
					}
				}
				else
				{
					byte[] bytes3 = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					string s2 = $"\r\n--{string_3}--\r\n";
					byte[] bytes4 = Encoding.UTF8.GetBytes(s2);
					httpWebRequest.ContentLength = bytes3.Length + bytes4.Length;
					stream = httpWebRequest.GetRequestStream();
					stream.Write(bytes3, 0, bytes3.Length);
					stream.Write(bytes4, 0, bytes4.Length);
				}
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				string result = string.Empty;
				using (Stream stream2 = httpWebResponse.GetResponseStream())
				{
					if (stream2 != null)
					{
						using (StreamReader streamReader = new StreamReader(stream2, Encoding.UTF8))
						{
							result = streamReader.ReadToEnd();
						}
					}
				}
				return result;
			}
			finally
			{
				stream?.Close();
				httpWebRequest?.Abort();
				httpWebResponse?.Close();
			}
		}

		public static string smethod_6(string string_0, IDictionary<string, string> idictionary_0, IDictionary<string, string> idictionary_1, WebHeaderCollection webHeaderCollection_0, Action<long, long> action_0)
		{
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			Stream stream = null;
			try
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(string_0);
				string string_1 = string.Format("-----------{0}", DateTime.Now.Ticks.ToString("x"));
				httpWebRequest.ContentType = $"multipart/form-data; boundary={string_1}";
				httpWebRequest.KeepAlive = false;
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.AllowWriteStreamBuffering = false;
				httpWebRequest.Method = "POST";
				httpWebRequest.Headers = webHeaderCollection_0 ?? new WebHeaderCollection();
				httpWebRequest.Headers.Add("Charsert", "UTF-8");
				httpWebRequest.Headers.Add("Cache-Control", "no-cache");
				httpWebRequest.Headers.Add("Pragma", "no-cache");
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item in idictionary_1.Select((KeyValuePair<string, string> keyValuePair_0) => $"--{string_1}\r\nContent-Disposition: form-data; name=\"{keyValuePair_0.Key}\"\r\n\r\n{keyValuePair_0.Value}\r\n"))
				{
					stringBuilder.Append(item);
				}
				if (!idictionary_0.Any())
				{
					long arg = idictionary_0.Values.Sum((string fileName) => new FileInfo(fileName).Length);
					foreach (KeyValuePair<string, string> item2 in idictionary_0)
					{
						string value = item2.Value;
						string value2 = $"--{string_1}\r\nContent-Disposition: form-data; name=\"{item2.Key}\"; filename=\"{Path.GetFileName(value)}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
						stringBuilder.Append(value2);
						byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
						string s = $"\r\n--{string_1}--\r\n";
						byte[] bytes2 = Encoding.UTF8.GetBytes(s);
						using (FileStream fileStream = new FileStream(value, FileMode.Open, FileAccess.Read))
						{
							httpWebRequest.ContentLength = bytes.Length + fileStream.Length + bytes2.Length;
							int readWriteTimeout = (httpWebRequest.Timeout = ((fileStream.Length > 10485760L) ? 1000000 : 30000));
							httpWebRequest.ReadWriteTimeout = readWriteTimeout;
							stream = httpWebRequest.GetRequestStream();
							stream.Write(bytes, 0, bytes.Length);
							byte[] array = new byte[32768];
							long num2 = 0L;
							int num3;
							while ((num3 = fileStream.Read(array, 0, array.Length)) != 0)
							{
								stream.Write(array, 0, num3);
								num2 += num3;
								action_0?.Invoke(num2, arg);
							}
							stream.Write(bytes2, 0, bytes2.Length);
						}
					}
				}
				else
				{
					byte[] bytes3 = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					string s2 = $"\r\n--{string_1}--\r\n";
					byte[] bytes4 = Encoding.UTF8.GetBytes(s2);
					httpWebRequest.ContentLength = bytes3.Length + bytes4.Length;
					stream = httpWebRequest.GetRequestStream();
					stream.Write(bytes3, 0, bytes3.Length);
					stream.Write(bytes4, 0, bytes4.Length);
				}
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				string result = string.Empty;
				using (Stream stream2 = httpWebResponse.GetResponseStream())
				{
					if (stream2 != null)
					{
						using (StreamReader streamReader = new StreamReader(stream2, Encoding.UTF8))
						{
							result = streamReader.ReadToEnd();
						}
					}
				}
				return result;
			}
			finally
			{
				stream?.Close();
				httpWebRequest?.Abort();
				httpWebResponse?.Close();
			}
		}

		public static void smethod_7(string string_0, string string_1, string string_2, string string_3, byte[] byte_0, Action<long, long> action_0, Action<Exception> action_1, int int_1 = 30000)
		{
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			Stream stream = null;
			try
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(string_0);
				httpWebRequest.Method = string_2;
				httpWebRequest.ContentType = string_3;
				httpWebRequest.ContentLength = byte_0.Length;
				httpWebRequest.Timeout = int_1;
				httpWebRequest.ReadWriteTimeout = int_1;
				stream = httpWebRequest.GetRequestStream();
				stream.Write(byte_0, 0, byte_0.Length);
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				using (Stream stream2 = httpWebResponse.GetResponseStream())
				{
					if (stream2 == null)
					{
						return;
					}
					byte[] array = new byte[4096];
					long contentLength = httpWebResponse.ContentLength;
					int num = 0;
					string fullName = Directory.GetParent(string_1).FullName;
					if (!Directory.Exists(fullName))
					{
						Directory.CreateDirectory(fullName);
					}
					using (FileStream fileStream = new FileStream(string_1, FileMode.Create))
					{
						int num2;
						while ((num2 = stream2.Read(array, 0, array.Length)) > 0)
						{
							fileStream.Write(array, 0, num2);
							num += num2;
							action_0?.Invoke(num, contentLength);
						}
					}
				}
			}
			catch (Exception obj)
			{
				action_1?.Invoke(obj);
			}
			finally
			{
				stream?.Close();
				httpWebRequest?.Abort();
				httpWebResponse?.Close();
			}
		}
	}
}
