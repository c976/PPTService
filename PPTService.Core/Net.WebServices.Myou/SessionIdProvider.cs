using System;
using System.Runtime.CompilerServices;

namespace PPTService.Core.Net.WebServices.Myou
{
	
	
	internal static class SessionIdProvider
	{
		public static string SessionId { get; private set; }

		static SessionIdProvider()
		{
			SessionId = Guid.NewGuid().ToString();
		}
	}
}
