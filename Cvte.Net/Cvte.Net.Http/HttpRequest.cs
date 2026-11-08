using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.Http
{
	
	
	[DataContract]
	public class HttpRequest
	{
		private int _timeOut;

		protected static int DefaultTimeOut = 30000;

		public string Url { get; set; } = string.Empty;

		public string RuntimeUrl { get; internal set; } = "";

		internal string Host { get; set; } = string.Empty;

		public string Referer { get; set; } = "";

		public CookieCollection CookieCollection { get; set; } = new CookieCollection();

		public WebHeaderCollection HeaderCollection { get; set; } = new WebHeaderCollection();

		public int TimeOut
		{
			get
			{
				if (_timeOut <= 0)
				{
					_timeOut = DefaultTimeOut;
				}
				return _timeOut;
			}
			set
			{
				_timeOut = value;
			}
		}

		public virtual string ContentType => "";

		public string UserAgent { get; set; }

		public string Method { get; internal set; }

		public virtual byte[] GetRequestData()
		{
			return new byte[0];
		}

		public virtual string GetQueryString()
		{
			return this.GetParams().BuildQueryString();
		}
	}
}
