using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PPTService.Core.Mcu
{
	internal class Usb
	{
		private IntPtr _hWriteFile;

		private IntPtr _hReadFile;

		private const uint GenericRead = 2147483648u;

		private const uint GenericWrite = 1073741824u;

		private const uint FileShareRead = 1u;

		private const uint FileShareWrite = 2u;

		private const uint FileAttributeNormal = 128u;

		private const uint OpenExisting = 3u;

		private const int MaxUsbDevices = 64;

		private const int InvalidHandleValue = -1;

		public bool FindUsb(int vid, int pid, string key)
		{
			string strSearch = $"vid_{vid:x4}&pid_{pid:x4}";
			return GetDeviceList().FirstOrDefault((string deviceItem) => deviceItem.IndexOf(strSearch, StringComparison.Ordinal) >= 0 && deviceItem.IndexOf(key, StringComparison.Ordinal) >= 0) != null;
		}

		public bool OnInitUsb(int vid, int pid, string key)
		{
			string strSearch = $"vid_{vid:x4}&pid_{pid:x4}";
			string text = GetDeviceList().FirstOrDefault((string deviceItem) => deviceItem.IndexOf(strSearch, StringComparison.Ordinal) >= 0 && deviceItem.IndexOf(key, StringComparison.Ordinal) >= 0);
			if (text == null)
			{
				return false;
			}
			_hWriteFile = WindowsApi.CreateFile(text, 1073741824u, 3u, 0u, 3u, 128u, 0);
			_hReadFile = WindowsApi.CreateFile(text, 2147483648u, 3u, 0u, 3u, 128u, 0);
			if (_hWriteFile.ToInt32() != -1)
			{
				return _hWriteFile.ToInt32() != -1;
			}
			return false;
		}

		public bool ReadUsbData(int length, out byte[] data)
		{
			uint lpNumberOfBytesRead = 0u;
			data = new byte[length];
			return WindowsApi.ReadFile(_hReadFile, data, (uint)length, ref lpNumberOfBytesRead, IntPtr.Zero);
		}

		public bool WriteUsbData(byte[] data)
		{
			uint lpNumberOfBytesWritten = 0u;
			return WindowsApi.WriteFile(_hWriteFile, data, (uint)data.Length, ref lpNumberOfBytesWritten, IntPtr.Zero);
		}

		public void CloseConnection()
		{
			if (_hWriteFile.ToInt32() != -1)
			{
				WindowsApi.CloseHandle(_hWriteFile);
				_hWriteFile = IntPtr.Zero;
			}
			if (_hWriteFile.ToInt32() != -1)
			{
				WindowsApi.CloseHandle(_hWriteFile);
				_hWriteFile = IntPtr.Zero;
			}
		}

		private IEnumerable<string> GetDeviceList()
		{
			List<string> list = new List<string>();
			Guid hidGuid = Guid.Empty;
			WindowsApi.GetDeviceGuid(ref hidGuid);
			IntPtr classDevOfHandle = WindowsApi.GetClassDevOfHandle(hidGuid);
			if (classDevOfHandle != IntPtr.Zero)
			{
				WindowsApi.SpDeviceInterfaceData interfaceInfo = default(WindowsApi.SpDeviceInterfaceData);
				interfaceInfo.CbSize = Marshal.SizeOf((object)interfaceInfo);
				for (uint num = 0u; num < 64; num++)
				{
					if (WindowsApi.GetEnumDeviceInterfaces(classDevOfHandle, ref hidGuid, num, ref interfaceInfo))
					{
						int buffsize = 0;
						WindowsApi.GetDeviceInterfaceDetail(classDevOfHandle, ref interfaceInfo, IntPtr.Zero, ref buffsize);
						IntPtr intPtr = Marshal.AllocHGlobal(buffsize);
						Marshal.StructureToPtr((object)new WindowsApi.SpDeviceInterfaceDetailData
						{
							cbSize = Marshal.SizeOf(typeof(WindowsApi.SpDeviceInterfaceDetailData))
						}, intPtr, false);
						if (WindowsApi.GetDeviceInterfaceDetail(classDevOfHandle, ref interfaceInfo, intPtr, ref buffsize))
						{
							list.Add(Marshal.PtrToStringAuto((IntPtr)((int)intPtr + 4)));
						}
						Marshal.FreeHGlobal(intPtr);
					}
				}
			}
			WindowsApi.DestroyDeviceInfoList(classDevOfHandle);
			return list;
		}
	}
}
