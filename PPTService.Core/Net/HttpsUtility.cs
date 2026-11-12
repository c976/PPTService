using System;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace PPTService.Core.Net
{
	
	
	internal static class HttpsUtility
	{
		internal static void SetSecurity(string url)
		{
			if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
			{
				ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			}
		}

		internal static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{
			return true;
		}
	}
}
