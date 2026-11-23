using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace lindexi.src
{
	public class MacAddress
	{
		
		public static List<string> GetActiveMacAddress(string separator = "-")
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			if (allNetworkInterfaces == null || allNetworkInterfaces.Length < 1)
			{
				return new List<string>();
			}
			List<string> list = new List<string>();
			foreach (NetworkInterface item in allNetworkInterfaces.Where((NetworkInterface c) => c.NetworkInterfaceType != NetworkInterfaceType.Loopback && c.OperationalStatus == OperationalStatus.Up))
			{
				if (item.GetIPProperties().UnicastAddresses.Any((UnicastIPAddressInformation temp) => temp.Address.AddressFamily == AddressFamily.InterNetwork))
				{
					PhysicalAddress physicalAddress = item.GetPhysicalAddress();
					if (string.IsNullOrEmpty(separator))
					{
						list.Add(physicalAddress.ToString());
					}
					else
					{
						list.Add(string.Join(separator, physicalAddress.GetAddressBytes()));
					}
				}
			}
			return list;
		}
	}
}
