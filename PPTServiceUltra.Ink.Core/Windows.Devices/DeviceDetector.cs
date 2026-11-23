using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Windows.Devices
{
	public static class DeviceDetector
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct SP_DEVINFO_DATA
		{
			public uint cbSize;

			public Guid ClassGuid;

			public uint DevInst;

			public UIntPtr Reserved;
		}

		private const uint DIGCF_PRESENT = 2u;

		private const uint SPDRP_DEVICEDESC = 0u;

		private const uint SPDRP_DRIVER = 9u;

		private const uint ERROR_INSUFFICIENT_BUFFER = 122u;

		private static readonly IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);

		[DllImport("SetupAPI.dll", CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetClassDevsW", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr SetupDiGetClassDevs([In] ref Guid ClassGuid, [In] string Enumerator, [In] IntPtr hwndParent, [In] uint Flags);

		[DllImport("SetupAPI.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetupDiEnumDeviceInfo([In] IntPtr DeviceInfoSet, [In] uint MemberIndex, [In][Out] ref SP_DEVINFO_DATA DeviceInfoData);

		[DllImport("SetupAPI.dll", CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetDeviceRegistryPropertyW", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetupDiGetDeviceRegistryProperty([In] IntPtr DeviceInfoSet, [In] ref SP_DEVINFO_DATA DeviceInfoData, [In] uint Property, out uint PropertyRegDataType, [In] IntPtr PropertyBuffer, [In] uint PropertyBufferSize, out uint RequiredSize);

		[DllImport("SetupAPI.dll", CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetDeviceInstanceIdW", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetupDiGetDeviceInstanceId([In] IntPtr DeviceInfoSet, [In] ref SP_DEVINFO_DATA DeviceInfoData, [In] IntPtr DeviceInstanceId, [In] uint DeviceInstanceIdSize, out uint RequiredSize);

		[DllImport("SetupAPI.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetupDiDestroyDeviceInfoList([In] IntPtr DeviceInfoSet);

		public static List<DeviceInfo> GetPresentDeviceInfos(Guid deviceSetupClass, string enumerator)
		{
			return GetPresentDeviceInfoEnumerable(deviceSetupClass, enumerator).ToList();
		}

		public static IEnumerable<DeviceInfo> GetPresentDeviceInfoEnumerable(Guid deviceSetupClass, string enumerator = null)
		{
			IntPtr devinfo = SetupDiGetClassDevs(ref deviceSetupClass, enumerator, IntPtr.Zero, 2u);
			if (!(devinfo != INVALID_HANDLE_VALUE))
			{
				yield break;
			}
			try
			{
				uint index = 0u;
				for (SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA
				{
					cbSize = (uint)Marshal.SizeOf(typeof(SP_DEVINFO_DATA))
				}; SetupDiEnumDeviceInfo(devinfo, index, ref deviceInfoData); index++)
				{
					yield return new DeviceInfo(GetDescription(devinfo, ref deviceInfoData), GetDeviceInstanceId(devinfo, ref deviceInfoData), GetDriverKey(devinfo, ref deviceInfoData));
				}
			}
			finally
			{
				SetupDiDestroyDeviceInfoList(devinfo);
			}
		}

		private unsafe static string GetDescription(IntPtr devinfo, [In] ref SP_DEVINFO_DATA deviceInfoData)
		{
			uint PropertyRegDataType;
			uint RequiredSize;
			SetupDiGetDeviceRegistryProperty(devinfo, ref deviceInfoData, 0u, out PropertyRegDataType, IntPtr.Zero, 0u, out RequiredSize);
			if ((long)Marshal.GetLastWin32Error() == 122)
			{
				byte* ptr = stackalloc byte[(int)RequiredSize];
				uint RequiredSize2;
				if (SetupDiGetDeviceRegistryProperty(devinfo, ref deviceInfoData, 0u, out PropertyRegDataType, (IntPtr)ptr, RequiredSize, out RequiredSize2))
				{
					return new string((char*)ptr);
				}
			}
			return string.Empty;
		}

		private unsafe static string GetDeviceInstanceId(IntPtr devinfo, [In] ref SP_DEVINFO_DATA deviceInfoData)
		{
			uint RequiredSize;
			SetupDiGetDeviceInstanceId(devinfo, ref deviceInfoData, IntPtr.Zero, 0u, out RequiredSize);
			if ((long)Marshal.GetLastWin32Error() == 122)
			{
				char* ptr = stackalloc char[(int)RequiredSize];
				uint RequiredSize2;
				if (SetupDiGetDeviceInstanceId(devinfo, ref deviceInfoData, (IntPtr)ptr, RequiredSize, out RequiredSize2))
				{
					return new string(ptr);
				}
			}
			return string.Empty;
		}

		private unsafe static string GetDriverKey(IntPtr devinfo, [In] ref SP_DEVINFO_DATA deviceInfoData)
		{
			uint PropertyRegDataType;
			uint RequiredSize;
			SetupDiGetDeviceRegistryProperty(devinfo, ref deviceInfoData, 9u, out PropertyRegDataType, IntPtr.Zero, 0u, out RequiredSize);
			if ((long)Marshal.GetLastWin32Error() == 122)
			{
				byte* ptr = stackalloc byte[(int)RequiredSize];
				uint RequiredSize2;
				if (SetupDiGetDeviceRegistryProperty(devinfo, ref deviceInfoData, 9u, out PropertyRegDataType, (IntPtr)ptr, RequiredSize, out RequiredSize2))
				{
					return new string((char*)ptr);
				}
			}
			return string.Empty;
		}
	}
}
