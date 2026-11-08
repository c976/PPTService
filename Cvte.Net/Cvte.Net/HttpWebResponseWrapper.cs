using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace Cvte.Net
{
	
	
	public class HttpWebResponseWrapper
	{
		public HttpWebResponse Response { get; }

		public Exception Exception { get; private set; }

		
		
		public string Html
		{
			
			get;
			
			private set;
		} = "";

		public bool IsCanceled => Exception is OperationCanceledException;

		internal bool Success => Exception == null;

		
		protected internal HttpWebResponseWrapper(HttpWebResponse response, string html)
		{
			Response = response;
			Html = html;
		}

		
		protected internal HttpWebResponseWrapper( HttpWebResponse response, Exception exception)
		{
			if (response == null)
			{
				WebException ex = exception as WebException;
				if (ex != null)
				{
					HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
					if (httpWebResponse != null)
					{
						response = httpWebResponse;
					}
				}
			}
			Response = response;
			Exception = exception;
		}
	}
}
