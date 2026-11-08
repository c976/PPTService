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
		public delegate void SA_HtmlCallback(string html);

		[CompilerGenerated]
		private sealed class _0002
		{
			public dynamic _0001;





			internal void _0001_M6(IAsyncResult _0002)
			{
				try
				{
					HttpWebRequest httpWebRequest = (HttpWebRequest)_0002.AsyncState;
					this._0001 = httpWebRequest.EndGetRequestStream(_0002);
					this._0001.Write(this._0001, 0, this._0001.Length);
					this._0001.Close();
					this._0001 = (HttpWebResponse)httpWebRequest.GetResponse();
					string html = string.Empty;
					using (Stream stream = this._0001.GetResponseStream())
					{
						if (stream != null)
						{
							using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
							{
								html = streamReader.ReadToEnd();
							}
						}
					}
					if (this._0001 != null)
					{
						this._0001(html);
					}
				}
				catch (Exception)
				{
					if (this._0001 != null)
					{
						this._0001(null);
					}
				}
				finally
				{
					if (this._0001 != null)
					{
						this._0001.Close();
					}
					this._0001.Abort();
					if (this._0001 != null)
					{
						this._0001.Close();
					}
				}
			}
		}

		[CompilerGenerated]
		private sealed class _0003
		{
			public dynamic _0001;


			internal string _0001_M3(KeyValuePair<string, string> _0002)
			{
				return $"--{this._0001}\r\nContent-Disposition: form-data; name=\"{_0002.Key}\"\r\n\r\n{_0002.Value}\r\n";
			}
		}

		[CompilerGenerated]
		private sealed class _0004
		{
			public dynamic _0001;


			internal string _0001_M3(KeyValuePair<string, string> _0002)
			{
				return $"--{this._0001}\r\nContent-Disposition: form-data; name=\"{_0002.Key}\"\r\n\r\n{_0002.Value}\r\n";
			}
		}

		private const int m__0001 = 30000;

		public static string _0001(string _0002, string _0003, string _0004, string _0005, WebHeaderCollection _0006, CookieCollection _0007, int _0008 = 30000)
		{
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			try
			{
				CookieContainer cookieContainer = new CookieContainer();
				if (_0007 != null)
				{
					foreach (Cookie item in _0007)
					{
						cookieContainer.Add(new Uri(_0002), item);
					}
				}
				httpWebRequest = (HttpWebRequest)WebRequest.Create(_0002);
				httpWebRequest.Headers = _0006 ?? new WebHeaderCollection();
				httpWebRequest.KeepAlive = false;
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.Timeout = _0008;
				httpWebRequest.Method = _0003;
				httpWebRequest.CookieContainer = cookieContainer;
				if (!string.IsNullOrEmpty(_0005))
				{
					httpWebRequest.UserAgent = _0005;
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

		public static string _0001(string _0002, string _0003, string _0004, string _0005, Dictionary<string, string> _0006, WebHeaderCollection _0007, CookieCollection _0008, int _000E = 30000)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in _0006)
			{
				stringBuilder.AppendFormat((stringBuilder.Length == 0) ? "{0}={1}" : "&{0}={1}", item.Key, item.Value);
			}
			string text = stringBuilder.ToString();
			return _0001(_0002, _0003, _0004, _0005, text, _0007, _0008, _000E);
		}

		public static string _0001(string _0002, string _0003, string _0004, string _0005, string _0006, WebHeaderCollection _0007, CookieCollection _0008, int _000E = 30000)
		{
			return _0001(_0002, _0003, _0004, _0005, Encoding.UTF8.GetBytes(_0006), _0007, _0008, _000E);
		}

		public static string _0001(string _0002, string _0003, string _0004, string _0005, byte[] _0006, WebHeaderCollection _0007, CookieCollection _0008, int _000E = 30000)
		{
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			Stream stream = null;
			try
			{
				CookieContainer cookieContainer = new CookieContainer();
				if (_0008 != null)
				{
					foreach (Cookie item in _0008)
					{
						cookieContainer.Add(new Uri(_0002), item);
					}
				}
				httpWebRequest = (HttpWebRequest)WebRequest.Create(_0002);
				httpWebRequest.Headers = _0007 ?? new WebHeaderCollection();
				httpWebRequest.KeepAlive = false;
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.ServicePoint.Expect100Continue = false;
				httpWebRequest.Timeout = _000E;
				httpWebRequest.ReadWriteTimeout = _000E;
				httpWebRequest.Method = _0003;
				httpWebRequest.ContentType = _0004;
				httpWebRequest.ContentLength = _0006.Length;
				httpWebRequest.CookieContainer = cookieContainer;
				if (!string.IsNullOrEmpty(_0005))
				{
					httpWebRequest.UserAgent = _0005;
				}
				stream = httpWebRequest.GetRequestStream();
				stream.Write(_0006, 0, _0006.Length);
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

		public static void _0001(string _0002, string _0003, string _0004, byte[] _0005, CookieCollection _0006, int _0007, SA_HtmlCallback _0008)
		{
			HttpWebResponse httpWebResponse = null;
			Stream stream = null;
			CookieContainer cookieContainer = new CookieContainer();
			if (_0006 != null)
			{
				foreach (Cookie item in _0006)
				{
					cookieContainer.Add(new Uri(_0002), item);
				}
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_0002);
			httpWebRequest.KeepAlive = false;
			httpWebRequest.AllowAutoRedirect = true;
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.Method = _0003;
			httpWebRequest.ContentType = _0004;
			httpWebRequest.ContentLength = _0005.Length;
			httpWebRequest.CookieContainer = cookieContainer;
			try
			{
				httpWebRequest.BeginGetRequestStream((IAsyncResult asyncResult) =>
				{
					try
					{
						HttpWebRequest httpWebRequest2 = (HttpWebRequest)asyncResult.AsyncState;
						stream = httpWebRequest2.EndGetRequestStream(asyncResult);
						stream.Write(_0005, 0, _0005.Length);
						stream.Close();
						httpWebResponse = (HttpWebResponse)httpWebRequest2.GetResponse();
						string html = string.Empty;
						using (Stream stream2 = httpWebResponse.GetResponseStream())
						{
							if (stream2 != null)
							{
								using (StreamReader streamReader = new StreamReader(stream2, Encoding.UTF8))
								{
									html = streamReader.ReadToEnd();
								}
							}
						}
						if (_0008 != null)
						{
							_0008(html);
						}
					}
					catch (Exception)
					{
						if (_0008 != null)
						{
							_0008(null);
						}
					}
					finally
					{
						if (stream != null)
						{
							stream.Close();
						}
						httpWebRequest.Abort();
						if (httpWebResponse != null)
						{
							httpWebResponse.Close();
						}
					}
				}, httpWebRequest);
			}
			catch (Exception)
			{
				if (stream != null)
				{
					stream.Close();
				}
				httpWebRequest.Abort();
				if (httpWebResponse != null)
				{
					httpWebResponse.Close();
				}
			}
		}

		public static string _0001(string _0002, string _0003, string _0004, IDictionary<string, string> _0005, WebHeaderCollection _0006, Action<long, long> _0007)
		{
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			Stream stream = null;
			try
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(_0002);
				string arg = string.Format("-----------{0}", DateTime.Now.Ticks.ToString("x"));
				httpWebRequest.Headers = _0006 ?? new WebHeaderCollection();
				httpWebRequest.ContentType = $"multipart/form-data; boundary={arg}";
				httpWebRequest.KeepAlive = false;
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.AllowWriteStreamBuffering = false;
				httpWebRequest.Method = "POST";
				httpWebRequest.Headers.Add("Charsert", "UTF-8");
				httpWebRequest.Headers.Add("Cache-Control", "no-cache");
				httpWebRequest.Headers.Add("Pragma", "no-cache");
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item in _0005.Select((KeyValuePair<string, string> keyValuePair) => $"--{arg}\r\nContent-Disposition: form-data; name=\"{keyValuePair.Key}\"\r\n\r\n{keyValuePair.Value}\r\n"))
				{
					stringBuilder.Append(item);
				}
				if (!string.IsNullOrEmpty(_0004) && File.Exists(_0004))
				{
					string value = $"--{arg}\r\nContent-Disposition: form-data; name=\"{_0003}\"; filename=\"{Path.GetFileName(_0004)}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
					stringBuilder.Append(value);
					byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					string s = $"\r\n--{arg}--\r\n";
					byte[] bytes2 = Encoding.UTF8.GetBytes(s);
					using (FileStream fileStream = new FileStream(_0004, FileMode.Open, FileAccess.Read))
					{
						httpWebRequest.ContentLength = bytes.Length + fileStream.Length + bytes2.Length;
						int readWriteTimeout = (httpWebRequest.Timeout = ((fileStream.Length > 10485760) ? 1000000 : 30000));
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
							_0007?.Invoke(num2, length);
						}
						stream.Write(bytes2, 0, bytes2.Length);
					}
				}
				else
				{
					byte[] bytes3 = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					string s2 = $"\r\n--{arg}--\r\n";
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

		public static string _0001(string _0002, IDictionary<string, string> _0003, IDictionary<string, string> _0004, WebHeaderCollection _0005, Action<long, long> _0006)
		{
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			Stream stream = null;
			try
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(_0002);
				string arg = string.Format("-----------{0}", DateTime.Now.Ticks.ToString("x"));
				httpWebRequest.ContentType = $"multipart/form-data; boundary={arg}";
				httpWebRequest.KeepAlive = false;
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.AllowWriteStreamBuffering = false;
				httpWebRequest.Method = "POST";
				httpWebRequest.Headers = _0005 ?? new WebHeaderCollection();
				httpWebRequest.Headers.Add("Charsert", "UTF-8");
				httpWebRequest.Headers.Add("Cache-Control", "no-cache");
				httpWebRequest.Headers.Add("Pragma", "no-cache");
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item in _0004.Select((KeyValuePair<string, string> keyValuePair) => $"--{arg}\r\nContent-Disposition: form-data; name=\"{keyValuePair.Key}\"\r\n\r\n{keyValuePair.Value}\r\n"))
				{
					stringBuilder.Append(item);
				}
				if (!_0003.Any())
				{
					long arg2 = _0003.Values.Sum((string fileName) => new FileInfo(fileName).Length);
					foreach (KeyValuePair<string, string> item2 in _0003)
					{
						string value = item2.Value;
						string value2 = $"--{arg}\r\nContent-Disposition: form-data; name=\"{item2.Key}\"; filename=\"{Path.GetFileName(value)}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
						stringBuilder.Append(value2);
						byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
						string s = $"\r\n--{arg}--\r\n";
						byte[] bytes2 = Encoding.UTF8.GetBytes(s);
						using (FileStream fileStream = new FileStream(value, FileMode.Open, FileAccess.Read))
						{
							httpWebRequest.ContentLength = bytes.Length + fileStream.Length + bytes2.Length;
							int readWriteTimeout = (httpWebRequest.Timeout = ((fileStream.Length > 10485760) ? 1000000 : 30000));
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
								_0006?.Invoke(num2, arg2);
							}
							stream.Write(bytes2, 0, bytes2.Length);
						}
					}
				}
				else
				{
					byte[] bytes3 = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					string s2 = $"\r\n--{arg}--\r\n";
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

		public static void _0001(string _0002, string _0003, string _0004, string _0005, byte[] _0006, Action<long, long> _0007, Action<Exception> _0008, int _000E = 30000)
		{
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			Stream stream = null;
			try
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(_0002);
				httpWebRequest.Method = _0004;
				httpWebRequest.ContentType = _0005;
				httpWebRequest.ContentLength = _0006.Length;
				httpWebRequest.Timeout = _000E;
				httpWebRequest.ReadWriteTimeout = _000E;
				stream = httpWebRequest.GetRequestStream();
				stream.Write(_0006, 0, _0006.Length);
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
					string fullName = Directory.GetParent(_0003).FullName;
					if (!Directory.Exists(fullName))
					{
						Directory.CreateDirectory(fullName);
					}
					using (FileStream fileStream = new FileStream(_0003, FileMode.Create))
					{
						int num2;
						while ((num2 = stream2.Read(array, 0, array.Length)) > 0)
						{
							fileStream.Write(array, 0, num2);
							num += num2;
							_0007?.Invoke(num, contentLength);
						}
					}
				}
			}
			catch (Exception obj)
			{
				_0008?.Invoke(obj);
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
