using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Cvte.Paint.Inks.Hardware.RawTouchEngine
{
	public class TouchSDK
	{
		public struct coord2D
		{
			public int x;

			public int y;
		}

		public struct LOGICAL_PROPERTY
		{
			public int XLogicalMin;

			public int YLogicalMin;

			public int XLogicalMax;

			public int YlogicalMax;
		}

		public enum TouchState
		{
			NONE,
			TOUCH_DOWN,
			TOUCH_MOVE,
			TOUCH_UP,
			TOUCH_RIGHT_DOWN,
			TOUCH_RIGHT_MOVE,
			TOUCH_RIGHT_UP,
			TOUCH_MIDDLE_DOWN,
			TOUCH_MIDDLE_MOVE,
			TOUCH_MIDDLE_UP
		}

		public enum DevicePortType
		{
			HidPortDevice = 1,
			ComPortDevice
		}

		public struct DeviceInfo
		{
			public DevicePortType ePortType;

			public short nDevNo;

			public short nVendowID;

			public short nProductID;

			public short nVersionNumber;

			public short nMaxPointNumber;

			public short nSvnNumber;

			public short nChecksum;

			public bool bIsWidthDataEn;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public byte[] ManufacturerString;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public byte[] ProductString;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public byte[] SerialNumberString;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public byte[] Barcode;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
			public byte[] DevName;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
			public byte[] ShortDevName;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
			public byte[] UsbDevName;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
			public byte[] FeatureDevPathName;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
			public byte[] CmdDevPathName;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
			public byte[] DataDevPathName;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
			public byte[] TouchDevPathName;
		}

		public struct TouchPointData
		{
			public int c;

			public TouchState ts;

			public int s;

			public int x;

			public int y;

			public int w;

			public int h;

			public int p;
		}

		public struct TouchData
		{
			public DeviceInfo DevInfo;

			public ushort nDataCount;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public byte[] DataBuffer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void FuncTouchRawData([In] TouchData stTouchData, [In] IntPtr pCbObj);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void FuncTouchPointData([In] DeviceInfo DevInfo, [In] IntPtr pTouchPointDatas, [In] int nMaxPointNum, [In] int nValidPointNum, [In] IntPtr pCbObj);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void FuncRawCommand([In] TouchData stTouchData, [In] IntPtr pCbObj);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void FuncHotplugPath(StringBuilder strPath, [In] byte bAttached, [In] IntPtr pCbObj);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void FuncHotplugDevInfo([In] DeviceInfo DevInfo, [In] byte bAttached, [In] IntPtr pCbObj);

		public const int MAX_STRING_LENGTH = 128;

		public const int MAX_PATH = 260;

		public const int MAX_DEVICE_NUMBER = 8;

		public const int MAX_DATA_LENGTH = 64;

		public const int MAX_POINT_NUMBER = 32;

		public const int COMMUNICATION_TIMEOUT = 20;

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern ushort GetTouchSDKVersion();

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int InitTouch([In][Out] DeviceInfo[] pDevInfos, int nMaxDevInfoNum, FuncTouchPointData funcTouchPointData, FuncHotplugDevInfo funcHotplugDevInfo, [In] IntPtr pObj);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ExitTouch();

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void RegisterHotplugDevInfo(FuncHotplugDevInfo funcHotplugDevInfo);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void RegisterHotplugPath(FuncHotplugPath funcHotplugPath);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void RegisterTouchPointData(FuncTouchPointData funcTouchPointData);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void RegisterTouchRawData(FuncTouchRawData funcTouchRawData);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void RegisterRawCmd(FuncRawCommand funcRawCmd);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetTouchDeviceCount();

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetTouchDeviceInfos([In][Out] DeviceInfo[] pDevInfos, int nMaxDevInfoNum);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetMaxPointNumber(DeviceInfo DevInfo);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SendData(DeviceInfo DevInfo, [In] byte[] SendDataBuf, int nSendDataLen, [In][Out] byte[] RecvDataBuf, int nRecvDataLen, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetProductModel(DeviceInfo DevInfo, [In][Out] char[] strModel, int nModelLen, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool EnableTouch(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool DisableTouch(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SetMouseMode(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SetSingleTouchMode(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SetMultiTouchMode(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetTouchFormat(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool EnableRawData(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool DisableRawData(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SetAllEnable(DeviceInfo DevInfo, bool bEnable = true, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GetAllEnable(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool DeviceRestart(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SetSignalInit(DeviceInfo DevInfo, int nTimeout = 3000);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetXLedTotal(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetYLedTotal(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GetRightClickEnable(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool EnableRightClick(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool DisableRightClick(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern ushort GetRightClickTime(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SetRightClickTime(DeviceInfo DevInfo, ushort nRightClickTime, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern ushort GetRightClickRange(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SetRightClickRange(DeviceInfo DevInfo, ushort nRightClickRangle, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GetWheelEnable(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool EnableWheel(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool DisableWheel(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetWheelSensitivity(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SetWheelSensitivity(DeviceInfo DevInfo, int nSensitivityValue, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GetDragEnable(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool EnableDrag(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool DisableDrag(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetUniqueID(DeviceInfo DevInfo, [In][Out] byte[] UniqueIDBuf, int nBufLen = 16, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetUserKey(DeviceInfo DevInfo, [In][Out] byte[] UserKeyBuf, int nBufLen = 56, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SetUserKeyInMemory(DeviceInfo DevInfo, [In][Out] byte[] UserKeyBuf, int nBufLen = 56, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ReloadMemoryUserKeyFromStorage(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SaveMemoryUserKeyToStorage(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool EnableTouchWidthData(DeviceInfo DevInfo, int nTimeout = 20);

		[DllImport("TouchSDKDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool DisableTouchWidthData(DeviceInfo DevInfo, int nTimeout = 20);
	}
}
