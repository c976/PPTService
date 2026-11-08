using System;
using System.Runtime.InteropServices;

namespace Cvte.Mcu
{
	internal class WindowsApi
	{
		[StructLayout(LayoutKind.Sequential)]
		public class SpDevinfoData
		{
			public int cbSize = Marshal.SizeOf(typeof(SpDevinfoData));

			public Guid classGuid = Guid.Empty;

			public int devInst;

			public int reserved;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		internal struct SpDeviceInterfaceDetailData
		{
			internal int cbSize;

			internal short devicePath;
		}

		internal struct SpDeviceInterfaceData
		{
			public int CbSize;

			public Guid InterfaceClassGuid;

			public int Flags;

			public int Reserved;
		}

		[DllImport("hid.dll")]
		private static extern void HidD_GetHidGuid(ref Guid hidGuid);

		[DllImport("setupapi.dll", SetLastError = true)]
		private static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, uint enumerator, IntPtr hwndParent, Digcf flags);

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, uint memberIndex, ref SpDeviceInterfaceData deviceInterfaceData);

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SpDeviceInterfaceData deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, SpDevinfoData deviceInfoData);

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr CreateFile(string fileName, uint desiredAccess, uint shareMode, uint securityAttributes, uint creationDisposition, uint flagsAndAttributes, int hTemplateFile);

		[DllImport("Kernel32.dll")]
		internal static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToRead, ref uint lpNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("Kernel32.dll")]
		internal static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, ref uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

		[DllImport("kernel32.dll")]
		internal static extern bool CloseHandle(IntPtr hObject);

		internal static void GetDeviceGuid(ref Guid hidGuid)
		{
			HidD_GetHidGuid(ref hidGuid);
		}

		internal static IntPtr GetClassDevOfHandle(Guid hidGuid)
		{
			return SetupDiGetClassDevs(ref hidGuid, 0u, IntPtr.Zero, Digcf.DigcfPresent | Digcf.DigcfDeviceinterface);
		}

		internal static bool GetEnumDeviceInterfaces(IntPtr hidInfoSet, ref Guid hidGuid, uint index, ref SpDeviceInterfaceData interfaceInfo)
		{
			return SetupDiEnumDeviceInterfaces(hidInfoSet, IntPtr.Zero, ref hidGuid, index, ref interfaceInfo);
		}

		internal static bool GetDeviceInterfaceDetail(IntPtr hidInfoSet, ref SpDeviceInterfaceData interfaceInfo, IntPtr pDetail, ref int buffsize)
		{
			return SetupDiGetDeviceInterfaceDetail(hidInfoSet, ref interfaceInfo, pDetail, buffsize, ref buffsize, null);
		}

		internal static void DestroyDeviceInfoList(IntPtr hidInfoSet)
		{
			SetupDiDestroyDeviceInfoList(hidInfoSet);
		}
	}
}
