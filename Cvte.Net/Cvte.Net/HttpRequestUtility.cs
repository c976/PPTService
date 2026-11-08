using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cvte.Net
{
	
	
	internal class HttpRequestUtility
	{
		public static async Task<HttpWebResponseWrapper> Get(string url, string host, string requestMethod, string userAgent, WebHeaderCollection headerCollection, CookieCollection cookieCollection, int timeOut = 30000,  string referer = null)
		{
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			try
			{
				CookieContainer cookieContainer = new CookieContainer();
				if (cookieCollection != null)
				{
					foreach (Cookie item in cookieCollection)
					{
						cookieContainer.Add(new Uri(ChangeUrlHost(url, host)), item);
					}
				}
				if (url == null || string.IsNullOrWhiteSpace(url))
				{
					throw new ArgumentNullException("url", "request url is null or empty");
				}
				HttpsUtility.SetSecurity(url);
				request = (HttpWebRequest)WebRequest.Create(url);
				request.Headers = headerCollection ?? new WebHeaderCollection();
				request.ServicePoint.Expect100Continue = false;
				request.AllowAutoRedirect = true;
				request.Timeout = timeOut;
				request.Method = requestMethod;
				request.CookieContainer = cookieContainer;
				request.Referer = referer;
				if (!string.IsNullOrWhiteSpace(host))
				{
					request.Host = host;
				}
				if (!string.IsNullOrEmpty(userAgent))
				{
					request.UserAgent = userAgent;
				}
				response = (HttpWebResponse)(await request.GetResponseAsync().ConfigureAwait(false));
				string html = string.Empty;
				using (Stream stream = response.GetResponseStream())
				{
					if (stream != null)
					{
						using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
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
				request?.Abort();
				response?.Close();
			}
		}

		public static async Task<HttpWebResponseWrapper> Post(string url, string host, string requestMethod, string contentType, string userAgent, Func<byte[]> getPostData, WebHeaderCollection headerCollection, CookieCollection cookieCollection, int timeOut = 30000,  string referer = null)
		{
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			Stream requestStream = null;
			try
			{
				byte[] array = getPostData();
				CookieContainer cookieContainer = new CookieContainer();
				if (cookieCollection != null)
				{
					foreach (Cookie item in cookieCollection)
					{
						if (item.Value != null)
						{
							cookieContainer.Add(new Uri(ChangeUrlHost(url, host)), item);
						}
					}
				}
				HttpsUtility.SetSecurity(url);
				request = (HttpWebRequest)WebRequest.Create(url);
				request.Headers = headerCollection ?? new WebHeaderCollection();
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = timeOut;
				request.ReadWriteTimeout = timeOut;
				request.Method = requestMethod;
				request.ContentType = contentType;
				request.CookieContainer = cookieContainer;
				request.Referer = referer;
				if (!string.IsNullOrWhiteSpace(host))
				{
					request.Host = host;
				}
				if (!string.IsNullOrEmpty(userAgent))
				{
					request.UserAgent = userAgent;
				}
				if (array != null)
				{
					request.ContentLength = array.Length;
					Stream requestStream2;
					requestStream = (requestStream2 = request.GetRequestStream());
					using (requestStream2)
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeOut);
						await requestStream.WriteAsync(array, 0, array.Length, cancellationTokenSource.Token).ConfigureAwait(false);
					}
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
				string html = string.Empty;
				using (Stream stream = response.GetResponseStream())
				{
					if (stream != null)
					{
						using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
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

		private static string ChangeUrlHost(string url, string host)
		{
			if (string.IsNullOrWhiteSpace(host))
			{
				return url;
			}
			Uri uri = new Uri(url);
			return uri.Scheme + "://" + host + uri.PathAndQuery;
		}
	}
}
