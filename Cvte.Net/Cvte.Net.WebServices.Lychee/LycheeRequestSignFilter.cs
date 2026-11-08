using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Lychee
{
	
	
	internal class LycheeRequestSignFilter : IHttpRequestFilter
	{
		private readonly string _appKey;

		private readonly string _signKey;

		public LycheeRequestSignFilter(string appKey, string signKey)
		{
			_appKey = appKey;
			_signKey = signKey;
		}

		public bool Match(HttpRequest request)
		{
			return true;
		}

		public void Handle(HttpRequest request)
		{
			LycheeHttpRequest lycheeHttpRequest = request as LycheeHttpRequest;
			if (lycheeHttpRequest != null)
			{
				lycheeHttpRequest.App = _appKey;
				lycheeHttpRequest.TimeStamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
				lycheeHttpRequest.Version = 1;
				lycheeHttpRequest.Sign = ComputeSign(lycheeHttpRequest);
				return;
			}
			UploadPptFileRequest uploadPptFileRequest = request as UploadPptFileRequest;
			if (uploadPptFileRequest != null)
			{
				uploadPptFileRequest.App = _appKey;
				uploadPptFileRequest.TimeStamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
				uploadPptFileRequest.Sign = ComputeSign(uploadPptFileRequest);
			}
		}

		private string ComputeSign(HttpRequest request)
		{
			return SecurityUtility.Hmacsha256((from p in request.GetParams()
				where p.Key != "sign"
				orderby p.Key
				select p).ToDictionary( (KeyValuePair<string, string> p) => p.Key,  (KeyValuePair<string, string> p) => p.Value).BuildQueryString(true), _signKey);
		}
	}
}
