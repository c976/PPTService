using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PPTService.Core.Net.Http
{
	
	
	[DataContract]
	public class HttpResponse
	{
		private string _html;

		public bool IsSuccessStatusCode
		{
			get
			{
				if (StatusCode != HttpStatusCode.OK)
				{
					return StatusCode == HttpStatusCode.PartialContent;
				}
				return true;
			}
		}

		public virtual bool IsSuccessBusiness => Error == null;

		public HttpStatusCode StatusCode { get; internal set; }

		public string Url { get; internal set; }

		public string Method { get; internal set; }

		public string Host { get; internal set; }

		public bool Success
		{
			get
			{
				if (IsSuccessStatusCode)
				{
					return IsSuccessBusiness;
				}
				return false;
			}
		}

		
		
		public Exception Error
		{
			
			get;
			
			internal set;
		}

		public string Html
		{
			get
			{
				return _html;
			}
			internal set
			{
				_html = value;
				if (!string.IsNullOrEmpty(_html))
				{
					ParseHtml(_html);
				}
			}
		}

		public CookieCollection Cookies { get; internal set; } = new CookieCollection();

		public WebHeaderCollection Headers { get; internal set; } = new WebHeaderCollection();

		public HttpResponse()
		{
		}

		
		public HttpResponse(HttpStatusCode statusCode, Exception error)
		{
			StatusCode = statusCode;
			Error = error;
		}

		protected virtual void ParseHtml(string html)
		{
		}
	}
}
