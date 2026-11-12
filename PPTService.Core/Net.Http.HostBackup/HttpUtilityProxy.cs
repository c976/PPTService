using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Core.Net.Http.HostBackup
{
	
	
	internal class HttpUtilityProxy
	{
		private sealed class _003C_003Ec__DisplayClass6_0
		{
			
			public string requestMethod;

			
			public string userAgent;

			
			public WebHeaderCollection headerCollection;

			
			public CookieCollection cookieCollection;

			public int timeOut;

			
			public string referer;

			internal Task<HttpWebResponseWrapper> _003CGetWithBackupAsync_003Eg__GetFunc_007C0(string u, string h)
			{
				return HttpRequestUtility.Get(u, h, requestMethod, userAgent, headerCollection, cookieCollection, timeOut, referer);
			}
		}

		private sealed class _003C_003Ec__DisplayClass7_0
		{
			
			public string requestMethod;

			
			public string contentType;

			
			public string userAgent;

			
			public Func<byte[]> getPostData;

			
			public WebHeaderCollection headerCollection;

			
			public CookieCollection cookieCollection;

			public int timeOut;

			
			public string referer;

			internal Task<HttpWebResponseWrapper> _003CPostWithBackupAsync_003Eg__PostFunc_007C0(string u, string h)
			{
				return HttpRequestUtility.Post(u, h, requestMethod, contentType, userAgent, getPostData, headerCollection, cookieCollection, timeOut, referer);
			}
		}

		
		private readonly Func<IHttpRequestHostBackupFilter> _getHostBackupFilter;

		
		private IHttpRequestHostBackupFilter HostBackupFilter
		{
			
			get
			{
				return _getHostBackupFilter?.Invoke();
			}
		}

		public HttpUtilityProxy( Func<IHttpRequestHostBackupFilter> hostBackupFilter)
		{
			_getHostBackupFilter = hostBackupFilter;
		}

		public async Task<HttpWebResponseWrapper> GetAsync(string url, string requestMethod, string userAgent, WebHeaderCollection headerCollection, CookieCollection cookieCollection, int timeOut = 30000,  string referer = null)
		{
			return await GetWithBackupAsync(url, requestMethod, userAgent, headerCollection, cookieCollection, timeOut, referer);
		}

		public async Task<HttpWebResponseWrapper> PostAsync(string url, string requestMethod, string contentType, string userAgent, Func<byte[]> getPostData, WebHeaderCollection headerCollection, CookieCollection cookieCollection, int timeOut = 30000,  string referer = null)
		{
			return await PostWithBackupAsync(url, requestMethod, contentType, userAgent, getPostData, headerCollection, cookieCollection, timeOut, referer);
		}

		private Task<HttpWebResponseWrapper> GetWithBackupAsync(string url, string requestMethod, string userAgent, WebHeaderCollection headerCollection, CookieCollection cookieCollection, int timeOut, string referer)
		{
			_003C_003Ec__DisplayClass6_0 obj = new _003C_003Ec__DisplayClass6_0();
			obj.requestMethod = requestMethod;
			obj.userAgent = userAgent;
			obj.headerCollection = headerCollection;
			obj.cookieCollection = cookieCollection;
			obj.timeOut = timeOut;
			obj.referer = referer;
			return HttpHostBackupUtils.ActionByHostBackup(HostBackupFilter, obj._003CGetWithBackupAsync_003Eg__GetFunc_007C0, url, obj.requestMethod);
		}

		private Task<HttpWebResponseWrapper> PostWithBackupAsync(string url, string requestMethod, string contentType, string userAgent, Func<byte[]> getPostData, WebHeaderCollection headerCollection, CookieCollection cookieCollection, int timeOut, string referer)
		{
			_003C_003Ec__DisplayClass7_0 obj = new _003C_003Ec__DisplayClass7_0();
			obj.requestMethod = requestMethod;
			obj.contentType = contentType;
			obj.userAgent = userAgent;
			obj.getPostData = getPostData;
			obj.headerCollection = headerCollection;
			obj.cookieCollection = cookieCollection;
			obj.timeOut = timeOut;
			obj.referer = referer;
			return HttpHostBackupUtils.ActionByHostBackup(HostBackupFilter, obj._003CPostWithBackupAsync_003Eg__PostFunc_007C0, url, obj.requestMethod);
		}
	}
}
