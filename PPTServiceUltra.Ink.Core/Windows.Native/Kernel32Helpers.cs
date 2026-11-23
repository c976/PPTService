using System;

namespace PPTServiceUltra.Ink.Core.Windows.Native
{
	public static class Kernel32Helpers
	{
		public static class ProcessUserModeExceptionFilter
		{
			private const uint PROCESS_CALLBACK_FILTER_ENABLED = 1u;

			private static bool IsApiAvailable()
			{
				if (GetIsProcessorAMD64())
				{
					Version version = GetVersion();
					if (version.Major == 6 && version.Minor == 1)
					{
						return version.Build >= 7601;
					}
					return false;
				}
				return false;
			}

			public static bool Disable()
			{
				uint dwFlags;
				if (IsApiAvailable() && Win32.Kernel32.GetProcessUserModeExceptionPolicy(out dwFlags))
				{
					dwFlags &= 0xFFFFFFFEu;
					return Win32.Kernel32.SetProcessUserModeExceptionPolicy(dwFlags);
				}
				return false;
			}

			public static bool Enable()
			{
				uint dwFlags;
				if (IsApiAvailable() && Win32.Kernel32.GetProcessUserModeExceptionPolicy(out dwFlags))
				{
					dwFlags |= 1;
					return Win32.Kernel32.SetProcessUserModeExceptionPolicy(dwFlags);
				}
				return false;
			}
		}

		public static Version GetVersion()
		{
			uint version = Win32.Kernel32.GetVersion();
			uint build = ((version < 2147483648u) ? version.HighAsUInt() : 0u);
			return new Version((byte)version.Low(), (version.Low() >> 8) & 0xFF, (int)build);
		}

		public static bool IsWin8OrGreater(Version version = null)
		{
			if (version == null)
			{
				version = GetVersion();
			}
			if (version.Major > 5 && (version.Major > 6 || version.Minor > 1))
			{
				return true;
			}
			return false;
		}

		public static bool IsWin8Point1OrGreater(Version version = null)
		{
			if (version == null)
			{
				version = GetVersion();
			}
			if (version.Major > 5 && (version.Major > 6 || version.Minor > 2))
			{
				return true;
			}
			return false;
		}

		public static bool IsWin10OrGreater(Version version = null)
		{
			if (version == null)
			{
				version = GetVersion();
			}
			return version.Major > 6;
		}

		public static bool GetIsProcessorAMD64()
		{
			Win32.SystemInfo lpSystemInfo;
			Win32.Kernel32.GetNativeSystemInfo(out lpSystemInfo);
			return lpSystemInfo.ProcessorArchitecture == 9;
		}
	}
}
