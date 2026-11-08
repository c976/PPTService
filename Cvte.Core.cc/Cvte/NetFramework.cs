using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cvte
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
