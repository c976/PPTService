using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core
{
	
	
	public static class NetFramework
	{
		public static Version ClrVersion => Environment.Version;

		public static Task<Dictionary<string, NdpInfo>> GetInstalledVersionsAsync()
		{
			return NdpInfo.ReadFromRegistryAsync();
		}
	}
}
