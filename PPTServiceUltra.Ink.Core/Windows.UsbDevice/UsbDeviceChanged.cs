using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using PPTServiceUltra.Ink.Core.Linq;

namespace PPTServiceUltra.Ink.Core.Windows.UsbDevice
{
	internal class UsbDeviceChanged : IUsbDeviceUpdated
	{
		private struct DEV_BROADCAST_HDR
		{
			public int dbch_size;

			public int dbch_devicetype;

			public int dbch_reserved;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct DEV_BROADCAST_DEVICEINTERFACE
		{
			public int dbcc_size;

			public int dbcc_devicetype;

			public int dbcc_reserved;

			public Guid dbcc_classguid;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public char[] dbcc_name;
		}

		private const int DBT_DEVTYP_DEVICEINTERFACE = 5;

		private IntPtr _handleRegisted = IntPtr.Zero;

		public event EventHandler<DeviceUpdatedEventArgs> DeviceUpdated;

		public bool AddDeviceChangedHook(Window window)
		{
			bool result = false;
			HwndSource hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
			if (hwndSource != null)
			{
				IntPtr handle = hwndSource.Handle;
				hwndSource.AddHook(HwndHandler);
				result = RegisterUsbDeviceNotification(handle);
			}
			return result;
		}

		public int RemoveDeviceChangeHook()
		{
			int result = 0;
			if (IntPtr.Zero != _handleRegisted)
			{
				result = UnregisterDeviceNotification(_handleRegisted);
				_handleRegisted = IntPtr.Zero;
			}
			return result;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

		[DllImport("user32.dll")]
		private static extern int UnregisterDeviceNotification(IntPtr handle);

		private bool RegisterUsbDeviceNotification(IntPtr windowHandle)
		{
			UsbDeviceInfo.DevBroadcastDeviceinterface devBroadcastDeviceinterface = new UsbDeviceInfo.DevBroadcastDeviceinterface
			{
				DeviceType = 5,
				Reserved = 0,
				ClassGuid = UsbDeviceInfo.GuidDevInterfaceUSBDevice,
				Name = 0
			};
			devBroadcastDeviceinterface.Size = Marshal.SizeOf((object)devBroadcastDeviceinterface);
			IntPtr intPtr = Marshal.AllocHGlobal(devBroadcastDeviceinterface.Size);
			Marshal.StructureToPtr((object)devBroadcastDeviceinterface, intPtr, true);
			_handleRegisted = RegisterDeviceNotification(windowHandle, intPtr, 0);
			return _handleRegisted != IntPtr.Zero;
		}

		private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
		{
			if (msg == 537)
			{
				switch ((int)wparam)
				{
				case 7:
				{
					DeviceUpdatedEventArgs args2 = new DeviceUpdatedEventArgs(DeviceUpdatedType.DeviceChanged);
					OnDeviceUpdated(args2);
					break;
				}
				case 32772:
				{
					DeviceUpdatedEventArgs args = new DeviceUpdatedEventArgs(DeviceUpdatedType.DeviceRemoved);
					OnDeviceUpdated(args);
					break;
				}
				case 32768:
				{
					DeviceUpdatedEventArgs e = null;
					if (((DEV_BROADCAST_HDR)Marshal.PtrToStructure(lparam, typeof(DEV_BROADCAST_HDR))).dbch_devicetype == 5)
					{
						DEV_BROADCAST_DEVICEINTERFACE dEV_BROADCAST_DEVICEINTERFACE = (DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lparam, typeof(DEV_BROADCAST_DEVICEINTERFACE));
						int num = dEV_BROADCAST_DEVICEINTERFACE.dbcc_name.FirstIndexOf((char c) => c == '\0');
						string deviceName = string.Empty;
						if (num > 0)
						{
							deviceName = new string(dEV_BROADCAST_DEVICEINTERFACE.dbcc_name, 0, num);
						}
						e = new DeviceUpdatedEventArgs(DeviceUpdatedType.DeviceArrival, dEV_BROADCAST_DEVICEINTERFACE.dbcc_classguid, deviceName);
					}
					if (e == null)
					{
						e = new DeviceUpdatedEventArgs(DeviceUpdatedType.DeviceArrival);
					}
					OnDeviceUpdated(e);
					break;
				}
				}
			}
			handled = false;
			return IntPtr.Zero;
		}

		private void OnDeviceUpdated(DeviceUpdatedEventArgs args)
		{
			DeviceUpdated?.Invoke(this, args);
		}
	}
}
