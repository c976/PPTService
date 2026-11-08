using System.Runtime.InteropServices;

namespace PPTService
{
	public static class NetworkHelper
	{
		public static bool IsNetworkConnect
		{
			get
			{
				int dwFlag = 0;
				return InternetGetConnectedState(ref dwFlag, 0);
			}
		}

		[DllImport("winInet.dll")]
		private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);
	}
}
