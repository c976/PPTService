using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices
{
	public class WebClientForResume : WebClient
	{
		private long _from;

		public WebClientForResume(long from = 0L)
		{
			_from = from;
		}

		public void SetHeaderRange(long from)
		{
			_from = from;
		}

		
		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest webRequest = base.GetWebRequest(address);
			HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
			if (httpWebRequest == null)
			{
				return webRequest;
			}
			httpWebRequest.ReadWriteTimeout = 30000;
			httpWebRequest.AddRange(_from);
			return httpWebRequest;
		}
	}
}
