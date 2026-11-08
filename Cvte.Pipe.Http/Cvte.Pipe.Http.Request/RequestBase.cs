using System.Net;
using System.Runtime.Serialization;
namespace Cvte.Pipe.Http.Request
{
	[DataContract]
	public abstract class RequestBase
	{
		private const string HostPrefix = "http://";

		private string _host;

		private int _timeOut;

		protected static int DefaultTimeOut = 10000;

		public string Host
		{
			get
			{
				return _host;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (value.StartsWith("http://"))
					{
						_host = value;
					}
					else
					{
						_host = "http://" + value;
					}
				}
			}
		}

		public CookieCollection CookieCollection { get; set; }

		public WebHeaderCollection HeaderCollection { get; set; }

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

		public virtual bool IsParametersLegal => true;

		public virtual string ContentType => "application/x-www-form-urlencoded;charset=utf-8";

		public string UserAgent { get; set; }

		public virtual string RequestMethod => string.Empty;

		public virtual string GetRequestMethod()
		{
			string text;
			if (string.IsNullOrEmpty(RequestMethod))
			{
				text = ((!SA_Attr<ReqeustAttribute>.Has(GetType(), false)) ? ReqeustType.Post.ToString() : SA_Attr<ReqeustAttribute>.Get(GetType(), false).Type.ToString().ToUpper());
			}
			else
			{
				text = RequestMethod;
			}
			return text.ToUpper();
		}

		public virtual string GetRequestUrl()
		{
			if (!SA_Attr<ReqeustAttribute>.Has(GetType(), false))
			{
				return Host;
			}
			ReqeustAttribute reqeustAttribute = SA_Attr<ReqeustAttribute>.Get(GetType(), false);
			return Host + reqeustAttribute.ApiPrefix + reqeustAttribute.Api;
		}

		public abstract byte[] GetRequestData();
	}
}
