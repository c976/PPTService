using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace PPTService.Ink.Core.Windows.Native
{
	public static class Win32
	{
		public static class Dwmapi
		{
			public const string LibraryName = "Dwmapi.dll";

			[DllImport("Dwmapi.dll", ExactSpelling = true, PreserveSig = false)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool DwmIsCompositionEnabled();

			[DllImport("Dwmapi.dll", ExactSpelling = true, SetLastError = true)]
			public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, [In] ref int pvAttribute, uint cbAttribute);
		}

		public enum DWMWINDOWATTRIBUTE : uint
		{
			DWMWA_NCRENDERING_ENABLED = 1u,
			DWMWA_NCRENDERING_POLICY,
			DWMWA_TRANSITIONS_FORCEDISABLED,
			DWMWA_ALLOW_NCPAINT,
			DWMWA_CAPTION_BUTTON_BOUNDS,
			DWMWA_NONCLIENT_RTL_LAYOUT,
			DWMWA_FORCE_ICONIC_REPRESENTATION,
			DWMWA_FLIP3D_POLICY,
			DWMWA_EXTENDED_FRAME_BOUNDS,
			DWMWA_HAS_ICONIC_BITMAP,
			DWMWA_DISALLOW_PEEK,
			DWMWA_EXCLUDED_FROM_PEEK,
			DWMWA_CLOAK,
			DWMWA_CLOAKED,
			DWMWA_FREEZE_REPRESENTATION,
			DWMWA_PASSIVE_UPDATE_MODE,
			DWMWA_LAST
		}

		public static class Gdi32
		{
			public const string LibraryName = "gdi32";

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr GetStockObject(int fnObject);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool DeleteObject(IntPtr hObject);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern uint SetPixel(IntPtr hdc, int x, int y, uint crColor);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int GetPixelFormat(IntPtr hdc);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int SetBkMode(IntPtr hdc, int iBkMode);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateDIBSection(IntPtr hdc, IntPtr pbmi, DibBmiColorUsageFlag iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

			[DllImport("gdi32", SetLastError = true)]
			public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

			[DllImport("gdi32", CharSet = CharSet.Unicode)]
			public static extern int GetObject(IntPtr hgdiobj, int cbBuffer, IntPtr lpvObject);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, IntPtr lpvBits);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateBitmapIndirect([In] ref Bitmap lpbm);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateDIBitmap(IntPtr hdc, [In] ref BitmapInfoHeader lpbmih, uint fdwInit, byte[] lpbInit, IntPtr lpbmi, DibBmiColorUsageFlag fuUsage);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateDIBitmap(IntPtr hdc, [In] ref BitmapInfoHeader lpbmih, uint fdwInit, IntPtr lpbInit, IntPtr lpbmi, DibBmiColorUsageFlag fuUsage);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int SetDIBitsToDevice(IntPtr hdc, int xDest, int yDest, uint dwWidth, uint dwHeight, int xSrc, int ySrc, uint uStartScan, uint cScanLines, byte[] lpvBits, IntPtr lpbmi, DibBmiColorUsageFlag fuColorUse);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int SetDIBitsToDevice(IntPtr hdc, int xDest, int yDest, uint dwWidth, uint dwHeight, int xSrc, int ySrc, uint uStartScan, uint cScanLines, IntPtr lpvBits, IntPtr lpbmi, DibBmiColorUsageFlag fuColorUse);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool DeleteDC([In] IntPtr hdc);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int SetBitmapBits(IntPtr hbmp, uint cBytes, [In] byte[] lpBits);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int GetBitmapBits(IntPtr hbmp, int cbBuffer, [Out] byte[] lpvBits);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int SetBitmapBits(IntPtr hbmp, uint cBytes, IntPtr lpBits);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int GetBitmapBits(IntPtr hbmp, int cbBuffer, IntPtr lpvBits);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateRectRgnIndirect([In] ref Rectangle lprc);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateEllipticRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateEllipticRgnIndirect([In] ref Rectangle lprc);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, RegionModeFlags fnCombineMode);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool OffsetViewportOrgEx(IntPtr hdc, int nXOffset, int nYOffset, out Point lpPoint);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool SetViewportOrgEx(IntPtr hdc, int x, int y, out Point lpPoint);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int SetMapMode(IntPtr hdc, int fnMapMode);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern RegionType GetClipBox(IntPtr hdc, out Rectangle lprc);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int GetClipRgn(IntPtr hdc, IntPtr hrgn);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern RegionType SelectClipRgn(IntPtr hdc, IntPtr hrgn);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern RegionType ExtSelectClipRgn(IntPtr hdc, IntPtr hrgn, RegionModeFlags fnMode);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern RegionType IntersectClipRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern RegionType ExcludeClipRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern RegionType OffsetClipRgn(IntPtr hdc, int nXOffset, int nYOffset);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool GetMetaRgn(IntPtr hdc, IntPtr hrgn);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern RegionType SetMetaRgn(IntPtr hdc);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool LPtoDP(IntPtr hdc, [In][Out] ref Point lpPoints, int nCount);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool DPtoLP(IntPtr hdc, [In][Out] ref Point lpPoints, int nCount);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool SelectClipPath(IntPtr hdc, RegionModeFlags iMode);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool FillRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreateSolidBrush(uint crColor);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool FrameRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr, int nWidth, int nHeight);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool PaintRgn(IntPtr hdc, IntPtr hrgn);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool InvertRgn(IntPtr hdc, IntPtr hrgn);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool MoveToEx(IntPtr hdc, int x, int y, out Point lpPoint);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool RoundRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidth, int nHeight);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

			[DllImport("gdi32", CharSet = CharSet.Unicode)]
			public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);

			[DllImport("gdi32")]
			public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, BitBltFlags dwRop);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, BitBltFlags dwRop);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int SaveDC(IntPtr hdc);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern bool RestoreDC(IntPtr hdc, int nSavedDc);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr PathToRegion(IntPtr hdc);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreatePolygonRgn(Point[] lppt, int cPoints, int fnPolyFillMode);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern IntPtr CreatePolyPolygonRgn(Point[] lppt, int[] lpPolyCounts, int nCount, int fnPolyFillMode);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

			[DllImport("gdi32", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
			public static extern int GetDeviceCaps(IntPtr hdc, DeviceCapIndexes nIndex);

			[DllImport("gdi32", EntryPoint = "GdiAlphaBlend", ExactSpelling = true)]
			public static extern bool AlphaBlend(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, BlendFunction blendFunction);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern int GetRandomRgn(IntPtr hdc, IntPtr hrgn, DcRegionType iNum);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern RegionType OffsetRgn(IntPtr hrgn, int nXOffset, int nYOffset);

			[DllImport("gdi32", ExactSpelling = true)]
			public static extern RegionType GetRgnBox(IntPtr hWnd, out Rectangle lprc);

			[DllImport("gdi32", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern bool GetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

			[DllImport("gdi32", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);
		}

		public struct BitmapInfoHeader
		{
			public uint Size;

			public int Width;

			public int Height;

			public ushort Planes;

			public ushort BitCount;

			public BitmapCompressionMode CompressionMode;

			public uint SizeImage;

			public int XPxPerMeter;

			public int YPxPerMeter;

			public uint ClrUsed;

			public uint ClrImportant;
		}

		public struct BitmapInfo
		{
			public BitmapInfoHeader Header;

			public RgbQuad[] Colors;
		}

		public struct RgbQuad
		{
			public byte Blue;

			public byte Green;

			public byte Red;

			private byte Reserved;
		}

		public struct Bitmap
		{
			public int Type;

			public int Width;

			public int Height;

			public int WidthBytes;

			public ushort Planes;

			public ushort BitsPerPixel;

			public IntPtr Bits;
		}

		public enum BitmapInfoColorFormat
		{
			DIB_RGB_COLORS,
			DIB_PAL_COLORS
		}

		public enum BitmapCompressionMode
		{
			BI_RGB,
			BI_RLE8,
			BI_RLE4,
			BI_BITFIELDS,
			BI_JPEG,
			BI_PNG
		}

		public enum StockObject
		{
			BLACK_BRUSH = 4,
			DKGRAY_BRUSH = 3,
			DC_BRUSH = 18,
			GRAY_BRUSH = 2,
			HOLLOW_BRUSH = 5,
			LTGRAY_BRUSH = 1,
			NULL_BRUSH = HOLLOW_BRUSH,
			WHITE_BRUSH = 0,
			BLACK_PEN = 7,
			DC_PEN = 19,
			NULL_PEN = 8,
			WHITE_PEN = 6,
			ANSI_FIXED_FONT = 11,
			ANSI_VAR_FONT = 12,
			DEVICE_DEFAULT_FONT = 14,
			DEFAULT_GUI_FONT = 17,
			OEM_FIXED_FONT = 10,
			SYSTEM_FONT = 13,
			SYSTEM_FIXED_FONT = 16,
			DEFAULT_PALETTE = 15
		}

		public enum DibBmiColorUsageFlag
		{
			DIB_RGB_COLORS,
			DIB_PAL_COLORS
		}

		public enum StockBrush
		{
			WHITE_BRUSH = 0,
			LTGRAY_BRUSH = 1,
			GRAY_BRUSH = 2,
			DKGRAY_BRUSH = 3,
			BLACK_BRUSH = 4,
			NULL_BRUSH = 5,
			HOLLOW_BRUSH = NULL_BRUSH
		}

		public enum StockPen
		{
			WHITE_PEN = 6,
			BLACK_PEN,
			NULL_PEN
		}

		public enum StockFont
		{
			OEM_FIXED_FONT = 10,
			ANSI_FIXED_FONT = 11,
			ANSI_VAR_FONT = 12,
			SYSTEM_FONT = 13,
			DEVICE_DEFAULT_FONT = 14,
			SYSTEM_FIXED_FONT = 16,
			DEFAULT_GUI_FONT = 17
		}

		public enum DcRegionType
		{
			Clip = 1,
			Meta,
			IntersectedMetaClip,
			System
		}

		[Flags]
		public enum BitBltFlags
		{
			SRCCOPY = 0xCC0020,
			SRCPAINT = 0xEE0086,
			SRCAND = 0x8800C6,
			SRCINVERT = 0x660046,
			SRCERASE = 0x440328,
			NOTSRCCOPY = 0x330008,
			NOTSRCERASE = 0x1100A6,
			MERGECOPY = 0xC000CA,
			MERGEPAINT = 0xBB0226,
			PATCOPY = 0xF00021,
			PATPAINT = 0xFB0A09,
			PATINVERT = 0x5A0049,
			DSTINVERT = 0x550009,
			BLACKNESS = 0x42,
			WHITENESS = 0xFF0062,
			CAPTUREBLT = 0x40000000,
			NOMIRRORBITMAP = int.MinValue
		}

		public enum DeviceCapIndexes
		{
			DRIVERVERSION = 0,
			TECHNOLOGY = 2,
			HORZSIZE = 4,
			VERTSIZE = 6,
			HORZRES = 8,
			VERTRES = 10,
			LOGPIXELSX = 88,
			LOGPIXELSY = 90,
			BITSPIXEL = 12,
			PLANES = 14,
			NUMBRUSHES = 16,
			NUMPENS = 18,
			NUMFONTS = 22,
			NUMCOLORS = 24,
			ASPECTX = 40,
			ASPECTY = 42,
			ASPECTXY = 44,
			PDEVICESIZE = 26,
			CLIPCAPS = 36,
			SIZEPALETTE = 104,
			NUMRESERVED = 106,
			COLORRES = 108,
			PHYSICALWIDTH = 110,
			PHYSICALHEIGHT = 111,
			PHYSICALOFFSETX = 112,
			PHYSICALOFFSETY = 113,
			VREFRESH = 116,
			SCALINGFACTORX = 114,
			SCALINGFACTORY = 115,
			BLTALIGNMENT = 119,
			SHADEBLENDCAPS = 45,
			RASTERCAPS = 38,
			CURVECAPS = 28,
			LINECAPS = 30,
			POLYGONALCAPS = 32,
			TEXTCAPS = 34,
			COLORMGMTCAPS = 121,
			DESKTOPVERTRES = 117,
			DESKTOPHORZRES = 118
		}

		public struct RAMP
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public ushort[] Red;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public ushort[] Green;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public ushort[] Blue;
		}

		public static class GdiPlus
		{
			public const string LibraryName = "gdiplus";

			[DllImport("gdiplus", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern GpStatus GdipCreateFromHDC([In] IntPtr hdc, out IntPtr graphics);

			[DllImport("gdiplus", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
			public static extern GpStatus GdipDisposeImage([In] IntPtr image);

			[DllImport("gdiplus", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern GpStatus GdipDrawImage([In] IntPtr graphics, [In] IntPtr image, [In] float x, [In] float y);

			[DllImport("gdiplus", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern GpStatus GdipDrawImageI([In] IntPtr graphics, [In] IntPtr image, [In] int x, [In] int y);

			[DllImport("gdiplus", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern GpStatus GdipDrawImageRect([In] IntPtr graphics, [In] IntPtr image, [In] float x, [In] float y, [In] float width, [In] float height);

			[DllImport("gdiplus", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern GpStatus GdipDrawImageRectI([In] IntPtr graphics, [In] IntPtr image, [In] int x, [In] int y, [In] int width, [In] int height);

			[DllImport("gdiplus", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern GpStatus GdipGetImageHeight([In] IntPtr image, out uint height);

			[DllImport("gdiplus", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern GpStatus GdipGetImageWidth([In] IntPtr image, out uint width);

			[DllImport("gdiplus", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern GpStatus GdipLoadImageFromFile([In][MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr image);

			[DllImport("gdiplus", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern void GdiplusShutdown(UIntPtr token);

			[DllImport("gdiplus", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern GpStatus GdiplusStartup(out UIntPtr token, [In] ref GdiplusStartupInput input, out GdiplusStartupOutput output);
		}

		public enum GpStatus
		{
			Ok,
			GenericError,
			InvalidParameter,
			OutOfMemory,
			ObjectBusy,
			InsufficientBuffer,
			NotImplemented,
			Win32Error,
			WrongState,
			Aborted,
			FileNotFound,
			ValueOverflow,
			AccessDenied,
			UnknownImageFormat,
			FontFamilyNotFound,
			FontStyleNotFound,
			NotTrueTypeFont,
			UnsupportedGdiplusVersion,
			GdiplusNotInitialized,
			PropertyNotFound,
			PropertyNotSupported,
			ProfileNotFound
		}

		public struct GdiplusStartupInput
		{
			public uint GdiplusVersion;

			public IntPtr DebugEventCallback;

			public bool SuppressBackgroundThread;

			public bool SuppressExternalCodecs;
		}

		public struct GdiplusStartupOutput
		{
			private IntPtr NotificationHook;

			private IntPtr NotificationUnhook;
		}

		public enum Month
		{
			January = 1,
			February,
			March,
			April,
			May,
			June,
			July,
			August,
			September,
			October,
			November,
			December
		}

		public enum DayOfWeek
		{
			Sunday,
			Monday,
			Tuesday,
			Wednesday,
			Thursday,
			Friday,
			Saturday
		}

		public enum SystemInvalidResult
		{
			INVALID_ATOM = 0,
			INVALID_OS_COUNT = 65535,
			INVALID_FILE_SIZE = -1,
			INVALID_SET_FILE_POINTER = INVALID_FILE_SIZE,
			INVALID_FILE_ATTRIBUTES = INVALID_FILE_SIZE,
			INVALID_HANDLE_VALUE = INVALID_FILE_SIZE
		}

		public enum ProcessArchitecture
		{
			PROCESSOR_ARCHITECTURE_INTEL = 0,
			PROCESSOR_ARCHITECTURE_MIPS = 1,
			PROCESSOR_ARCHITECTURE_ALPHA = 2,
			PROCESSOR_ARCHITECTURE_PPC = 3,
			PROCESSOR_ARCHITECTURE_SHX = 4,
			PROCESSOR_ARCHITECTURE_ARM = 5,
			PROCESSOR_ARCHITECTURE_IA64 = 6,
			PROCESSOR_ARCHITECTURE_ALPHA64 = 7,
			PROCESSOR_ARCHITECTURE_MSIL = 8,
			PROCESSOR_ARCHITECTURE_AMD64 = 9,
			PROCESSOR_ARCHITECTURE_IA32_ON_WIN64 = 10,
			PROCESSOR_ARCHITECTURE_NEUTRAL = 11,
			PROCESSOR_ARCHITECTURE_UNKNOWN = 65535
		}

		public enum StdHandle
		{
			STD_INPUT_HANDLE = -10,
			STD_OUTPUT_HANDLE = -11,
			STD_ERROR_HANDLE = -12
		}

		public enum SecurityImpersonationLevel
		{
			SecurityAnonymous,
			SecurityIdentification,
			SecurityImpersonation,
			SecurityDelegation
		}

		public enum FileCreationDisposition
		{
			CREATE_ALWAYS = 2,
			CREATE_NEW = 1,
			OPEN_ALWAYS = 4,
			OPEN_EXISTING = 3,
			TRUNCATE_EXISTING = 5
		}

		public enum FileAttributeInfoLevel
		{
			GetFileExInfoStandard,
			GetFileExMaxInfoLevel
		}

		[Flags]
		public enum LoadLibraryFlags
		{
			DONT_RESOLVE_DLL_REFERENCES = 1,
			LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x10,
			LOAD_LIBRARY_AS_DATAFILE = 2,
			LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x40,
			LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x20,
			LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x200,
			LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x1000,
			LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x100,
			LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x800,
			LOAD_LIBRARY_SEARCH_USER_DIRS = 0x400,
			LOAD_WITH_ALTERED_SEARCH_PATH = 8
		}

		[Flags]
		public enum LibrarySearchFlags
		{
			LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x200,
			LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x1000,
			LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x800,
			LOAD_LIBRARY_SEARCH_USER_DIRS = 0x400
		}

		[Flags]
		public enum DuplicateHandleFlags
		{
			DUPLICATE_CLOSE_SOURCE = 1,
			DUPLICATE_SAME_ACCESS = 2
		}

		[Flags]
		public enum HandleInfoFlags
		{
			HANDLE_FLAG_INHERIT = 1,
			HANDLE_FLAG_PROTECT_FROM_CLOSE = 2
		}

		[Flags]
		public enum GetModuleHandleFlags
		{
			GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS = 4,
			GET_MODULE_HANDLE_EX_FLAG_PIN = 1,
			GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT = 2
		}

		[Flags]
		public enum FileShareMode
		{
			FILE_SHARE_NONE = 0,
			FILE_SHARE_DELETE = 4,
			FILE_SHARE_READ = 1,
			FILE_SHARE_WRITE = 2
		}

		[Flags]
		public enum FileAttributes
		{
			FILE_ATTRIBUTE_ARCHIVE = 0x20,
			FILE_ATTRIBUTE_COMPRESSED = 0x800,
			FILE_ATTRIBUTE_DEVICE = 0x40,
			FILE_ATTRIBUTE_DIRECTORY = 0x10,
			FILE_ATTRIBUTE_ENCRYPTED = 0x4000,
			FILE_ATTRIBUTE_HIDDEN = 2,
			FILE_ATTRIBUTE_INTEGRITY_STREAM = 0x8000,
			FILE_ATTRIBUTE_NORMAL = 0x80,
			FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x2000,
			FILE_ATTRIBUTE_NO_SCRUB_DATA = 0x20000,
			FILE_ATTRIBUTE_OFFLINE = 0x1000,
			FILE_ATTRIBUTE_READONLY = 1,
			FILE_ATTRIBUTE_REPARSE_POINT = 0x400,
			FILE_ATTRIBUTE_SPARSE_FILE = 0x200,
			FILE_ATTRIBUTE_SYSTEM = 4,
			FILE_ATTRIBUTE_TEMPORARY = 0x100,
			FILE_ATTRIBUTE_VIRTUAL = 0x10000
		}

		public static class Kernel32
		{
			public const string LibraryName = "kernel32";

			[DllImport("kernel32")]
			public static extern bool SetProcessUserModeExceptionPolicy(uint dwFlags);

			[DllImport("kernel32")]
			public static extern bool GetProcessUserModeExceptionPolicy(out uint dwFlags);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern uint GetLastError();

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool DisableThreadLibraryCalls(IntPtr hModule);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern void Sleep(uint dwMilliseconds);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern uint GetTickCount();

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern ulong GetTickCount64();

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool QueryPerformanceCounter(out long value);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool QueryPerformanceFrequency(out long value);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern void QueryUnbiasedInterruptTime(out ulong unbiasedTime);

			[DllImport("kernel32")]
			public static extern void GetLocalTime(out SystemTime lpSystemTime);

			[DllImport("kernel32")]
			public static extern bool SetLocalTime(ref SystemTime lpSystemTime);

			[DllImport("kernel32")]
			public static extern void GetSystemTime(out SystemTime lpSystemTime);

			[DllImport("kernel32")]
			public static extern bool SetSystemTime(ref SystemTime lpSystemTime);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool AllocConsole();

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool FreeConsole();

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool AttachConsole(uint dwProcessId);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern IntPtr GetConsoleWindow();

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern IntPtr GetStdHandle(uint nStdHandle);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool SetStdHandle(uint nStdHandle, IntPtr hHandle);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern bool SetConsoleTitle(string lpConsoleTitle);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern uint GetConsoleTitle(StringBuilder lpConsoleTitle, uint nSize);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput, int bAbsolute, [In] ref RectangleS lpConsoleWindow);

			[DllImport("kernel32", EntryPoint = "RtlZeroMemory", ExactSpelling = true)]
			public static extern void ZeroMemory(IntPtr dest, IntPtr size);

			[DllImport("kernel32", EntryPoint = "RtlSecureZeroMemory", ExactSpelling = true)]
			public static extern void SecureZeroMemory(IntPtr dest, IntPtr size);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool CloseHandle(IntPtr hObject);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle, uint dwDesiredAccess, int bInheritHandle, DuplicateHandleFlags dwOptions);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool GetHandleInformation(IntPtr hObject, out HandleInfoFlags lpdwFlags);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool SetHandleInformation(IntPtr hObject, HandleInfoFlags dwMask, HandleInfoFlags dwFlags);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern IntPtr GetModuleHandle(IntPtr modulePtr);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern bool GetModuleHandleEx(GetModuleHandleFlags dwFlags, string lpModuleName, out IntPtr phModule);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern IntPtr GetModuleHandle(string lpModuleName);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadLibrary(string fileName);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadLibraryEx(string fileName, IntPtr hFileReservedAlwaysZero, LoadLibraryFlags dwFlags);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool FreeLibrary(IntPtr hModule);

			[DllImport("kernel32", CharSet = CharSet.Ansi)]
			public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

			[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern bool SetDllDirectory(string directoryPath);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool SetDefaultDllDirectories(LibrarySearchFlags directoryFlags);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern IntPtr AddDllDirectory(string newDirectory);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool RemoveDllDirectory(IntPtr cookieFromAddDllDirectory);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern uint GetSystemDirectory(StringBuilder lpBuffer, uint uSize);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern uint GetWindowsDirectory(StringBuilder lpBuffer, uint uSize);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern uint GetVersion();

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern bool IsWow64Process(IntPtr hProcess, out int isWow64Process);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern void GetNativeSystemInfo(out SystemInfo lpSystemInfo);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern void GetSystemInfo(out SystemInfo lpSystemInfo);

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern uint GetCurrentProcessId();

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern IntPtr GetCurrentProcess();

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern uint GetCurrentThreadId();

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern IntPtr GetCurrentThread();

			[DllImport("kernel32", ExactSpelling = true)]
			public static extern uint GetCurrentProcessorNumber();

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern FileAttributes GetFileAttributes(string lpFileName);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern bool SetFileAttributes(string lpFileName, FileAttributes dwFileAttributes);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern bool GetFileAttributesEx(string lpFileName, FileAttributeInfoLevel fInfoLevelId, out FileAttributeData lpFileInformation);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, FileShareMode dwShareMode, IntPtr lpSecurityAttributes, FileCreationDisposition dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, FileShareMode dwShareMode, ref SecurityAttributes lpSecurityAttributes, FileCreationDisposition dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
		}

		public struct SystemInfo
		{
			public ushort ProcessorArchitecture;

			private ushort Reserved;

			public uint PageSize;

			public IntPtr MinimumApplicationAddress;

			public IntPtr MaximumApplicationAddress;

			public IntPtr ActiveProcessorMask;

			public uint NumberOfProcessors;

			public uint ProcessorType;

			public uint AllocationGranularity;

			public ushort ProcessorLevel;

			public ushort ProcessorRevision;

			public uint OemId => (uint)((ProcessorArchitecture << 8) | Reserved);
		}

		public struct SecurityAttributes
		{
			public uint Length;

			public IntPtr SecurityDescriptor;

			public uint IsHandleInheritedValue;

			public bool IsHandleInherited => IsHandleInheritedValue != 0;
		}

		public struct FileTime
		{
			public uint Low;

			public uint High;

			public ulong Value => ((ulong)High << 32) | Low;
		}

		public struct SystemTime
		{
			public ushort Year;

			public ushort Month;

			public ushort DayOfWeek;

			public ushort Day;

			public ushort Hour;

			public ushort Minute;

			public ushort Second;

			public ushort Milliseconds;
		}

		public struct FileAttributeData
		{
			public FileAttributes Attributes;

			public FileTime CreationTime;

			public FileTime LastAccessTime;

			public FileTime LastWriteTime;

			public uint FileSizeHigh;

			public uint FileSizeLow;

			public ulong FileSize => ((ulong)FileSizeHigh << 32) | FileSizeLow;
		}

		public static class ShCore
		{
			public const string LibraryName = "SHCore.dll";

			[DllImport("SHCore.dll", CharSet = CharSet.Unicode)]
			public static extern HResult GetDpiForMonitor(IntPtr hMonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);
		}

		public enum MonitorDpiType
		{
			MDT_EFFECTIVE_DPI = 0,
			MDT_ANGULAR_DPI = 1,
			MDT_RAW_DPI = 2,
			MDT_DEFAULT = MDT_EFFECTIVE_DPI
		}

		public static class Shell32
		{
			public struct APPBARDATA
			{
				public int cbSize;

				public IntPtr hWnd;

				public int uCallbackMessage;

				public int uEdge;

				public RECT rc;

				public readonly IntPtr lParam;
			}

			public enum ABMsg
			{
				ABM_NEW,
				ABM_REMOVE,
				ABM_QUERYPOS,
				ABM_SETPOS,
				ABM_GETSTATE,
				ABM_GETTASKBARPOS,
				ABM_ACTIVATE,
				ABM_GETAUTOHIDEBAR,
				ABM_SETAUTOHIDEBAR,
				ABM_WINDOWPOSCHANGED,
				ABM_SETSTATE
			}

			public enum ABNotify
			{
				ABN_STATECHANGE,
				ABN_POSCHANGED,
				ABN_FULLSCREENAPP,
				ABN_WINDOWARRANGE
			}

			[DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
			public static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);
		}

		public static class User32ExperimentalMethods
		{
			internal enum AccentState
			{
				ACCENT_DISABLED,
				ACCENT_ENABLE_GRADIENT,
				ACCENT_ENABLE_TRANSPARENTGRADIENT,
				ACCENT_ENABLE_BLURBEHIND,
				ACCENT_INVALID_STATE
			}

			internal struct AccentPolicy
			{
				public AccentState AccentState;

				public int AccentFlags;

				public int GradientColor;

				public int AnimationId;
			}

			internal struct WindowCompositionAttributeData
			{
				public WindowCompositionAttribute Attribute;

				public IntPtr Data;

				public int SizeOfData;
			}

			internal enum WindowCompositionAttribute
			{
				WCA_ACCENT_POLICY = 19
			}

			[DllImport("user32", CharSet = CharSet.Unicode)]
			internal static extern bool SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
		}

		public struct IconInfo
		{
			public bool fIcon;

			public int xHotspot;

			public int yHotspot;

			public IntPtr hbmMask;

			public IntPtr hbmColor;
		}

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		public class SafeIconHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			public SafeIconHandle()
				: base(true)
			{
			}

			protected override bool ReleaseHandle()
			{
				return DestroyIcon(handle);
			}
		}

		public static class User32
		{
			public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

			public const string LibraryName = "user32";

			[DllImport("user32.dll")]
			public static extern SafeIconHandle CreateIconIndirect(ref IconInfo icon);

			[DllImport("user32.dll")]
			public static extern bool DestroyIcon(IntPtr hIcon);

			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconResource);

			[DllImport("user32", CharSet = CharSet.Unicode, EntryPoint = "LoadIconW", SetLastError = true)]
			public static extern IntPtr LoadIcon([In] IntPtr hInstance, [In] SystemIcon lpIconName);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadCursor(IntPtr hInstance, string lpCursorName);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorResource);

			[DllImport("user32", CharSet = CharSet.Unicode, EntryPoint = "LoadCursorW", SetLastError = true)]
			[Obsolete("请使用 LoadImage 替代。")]
			public static extern IntPtr LoadCursor([In] IntPtr hInstance, [In] SystemCursor lpCursorName);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadImage(IntPtr hInstance, string lpszName, ResourceImageType uType, int cxDesired, int cyDesired, LoadResourceFlags fuLoad);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadImage(IntPtr hInstance, IntPtr resourceId, ResourceImageType uType, int cxDesired, int cyDesired, LoadResourceFlags fuLoad);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadBitmap(IntPtr hInstance, string lpBitmapName);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadBitmap(IntPtr hInstance, IntPtr resourceId);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr BeginPaint(IntPtr hWnd, out PaintStruct lpPaint);

			[DllImport("user32", ExactSpelling = true)]
			public static extern void EndPaint(IntPtr hWnd, [In] ref PaintStruct lpPaint);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetDesktopWindow();

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr MonitorFromPoint(Point pt, MonitorFlag dwFlags);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr MonitorFromRect([In] ref Rectangle lprc, MonitorFlag dwFlags);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr MonitorFromWindow(IntPtr hWnd, MonitorFlag dwFlags);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern int DrawText(IntPtr hdc, string lpString, int nCount, [In] ref Rectangle lpRect, uint uFormat);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifierFlags fsModifiers, VirtualKey vk);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

			[DllImport("user32", ExactSpelling = true)]
			public static extern uint SendInput(uint nInputs, IntPtr pInputs, int cbSize);

			[DllImport("user32", ExactSpelling = true)]
			public static extern uint SendInput(uint nInputs, [In] Input[] pInputs, int cbSize);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIdEvent, uint uElapseMillis, TimerProc lpTimerFunc);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool KillTimer(IntPtr hWnd, IntPtr uIdEvent);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ValidateRect(IntPtr hWnd, [In] ref Rectangle lpRect);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ValidateRect(IntPtr hWnd, IntPtr lpRect);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool InvalidateRect(IntPtr hWnd, [In] ref Rectangle lpRect, bool bErase);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetDC(IntPtr hwnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern int GetSystemMetrics(SystemMetrics nIndex);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, SystemParamtersInfoFlags fWinIni);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetWindowDC(IntPtr hWnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr WindowFromDC(IntPtr hdc);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool InvertRect(IntPtr hdc, [In] ref Rectangle lprc);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool SetRectEmpty(out Rectangle lprc);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool AdjustWindowRect([In][Out] ref Rectangle lpRect, WindowStyles dwStyle, bool hasMenu);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool AdjustWindowRectEx([In][Out] ref Rectangle lpRect, WindowStyles dwStyle, bool hasMenu, ExtendedWindowStyles dwExStyle);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool CopyRect(out Rectangle lprcDst, [In] ref Rectangle lprcSrc);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool IntersectRect(out Rectangle lprcDst, [In] ref Rectangle lprcSrc1, [In] ref Rectangle lprcSrc2);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool UnionRect(out Rectangle lprcDst, [In] ref Rectangle lprcSrc1, [In] ref Rectangle lprcSrc2);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool IsRectEmpty([In] ref Rectangle lprc);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool PtInRect([In] ref Rectangle lprc, Point pt);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool OffsetRect([In][Out] ref Rectangle lprc, int dx, int dy);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool InflateRect([In][Out] ref Rectangle lprc, int dx, int dy);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool FrameRect(IntPtr hdc, [In] ref Rectangle lprc, IntPtr hbr);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool FillRect(IntPtr hdc, [In] ref Rectangle lprc, IntPtr hbr);

			[DllImport("user32", ExactSpelling = true)]
			public static extern RegionType GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

			[DllImport("user32", ExactSpelling = true)]
			public static extern RegionType GetWindowRgnBox(IntPtr hWnd, out Rectangle lprc);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, LayeredWindowAttributeFlag dwFlags);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern bool WinHelp(IntPtr hWndMain, string lpszHelp, uint uCommand, uint dwData);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool InvalidateRgn(IntPtr hWnd, IntPtr hRgn, bool bErase);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool GetUpdateRect(IntPtr hWnd, out Rectangle rect, bool bErase);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ValidateRgn(IntPtr hWnd, IntPtr hRgn);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, DeviceContextFlags flags);

			[DllImport("user32", ExactSpelling = true)]
			public static extern void DisableProcessWindowsGhosting();

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool UpdateLayeredWindow(IntPtr hWnd, IntPtr hdcDst, [In] ref Point pptDst, [In] ref Size psize, IntPtr hdcSrc, [In] ref Point pptSrc, uint crKey, [In] ref BlendFunction pblend, uint dwFlags);

			[DllImport("user32", ExactSpelling = true)]
			public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, IntPtr lpPoints, int cPoints);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ScreenToClient(IntPtr hWnd, [In][Out] ref Point lpPoint);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr WindowFromPhysicalPoint(Point point);

			[DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

			[DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool UnhookWindowsHookEx(IntPtr hhk);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr WindowFromPoint(Point point);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool IsWindowVisible(IntPtr hwnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool OpenIcon(IntPtr hwnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool IsWindow(IntPtr hwnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool IsHungAppWindow(IntPtr hwnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool IsZoomed(IntPtr hwnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool IsIconic(IntPtr hwnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool LogicalToPhysicalPoint(IntPtr hWnd, [In][Out] ref Point point);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr ChildWindowFromPoint(IntPtr hwndParent, Point point);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr ChildWindowFromPointEx(IntPtr hwndParent, Point point, ChildWindowFromPointFlags flags);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetMenu(IntPtr hwnd);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern MessageBoxResult MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint type);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern MessageBoxResult MessageBoxEx(IntPtr hWnd, string lpText, string lpCaption, uint type, ushort wLanguageId);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool SetThreadDesktop(IntPtr hDesk);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetThreadDesktop(uint threadId);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

			[DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern uint GetDpiForWindow([In] IntPtr hwnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool IsWindowEnabled(IntPtr hWnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ReleaseCapture();

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr SetCapture(IntPtr hWnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetFocus();

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr SetFocus(IntPtr hWnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr SetActiveWindow(IntPtr hWnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetActiveWindow();

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool BlockInput(bool fBlockIt);

			[DllImport("user32", ExactSpelling = true)]
			public static extern uint WaitForInputIdle(IntPtr hProcess, uint dwMilliseconds);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool DragDetect(IntPtr hWnd, Point point);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ClientToScreen(IntPtr hWnd, [In][Out] ref Point point);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ClipCursor([In] ref Rectangle rect);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ClipCursor(IntPtr ptr);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool TrackMouseEvent([In][Out] ref TrackMouseEventOptions lpEventTrack);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool GetLastInputInfo(out LastInputInfo plii);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern uint MapVirtualKey(uint uCode, VirtualKeyMapType uMapType);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern uint MapVirtualKey(VirtualKey uCode, VirtualKeyMapType uMapType);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern uint MapVirtualKeyEx(uint uCode, VirtualKeyMapType uMapType, IntPtr dwhkl);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern uint MapVirtualKeyEx(VirtualKey uCode, VirtualKeyMapType uMapType, IntPtr dwhkl);

			[DllImport("user32", ExactSpelling = true)]
			public static extern KeyState GetAsyncKeyState(int vKey);

			[DllImport("user32", ExactSpelling = true)]
			public static extern KeyState GetAsyncKeyState(VirtualKey vKey);

			[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "keybd_event")]
			public static extern void Keyboard_Event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

			[DllImport("user32", ExactSpelling = true)]
			public static extern KeyState GetKeyState(int nVirtKey);

			[DllImport("user32", ExactSpelling = true)]
			public static extern KeyState GetKeyState(VirtualKey nVirtKey);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool GetKeyboardState([MarshalAs(UnmanagedType.LPArray, SizeConst = 256)] out byte[] lpKeyState);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool GetKeyboardState(IntPtr lpKeyState);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool SetKeyboardState([In][MarshalAs(UnmanagedType.LPArray, SizeConst = 256)] byte[] lpKeyState);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool SetKeyboardState(IntPtr lpKeyState);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool GetTitleBarInfo(IntPtr hWnd, IntPtr pti);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool GetCursorPos(out Point point);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool SetCursorPos(int x, int y);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool SetPhysicalCursorPos(int x, int y);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool SetSystemCursor(IntPtr cursor, SystemCursor id);

			[DllImport("user32", ExactSpelling = true)]
			public static extern int ShowCursor(bool bShow);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr SetCursor(IntPtr hCursor);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr CopyCursor(IntPtr hCursor);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool GetCursorInfo(ref CursorInfo info);

			[DllImport("user32")]
			public static extern int GetCaretPos(out Point p);

			[DllImport("user32")]
			public static extern int SetCaretPos(int x, int y);

			[DllImport("user32")]
			public static extern bool ShowCaret(IntPtr hwnd);

			[DllImport("user32")]
			public static extern int CreateCaret(IntPtr hwnd, IntPtr hBitmap, int width, int height);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool AnimateWindow(IntPtr hWnd, int dwTime, AnimateWindowFlags dwFlags);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool UpdateWindow(IntPtr hwnd);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr CreateWindowEx(WindowExStyles dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hwndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

			[DllImport("user32", ExactSpelling = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool GetClientRect(IntPtr hWnd, out Rectangle rect);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetForegroundWindow();

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetTopWindow();

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetNextWindow(IntPtr hWnd, uint wCmd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool AllowSetForegroundWindow(uint dwProcessId);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool SetForegroundWindow(IntPtr hWnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool BringWindowToTop(IntPtr hWnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

			[DllImport("user32", ExactSpelling = true, SetLastError = true)]
			public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int wFlagslong);

			[DllImport("user32", ExactSpelling = true)]
			[Obsolete("请使用SetWindowPos重载方法，hWndInsertAfter的类型错误，在64位下长度错误")]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlagslong);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern int GetWindowTextLength(IntPtr hWnd);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern bool SetWindowText(IntPtr hWnd, string lpString);

			[DllImport("user32", ExactSpelling = true)]
			public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

			[DllImport("user32", ExactSpelling = true)]
			public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool GetWindowInfo(IntPtr hWnd, [In][Out] ref WindowInfo pwi);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool RedrawWindow(IntPtr hWnd, [In] ref Rectangle lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool DestroyWindow(IntPtr hwnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool CloseWindow(IntPtr hwnd);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern ushort RegisterClassEx([In] ref WindowClassEx lpwcx);

			[DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern ushort RegisterClassEx([In] ref WindowClassExPtr lpwcx);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern ushort RegisterClassEx([In] ref WindowClassExBlittable lpwcx);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

			public static IntPtr GetWindowLongPtr(IntPtr hWnd, GetWindowLongFields nIndex)
			{
				return GetWindowLongPtr(hWnd, (int)nIndex);
			}

			public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
			{
				if (IntPtr.Size <= 4)
				{
					return new IntPtr(GetWindowLong(hWnd, nIndex));
				}
				return GetWindowLongPtr_x64(hWnd, nIndex);
			}

			[DllImport("user32", CharSet = CharSet.Unicode)]
			[Obsolete("请使用 GetWindowLongPtr 解决 x86 和 x64 需要使用不同方法")]
			public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

			[DllImport("user32", CharSet = CharSet.Unicode, EntryPoint = "GetWindowLongPtr")]
			[Obsolete("请使用 GetWindowLongPtr 解决 x86 和 x64 需要使用不同方法")]
			public static extern IntPtr GetWindowLongPtr_x64(IntPtr hWnd, int nIndex);

			public static IntPtr SetWindowLongPtr(IntPtr hWnd, GetWindowLongFields nIndex, IntPtr dwNewLong)
			{
				return SetWindowLongPtr(hWnd, (int)nIndex, dwNewLong);
			}

			public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
			{
				if (IntPtr.Size <= 4)
				{
					return new IntPtr(SetWindowLong(hWnd, nIndex, dwNewLong.ToInt32()));
				}
				return SetWindowLongPtr_x64(hWnd, nIndex, dwNewLong);
			}

			[DllImport("user32", CharSet = CharSet.Unicode)]
			[Obsolete("请使用 SetWindowLongPtr 解决 x86 和 x64 需要使用不同方法")]
			public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

			[DllImport("user32", CharSet = CharSet.Unicode, EntryPoint = "SetWindowLongPtr")]
			[Obsolete("请使用 SetWindowLongPtr 解决 x86 和 x64 需要使用不同方法")]
			public static extern IntPtr SetWindowLongPtr_x64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern bool GetClassInfoEx(IntPtr hInstance, string lpClassName, out WindowClassExBlittable lpWndClass);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

			public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
			{
				if (IntPtr.Size <= 4)
				{
					return new IntPtr((int)GetClassLong(hWnd, nIndex));
				}
				return GetClassLongPtr_x64(hWnd, nIndex);
			}

			[DllImport("user32", CharSet = CharSet.Unicode)]
			private static extern uint GetClassLong(IntPtr hWnd, int nIndex);

			[DllImport("user32", CharSet = CharSet.Unicode, EntryPoint = "GetClassLongPtr")]
			private static extern IntPtr GetClassLongPtr_x64(IntPtr hWnd, int nIndex);

			public static IntPtr SetClassLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
			{
				if (IntPtr.Size <= 4)
				{
					return new IntPtr((int)SetClassLong(hWnd, nIndex, dwNewLong.ToInt32()));
				}
				return SetClassLongPtr_x64(hWnd, nIndex, dwNewLong);
			}

			[DllImport("user32", CharSet = CharSet.Unicode)]
			private static extern uint SetClassLong(IntPtr hWnd, int nIndex, int dwNewLong);

			[DllImport("user32", CharSet = CharSet.Unicode, EntryPoint = "SetClassLongPtr")]
			private static extern IntPtr SetClassLongPtr_x64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr CallWindowProc(WindowProc lpPrevWndFunc, IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern bool PeekMessage(out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr DispatchMessage([In] ref Message lpMsg);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool TranslateMessage([In] ref Message lpMsg);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern int GetMessage(out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

			[DllImport("user32", ExactSpelling = true)]
			public static extern void PostQuitMessage(int nExitCode);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetMessageExtraInfo();

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr SetMessageExtraInfo(IntPtr lParam);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool WaitMessage();

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern bool SendNotifyMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ReplyMessage(IntPtr lResult);

			[DllImport("user32", CharSet = CharSet.Auto)]
			public static extern int RegisterWindowMessage(string msg);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool GetInputState();

			[DllImport("user32", ExactSpelling = true)]
			public static extern uint GetMessagePos();

			[DllImport("user32", ExactSpelling = true)]
			public static extern uint GetMessageTime();

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool InSendMessage();

			[DllImport("user32", ExactSpelling = true)]
			public static extern uint GetQueueStatus(QueueStatusFlags flags);

			[DllImport("user32", CharSet = CharSet.Unicode)]
			public static extern bool PostThreadMessage(uint threadId, uint msg, IntPtr wParam, IntPtr lParam);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool OpenClipboard(IntPtr hWndNewOwner);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool CloseClipboard();

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr GetClipboardData(uint uFormat);

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr SetClipboardData(uint uFormat, IntPtr handle);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool EmptyClipboard();

			[DllImport("user32", ExactSpelling = true)]
			public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

			[DllImport("user32", ExactSpelling = true)]
			public static extern bool AddClipboardFormatListener(IntPtr hwnd);

			[DllImport("user32", ExactSpelling = true)]
			public static extern int GetPriorityClipboardFormat(IntPtr paFormatPriorityList, int cFormats);

			[DllImport("user32", ExactSpelling = true)]
			public static extern uint EnumClipboardFormats(uint format);
		}

		[Flags]
		public enum KeyModifierFlags
		{
			MOD_ALT = 1,
			MOD_CONTROL = 2,
			MOD_NOREPEAT = 0x4000,
			MOD_SHIFT = 4,
			MOD_WIN = 8
		}

		[Flags]
		public enum MouseInputKeyStateFlags
		{
			MK_CONTROL = 8,
			MK_LBUTTON = 1,
			MK_MBUTTON = 0x10,
			MK_RBUTTON = 2,
			MK_SHIFT = 4,
			MK_XBUTTON1 = 0x20,
			MK_XBUTTON2 = 0x40
		}

		[Flags]
		public enum HotKeyInputState
		{
			MOD_ALT = 1,
			MOD_CONTROL = 2,
			MOD_SHIFT = 4,
			MOD_WIN = 8
		}

		[Flags]
		public enum MouseInputXButtonFlag
		{
			XBUTTON1 = 1,
			XBUTTON2 = 2
		}

		[Flags]
		public enum MouseInputFlags
		{
			MOUSEEVENTF_ABSOLUTE = 0x8000,
			MOUSEEVENTF_HWHEEL = 0x1000,
			MOUSEEVENTF_MOVE = 1,
			MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000,
			MOUSEEVENTF_LEFTDOWN = 2,
			MOUSEEVENTF_LEFTUP = 4,
			MOUSEEVENTF_RIGHTDOWN = 8,
			MOUSEEVENTF_RIGHTUP = 0x10,
			MOUSEEVENTF_MIDDLEDOWN = 0x20,
			MOUSEEVENTF_MIDDLEUP = 0x40,
			MOUSEEVENTF_VIRTUALDESK = 0x4000,
			MOUSEEVENTF_WHEEL = 0x800,
			MOUSEEVENTF_XDOWN = 0x80,
			MOUSEEVENTF_XUP = 0x100
		}

		[Flags]
		public enum KeyboardInputFlags
		{
			KEYEVENTF_EXTENDEDKEY = 1,
			KEYEVENTF_KEYUP = 2,
			KEYEVENTF_SCANCODE = 8,
			KEYEVENTF_UNICODE = 4
		}

		[Flags]
		public enum CursorInfoFlags
		{
			CURSOR_SHOWING = 1,
			CURSOR_SUPPRESSED = 2
		}

		public enum InputType
		{
			INPUT_MOUSE,
			INPUT_KEYBOARD,
			INPUT_HARDWARE
		}

		public enum VirtualKeyMapType
		{
			MAPVK_VK_TO_CHAR = 2,
			MAPVK_VK_TO_VSC = 0,
			MAPVK_VK_TO_VSC_EX = 4,
			MAPVK_VSC_TO_VK = 1,
			MAPVK_VSC_TO_VK_EX = 3
		}

		public enum VirtualKey
		{
			KeyCode = 65535,
			Modifiers = -65536,
			None = 0,
			LButton = 1,
			RButton = 2,
			Cancel = 3,
			MButton = 4,
			XButton1 = 5,
			XButton2 = 6,
			Back = 8,
			Tab = 9,
			LineFeed = 10,
			Clear = 12,
			Return = 13,
			Enter = Return,
			ShiftKey = 16,
			ControlKey = 17,
			Menu = 18,
			Pause = 19,
			Capital = 20,
			CapsLock = Capital,
			KanaMode = 21,
			HanguelMode = KanaMode,
			HangulMode = KanaMode,
			JunjaMode = 23,
			FinalMode = 24,
			HanjaMode = 25,
			KanjiMode = HanjaMode,
			Escape = 27,
			IMEConvert = 28,
			IMENonconvert = 29,
			IMEAccept = 30,
			IMEAceept = IMEAccept,
			IMEModeChange = 31,
			Space = 32,
			Prior = 33,
			PageUp = Prior,
			Next = 34,
			PageDown = Next,
			End = 35,
			Home = 36,
			Left = 37,
			Up = 38,
			Right = 39,
			Down = 40,
			Select = 41,
			Print = 42,
			Execute = 43,
			Snapshot = 44,
			PrintScreen = Snapshot,
			Insert = 45,
			Delete = 46,
			Help = 47,
			D0 = 48,
			D1 = 49,
			D2 = 50,
			D3 = 51,
			D4 = 52,
			D5 = 53,
			D6 = 54,
			D7 = 55,
			D8 = 56,
			D9 = 57,
			A = 65,
			B = 66,
			C = 67,
			D = 68,
			E = 69,
			F = 70,
			G = 71,
			H = 72,
			I = 73,
			J = 74,
			K = 75,
			L = 76,
			M = 77,
			N = 78,
			O = 79,
			P = 80,
			Q = 81,
			R = 82,
			S = 83,
			T = 84,
			U = 85,
			V = 86,
			W = 87,
			X = 88,
			Y = 89,
			Z = 90,
			LWin = 91,
			RWin = 92,
			Apps = 93,
			Sleep = 95,
			NumPad0 = 96,
			NumPad1 = 97,
			NumPad2 = 98,
			NumPad3 = 99,
			NumPad4 = 100,
			NumPad5 = 101,
			NumPad6 = 102,
			NumPad7 = 103,
			NumPad8 = 104,
			NumPad9 = 105,
			Multiply = 106,
			Add = 107,
			Separator = 108,
			Subtract = 109,
			Decimal = 110,
			Divide = 111,
			F1 = 112,
			F2 = 113,
			F3 = 114,
			F4 = 115,
			F5 = 116,
			F6 = 117,
			F7 = 118,
			F8 = 119,
			F9 = 120,
			F10 = 121,
			F11 = 122,
			F12 = 123,
			F13 = 124,
			F14 = 125,
			F15 = 126,
			F16 = 127,
			F17 = 128,
			F18 = 129,
			F19 = 130,
			F20 = 131,
			F21 = 132,
			F22 = 133,
			F23 = 134,
			F24 = 135,
			NumLock = 144,
			Scroll = 145,
			LShiftKey = 160,
			RShiftKey = 161,
			LControlKey = 162,
			RControlKey = 163,
			LMenu = 164,
			RMenu = 165,
			BrowserBack = 166,
			BrowserForward = 167,
			BrowserRefresh = 168,
			BrowserStop = 169,
			BrowserSearch = 170,
			BrowserFavorites = 171,
			BrowserHome = 172,
			VolumeMute = 173,
			VolumeDown = 174,
			VolumeUp = 175,
			MediaNextTrack = 176,
			MediaPreviousTrack = 177,
			MediaStop = 178,
			MediaPlayPause = 179,
			LaunchMail = 180,
			SelectMedia = 181,
			LaunchApplication1 = 182,
			LaunchApplication2 = 183,
			OemSemicolon = 186,
			Oem1 = OemSemicolon,
			Oemplus = 187,
			Oemcomma = 188,
			OemMinus = 189,
			OemPeriod = 190,
			OemQuestion = 191,
			Oem2 = OemQuestion,
			Oemtilde = 192,
			Oem3 = Oemtilde,
			OemOpenBrackets = 219,
			Oem4 = OemOpenBrackets,
			OemPipe = 220,
			Oem5 = OemPipe,
			OemCloseBrackets = 221,
			Oem6 = OemCloseBrackets,
			OemQuotes = 222,
			Oem7 = OemQuotes,
			Oem8 = 223,
			OemBackslash = 226,
			Oem102 = OemBackslash,
			ProcessKey = 229,
			Packet = 231,
			Attn = 246,
			Crsel = 247,
			Exsel = 248,
			EraseEof = 249,
			Play = 250,
			Zoom = 251,
			NoName = 252,
			Pa1 = 253,
			OemClear = 254,
			[Obsolete("ONLY USE IN WINFORM, NOT IN WIN32")]
			[Browsable(false)]
			Shift = 65536,
			[Obsolete("ONLY USE IN WINFORM, NOT IN WIN32")]
			[Browsable(false)]
			Control = 131072,
			[Obsolete("ONLY USE IN WINFORM, NOT IN WIN32")]
			[Browsable(false)]
			Alt = 262144
		}

		public enum ScanCodes
		{
			LBUTTON = 0,
			RBUTTON = LBUTTON,
			CANCEL = 70,
			MBUTTON = LBUTTON,
			XBUTTON1 = LBUTTON,
			XBUTTON2 = LBUTTON,
			BACK = 14,
			TAB = 15,
			CLEAR = 76,
			RETURN = 28,
			SHIFT = 42,
			CONTROL = 29,
			MENU = 56,
			PAUSE = LBUTTON,
			CAPITAL = 58,
			KANA = LBUTTON,
			HANGUL = LBUTTON,
			JUNJA = LBUTTON,
			FINAL = LBUTTON,
			HANJA = LBUTTON,
			KANJI = LBUTTON,
			ESCAPE = 1,
			CONVERT = LBUTTON,
			NONCONVERT = LBUTTON,
			ACCEPT = LBUTTON,
			MODECHANGE = LBUTTON,
			SPACE = 57,
			PRIOR = 73,
			NEXT = 81,
			END = 79,
			HOME = 71,
			LEFT = 75,
			UP = 72,
			RIGHT = 77,
			DOWN = 80,
			SELECT = LBUTTON,
			PRINT = LBUTTON,
			EXECUTE = LBUTTON,
			SNAPSHOT = 84,
			INSERT = 82,
			DELETE = 83,
			HELP = 99,
			KEY_0 = 11,
			KEY_1 = 2,
			KEY_2 = 3,
			KEY_3 = 4,
			KEY_4 = 5,
			KEY_5 = 6,
			KEY_6 = 7,
			KEY_7 = 8,
			KEY_8 = 9,
			KEY_9 = 10,
			KEY_A = 30,
			KEY_B = 48,
			KEY_C = 46,
			KEY_D = 32,
			KEY_E = 18,
			KEY_F = 33,
			KEY_G = 34,
			KEY_H = 35,
			KEY_I = 23,
			KEY_J = 36,
			KEY_K = 37,
			KEY_L = 38,
			KEY_M = 50,
			KEY_N = 49,
			KEY_O = 24,
			KEY_P = 25,
			KEY_Q = 16,
			KEY_R = 19,
			KEY_S = 31,
			KEY_T = 20,
			KEY_U = 22,
			KEY_V = 47,
			KEY_W = 17,
			KEY_X = 45,
			KEY_Y = 21,
			KEY_Z = 44,
			LWIN = 91,
			RWIN = 92,
			APPS = 93,
			SLEEP = 95,
			NUMPAD0 = INSERT,
			NUMPAD1 = END,
			NUMPAD2 = DOWN,
			NUMPAD3 = NEXT,
			NUMPAD4 = LEFT,
			NUMPAD5 = CLEAR,
			NUMPAD6 = RIGHT,
			NUMPAD7 = HOME,
			NUMPAD8 = UP,
			NUMPAD9 = PRIOR,
			MULTIPLY = 55,
			ADD = 78,
			SEPARATOR = LBUTTON,
			SUBTRACT = 74,
			DECIMAL = DELETE,
			DIVIDE = 53,
			F1 = 59,
			F2 = 60,
			F3 = 61,
			F4 = 62,
			F5 = 63,
			F6 = 64,
			F7 = 65,
			F8 = 66,
			F9 = 67,
			F10 = 68,
			F11 = 87,
			F12 = 88,
			F13 = 100,
			F14 = 101,
			F15 = 102,
			F16 = 103,
			F17 = 104,
			F18 = 105,
			F19 = 106,
			F20 = 107,
			F21 = 108,
			F22 = 109,
			F23 = 110,
			F24 = 118,
			NUMLOCK = 69,
			SCROLL = CANCEL,
			LSHIFT = SHIFT,
			RSHIFT = 54,
			LCONTROL = CONTROL,
			RCONTROL = CONTROL,
			LMENU = MENU,
			RMENU = MENU,
			BROWSER_BACK = F19,
			BROWSER_FORWARD = F18,
			BROWSER_REFRESH = F16,
			BROWSER_STOP = F17,
			BROWSER_SEARCH = F14,
			BROWSER_FAVORITES = F15,
			BROWSER_HOME = KEY_M,
			VOLUME_MUTE = KEY_D,
			VOLUME_DOWN = KEY_C,
			VOLUME_UP = KEY_B,
			MEDIA_NEXT_TRACK = KEY_P,
			MEDIA_PREV_TRACK = KEY_Q,
			MEDIA_STOP = KEY_J,
			MEDIA_PLAY_PAUSE = KEY_G,
			LAUNCH_MAIL = F21,
			LAUNCH_MEDIA_SELECT = F22,
			LAUNCH_APP1 = F20,
			LAUNCH_APP2 = KEY_F,
			OEM_1 = 39,
			OEM_PLUS = 13,
			OEM_COMMA = 51,
			OEM_MINUS = 12,
			OEM_PERIOD = 52,
			OEM_2 = DIVIDE,
			OEM_3 = 41,
			OEM_4 = 26,
			OEM_5 = 43,
			OEM_6 = 27,
			OEM_7 = 40,
			OEM_8 = LBUTTON,
			OEM_102 = 86,
			PROCESSKEY = LBUTTON,
			PACKET = LBUTTON,
			ATTN = LBUTTON,
			CRSEL = LBUTTON,
			EXSEL = LBUTTON,
			EREOF = APPS,
			PLAY = LBUTTON,
			ZOOM = 98,
			NONAME = LBUTTON,
			PA1 = LBUTTON,
			OEM_CLEAR = LBUTTON
		}

		public struct KeyboardInputState
		{
			public uint Value;

			public uint RepeatCount => Value & 0xFFFF;

			public uint ScanCode => (Value >> 16) & 0xFF;

			public bool IsExtendedKey => (((int)Value >> 24) & 1) == 1;

			public bool IsContextual => (((int)Value >> 29) & 1) == 1;

			public bool IsPreviousKeyStatePressed => (((int)Value >> 30) & 1) == 1;

			public bool IsKeyUpTransition => (((int)Value >> 31) & 1) == 1;

			public KeyboardInputState(uint value)
			{
				Value = value;
			}
		}

		public struct HardwareInput
		{
			public uint Message;

			public ushort Low;

			public ushort High;

			public uint WParam
			{
				get
				{
					return (uint)((High << 16) | Low);
				}
				set
				{
					Low = (ushort)value;
					High = (ushort)(value >> 16);
				}
			}
		}

		public struct MouseInput
		{
			public int X;

			public int Y;

			public uint Data;

			public MouseInputFlags Flags;

			public uint Time;

			public IntPtr ExtraInfo;
		}

		public struct KeyboardInput
		{
			public ushort VirtualKeyCode;

			public ushort ScanCode;

			public KeyboardInputFlags Flags;

			public uint Time;

			public IntPtr ExtraInfo;

			public VirtualKey Key
			{
				get
				{
					return (VirtualKey)VirtualKeyCode;
				}
				set
				{
					VirtualKeyCode = (ushort)value;
				}
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct InputPacket
		{
			[FieldOffset(0)]
			public MouseInput MouseInput;

			[FieldOffset(0)]
			public KeyboardInput KeyboardInput;

			[FieldOffset(0)]
			public HardwareInput HardwareInput;
		}

		public struct Input
		{
			public InputType Type;

			public InputPacket Packet;

			public static void InitHardwareInput(out Input input, uint message, ushort low, ushort high)
			{
				input = new Input
				{
					Type = InputType.INPUT_HARDWARE,
					Packet = new InputPacket
					{
						HardwareInput = new HardwareInput
						{
							Message = message,
							Low = low,
							High = high
						}
					}
				};
			}

			public static void InitHardwareInput(out Input input, uint message, uint wParam)
			{
				InitHardwareInput(out input, message, (ushort)wParam, (ushort)(wParam >> 16));
			}

			public static void InitKeyboardInput(out Input input, ushort scanCode, bool isKeyUp, bool isExtendedKey = false, uint timestampMillis = 0u)
			{
				input = new Input
				{
					Type = InputType.INPUT_KEYBOARD,
					Packet = new InputPacket
					{
						KeyboardInput = 
						{
							Time = timestampMillis,
							Flags = KeyboardInputFlags.KEYEVENTF_SCANCODE,
							ScanCode = scanCode,
							VirtualKeyCode = 0
						}
					}
				};
				if (isKeyUp)
				{
					input.Packet.KeyboardInput.Flags |= KeyboardInputFlags.KEYEVENTF_KEYUP;
				}
				if (isExtendedKey)
				{
					input.Packet.KeyboardInput.Flags |= KeyboardInputFlags.KEYEVENTF_EXTENDEDKEY;
				}
			}

			public static void InitKeyboardInput(out Input input, char charCode, bool isKeyUp, uint timestampMillis = 0u)
			{
				input = new Input
				{
					Type = InputType.INPUT_KEYBOARD,
					Packet = new InputPacket
					{
						KeyboardInput = 
						{
							Time = timestampMillis,
							Flags = KeyboardInputFlags.KEYEVENTF_UNICODE,
							ScanCode = charCode,
							VirtualKeyCode = 0
						}
					}
				};
				if (isKeyUp)
				{
					input.Packet.KeyboardInput.Flags |= KeyboardInputFlags.KEYEVENTF_KEYUP;
				}
			}

			public static void InitKeyboardInput(out Input input, VirtualKey key, bool isKeyUp, uint timestampMillis = 0u)
			{
				input = new Input
				{
					Type = InputType.INPUT_KEYBOARD,
					Packet = new InputPacket
					{
						KeyboardInput = 
						{
							Time = timestampMillis,
							Key = key,
							ScanCode = 0,
							Flags = (KeyboardInputFlags)0
						}
					}
				};
				if (isKeyUp)
				{
					input.Packet.KeyboardInput.Flags |= KeyboardInputFlags.KEYEVENTF_KEYUP;
				}
			}

			public static void InitMouseInput(out Input input, int x, int y, MouseInputFlags flags, uint data = 0u, uint timestampMillis = 0u)
			{
				input = new Input
				{
					Type = InputType.INPUT_MOUSE,
					Packet = new InputPacket
					{
						MouseInput = 
						{
							Time = timestampMillis,
							X = x,
							Y = y,
							Data = data,
							Flags = flags
						}
					}
				};
			}
		}

		public struct LastInputInfo
		{
			public uint Size;

			public uint Time;
		}

		public struct KeyState
		{
			public short Value;

			public bool IsPressed
			{
				get
				{
					return (Value & 0x8000) > 0;
				}
				set
				{
					if (value)
					{
						Value = (short)(Value | 0x8000);
					}
					else
					{
						Value &= short.MaxValue;
					}
				}
			}

			public bool IsToggled
			{
				get
				{
					return (Value & 1) == 1;
				}
				set
				{
					if (value)
					{
						Value |= 1;
					}
					else
					{
						Value = (short)(Value & 0xFFFE);
					}
				}
			}

			public KeyState(short value)
			{
				Value = value;
			}
		}

		public struct CursorInfo
		{
			public uint Size;

			public CursorInfoFlags Flags;

			public IntPtr CursorHandle;

			public Point ScreenPosition;
		}

		public delegate IntPtr WindowProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

		public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		public delegate IntPtr GetMsgProc(int code, IntPtr wParam, IntPtr lParam);

		public delegate void TimerProc(IntPtr hWnd, uint uMsg, IntPtr nIdEvent, uint dwTickCountMillis);

		public struct Message
		{
			public IntPtr Hwnd;

			public uint Value;

			public IntPtr WParam;

			public IntPtr LParam;

			public uint Time;

			public Point Point;
		}

		public struct PaintStruct
		{
			public IntPtr hdc;

			public bool fErase;

			public RECT rcPaint;

			public bool fRestore;

			public bool fIncUpdate;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public byte[] rgbReserved;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct WindowClassEx
		{
			public uint Size;

			public WindowClassStyles Styles;

			[MarshalAs(UnmanagedType.FunctionPtr)]
			public WindowProc WindowProc;

			public int ClassExtraBytes;

			public int WindowExtraBytes;

			public IntPtr InstanceHandle;

			public IntPtr IconHandle;

			public IntPtr CursorHandle;

			public IntPtr BackgroundBrushHandle;

			public string MenuName;

			public string ClassName;

			public IntPtr SmallIconHandle;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct WindowClassExPtr
		{
			public uint Size;

			public WindowClassStyles Styles;

			[MarshalAs(UnmanagedType.FunctionPtr)]
			public WindowProc WindowProc;

			public int ClassExtraBytes;

			public int WindowExtraBytes;

			public IntPtr InstanceHandle;

			public IntPtr IconHandle;

			public IntPtr CursorHandle;

			public IntPtr BackgroundBrushHandle;

			public IntPtr MenuName;

			public IntPtr ClassName;

			public IntPtr SmallIconHandle;
		}

		public struct WindowClassExBlittable
		{
			public uint Size;

			public WindowClassStyles Styles;

			public IntPtr WindowProc;

			public int ClassExtraBytes;

			public int WindowExtraBytes;

			public IntPtr InstanceHandle;

			public IntPtr IconHandle;

			public IntPtr CursorHandle;

			public IntPtr BackgroundBrushHandle;

			public IntPtr MenuName;

			public IntPtr ClassName;

			public IntPtr SmallIconHandle;
		}

		public struct WindowInfo
		{
			public uint Size;

			public Rectangle WindowRect;

			public Rectangle ClientRect;

			public WindowStyles Styles;

			public ExtendedWindowStyles ExStyles;

			public uint WindowStatus;

			public uint BorderX;

			public uint BorderY;

			public ushort WindowType;

			public ushort CreatorVersion;
		}

		public struct CreateStruct
		{
			public IntPtr CreateParams;

			public IntPtr InstanceHandle;

			public IntPtr MenuHandle;

			public IntPtr ParentHwnd;

			public int Height;

			public int Width;

			public int Y;

			public int X;

			public WindowStyles Styles;

			public IntPtr Name;

			public IntPtr ClassName;

			public WindowExStyles ExStyles;
		}

		public struct WindowPlacement
		{
			public uint Size;

			public WindowPlacementFlags Flags;

			public ShowWindowCommands ShowCmd;

			public Point MinPosition;

			public Point MaxPosition;

			public Rectangle NormalPosition;
		}

		public struct BlendFunction
		{
			public byte BlendOp;

			public byte BlendFlags;

			public byte SourceConstantAlpha;

			public AlphaFormat AlphaFormat;
		}

		public struct AnimationInfo
		{
			public uint Size;

			public int MinAnimate;

			public AnimationInfo(int iMinAnimate)
			{
				Size = (uint)Marshal.SizeOf(typeof(AnimationInfo));
				MinAnimate = iMinAnimate;
			}
		}

		public struct MinimizedMetrics
		{
			public uint Size;

			public int Width;

			public int HorizontalGap;

			public int VerticalGap;

			public ArrangeFlags Arrange;
		}

		public struct TrackMouseEventOptions
		{
			public uint Size;

			public TrackMouseEventFlags Flags;

			public IntPtr TrackedHwnd;

			public uint HoverTime;

			public const uint DefaultHoverTime = uint.MaxValue;
		}

		public struct MinMaxInfo
		{
			private Point Reserved;

			public Point MaxSize;

			public Point MaxPosition;

			public Point MinTrackSize;

			public Point MaxTrackSize;
		}

		public struct WindowPosition
		{
			public IntPtr Hwnd;

			public IntPtr HwndZOrderInsertAfter;

			public int X;

			public int Y;

			public int Width;

			public int Height;

			public WindowPositionFlags Flags;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct NcCalcSizeRegionUnion
		{
			[FieldOffset(0)]
			public NcCalcSizeInput Input;

			[FieldOffset(0)]
			public NcCalcSizeOutput Output;
		}

		public struct NcCalcSizeInput
		{
			public Rectangle TargetWindowRect;

			public Rectangle CurrentWindowRect;

			public Rectangle CurrentClientRect;
		}

		public struct NcCalcSizeOutput
		{
			public Rectangle TargetClientRect;

			public Rectangle DestRect;

			public Rectangle SrcRect;
		}

		public struct MonitorInfo
		{
			public uint Size;

			public Rectangle MonitorRect;

			public Rectangle WorkRect;

			public MonitorInfoFlag Flags;
		}

		public struct TitleBarInfo
		{
			public uint Size;

			public Rectangle TitleBarRect;

			public ElementSystemStates TitleBarStates;

			private ElementSystemStates Reserved;

			public ElementSystemStates MinimizeButtonStates;

			public ElementSystemStates MaximizeButtonStates;

			public ElementSystemStates HelpButtonStates;

			public ElementSystemStates CloseButtonStates;
		}

		[Flags]
		public enum WindowStyles
		{
			WS_BORDER = 0x800000,
			WS_CAPTION = 0xC00000,
			WS_CHILD = 0x40000000,
			WS_CHILDWINDOW = WS_CHILD,
			WS_CLIPCHILDREN = 0x2000000,
			WS_CLIPSIBLINGS = 0x4000000,
			WS_DISABLED = 0x8000000,
			WS_DLGFRAME = 0x400000,
			WS_GROUP = 0x20000,
			WS_HSCROLL = 0x100000,
			WS_ICONIC = 0x20000000,
			WS_MAXIMIZE = 0x1000000,
			WS_MAXIMIZEBOX = 0x10000,
			WS_MINIMIZE = WS_ICONIC,
			WS_MINIMIZEBOX = WS_GROUP,
			WS_OVERLAPPED = 0,
			WS_OVERLAPPEDWINDOW = 0xCF0000,
			WS_POPUP = int.MinValue,
			WS_POPUPWINDOW = -2138570752,
			WS_SIZEBOX = 0x40000,
			WS_SYSMENU = 0x80000,
			WS_TABSTOP = WS_MAXIMIZEBOX,
			WS_THICKFRAME = WS_SIZEBOX,
			WS_TILED = 0,
			WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
			WS_VISIBLE = 0x10000000,
			WS_VSCROLL = 0x200000
		}

		[Flags]
		public enum WindowExStyles
		{
			WS_EX_ACCEPTFILES = 0x10,
			WS_EX_APPWINDOW = 0x40000,
			WS_EX_CLIENTEDGE = 0x200,
			WS_EX_COMPOSITED = 0x2000000,
			WS_EX_CONTEXTHELP = 0x400,
			WS_EX_CONTROLPARENT = 0x10000,
			WS_EX_DLGMODALFRAME = 1,
			WS_EX_LAYERED = 0x80000,
			WS_EX_LAYOUTRTL = 0x400000,
			WS_EX_LEFT = 0,
			WS_EX_LEFTSCROLLBAR = 0x4000,
			WS_EX_LTRREADING = 0,
			WS_EX_MDICHILD = 0x40,
			WS_EX_NOACTIVATE = 0x8000000,
			WS_EX_NOINHERITLAYOUT = 0x100000,
			WS_EX_NOPARENTNOTIFY = 4,
			WS_EX_NOREDIRECTIONBITMAP = 0x200000,
			WS_EX_OVERLAPPEDWINDOW = 0x300,
			WS_EX_PALETTEWINDOW = 0x188,
			WS_EX_RIGHT = 0x1000,
			WS_EX_RIGHTSCROLLBAR = 0,
			WS_EX_RTLREADING = 0x2000,
			WS_EX_STATICEDGE = 0x20000,
			WS_EX_TOOLWINDOW = 0x80,
			WS_EX_TOPMOST = 8,
			WS_EX_TRANSPARENT = 0x20,
			WS_EX_WINDOWEDGE = 0x100
		}

		[Flags]
		public enum WindowClassStyles
		{
			CS_BYTEALIGNCLIENT = 0x1000,
			CS_BYTEALIGNWINDOW = 0x2000,
			CS_CLASSDC = 0x40,
			CS_DBLCLKS = 8,
			CS_DROPSHADOW = 0x20000,
			CS_GLOBALCLASS = 0x4000,
			CS_HREDRAW = 2,
			CS_NOCLOSE = 0x200,
			CS_OWNDC = 0x20,
			CS_PARENTDC = 0x80,
			CS_SAVEBITS = 0x800,
			CS_VREDRAW = 1
		}

		[Flags]
		public enum AnimateWindowFlags
		{
			AW_ACTIVATE = 0x20000,
			AW_BLEND = 0x80000,
			AW_CENTER = 0x10,
			AW_HIDE = 0x10000,
			AW_HOR_POSITIVE = 1,
			AW_HOR_NEGATIVE = 2,
			AW_SLIDE = 0x40000,
			AW_VER_POSITIVE = 4,
			AW_VER_NEGATIVE = 8
		}

		[Flags]
		public enum DrawTextFormatFlags
		{
			DT_BOTTOM = 8,
			DT_CALCRECT = 0x400,
			DT_CENTER = 1,
			DT_EDITCONTROL = 0x2000,
			DT_END_ELLIPSIS = 0x8000,
			DT_EXPANDTABS = 0x40,
			DT_EXTERNALLEADING = 0x200,
			DT_HIDEPREFIX = 0x100000,
			DT_INTERNAL = 0x1000,
			DT_LEFT = 0,
			DT_MODIFYSTRING = 0x10000,
			DT_NOCLIP = 0x100,
			DT_NOFULLWIDTHCHARBREAK = 0x80000,
			DT_NOPREFIX = 0x800,
			DT_PATH_ELLIPSIS = 0x4000,
			DT_PREFIXONLY = 0x200000,
			DT_RIGHT = 2,
			DT_RTLREADING = 0x20000,
			DT_SINGLELINE = 0x20,
			DT_TABSTOP = 0x80,
			DT_TOP = 0,
			DT_VCENTER = 4,
			DT_WORDBREAK = 0x10,
			DT_WORD_ELLIPSIS = 0x40000
		}

		[Flags]
		public enum CreateWindowFlags
		{
			CW_USEDEFAULT = int.MinValue
		}

		[Flags]
		public enum RegionModeFlags
		{
			RGN_AND = 1,
			RGN_OR = 2,
			RGN_XOR = RGN_AND | RGN_OR,
			RGN_DIFF = 4,
			RGN_COPY = RGN_AND | RGN_DIFF,
			RGN_MIN = RGN_AND,
			RGN_MAX = RGN_COPY
		}

		[Flags]
		public enum DeviceCapability
		{
			DC_ACTIVE = 1,
			DC_SMALLCAP = 2,
			DC_ICON = 4,
			DC_TEXT = 8,
			DC_INBUTTON = 0x10,
			DC_GRADIENT = 0x20,
			DC_BUTTONS = 0x1000,
			DC_HASDEFID = 0x534B,
			DC_BRUSH = DC_SMALLCAP | DC_INBUTTON,
			DC_PEN = DC_BRUSH | DC_ACTIVE,
			DC_FIELDS = DC_ACTIVE,
			DC_PAPERS = DC_SMALLCAP,
			DC_PAPERSIZE = 3,
			DC_MINEXTENT = DC_ICON,
			DC_MAXEXTENT = DC_ACTIVE | DC_ICON,
			DC_BINS = DC_SMALLCAP | DC_ICON,
			DC_DUPLEX = DC_PAPERSIZE | DC_ICON,
			DC_SIZE = DC_TEXT,
			DC_EXTRA = 9,
			DC_VERSION = 0xA,
			DC_DRIVER = 0xB,
			DC_BINNAMES = DC_ICON | DC_TEXT,
			DC_ENUMRESOLUTIONS = DC_MAXEXTENT | DC_TEXT,
			DC_FILEDEPENDENCIES = DC_BINS | DC_TEXT,
			DC_TRUETYPE = DC_DUPLEX | DC_TEXT,
			DC_PAPERNAMES = DC_INBUTTON,
			DC_ORIENTATION = 0x11,
			DC_COPIES = 0x12,
			DC_BINADJUST = DC_PEN,
			DC_EMF_COMPLIANT = DC_ICON | DC_INBUTTON,
			DC_DATATYPE_PRODUCED = DC_MAXEXTENT | DC_INBUTTON,
			DC_COLLATE = DC_BRUSH | DC_ICON,
			DC_MANUFACTURER = DC_PEN | DC_ICON,
			DC_MODEL = DC_TEXT | DC_INBUTTON,
			DC_PERSONALITY = DC_EXTRA | DC_INBUTTON,
			DC_PRINTRATE = DC_BRUSH | DC_TEXT,
			DC_PRINTRATEUNIT = DC_PEN | DC_TEXT,
			DC_PRINTERMEM = DC_BINNAMES | DC_INBUTTON,
			DC_MEDIAREADY = DC_ENUMRESOLUTIONS | DC_INBUTTON,
			DC_STAPLE = DC_FILEDEPENDENCIES | DC_INBUTTON,
			DC_PRINTRATEPPM = DC_TRUETYPE | DC_INBUTTON,
			DC_COLORDEVICE = DC_GRADIENT,
			DC_NUP = DC_ACTIVE | DC_GRADIENT,
			DC_MEDIATYPENAMES = DC_SMALLCAP | DC_GRADIENT,
			DC_MEDIATYPES = DC_PAPERSIZE | DC_GRADIENT
		}

		[Flags]
		public enum ShowWindowCommands
		{
			SW_FORCEMINIMIZE = 0xB,
			SW_HIDE = 0,
			SW_MAXIMIZE = 3,
			SW_MINIMIZE = 6,
			SW_RESTORE = 9,
			SW_SHOW = 5,
			SW_SHOWDEFAULT = 0xA,
			SW_SHOWMAXIMIZED = 3,
			SW_SHOWMINIMIZED = 2,
			SW_SHOWMINNOACTIVE = 7,
			SW_SHOWNA = 8,
			SW_SHOWNOACTIVATE = 4,
			SW_SHOWNORMAL = 1
		}

		[Flags]
		public enum WindowPositionFlags
		{
			SWP_ASYNCWINDOWPOS = 0x4000,
			SWP_DEFERERASE = 0x2000,
			SWP_DRAWFRAME = 0x20,
			SWP_FRAMECHANGED = SWP_DRAWFRAME,
			SWP_HIDEWINDOW = 0x80,
			SWP_NOACTIVATE = 0x10,
			SWP_NOCOPYBITS = 0x100,
			SWP_NOMOVE = 2,
			SWP_NOOWNERZORDER = 0x200,
			SWP_NOREDRAW = 8,
			SWP_NOREPOSITION = SWP_NOOWNERZORDER,
			SWP_NOSENDCHANGING = 0x400,
			SWP_NOSIZE = 1,
			SWP_NOZORDER = 4,
			SWP_SHOWWINDOW = 0x40
		}

		[Flags]
		public enum DeviceContextFlags
		{
			DCX_WINDOW = 0,
			DCX_CACHE = 0,
			DCX_NORESETATTRS = 0,
			DCX_CLIPCHILDREN = 0,
			DCX_CLIPSIBLINGS = 1,
			DCX_PARENTCLIP = 2,
			DCX_EXCLUDERGN = 4,
			DCX_INTERSECTRGN = 8,
			DCX_EXCLUDEUPDATE = 0x10,
			DCX_INTERSECTUPDATE = 0x20,
			DCX_LOCKWINDOWUPDATE = 0x40,
			DCX_VALIDATE = 0x20000
		}

		[Flags]
		public enum WindowPlacementFlags
		{
			SETMINPOSITION = 1,
			RESTORETOMAXIMIZED = 2,
			ASYNCWINDOWPLACEMENT = 4
		}

		[Flags]
		public enum RedrawWindowFlags
		{
			RDW_INVALIDATE = 1,
			RDW_INTERNALPAINT = 2,
			RDW_ERASE = 4,
			RDW_VALIDATE = 8,
			RDW_NOINTERNALPAINT = 0x10,
			RDW_NOERASE = 0x20,
			RDW_NOCHILDREN = 0x40,
			RDW_ALLCHILDREN = 0x80,
			RDW_UPDATENOW = 0x100,
			RDW_ERASENOW = 0x200,
			RDW_FRAME = 0x400,
			RDW_NOFRAME = 0x800
		}

		[Flags]
		public enum ClassLongFlags
		{
			GCL_CBCLSEXTRA = -20,
			GCL_CBWNDEXTRA = -18,
			GCL_HBRBACKGROUND = -10,
			GCL_HCURSOR = -12,
			GCL_HICON = -14,
			GCL_HICONSM = -34,
			GCL_HMODULE = -16,
			GCL_MENUNAME = -8,
			GCL_STYLE = -26,
			GCL_WNDPROC = -24
		}

		[Flags]
		public enum QueueStatusFlags
		{
			QS_ALLEVENTS = 0x4BF,
			QS_ALLINPUT = 0x4FF,
			QS_ALLPOSTMESSAGE = 0x100,
			QS_HOTKEY = 0x80,
			QS_INPUT = 0x407,
			QS_KEY = 1,
			QS_MOUSE = 6,
			QS_MOUSEBUTTON = 4,
			QS_MOUSEMOVE = 2,
			QS_PAINT = 0x20,
			QS_POSTMESSAGE = 8,
			QS_RAWINPUT = 0x400,
			QS_SENDMESSAGE = 0x40,
			QS_TIMER = 0x10,
			QS_REFRESH = 0xA5
		}

		[Flags]
		public enum PeekMessageFlags
		{
			PM_NOREMOVE = 0,
			PM_REMOVE = 1,
			PM_NOYIELD = 2,
			PM_QS_INPUT = 0x4070000,
			PM_QS_PAINT = 0x200000,
			PM_QS_POSTMESSAGE = 0x980000,
			PM_QS_SENDMESSAGE = 0x400000
		}

		[Flags]
		public enum ChildWindowFromPointFlags
		{
			CWP_ALL = 0,
			CWP_SKIPDISABLED = 2,
			CWP_SKIPINVISIBLE = 1,
			CWP_SKIPTRANSPARENT = 4
		}

		[Flags]
		public enum SystemParamtersInfoFlags
		{
			None = 0,
			SPIF_UPDATEINIFILE = 1,
			SPIF_SENDCHANGE = 2,
			SPIF_SENDWININICHANGE = SPIF_SENDCHANGE
		}

		[Flags]
		public enum ArrangeFlags
		{
			ARW_BOTTOMLEFT = 0,
			ARW_BOTTOMRIGHT = 1,
			ARW_TOPLEFT = 2,
			ARW_TOPRIGHT = ARW_BOTTOMRIGHT | ARW_TOPLEFT,
			ARW_LEFT = 0,
			ARW_RIGHT = 0,
			ARW_UP = 4,
			ARW_DOWN = ARW_UP,
			ARW_HIDE = 8
		}

		[Flags]
		public enum LoadResourceFlags
		{
			LR_CREATEDIBSECTION = 0x2000,
			LR_DEFAULTCOLOR = 0,
			LR_DEFAULTSIZE = 0x40,
			LR_LOADFROMFILE = 0x10,
			LR_LOADMAP3DCOLORS = 0x1000,
			LR_LOADTRANSPARENT = 0x20,
			LR_MONOCHROME = 1,
			LR_SHARED = 0x8000,
			LR_VGACOLOR = 0x80
		}

		[Flags]
		public enum MessageBoxFlags
		{
			MB_ABORTRETRYIGNORE = 2,
			MB_CANCELTRYCONTINUE = 6,
			MB_HELP = 0x4000,
			MB_OK = 0,
			MB_OKCANCEL = 1,
			MB_RETRYCANCEL = 5,
			MB_YESNO = 4,
			MB_YESNOCANCEL = MB_ABORTRETRYIGNORE | MB_OKCANCEL,
			MB_ICONEXCLAMATION = 0x30,
			MB_ICONWARNING = MB_ICONEXCLAMATION,
			MB_ICONINFORMATION = 0x40,
			MB_ICONASTERISK = MB_ICONINFORMATION,
			MB_ICONQUESTION = 0x20,
			MB_ICONSTOP = 0x10,
			MB_ICONERROR = MB_ICONSTOP,
			MB_ICONHAND = MB_ICONSTOP,
			MB_DEFBUTTON1 = 0,
			MB_DEFBUTTON2 = 0x100,
			MB_DEFBUTTON3 = 0x200,
			MB_DEFBUTTON4 = MB_DEFBUTTON2 | MB_DEFBUTTON3,
			MB_APPLMODAL = 0,
			MB_SYSTEMMODAL = 0x1000,
			MB_TASKMODAL = 0x2000,
			MB_DEFAULT_DESKTOP_ONLY = 0x20000,
			MB_RIGHT = 0x80000,
			MB_RTLREADING = 0x100000,
			MB_SETFOREGROUND = 0x10000,
			MB_TOPMOST = 0x40000,
			MB_SERVICE_NOTIFICATION = 0x200000
		}

		[Flags]
		public enum TrackMouseEventFlags
		{
			TME_CANCEL = int.MinValue,
			TME_HOVER = 1,
			TME_LEAVE = 2,
			TME_NONCLIENT = 0x10,
			TME_QUERY = 0x40000000
		}

		[Flags]
		public enum ShowWindowStatusFlags
		{
			SW_USER_CALL = 0,
			SW_OTHERUNZOOM = 4,
			SW_OTHERZOOM = 2,
			SW_PARENTCLOSING = 1,
			SW_PARENTOPENING = SW_OTHERZOOM | SW_PARENTCLOSING
		}

		[Flags]
		public enum WindowViewRegionFlags
		{
			WVR_DEFAULT = 0,
			WVR_ALIGNTOP = 0x10,
			WVR_ALIGNRIGHT = 0x80,
			WVR_ALIGNLEFT = 0x20,
			WVR_ALIGNBOTTOM = 0x40,
			WVR_HREDRAW = 0x100,
			WVR_VREDRAW = 0x200,
			WVR_REDRAW = WVR_HREDRAW | WVR_VREDRAW,
			WVR_VALIDRECTS = 0x400
		}

		[Flags]
		public enum ElementSystemStates
		{
			STATE_SYSTEM_FOCUSABLE = 0x100000,
			STATE_SYSTEM_INVISIBLE = 0x8000,
			STATE_SYSTEM_OFFSCREEN = 0x10000,
			STATE_SYSTEM_UNAVAILABLE = 1,
			STATE_SYSTEM_PRESSED = 8
		}

		public enum SystemCursor
		{
			IDC_APPSTARTING = 32650,
			IDC_ARROW = 32512,
			IDC_CROSS = 32515,
			IDC_HAND = 32649,
			IDC_HELP = 32651,
			IDC_IBEAM = 32513,
			IDC_ICON = 32641,
			IDC_NO = 32648,
			IDC_SIZE = 32640,
			IDC_SIZEALL = 32646,
			IDC_SIZENESW = 32643,
			IDC_SIZENS = 32645,
			IDC_SIZENWSE = 32642,
			IDC_SIZEWE = 32644,
			IDC_UPARROW = 32516,
			IDC_WAIT = 32514
		}

		public enum SystemIcon
		{
			IDI_APPLICATION = 32512,
			IDI_HAND = 32513,
			IDI_QUESTION = 32514,
			IDI_EXCLAMATION = 32515,
			IDI_ASTERISK = 32516,
			IDI_WINLOGO = 32517,
			IDI_SHIELD = 32518,
			IDI_WARNING = IDI_EXCLAMATION,
			IDI_ERROR = IDI_HAND,
			IDI_INFORMATION = IDI_ASTERISK
		}

		public enum HwndZOrder
		{
			HWND_BOTTOM = 1,
			HWND_NOTOPMOST = -2,
			HWND_TOP = 0,
			HWND_TOPMOST = -1
		}

		public enum RegionType
		{
			ERROR,
			NULLREGION,
			SIMPLEREGION,
			COMPLEXREGION
		}

		public enum GetWindowFlag
		{
			GW_CHILD = 5,
			GW_ENABLEDPOPUP = 6,
			GW_HWNDFIRST = 0,
			GW_HWNDLAST = 1,
			GW_HWNDNEXT = 2,
			GW_HWNDPREV = 3,
			GW_OWNER = 4
		}

		public enum AlphaFormat : byte
		{
			AC_SRC_OVER,
			AC_SRC_ALPHA
		}

		public enum MonitorFlag
		{
			MONITOR_DEFAULTTONULL = 0,
			MonitorDefaultToNull = MONITOR_DEFAULTTONULL,
			MONITOR_DEFAULTTOPRIMARY = 1,
			MonitorDefaultToPrimary = MONITOR_DEFAULTTOPRIMARY,
			MONITOR_DEFAULTTONEAREST = 2,
			MonitorDefaultToNearest = MONITOR_DEFAULTTONEAREST
		}

		public enum WindowSizeFlag
		{
			SIZE_MAXHIDE = 4,
			SIZE_MAXIMIZED = 2,
			SIZE_MAXSHOW = 3,
			SIZE_MINIMIZED = 1,
			SIZE_RESTORED = 0
		}

		public enum WindowActivateFlag
		{
			WA_ACTIVE = 1,
			WA_CLICKACTIVE = 2,
			WA_INACTIVE = 0
		}

		public enum HitTestResult
		{
			HTERROR = -2,
			HTTRANSPARENT = -1,
			HTNOWHERE = 0,
			HTCLIENT = 1,
			HTCAPTION = 2,
			HTSYSMENU = 3,
			HTGROWBOX = 4,
			HTSIZE = HTGROWBOX,
			HTMENU = 5,
			HTHSCROLL = 6,
			HTVSCROLL = 7,
			HTMINBUTTON = 8,
			HTREDUCE = HTMINBUTTON,
			HTMAXBUTTON = 9,
			HTZOOM = HTMAXBUTTON,
			HTLEFT = 10,
			HTRIGHT = 11,
			HTTOP = 12,
			HTTOPLEFT = 13,
			HTTOPRIGHT = 14,
			HTBOTTOM = 15,
			HTBOTTOMLEFT = 16,
			HTBOTTOMRIGHT = 17,
			HTBORDER = 18,
			HTOBJECT = 19,
			HTCLOSE = 20,
			HTHELP = 21
		}

		public enum ResourceImageType
		{
			IMAGE_BITMAP = 0,
			IMAGE_CURSOR = 2,
			IMAGE_ICON = 1
		}

		public enum MessageBoxResult
		{
			IDABORT = 3,
			IDCANCEL = 2,
			IDCONTINUE = 11,
			IDIGNORE = 5,
			IDNO = 7,
			IDOK = 1,
			IDRETRY = 4,
			IDTRYAGAIN = 10,
			IDYES = 6
		}

		public enum SystemColor
		{
			COLOR_SCROLLBAR = 0,
			COLOR_BACKGROUND = 1,
			COLOR_ACTIVECAPTION = 2,
			COLOR_INACTIVECAPTION = 3,
			COLOR_MENU = 4,
			COLOR_WINDOW = 5,
			COLOR_WINDOWFRAME = 6,
			COLOR_MENUTEXT = 7,
			COLOR_WINDOWTEXT = 8,
			COLOR_CAPTIONTEXT = 9,
			COLOR_ACTIVEBORDER = 10,
			COLOR_INACTIVEBORDER = 11,
			COLOR_APPWORKSPACE = 12,
			COLOR_HIGHLIGHT = 13,
			COLOR_HIGHLIGHTTEXT = 14,
			COLOR_BTNFACE = 15,
			COLOR_BTNSHADOW = 16,
			COLOR_GRAYTEXT = 17,
			COLOR_BTNTEXT = 18,
			COLOR_INACTIVECAPTIONTEXT = 19,
			COLOR_BTNHIGHLIGHT = 20,
			COLOR_3DDKSHADOW = 21,
			COLOR_3DLIGHT = 22,
			COLOR_INFOTEXT = 23,
			COLOR_INFOBK = 24,
			COLOR_HOTLIGHT = 26,
			COLOR_GRADIENTACTIVECAPTION = 27,
			COLOR_GRADIENTINACTIVECAPTION = 28,
			COLOR_MENUHILIGHT = 29,
			COLOR_MENUBAR = 30,
			COLOR_DESKTOP = COLOR_BACKGROUND,
			COLOR_3DFACE = COLOR_BTNFACE,
			COLOR_3DSHADOW = COLOR_BTNSHADOW,
			COLOR_3DHIGHLIGHT = COLOR_BTNHIGHLIGHT,
			COLOR_3DHILIGHT = COLOR_BTNHIGHLIGHT,
			COLOR_BTNHILIGHT = COLOR_BTNHIGHLIGHT
		}

		public enum SystemMetrics
		{
			SM_ARRANGE = 56,
			SM_CLEANBOOT = 67,
			SM_CMONITORS = 80,
			SM_CMOUSEBUTTONS = 43,
			SM_CONVERTIBLESLATEMODE = 8195,
			SM_CXBORDER = 5,
			SM_CXCURSOR = 13,
			SM_CXDLGFRAME = 7,
			SM_CXDOUBLECLK = 36,
			SM_CXDRAG = 68,
			SM_CXEDGE = 45,
			SM_CXFIXEDFRAME = SM_CXDLGFRAME,
			SM_CXFOCUSBORDER = 83,
			SM_CXFRAME = 32,
			SM_CXFULLSCREEN = 16,
			SM_CXHSCROLL = 21,
			SM_CXHTHUMB = 10,
			SM_CXICON = 11,
			SM_CXICONSPACING = 38,
			SM_CXMAXIMIZED = 61,
			SM_CXMAXTRACK = 59,
			SM_CXMENUCHECK = 71,
			SM_CXMENUSIZE = 54,
			SM_CXMIN = 28,
			SM_CXMINIMIZED = 57,
			SM_CXMINSPACING = 47,
			SM_CXMINTRACK = 34,
			SM_CXPADDEDBORDER = 92,
			SM_CXSCREEN = 0,
			SM_CXSIZE = 30,
			SM_CXSIZEFRAME = SM_CXFRAME,
			SM_CXSMICON = 49,
			SM_CXSMSIZE = 52,
			SM_CXVIRTUALSCREEN = 78,
			SM_CXVSCROLL = 2,
			SM_CYBORDER = 6,
			SM_CYCAPTION = 4,
			SM_CYCURSOR = 14,
			SM_CYDLGFRAME = 8,
			SM_CYDOUBLECLK = 37,
			SM_CYDRAG = 69,
			SM_CYEDGE = 46,
			SM_CYFIXEDFRAME = SM_CYDLGFRAME,
			SM_CYFOCUSBORDER = 84,
			SM_CYFRAME = 33,
			SM_CYFULLSCREEN = 17,
			SM_CYHSCROLL = 3,
			SM_CYICON = 12,
			SM_CYICONSPACING = 39,
			SM_CYKANJIWINDOW = 18,
			SM_CYMAXIMIZED = 62,
			SM_CYMAXTRACK = 60,
			SM_CYMENU = 15,
			SM_CYMENUCHECK = 72,
			SM_CYMENUSIZE = 55,
			SM_CYMIN = 29,
			SM_CYMINIMIZED = 58,
			SM_CYMINSPACING = 48,
			SM_CYMINTRACK = 35,
			SM_CYSCREEN = 1,
			SM_CYSIZE = 31,
			SM_CYSIZEFRAME = SM_CYFRAME,
			SM_CYSMCAPTION = 51,
			SM_CYSMICON = 50,
			SM_CYSMSIZE = 53,
			SM_CYVIRTUALSCREEN = 79,
			SM_CYVSCROLL = 20,
			SM_CYVTHUMB = 9,
			SM_DBCSENABLED = 42,
			SM_DEBUG = 22,
			SM_DIGITIZER = 94,
			SM_IMMENABLED = 82,
			SM_MAXIMUMTOUCHES = 95,
			SM_MEDIACENTER = 87,
			SM_MENUDROPALIGNMENT = 40,
			SM_MIDEASTENABLED = 74,
			SM_MOUSEPRESENT = 19,
			SM_MOUSEHORIZONTALWHEELPRESENT = 91,
			SM_MOUSEWHEELPRESENT = 75,
			SM_NETWORK = 63,
			SM_PENWINDOWS = 41,
			SM_REMOTECONTROL = 8193,
			SM_REMOTESESSION = 4096,
			SM_SAMEDISPLAYFORMAT = 81,
			SM_SECURE = 44,
			SM_SERVERR2 = 89,
			SM_SHOWSOUNDS = 70,
			SM_SHUTTINGDOWN = 8192,
			SM_SLOWMACHINE = 73,
			SM_STARTER = 88,
			SM_SWAPBUTTON = 23,
			SM_SYSTEMDOCKED = 8196,
			SM_TABLETPC = 86,
			SM_XVIRTUALSCREEN = 76,
			SM_YVIRTUALSCREEN = 77
		}

		public enum SystemParametersAccessibilityInfo
		{
			SPI_GETACCESSTIMEOUT = 60,
			SPI_GETAUDIODESCRIPTION = 116,
			SPI_GETCLIENTAREAANIMATION = 4162,
			SPI_GETDISABLEOVERLAPPEDCONTENT = 4160,
			SPI_GETFILTERKEYS = 50,
			SPI_GETFOCUSBORDERHEIGHT = 8208,
			SPI_GETFOCUSBORDERWIDTH = 8206,
			SPI_GETHIGHCONTRAST = 66,
			SPI_GETLOGICALDPIOVERRIDE = 158,
			SPI_GETMESSAGEDURATION = 8214,
			SPI_GETMOUSECLICKLOCK = 4126,
			SPI_GETMOUSECLICKLOCKTIME = 8200,
			SPI_GETMOUSEKEYS = 54,
			SPI_GETMOUSESONAR = 4124,
			SPI_GETMOUSEVANISH = 4128,
			SPI_GETSCREENREADER = 70,
			SPI_GETSERIALKEYS = 62,
			SPI_GETSHOWSOUNDS = 56,
			SPI_GETSOUNDSENTRY = 64,
			SPI_GETSTICKYKEYS = 58,
			SPI_GETTOGGLEKEYS = 52,
			SPI_SETACCESSTIMEOUT = 61,
			SPI_SETAUDIODESCRIPTION = 117,
			SPI_SETCLIENTAREAANIMATION = 4163,
			SPI_SETDISABLEOVERLAPPEDCONTENT = 4161,
			SPI_SETFILTERKEYS = 51,
			SPI_SETFOCUSBORDERHEIGHT = 8209,
			SPI_SETFOCUSBORDERWIDTH = 8207,
			SPI_SETHIGHCONTRAST = 67,
			SPI_SETLOGICALDPIOVERRIDE = 159,
			SPI_SETMESSAGEDURATION = 8215,
			SPI_SETMOUSECLICKLOCK = 4127,
			SPI_SETMOUSECLICKLOCKTIME = 8201,
			SPI_SETMOUSEKEYS = 55,
			SPI_SETMOUSESONAR = 4125,
			SPI_SETMOUSEVANISH = 4129,
			SPI_SETSCREENREADER = 71,
			SPI_SETSERIALKEYS = 63,
			SPI_SETSHOWSOUNDS = 57,
			SPI_SETSOUNDSENTRY = 65,
			SPI_SETSTICKYKEYS = 59,
			SPI_SETTOGGLEKEYS = 53
		}

		public enum SystemParametersDesktopInfo
		{
			SPI_GETCLEARTYPE = 4168,
			SPI_GETDESKWALLPAPER = 115,
			SPI_GETDROPSHADOW = 4132,
			SPI_GETFLATMENU = 4130,
			SPI_GETFONTSMOOTHING = 74,
			SPI_GETFONTSMOOTHINGCONTRAST = 8204,
			SPI_GETFONTSMOOTHINGORIENTATION = 8210,
			SPI_GETFONTSMOOTHINGTYPE = 8202,
			SPI_GETWORKAREA = 48,
			SPI_SETCLEARTYPE = 4169,
			SPI_SETCURSORS = 87,
			SPI_SETDESKPATTERN = 21,
			SPI_SETDESKWALLPAPER = 20,
			SPI_SETDROPSHADOW = 4133,
			SPI_SETFLATMENU = 4131,
			SPI_SETFONTSMOOTHING = 75,
			SPI_SETFONTSMOOTHINGCONTRAST = 8205,
			SPI_SETFONTSMOOTHINGORIENTATION = 8211,
			SPI_SETFONTSMOOTHINGTYPE = 8203,
			SPI_SETWORKAREA = 47
		}

		public enum SystemParametersIconInfo
		{
			SPI_GETICONMETRICS = 45,
			SPI_GETICONTITLELOGFONT = 31,
			SPI_GETICONTITLEWRAP = 25,
			SPI_ICONHORIZONTALSPACING = 13,
			SPI_ICONVERTICALSPACING = 24,
			SPI_SETICONMETRICS = 46,
			SPI_SETICONS = 88,
			SPI_SETICONTITLELOGFONT = 34,
			SPI_SETICONTITLEWRAP = 26
		}

		public enum SystemParametersInputInfo
		{
			SPI_GETBEEP = 1,
			SPI_GETBLOCKSENDINPUTRESETS = 4134,
			SPI_GETCONTACTVISUALIZATION = 8216,
			SPI_GETDEFAULTINPUTLANG = 89,
			SPI_GETGESTUREVISUALIZATION = 8218,
			SPI_GETKEYBOARDCUES = 4106,
			SPI_GETKEYBOARDDELAY = 22,
			SPI_GETKEYBOARDPREF = 68,
			SPI_GETKEYBOARDSPEED = 10,
			SPI_GETMOUSE = 3,
			SPI_GETMOUSEHOVERHEIGHT = 100,
			SPI_GETMOUSEHOVERTIME = 102,
			SPI_GETMOUSEHOVERWIDTH = 98,
			SPI_GETMOUSESPEED = 112,
			SPI_GETMOUSETRAILS = 94,
			SPI_GETMOUSEWHEELROUTING = 8220,
			SPI_GETPENVISUALIZATION = 8222,
			SPI_GETSNAPTODEFBUTTON = 95,
			SPI_GETSYSTEMLANGUAGEBAR = 4176,
			SPI_GETTHREADLOCALINPUTSETTINGS = 4174,
			SPI_GETWHEELSCROLLCHARS = 108,
			SPI_GETWHEELSCROLLLINES = 104,
			SPI_SETBEEP = 2,
			SPI_SETBLOCKSENDINPUTRESETS = 4135,
			SPI_SETCONTACTVISUALIZATION = 8217,
			SPI_SETDEFAULTINPUTLANG = 90,
			SPI_SETDOUBLECLICKTIME = 32,
			SPI_SETDOUBLECLKHEIGHT = 30,
			SPI_SETDOUBLECLKWIDTH = 29,
			SPI_SETGESTUREVISUALIZATION = 8219,
			SPI_SETKEYBOARDCUES = 4107,
			SPI_SETKEYBOARDDELAY = 23,
			SPI_SETKEYBOARDPREF = 69,
			SPI_SETKEYBOARDSPEED = 11,
			SPI_SETLANGTOGGLE = 91,
			SPI_SETMOUSE = 4,
			SPI_SETMOUSEBUTTONSWAP = 33,
			SPI_SETMOUSEHOVERHEIGHT = 101,
			SPI_SETMOUSEHOVERTIME = 103,
			SPI_SETMOUSEHOVERWIDTH = 99,
			SPI_SETMOUSESPEED = 113,
			SPI_SETMOUSETRAILS = 93,
			SPI_SETMOUSEWHEELROUTING = 8221,
			SPI_SETPENVISUALIZATION = 8223,
			SPI_SETSNAPTODEFBUTTON = 96,
			SPI_SETSYSTEMLANGUAGEBAR = 4177,
			SPI_SETTHREADLOCALINPUTSETTINGS = 4175,
			SPI_SETWHEELSCROLLCHARS = 109,
			SPI_SETWHEELSCROLLLINES = 105
		}

		public enum SystemParametersMenuInfo
		{
			SPI_GETMENUDROPALIGNMENT = 27,
			SPI_GETMENUFADE = 4114,
			SPI_GETMENUSHOWDELAY = 106,
			SPI_SETMENUDROPALIGNMENT = 28,
			SPI_SETMENUFADE = 4115,
			SPI_SETMENUSHOWDELAY = 107
		}

		public enum SystemParametersPowerInfo
		{
			SPI_GETLOWPOWERACTIVE = 83,
			SPI_GETLOWPOWERTIMEOUT = 79,
			SPI_GETPOWEROFFACTIVE = 84,
			SPI_GETPOWEROFFTIMEOUT = 80,
			SPI_SETLOWPOWERACTIVE = 85,
			SPI_SETLOWPOWERTIMEOUT = 81,
			SPI_SETPOWEROFFACTIVE = 86,
			SPI_SETPOWEROFFTIMEOUT = 82
		}

		public enum SystemParametersScreenSaverInfo
		{
			SPI_GETSCREENSAVEACTIVE = 16,
			SPI_GETSCREENSAVERRUNNING = 114,
			SPI_GETSCREENSAVESECURE = 118,
			SPI_GETSCREENSAVETIMEOUT = 14,
			SPI_SETSCREENSAVEACTIVE = 17,
			SPI_SETSCREENSAVESECURE = 119,
			SPI_SETSCREENSAVETIMEOUT = 15
		}

		public enum SystemParametersTimeoutInfo
		{
			SPI_GETHUNGAPPTIMEOUT = 120,
			SPI_GETWAITTOKILLTIMEOUT = 122,
			SPI_GETWAITTOKILLSERVICETIMEOUT = 124,
			SPI_SETHUNGAPPTIMEOUT = 121,
			SPI_SETWAITTOKILLTIMEOUT = 123,
			SPI_SETWAITTOKILLSERVICETIMEOUT = 125
		}

		public enum SystemParametersUiEffectsInfo
		{
			SPI_GETCOMBOBOXANIMATION = 4100,
			SPI_GETCURSORSHADOW = 4122,
			SPI_GETGRADIENTCAPTIONS = 4104,
			SPI_GETHOTTRACKING = 4110,
			SPI_GETLISTBOXSMOOTHSCROLLING = 4102,
			SPI_GETMENUANIMATION = 4098,
			SPI_GETMENUUNDERLINES = 4106,
			SPI_GETSELECTIONFADE = 4116,
			SPI_GETTOOLTIPANIMATION = 4118,
			SPI_GETTOOLTIPFADE = 4120,
			SPI_GETUIEFFECTS = 4158,
			SPI_SETCOMBOBOXANIMATION = 4101,
			SPI_SETCURSORSHADOW = 4123,
			SPI_SETGRADIENTCAPTIONS = 4105,
			SPI_SETHOTTRACKING = 4111,
			SPI_SETLISTBOXSMOOTHSCROLLING = 4103,
			SPI_SETMENUANIMATION = 4099,
			SPI_SETMENUUNDERLINES = 4107,
			SPI_SETSELECTIONFADE = 4117,
			SPI_SETTOOLTIPANIMATION = 4119,
			SPI_SETTOOLTIPFADE = 4121,
			SPI_SETUIEFFECTS = 4159
		}

		public enum SystemParametersWindowInfo
		{
			SPI_GETACTIVEWINDOWTRACKING = 4096,
			SPI_GETACTIVEWNDTRKZORDER = 4108,
			SPI_GETACTIVEWNDTRKTIMEOUT = 8194,
			SPI_GETANIMATION = 72,
			SPI_GETBORDER = 5,
			SPI_GETCARETWIDTH = 8198,
			SPI_GETDOCKMOVING = 144,
			SPI_GETDRAGFROMMAXIMIZE = 140,
			SPI_GETDRAGFULLWINDOWS = 38,
			SPI_GETFOREGROUNDFLASHCOUNT = 8196,
			SPI_GETFOREGROUNDLOCKTIMEOUT = 8192,
			SPI_GETMINIMIZEDMETRICS = 43,
			SPI_GETMOUSEDOCKTHRESHOLD = 126,
			SPI_GETMOUSEDRAGOUTTHRESHOLD = 132,
			SPI_GETMOUSESIDEMOVETHRESHOLD = 136,
			SPI_GETNONCLIENTMETRICS = 41,
			SPI_GETPENDOCKTHRESHOLD = 128,
			SPI_GETPENDRAGOUTTHRESHOLD = 134,
			SPI_GETPENSIDEMOVETHRESHOLD = 138,
			SPI_GETSHOWIMEUI = 110,
			SPI_GETSNAPSIZING = 142,
			SPI_GETWINARRANGING = 130,
			SPI_SETACTIVEWINDOWTRACKING = 4097,
			SPI_SETACTIVEWNDTRKZORDER = 4109,
			SPI_SETACTIVEWNDTRKTIMEOUT = 8195,
			SPI_SETANIMATION = 73,
			SPI_SETBORDER = 6,
			SPI_SETCARETWIDTH = 8199,
			SPI_SETDOCKMOVING = 145,
			SPI_SETDRAGFROMMAXIMIZE = 141,
			SPI_SETDRAGFULLWINDOWS = 37,
			SPI_SETDRAGHEIGHT = 77,
			SPI_SETDRAGWIDTH = 76,
			SPI_SETFOREGROUNDFLASHCOUNT = 8197,
			SPI_SETFOREGROUNDLOCKTIMEOUT = 8193,
			SPI_SETMINIMIZEDMETRICS = 44,
			SPI_SETMOUSEDOCKTHRESHOLD = 127,
			SPI_SETMOUSEDRAGOUTTHRESHOLD = 133,
			SPI_SETMOUSESIDEMOVETHRESHOLD = 137,
			SPI_SETNONCLIENTMETRICS = 42,
			SPI_SETPENDOCKTHRESHOLD = 129,
			SPI_SETPENDRAGOUTTHRESHOLD = 135,
			SPI_SETPENSIDEMOVETHRESHOLD = 139,
			SPI_SETSHOWIMEUI = 111,
			SPI_SETSNAPSIZING = 143,
			SPI_SETWINARRANGING = 131
		}

		public enum MouseActivationResult
		{
			MA_ACTIVATE = 1,
			MA_ACTIVATEANDEAT,
			MA_NOACTIVATE,
			MA_NOACTIVATEANDEAT
		}

		public enum ScreenshotHotKey
		{
			IDHOT_NONE = 0,
			IDHOT_SNAPDESKTOP = -2,
			IDHOT_SNAPWINDOW = -1
		}

		public enum CommandSource
		{
			Menu,
			Accelerator
		}

		public enum SysCommand
		{
			SC_CLOSE = 61536,
			SC_CONTEXTHELP = 61824,
			SC_DEFAULT = 61792,
			SC_HOTKEY = 61776,
			SC_HSCROLL = 61568,
			SCF_ISSECURE = 1,
			SC_KEYMENU = 61696,
			SC_MAXIMIZE = 61488,
			SC_MINIMIZE = 61472,
			SC_MONITORPOWER = 61808,
			SC_MOUSEMENU = 61584,
			SC_MOVE = 61456,
			SC_NEXTWINDOW = 61504,
			SC_PREVWINDOW = 61520,
			SC_RESTORE = 61728,
			SC_SCREENSAVE = 61760,
			SC_SIZE = 61440,
			SC_TASKLIST = 61744,
			SC_VSCROLL = 61552
		}

		public enum AppCommand
		{
			APPCOMMAND_BASS_BOOST = 20,
			APPCOMMAND_BASS_DOWN = 19,
			APPCOMMAND_BASS_UP = 21,
			APPCOMMAND_BROWSER_BACKWARD = 1,
			APPCOMMAND_BROWSER_FAVORITES = 6,
			APPCOMMAND_BROWSER_FORWARD = 2,
			APPCOMMAND_BROWSER_HOME = 7,
			APPCOMMAND_BROWSER_REFRESH = 3,
			APPCOMMAND_BROWSER_SEARCH = 5,
			APPCOMMAND_BROWSER_STOP = 4,
			APPCOMMAND_CLOSE = 31,
			APPCOMMAND_COPY = 36,
			APPCOMMAND_CORRECTION_LIST = 45,
			APPCOMMAND_CUT = 37,
			APPCOMMAND_DICTATE_OR_COMMAND_CONTROL_TOGGLE = 43,
			APPCOMMAND_FIND = 28,
			APPCOMMAND_FORWARD_MAIL = 40,
			APPCOMMAND_HELP = 27,
			APPCOMMAND_LAUNCH_APP1 = 17,
			APPCOMMAND_LAUNCH_APP2 = 18,
			APPCOMMAND_LAUNCH_MAIL = 15,
			APPCOMMAND_LAUNCH_MEDIA_SELECT = 16,
			APPCOMMAND_MEDIA_CHANNEL_DOWN = 52,
			APPCOMMAND_MEDIA_CHANNEL_UP = 51,
			APPCOMMAND_MEDIA_FAST_FORWARD = 49,
			APPCOMMAND_MEDIA_NEXTTRACK = 11,
			APPCOMMAND_MEDIA_PAUSE = 47,
			APPCOMMAND_MEDIA_PLAY = 46,
			APPCOMMAND_MEDIA_PLAY_PAUSE = 14,
			APPCOMMAND_MEDIA_PREVIOUSTRACK = 12,
			APPCOMMAND_MEDIA_RECORD = 48,
			APPCOMMAND_MEDIA_REWIND = 50,
			APPCOMMAND_MEDIA_STOP = 13,
			APPCOMMAND_MIC_ON_OFF_TOGGLE = 44,
			APPCOMMAND_MICROPHONE_VOLUME_DOWN = 25,
			APPCOMMAND_MICROPHONE_VOLUME_MUTE = 24,
			APPCOMMAND_MICROPHONE_VOLUME_UP = 26,
			APPCOMMAND_NEW = 29,
			APPCOMMAND_OPEN = 30,
			APPCOMMAND_PASTE = 38,
			APPCOMMAND_PRINT = 33,
			APPCOMMAND_REDO = 35,
			APPCOMMAND_REPLY_TO_MAIL = 39,
			APPCOMMAND_SAVE = 32,
			APPCOMMAND_SEND_MAIL = 41,
			APPCOMMAND_SPELL_CHECK = 42,
			APPCOMMAND_TREBLE_DOWN = 22,
			APPCOMMAND_TREBLE_UP = 23,
			APPCOMMAND_UNDO = 34,
			APPCOMMAND_VOLUME_DOWN = 9,
			APPCOMMAND_VOLUME_MUTE = 8,
			APPCOMMAND_VOLUME_UP = 10
		}

		public enum AppCommandDevice
		{
			FAPPCOMMAND_KEY = 0,
			FAPPCOMMAND_MOUSE = 32768,
			FAPPCOMMAND_OEM = 4096,
			FAPPCOMMAND_MASK = 61440
		}

		public enum MonitorInfoFlag
		{
			MONITORINFOF_NONE,
			MONITORINFOF_PRIMARY
		}

		public enum LayeredWindowAttributeFlag
		{
			LWA_ALPHA = 2,
			LWA_COLORKEY = 1
		}

		public enum WM
		{
			NULL = 0,
			CREATE = 1,
			DESTROY = 2,
			MOVE = 3,
			SIZE = 5,
			ACTIVATE = 6,
			SETFOCUS = 7,
			KILLFOCUS = 8,
			ENABLE = 10,
			SETREDRAW = 11,
			SETTEXT = 12,
			GETTEXT = 13,
			GETTEXTLENGTH = 14,
			PAINT = 15,
			CLOSE = 16,
			QUERYENDSESSION = 17,
			QUERYOPEN = 19,
			ENDSESSION = 22,
			QUIT = 18,
			ERASEBKGND = 20,
			SYSCOLORCHANGE = 21,
			SHOWWINDOW = 24,
			WININICHANGE = 26,
			SETTINGCHANGE = WININICHANGE,
			DEVMODECHANGE = 27,
			ACTIVATEAPP = 28,
			FONTCHANGE = 29,
			TIMECHANGE = 30,
			CANCELMODE = 31,
			SETCURSOR = 32,
			MOUSEACTIVATE = 33,
			CHILDACTIVATE = 34,
			QUEUESYNC = 35,
			GETMINMAXINFO = 36,
			PAINTICON = 38,
			ICONERASEBKGND = 39,
			NEXTDLGCTL = 40,
			SPOOLERSTATUS = 42,
			DRAWITEM = 43,
			MEASUREITEM = 44,
			DELETEITEM = 45,
			VKEYTOITEM = 46,
			CHARTOITEM = 47,
			SETFONT = 48,
			GETFONT = 49,
			SETHOTKEY = 50,
			GETHOTKEY = 51,
			QUERYDRAGICON = 55,
			COMPAREITEM = 57,
			GETOBJECT = 61,
			COMPACTING = 65,
			COMMNOTIFY = 68,
			WINDOWPOSCHANGING = 70,
			WINDOWPOSCHANGED = 71,
			POWER = 72,
			COPYDATA = 74,
			CANCELJOURNAL = 75,
			NOTIFY = 78,
			INPUTLANGCHANGEREQUEST = 80,
			INPUTLANGCHANGE = 81,
			TCARD = 82,
			HELP = 83,
			USERCHANGED = 84,
			NOTIFYFORMAT = 85,
			CONTEXTMENU = 123,
			STYLECHANGING = 124,
			STYLECHANGED = 125,
			DISPLAYCHANGE = 126,
			GETICON = 127,
			SETICON = 128,
			NCCREATE = 129,
			NCDESTROY = 130,
			NCCALCSIZE = 131,
			NCHITTEST = 132,
			NCPAINT = 133,
			NCACTIVATE = 134,
			GETDLGCODE = 135,
			SYNCPAINT = 136,
			NCMOUSEMOVE = 160,
			NCLBUTTONDOWN = 161,
			NCLBUTTONUP = 162,
			NCLBUTTONDBLCLK = 163,
			NCRBUTTONDOWN = 164,
			NCRBUTTONUP = 165,
			NCRBUTTONDBLCLK = 166,
			NCMBUTTONDOWN = 167,
			NCMBUTTONUP = 168,
			NCMBUTTONDBLCLK = 169,
			NCXBUTTONDOWN = 171,
			NCXBUTTONUP = 172,
			NCXBUTTONDBLCLK = 173,
			INPUT_DEVICE_CHANGE = 254,
			INPUT = 255,
			KEYFIRST = 256,
			KEYDOWN = KEYFIRST,
			KEYUP = 257,
			CHAR = 258,
			DEADCHAR = 259,
			SYSKEYDOWN = 260,
			SYSKEYUP = 261,
			SYSCHAR = 262,
			SYSDEADCHAR = 263,
			UNICHAR = 265,
			KEYLAST = UNICHAR,
			IME_STARTCOMPOSITION = 269,
			IME_ENDCOMPOSITION = 270,
			IME_COMPOSITION = 271,
			IME_KEYLAST = IME_COMPOSITION,
			INITDIALOG = 272,
			COMMAND = 273,
			SYSCOMMAND = 274,
			TIMER = 275,
			HSCROLL = 276,
			VSCROLL = 277,
			INITMENU = 278,
			INITMENUPOPUP = 279,
			GESTURE = 281,
			GESTURENOTIFY = 282,
			MENUSELECT = 287,
			MENUCHAR = 288,
			ENTERIDLE = 289,
			MENURBUTTONUP = 290,
			MENUDRAG = 291,
			MENUGETOBJECT = 292,
			UNINITMENUPOPUP = 293,
			MENUCOMMAND = 294,
			CHANGEUISTATE = 295,
			UPDATEUISTATE = 296,
			QUERYUISTATE = 297,
			CTLCOLORMSGBOX = 306,
			CTLCOLOREDIT = 307,
			CTLCOLORLISTBOX = 308,
			CTLCOLORBTN = 309,
			CTLCOLORDLG = 310,
			CTLCOLORSCROLLBAR = 311,
			CTLCOLORSTATIC = 312,
			MOUSEFIRST = 512,
			MOUSEMOVE = MOUSEFIRST,
			LBUTTONDOWN = 513,
			LBUTTONUP = 514,
			LBUTTONDBLCLK = 515,
			RBUTTONDOWN = 516,
			RBUTTONUP = 517,
			RBUTTONDBLCLK = 518,
			MBUTTONDOWN = 519,
			MBUTTONUP = 520,
			MBUTTONDBLCLK = 521,
			MOUSEWHEEL = 522,
			XBUTTONDOWN = 523,
			XBUTTONUP = 524,
			XBUTTONDBLCLK = 525,
			MOUSEHWHEEL = 526,
			MOUSELAST = MOUSEHWHEEL,
			PARENTNOTIFY = 528,
			ENTERMENULOOP = 529,
			EXITMENULOOP = 530,
			NEXTMENU = 531,
			SIZING = 532,
			CAPTURECHANGED = 533,
			MOVING = 534,
			POWERBROADCAST = 536,
			DEVICECHANGE = 537,
			MDICREATE = 544,
			MDIDESTROY = 545,
			MDIACTIVATE = 546,
			MDIRESTORE = 547,
			MDINEXT = 548,
			MDIMAXIMIZE = 549,
			MDITILE = 550,
			MDICASCADE = 551,
			MDIICONARRANGE = 552,
			MDIGETACTIVE = 553,
			MDISETMENU = 560,
			ENTERSIZEMOVE = 561,
			EXITSIZEMOVE = 562,
			DROPFILES = 563,
			MDIREFRESHMENU = 564,
			POINTERDEVICECHANGE = 568,
			POINTERDEVICEINRANGE = 569,
			POINTERDEVICEOUTOFRANGE = 570,
			TOUCH = 576,
			NCPOINTERUPDATE = 577,
			NCPOINTERDOWN = 578,
			NCPOINTERUP = 579,
			POINTERUPDATE = 581,
			POINTERDOWN = 582,
			POINTERUP = 583,
			POINTERENTER = 585,
			POINTERLEAVE = 586,
			POINTERACTIVATE = 587,
			POINTERCAPTURECHANGED = 588,
			TOUCHHITTESTING = 589,
			POINTERWHEEL = 590,
			POINTERHWHEEL = 591,
			IME_SETCONTEXT = 641,
			IME_NOTIFY = 642,
			IME_CONTROL = 643,
			IME_COMPOSITIONFULL = 644,
			IME_SELECT = 645,
			IME_CHAR = 646,
			IME_REQUEST = 648,
			IME_KEYDOWN = 656,
			IME_KEYUP = 657,
			MOUSEHOVER = 673,
			MOUSELEAVE = 675,
			NCMOUSEHOVER = 672,
			NCMOUSELEAVE = 674,
			WTSSESSION_CHANGE = 689,
			TABLET_FIRST = 704,
			TABLET_LAST = 735,
			DPICHANGED = 736,
			CUT = 768,
			COPY = 769,
			PASTE = 770,
			CLEAR = 771,
			UNDO = 772,
			RENDERFORMAT = 773,
			RENDERALLFORMATS = 774,
			DESTROYCLIPBOARD = 775,
			DRAWCLIPBOARD = 776,
			PAINTCLIPBOARD = 777,
			VSCROLLCLIPBOARD = 778,
			SIZECLIPBOARD = 779,
			ASKCBFORMATNAME = 780,
			CHANGECBCHAIN = 781,
			HSCROLLCLIPBOARD = 782,
			QUERYNEWPALETTE = 783,
			PALETTEISCHANGING = 784,
			PALETTECHANGED = 785,
			HOTKEY = 786,
			PRINT = 791,
			PRINTCLIENT = 792,
			APPCOMMAND = 793,
			THEMECHANGED = 794,
			CLIPBOARDUPDATE = 797,
			DWMCOMPOSITIONCHANGED = 798,
			DWMNCRENDERINGCHANGED = 799,
			DWMCOLORIZATIONCOLORCHANGED = 800,
			DWMWINDOWMAXIMIZEDCHANGE = 801,
			DWMSENDICONICTHUMBNAIL = 803,
			DWMSENDICONICLIVEPREVIEWBITMAP = 806,
			GETTITLEBARINFOEX = 831,
			HANDHELDFIRST = 856,
			HANDHELDLAST = 863,
			AFXFIRST = 864,
			AFXLAST = 895,
			PENWINFIRST = 896,
			PENWINLAST = 911,
			APP = 32768,
			USER = 1024
		}

		public enum ClipboardFormat : uint
		{
			CF_BITMAP = 2u,
			CF_TEXT = 1u,
			CF_UNICODETEXT = 13u,
			CF_ZERO = 0u
		}

		[Obsolete("请使用 WindowPositionFlags ")]
		internal static class SWP
		{
			public static readonly int NOSIZE = 1;

			public static readonly int NOMOVE = 2;

			public static readonly int NOZORDER = 4;

			public static readonly int NOREDRAW = 8;

			public static readonly int NOACTIVATE = 16;

			public static readonly int DRAWFRAME = 32;

			public static readonly int FRAMECHANGED = 32;

			public static readonly int SHOWWINDOW = 64;

			public static readonly int HIDEWINDOW = 128;

			public static readonly int NOCOPYBITS = 256;

			public static readonly int NOOWNERZORDER = 512;

			public static readonly int NOREPOSITION = 512;

			public static readonly int NOSENDCHANGING = 1024;

			public static readonly int DEFERERASE = 8192;

			public static readonly int ASYNCWINDOWPOS = 16384;
		}

		[Obsolete("请使用 CoreLib.Windows.Native.Win32.Rectangle")]
		public struct RECT
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;
		}

		[Obsolete("请使用 Win32.Point")]
		internal struct POINT
		{
			public int x;

			public int y;

			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		[Obsolete("请使用 MinMaxInfo")]
		internal struct MINMAXINFO
		{
			public POINT ptReserved;

			public POINT ptMaxSize;

			public POINT ptMaxPosition;

			public POINT ptMinTrackSize;

			public POINT ptMaxTrackSize;
		}

		public enum HResult : uint
		{
			S_OK = 0u,
			E_NOTIMPL = 2147500033u,
			E_NOINTERFACE = 2147500034u,
			E_POINTER = 2147500035u,
			E_ABORT = 2147500036u,
			E_FAIL = 2147500037u,
			E_UNEXPECTED = 2147549183u,
			E_ACCESSDENIED = 2147942405u,
			E_HANDLE = 2147942406u,
			E_OUTOFMEMORY = 2147942414u,
			E_INVALIDARG = 2147942487u
		}

		[Flags]
		public enum ExtendedWindowStyles : long
		{
			WS_EX_ACCEPTFILES = 0x10L,
			WS_EX_APPWINDOW = 0x40000L,
			WS_EX_CLIENTEDGE = 0x200L,
			WS_EX_COMPOSITED = 0x2000000L,
			WS_EX_CONTEXTHELP = 0x400L,
			WS_EX_CONTROLPARENT = 0x10000L,
			WS_EX_DLGMODALFRAME = 1L,
			WS_EX_LAYERED = 0x80000L,
			WS_EX_LAYOUTRTL = 0x400000L,
			WS_EX_LEFT = 0L,
			WS_EX_LEFTSCROLLBAR = 0x4000L,
			WS_EX_LTRREADING = 0L,
			WS_EX_MDICHILD = 0x40L,
			WS_EX_NOACTIVATE = 0x8000000L,
			WS_EX_NOINHERITLAYOUT = 0x100000L,
			WS_EX_NOPARENTNOTIFY = 4L,
			WS_EX_NOREDIRECTIONBITMAP = 0x200000L,
			WS_EX_OVERLAPPEDWINDOW = 0x300L,
			WS_EX_PALETTEWINDOW = 0x188L,
			WS_EX_RIGHT = 0x1000L,
			WS_EX_RIGHTSCROLLBAR = 0L,
			WS_EX_RTLREADING = 0x2000L,
			WS_EX_STATICEDGE = 0x20000L,
			WS_EX_TOOLWINDOW = 0x80L,
			WS_EX_TOPMOST = 8L,
			WS_EX_TRANSPARENT = 0x20L,
			WS_EX_WINDOWEDGE = 0x100L
		}

		public struct Rectangle : IEquatable<Rectangle>
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;

			public Size Size
			{
				get
				{
					return new Size(Width, Height);
				}
				set
				{
					Width = value.Width;
					Height = value.Height;
				}
			}

			public bool IsEmpty
			{
				get
				{
					if (Left == 0 && Top == 0 && Right == 0)
					{
						return Bottom == 0;
					}
					return false;
				}
			}

			public int Width
			{
				get
				{
					return Right - Left;
				}
				set
				{
					Right = Left + value;
				}
			}

			public int Height
			{
				get
				{
					return Bottom - Top;
				}
				set
				{
					Bottom = Top + value;
				}
			}

			public Rectangle(int left = 0, int top = 0, int right = 0, int bottom = 0)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}

			public Rectangle(int width = 0, int height = 0)
				: this(0, 0, width, height)
			{
			}

			public bool Equals(Rectangle other)
			{
				if (Left == other.Left && Right == other.Right && Top == other.Top)
				{
					return Bottom == other.Bottom;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				if (obj is Rectangle)
				{
					return Equals((Rectangle)obj);
				}
				return false;
			}

			public static bool operator ==(Rectangle left, Rectangle right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(Rectangle left, Rectangle right)
			{
				return !(left == right);
			}

			public override int GetHashCode()
			{
				return (((((Left * 397) ^ Top) * 397) ^ Right) * 397) ^ Bottom;
			}

			public static Rectangle Create(int x, int y, int width, int height)
			{
				return new Rectangle(x, y, width + x, height + y);
			}

			public override string ToString()
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				return "{ Left = " + Left.ToString(currentCulture) + ", Top = " + Top.ToString(currentCulture) + " , Right = " + Right.ToString(currentCulture) + ", Bottom = " + Bottom.ToString(currentCulture) + " }, { Width: " + Width.ToString(currentCulture) + ", Height: " + Height.ToString(currentCulture) + " }";
			}

			public static Rectangle From(ref Rectangle lvalue, ref Rectangle rvalue, Func<int, int, int> leftTopOperation, Func<int, int, int> rightBottomOperation = null)
			{
				if (rightBottomOperation == null)
				{
					rightBottomOperation = leftTopOperation;
				}
				return new Rectangle(leftTopOperation(lvalue.Left, rvalue.Left), leftTopOperation(lvalue.Top, rvalue.Top), rightBottomOperation(lvalue.Right, rvalue.Right), rightBottomOperation(lvalue.Bottom, rvalue.Bottom));
			}

			public void Add(Rectangle value)
			{
				Add(ref this, ref value);
			}

			public void Subtract(Rectangle value)
			{
				Subtract(ref this, ref value);
			}

			public void Multiply(Rectangle value)
			{
				Multiply(ref this, ref value);
			}

			public void Divide(Rectangle value)
			{
				Divide(ref this, ref value);
			}

			public void Deflate(Rectangle value)
			{
				Deflate(ref this, ref value);
			}

			public void Inflate(Rectangle value)
			{
				Inflate(ref this, ref value);
			}

			public void Offset(int x, int y)
			{
				Offset(ref this, x, y);
			}

			public void OffsetTo(int x, int y)
			{
				OffsetTo(ref this, x, y);
			}

			public void Scale(int x, int y)
			{
				Scale(ref this, x, y);
			}

			public void ScaleTo(int x, int y)
			{
				ScaleTo(ref this, x, y);
			}

			public static void Add(ref Rectangle lvalue, ref Rectangle rvalue)
			{
				lvalue.Left += rvalue.Left;
				lvalue.Top += rvalue.Top;
				lvalue.Right += rvalue.Right;
				lvalue.Bottom += rvalue.Bottom;
			}

			public static void Subtract(ref Rectangle lvalue, ref Rectangle rvalue)
			{
				lvalue.Left -= rvalue.Left;
				lvalue.Top -= rvalue.Top;
				lvalue.Right -= rvalue.Right;
				lvalue.Bottom -= rvalue.Bottom;
			}

			public static void Multiply(ref Rectangle lvalue, ref Rectangle rvalue)
			{
				lvalue.Left *= rvalue.Left;
				lvalue.Top *= rvalue.Top;
				lvalue.Right *= rvalue.Right;
				lvalue.Bottom *= rvalue.Bottom;
			}

			public static void Divide(ref Rectangle lvalue, ref Rectangle rvalue)
			{
				lvalue.Left /= rvalue.Left;
				lvalue.Top /= rvalue.Top;
				lvalue.Right /= rvalue.Right;
				lvalue.Bottom /= rvalue.Bottom;
			}

			public static void Deflate(ref Rectangle target, ref Rectangle deflation)
			{
				target.Top += deflation.Top;
				target.Left += deflation.Left;
				target.Bottom -= deflation.Bottom;
				target.Right -= deflation.Right;
			}

			public static void Inflate(ref Rectangle target, ref Rectangle inflation)
			{
				target.Top -= inflation.Top;
				target.Left -= inflation.Left;
				target.Bottom += inflation.Bottom;
				target.Right += inflation.Right;
			}

			public static void Offset(ref Rectangle target, int x, int y)
			{
				target.Top += y;
				target.Left += x;
				target.Bottom += y;
				target.Right += x;
			}

			public static void OffsetTo(ref Rectangle target, int x, int y)
			{
				int width = target.Width;
				int height = target.Height;
				target.Left = x;
				target.Top = y;
				target.Right = width;
				target.Bottom = height;
			}

			public static void Scale(ref Rectangle target, int x, int y)
			{
				target.Top *= y;
				target.Left *= x;
				target.Bottom *= y;
				target.Right *= x;
			}

			public static void ScaleTo(ref Rectangle target, int x, int y)
			{
				x = target.Left / x;
				y = target.Top / y;
				Scale(ref target, x, y);
			}
		}

		public struct RectangleS : IEquatable<RectangleS>
		{
			public short Left;

			public short Top;

			public short Right;

			public short Bottom;

			public SizeS Size
			{
				get
				{
					return new SizeS(Width, Height);
				}
				set
				{
					Width = value.Width;
					Height = value.Height;
				}
			}

			public bool IsEmpty
			{
				get
				{
					if (Left == 0 && Top == 0 && Right == 0)
					{
						return Bottom == 0;
					}
					return false;
				}
			}

			public short Width
			{
				get
				{
					return (short)(Right - Left);
				}
				set
				{
					Right = (short)(Left + value);
				}
			}

			public short Height
			{
				get
				{
					return (short)(Bottom - Top);
				}
				set
				{
					Bottom = (short)(Top + value);
				}
			}

			public RectangleS(short left = 0, short top = 0, short right = 0, short bottom = 0)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}

			public RectangleS(short width = 0, short height = 0)
				: this(0, 0, width, height)
			{
			}

			public RectangleS(short all = 0)
				: this(all, all, all, all)
			{
			}

			public bool Equals(RectangleS other)
			{
				if (Left == other.Left && Right == other.Right && Top == other.Top)
				{
					return Bottom == other.Bottom;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				if (obj is RectangleS)
				{
					return Equals((RectangleS)obj);
				}
				return false;
			}

			public static bool operator ==(RectangleS left, RectangleS right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(RectangleS left, RectangleS right)
			{
				return !(left == right);
			}

			public override int GetHashCode()
			{
				return (((((Left * 397) ^ Top) * 397) ^ Right) * 397) ^ Bottom;
			}

			public static RectangleS Create(short x, short y, short width, short height)
			{
				return new RectangleS(x, y, (short)(width + x), (short)(height + y));
			}

			public override string ToString()
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				return "{ Left = " + Left.ToString(currentCulture) + ", Top = " + Top.ToString(currentCulture) + " , Right = " + Right.ToString(currentCulture) + ", Bottom = " + Bottom.ToString(currentCulture) + " }, { Width: " + Width.ToString(currentCulture) + ", Height: " + Height.ToString(currentCulture) + " }";
			}

			public static RectangleS From(ref RectangleS lvalue, ref RectangleS rvalue, Func<short, short, short> leftTopOperation, Func<short, short, short> rightBottomOperation = null)
			{
				if (rightBottomOperation == null)
				{
					rightBottomOperation = leftTopOperation;
				}
				return new RectangleS(leftTopOperation(lvalue.Left, rvalue.Left), leftTopOperation(lvalue.Top, rvalue.Top), rightBottomOperation(lvalue.Right, rvalue.Right), rightBottomOperation(lvalue.Bottom, rvalue.Bottom));
			}

			public void Add(RectangleS value)
			{
				Add(ref this, ref value);
			}

			public void Subtract(RectangleS value)
			{
				Subtract(ref this, ref value);
			}

			public void Multiply(RectangleS value)
			{
				Multiply(ref this, ref value);
			}

			public void Divide(RectangleS value)
			{
				Divide(ref this, ref value);
			}

			public void Deflate(RectangleS value)
			{
				Deflate(ref this, ref value);
			}

			public void Inflate(RectangleS value)
			{
				Inflate(ref this, ref value);
			}

			public void Offset(short x, short y)
			{
				Offset(ref this, x, y);
			}

			public void OffsetTo(short x, short y)
			{
				OffsetTo(ref this, x, y);
			}

			public void Scale(short x, short y)
			{
				Scale(ref this, x, y);
			}

			public void ScaleTo(short x, short y)
			{
				ScaleTo(ref this, x, y);
			}

			public static void Add(ref RectangleS lvalue, ref RectangleS rvalue)
			{
				lvalue.Left += rvalue.Left;
				lvalue.Top += rvalue.Top;
				lvalue.Right += rvalue.Right;
				lvalue.Bottom += rvalue.Bottom;
			}

			public static void Subtract(ref RectangleS lvalue, ref RectangleS rvalue)
			{
				lvalue.Left -= rvalue.Left;
				lvalue.Top -= rvalue.Top;
				lvalue.Right -= rvalue.Right;
				lvalue.Bottom -= rvalue.Bottom;
			}

			public static void Multiply(ref RectangleS lvalue, ref RectangleS rvalue)
			{
				lvalue.Left *= rvalue.Left;
				lvalue.Top *= rvalue.Top;
				lvalue.Right *= rvalue.Right;
				lvalue.Bottom *= rvalue.Bottom;
			}

			public static void Divide(ref RectangleS lvalue, ref RectangleS rvalue)
			{
				lvalue.Left /= rvalue.Left;
				lvalue.Top /= rvalue.Top;
				lvalue.Right /= rvalue.Right;
				lvalue.Bottom /= rvalue.Bottom;
			}

			public static void Deflate(ref RectangleS target, ref RectangleS deflation)
			{
				target.Top += deflation.Top;
				target.Left += deflation.Left;
				target.Bottom -= deflation.Bottom;
				target.Right -= deflation.Right;
			}

			public static void Inflate(ref RectangleS target, ref RectangleS inflation)
			{
				target.Top -= inflation.Top;
				target.Left -= inflation.Left;
				target.Bottom += inflation.Bottom;
				target.Right += inflation.Right;
			}

			public static void Offset(ref RectangleS target, short x, short y)
			{
				target.Top += y;
				target.Left += x;
				target.Bottom += y;
				target.Right += x;
			}

			public static void OffsetTo(ref RectangleS target, short x, short y)
			{
				short width = target.Width;
				short height = target.Height;
				target.Left = x;
				target.Top = y;
				target.Right = width;
				target.Bottom = height;
			}

			public static void Scale(ref RectangleS target, short x, short y)
			{
				target.Top *= y;
				target.Left *= x;
				target.Bottom *= y;
				target.Right *= x;
			}

			public static void ScaleTo(ref RectangleS target, short x, short y)
			{
				x = (short)(target.Left / x);
				y = (short)(target.Top / y);
				Scale(ref target, x, y);
			}
		}

		public struct RectangleF : IEquatable<RectangleF>
		{
			public float Left;

			public float Top;

			public float Right;

			public float Bottom;

			public SizeF Size
			{
				get
				{
					return new SizeF(Width, Height);
				}
				set
				{
					Width = value.Width;
					Height = value.Height;
				}
			}

			public bool IsEmpty
			{
				get
				{
					if (Left == 0f && Top == 0f && Right == 0f)
					{
						return Bottom == 0f;
					}
					return false;
				}
			}

			public float Width
			{
				get
				{
					return Right - Left;
				}
				set
				{
					Right = Left + value;
				}
			}

			public float Height
			{
				get
				{
					return Bottom - Top;
				}
				set
				{
					Bottom = Top + value;
				}
			}

			public RectangleF(float left = 0f, float top = 0f, float right = 0f, float bottom = 0f)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}

			public RectangleF(float width = 0f, float height = 0f)
				: this(0f, 0f, width, height)
			{
			}

			public RectangleF(float all = 0f)
				: this(all, all, all, all)
			{
			}

			public bool Equals(RectangleF other)
			{
				if (Left == other.Left && Right == other.Right && Top == other.Top)
				{
					return Bottom == other.Bottom;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				if (obj is RectangleF)
				{
					return Equals((RectangleF)obj);
				}
				return false;
			}

			public static bool operator ==(RectangleF left, RectangleF right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(RectangleF left, RectangleF right)
			{
				return !(left == right);
			}

			public override int GetHashCode()
			{
				return ((((((int)Left * 397) ^ (int)Top) * 397) ^ (int)Right) * 397) ^ (int)Bottom;
			}

			public static RectangleF Create(float x, float y, float width, float height)
			{
				return new RectangleF(x, y, width + x, height + y);
			}

			public override string ToString()
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				return "{ Left = " + Left.ToString(currentCulture) + ", Top = " + Top.ToString(currentCulture) + " , Right = " + Right.ToString(currentCulture) + ", Bottom = " + Bottom.ToString(currentCulture) + " }, { Width: " + Width.ToString(currentCulture) + ", Height: " + Height.ToString(currentCulture) + " }";
			}

			public static RectangleF From(ref RectangleF lvalue, ref RectangleF rvalue, Func<float, float, float> leftTopOperation, Func<float, float, float> rightBottomOperation = null)
			{
				if (rightBottomOperation == null)
				{
					rightBottomOperation = leftTopOperation;
				}
				return new RectangleF(leftTopOperation(lvalue.Left, rvalue.Left), leftTopOperation(lvalue.Top, rvalue.Top), rightBottomOperation(lvalue.Right, rvalue.Right), rightBottomOperation(lvalue.Bottom, rvalue.Bottom));
			}

			public void Add(RectangleF value)
			{
				Add(ref this, ref value);
			}

			public void Subtract(RectangleF value)
			{
				Subtract(ref this, ref value);
			}

			public void Multiply(RectangleF value)
			{
				Multiply(ref this, ref value);
			}

			public void Divide(RectangleF value)
			{
				Divide(ref this, ref value);
			}

			public void Deflate(RectangleF value)
			{
				Deflate(ref this, ref value);
			}

			public void Inflate(RectangleF value)
			{
				Inflate(ref this, ref value);
			}

			public void Offset(float x, float y)
			{
				Offset(ref this, x, y);
			}

			public void OffsetTo(float x, float y)
			{
				OffsetTo(ref this, x, y);
			}

			public void Scale(float x, float y)
			{
				Scale(ref this, x, y);
			}

			public void ScaleTo(float x, float y)
			{
				ScaleTo(ref this, x, y);
			}

			public static void Add(ref RectangleF lvalue, ref RectangleF rvalue)
			{
				lvalue.Left += rvalue.Left;
				lvalue.Top += rvalue.Top;
				lvalue.Right += rvalue.Right;
				lvalue.Bottom += rvalue.Bottom;
			}

			public static void Subtract(ref RectangleF lvalue, ref RectangleF rvalue)
			{
				lvalue.Left -= rvalue.Left;
				lvalue.Top -= rvalue.Top;
				lvalue.Right -= rvalue.Right;
				lvalue.Bottom -= rvalue.Bottom;
			}

			public static void Multiply(ref RectangleF lvalue, ref RectangleF rvalue)
			{
				lvalue.Left *= rvalue.Left;
				lvalue.Top *= rvalue.Top;
				lvalue.Right *= rvalue.Right;
				lvalue.Bottom *= rvalue.Bottom;
			}

			public static void Divide(ref RectangleF lvalue, ref RectangleF rvalue)
			{
				lvalue.Left /= rvalue.Left;
				lvalue.Top /= rvalue.Top;
				lvalue.Right /= rvalue.Right;
				lvalue.Bottom /= rvalue.Bottom;
			}

			public static void Deflate(ref RectangleF target, ref RectangleF deflation)
			{
				target.Top += deflation.Top;
				target.Left += deflation.Left;
				target.Bottom -= deflation.Bottom;
				target.Right -= deflation.Right;
			}

			public static void Inflate(ref RectangleF target, ref RectangleF inflation)
			{
				target.Top -= inflation.Top;
				target.Left -= inflation.Left;
				target.Bottom += inflation.Bottom;
				target.Right += inflation.Right;
			}

			public static void Offset(ref RectangleF target, float x, float y)
			{
				target.Top += y;
				target.Left += x;
				target.Bottom += y;
				target.Right += x;
			}

			public static void OffsetTo(ref RectangleF target, float x, float y)
			{
				float width = target.Width;
				float height = target.Height;
				target.Left = x;
				target.Top = y;
				target.Right = width;
				target.Bottom = height;
			}

			public static void Scale(ref RectangleF target, float x, float y)
			{
				target.Top *= y;
				target.Left *= x;
				target.Bottom *= y;
				target.Right *= x;
			}

			public static void ScaleTo(ref RectangleF target, float x, float y)
			{
				x = target.Left / x;
				y = target.Top / y;
				Scale(ref target, x, y);
			}
		}

		public struct Margins
		{
			public int Left;

			public int Right;

			public int Top;

			public int Bottom;

			public Margins(int left = 0, int right = 0, int top = 0, int bottom = 0)
			{
				Left = left;
				Right = right;
				Top = top;
				Bottom = bottom;
			}

			public Margins(int x = 0, int y = 0)
				: this(x, x, y, y)
			{
			}

			public Margins(int all = 0)
				: this(all, all)
			{
			}
		}

		public struct Point : IEquatable<Point>
		{
			public int X;

			public int Y;

			public bool IsEmpty
			{
				get
				{
					if (X == 0)
					{
						return Y == 0;
					}
					return false;
				}
			}

			public Point(int x, int y)
			{
				X = x;
				Y = y;
			}

			public bool Equals(Point other)
			{
				if (X == other.X)
				{
					return Y == other.Y;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				if (obj is Point)
				{
					return Equals((Point)obj);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (X * 397) ^ Y;
			}

			public static bool operator ==(Point left, Point right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(Point left, Point right)
			{
				return !(left == right);
			}

			public void Offset(int x, int y)
			{
				X += x;
				Y += y;
			}

			public void Set(int x, int y)
			{
				X = x;
				Y = y;
			}

			public override string ToString()
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				return "{ X = " + X.ToString(currentCulture) + ", Y = " + Y.ToString(currentCulture) + " }";
			}
		}

		public struct PointS : IEquatable<PointS>
		{
			public short X;

			public short Y;

			public bool IsEmpty
			{
				get
				{
					if (X == 0)
					{
						return Y == 0;
					}
					return false;
				}
			}

			public PointS(short x, short y)
			{
				X = x;
				Y = y;
			}

			public bool Equals(PointS other)
			{
				if (X == other.X)
				{
					return Y == other.Y;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				if (obj is PointS)
				{
					return Equals((PointS)obj);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (X * 397) ^ Y;
			}

			public static bool operator ==(PointS left, PointS right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(PointS left, PointS right)
			{
				return !(left == right);
			}

			public void Offset(short x, short y)
			{
				X += x;
				Y += y;
			}

			public void Set(short x, short y)
			{
				X = x;
				Y = y;
			}

			public override string ToString()
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				return "{ X = " + X.ToString(currentCulture) + ", Y = " + Y.ToString(currentCulture) + " }";
			}
		}

		public struct PointF : IEquatable<PointF>
		{
			public float X;

			public float Y;

			public bool IsEmpty
			{
				get
				{
					if (X == 0f)
					{
						return Y == 0f;
					}
					return false;
				}
			}

			public PointF(float x, float y)
			{
				X = x;
				Y = y;
			}

			public bool Equals(PointF other)
			{
				if (X == other.X)
				{
					return Y == other.Y;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				if (obj is PointF)
				{
					return Equals((PointF)obj);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return ((int)X * 397) ^ (int)Y;
			}

			public static bool operator ==(PointF left, PointF right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(PointF left, PointF right)
			{
				return !(left == right);
			}

			public void Offset(float x, float y)
			{
				X += x;
				Y += y;
			}

			public void Set(float x, float y)
			{
				X = x;
				Y = y;
			}

			public override string ToString()
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				return "{ X = " + X.ToString(currentCulture) + ", Y = " + Y.ToString(currentCulture) + " }";
			}
		}

		public struct Size : IEquatable<Size>
		{
			public int Width;

			public int Height;

			public bool IsEmpty
			{
				get
				{
					if (Width == 0)
					{
						return Height == 0;
					}
					return false;
				}
			}

			public Size(int width, int height)
			{
				Width = width;
				Height = height;
			}

			public bool Equals(Size other)
			{
				if (Width == other.Width)
				{
					return Height == other.Height;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				if (obj is Size)
				{
					return Equals((Size)obj);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (Width * 397) ^ Height;
			}

			public static bool operator ==(Size left, Size right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(Size left, Size right)
			{
				return !(left == right);
			}

			public void Offset(int width, int height)
			{
				Width += width;
				Height += height;
			}

			public void Set(int width, int height)
			{
				Width = width;
				Height = height;
			}

			public override string ToString()
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				return "{ Width = " + Width.ToString(currentCulture) + ", Height = " + Height.ToString(currentCulture) + " }";
			}
		}

		public struct SizeS : IEquatable<SizeS>
		{
			public short Width;

			public short Height;

			public bool IsEmpty
			{
				get
				{
					if (Width == 0)
					{
						return Height == 0;
					}
					return false;
				}
			}

			public SizeS(short width, short height)
			{
				Width = width;
				Height = height;
			}

			public bool Equals(SizeS other)
			{
				if (Width == other.Width)
				{
					return Height == other.Height;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				if (obj is SizeS)
				{
					return Equals((SizeS)obj);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (Width * 397) ^ Height;
			}

			public static bool operator ==(SizeS left, SizeS right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(SizeS left, SizeS right)
			{
				return !(left == right);
			}

			public void Offset(short width, short height)
			{
				Width += width;
				Height += height;
			}

			public void Set(short width, short height)
			{
				Width = width;
				Height = height;
			}

			public override string ToString()
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				return "{ Width = " + Width.ToString(currentCulture) + ", Height = " + Height.ToString(currentCulture) + " }";
			}
		}

		public struct SizeF : IEquatable<SizeF>
		{
			public float Width;

			public float Height;

			public bool IsEmpty
			{
				get
				{
					if (Width == 0f)
					{
						return Height == 0f;
					}
					return false;
				}
			}

			public SizeF(float width, float height)
			{
				Width = width;
				Height = height;
			}

			public bool Equals(SizeF other)
			{
				if (Width == other.Width)
				{
					return Height == other.Height;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				if (obj is SizeF)
				{
					return Equals((SizeF)obj);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return ((int)Width * 397) ^ (int)Height;
			}

			public static bool operator ==(SizeF left, SizeF right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(SizeF left, SizeF right)
			{
				return !(left == right);
			}

			public void Offset(float width, float height)
			{
				Width += width;
				Height += height;
			}

			public void Set(float width, float height)
			{
				Width = width;
				Height = height;
			}

			public override string ToString()
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				return "{ Width = " + Width.ToString(currentCulture) + ", Height = " + Height.ToString(currentCulture) + " }";
			}
		}

		public enum GetWindowLongFields
		{
			GWL_EXSTYLE = -20,
			GWL_HINSTANCE = -6,
			GWL_HWNDPARENT = -8,
			GWL_ID = -12,
			GWL_STYLE = -16,
			GWL_USERDATA = -21,
			GWL_WNDPROC = -4
		}

		internal static class Properties
		{
			public const CharSet BuildCharSet = CharSet.Unicode;
		}

		[Obsolete("请使用 GetWindowLongFields.GWL_STYLE")]
		public const int GWL_STYLE = -16;

		[Obsolete("请使用 Win32.GetWindowLongFields.GWL_HWNDPARENT")]
		public const int GWLP_HWNDPARENT = -8;

		[Obsolete("请使用 WindowStyles.WS_THICKFRAME")]
		public const int WS_THICKFRAME = 262144;

		[Obsolete("请使用 WindowStyles.WS_CHILD")]
		public const int WS_CHILD = 1073741824;

		[Obsolete("请使用 GetWindowLongFields.GWL_EXSTYLE")]
		public static int GWL_EXSTYLE = -20;

		[Obsolete("请使用 GetWindowLongFields.GWL_HINSTANCE")]
		public const int GWL_HINSTANCE = -6;

		[Obsolete("请使用 GetWindowLongFields.GWL_ID")]
		public const int GWL_ID = -12;

		[Obsolete("请使用 ExtendedWindowStyles.WS_EX_LAYERED")]
		public static int WS_EX_LAYERED = 524288;

		[Obsolete("请使用 ExtendedWindowStyles.WS_EX_TRANSPARENT")]
		public static int WS_EX_TRANSPARENT = 32;

		[Obsolete("请使用 ExtendedWindowStyles.WS_EX_NOACTIVATE")]
		public static int WS_EX_NOACTIVATE = 134217728;

		[Obsolete("请使用 ExtendedWindowStyles.WS_EX_TOOLWINDOW")]
		public const int WS_EX_TOOLWINDOW = 128;

		public const int HT_CAPTION = 2;

		[Obsolete("请使用 WS_VISIBLE")]
		public const int WS_VISIBLE = 268435456;

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.CreateIconIndirect")]
		public static extern SafeIconHandle CreateIconIndirect(ref IconInfo icon);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.DestroyIcon")]
		public static extern bool DestroyIcon(IntPtr hIcon);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.GetIconInfo")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

		[DllImport("gdi32.dll")]
		[Obsolete("请使用 Win32.Gdi32.DeleteDC")]
		public static extern bool DeleteDC([In] IntPtr hdc);

		[DllImport("gdi32.dll")]
		[Obsolete("请使用 Win32.Gdi32.BitBlt")]
		public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);

		[DllImport("gdi32.dll", SetLastError = true)]
		[Obsolete("请使用 Win32.Gdi32.SelectObject")]
		public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

		[DllImport("gdi32.dll")]
		[Obsolete("请使用 Win32.Gdi32.CreateCompatibleBitmap")]
		public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll", SetLastError = true)]
		[Obsolete("请使用 Win32.Gdi32.CreateCompatibleDC")]
		public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.ReleaseDC")]
		public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

		[DllImport("gdi32.dll")]
		[Obsolete("请使用 Win32.Gdi32.DeleteObject")]
		internal static extern IntPtr DeleteObject(IntPtr hDc);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.GetWindowDC")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		[DllImport("User32.dll")]
		[Obsolete("请使用 Win32.User32.FindWindow")]
		public static extern int FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.GetDesktopWindow")]
		internal static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.WindowFromPoint")]
		public static extern IntPtr WindowFromPoint(Point Point);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.GetWindowThreadProcessId")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.GetForegroundWindow")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		[Obsolete("请使用 Win32.User32.BringWindowToTop")]
		public static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.ShowWindow")]
		public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.SetForegroundWindow")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.GetClientRect")]
		public static extern bool GetClientRect(IntPtr hWnd, ref RECT rect);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.GetWindowRect")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32")]
		[Obsolete("请使用 Win32.User32.SetWindowPos")]
		public static extern int SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlagslong);

		[DllImport("User32.dll")]
		[Obsolete("请使用 Win32.User32.SendMessage")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[DllImport("User32.dll")]
		[Obsolete("请使用 Win32.User32.SendMessage")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.SetWindowLong")]
		public static extern int SetWindowLong(IntPtr window, int index, int value);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.SetWindowLongPtr")]
		public static extern int SetWindowLongPtr(IntPtr window, int index, int value);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.GetWindowLong")]
		public static extern int GetWindowLong(IntPtr window, int index);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[Obsolete("请使用 Win32.User32.GetCursorPos")]
		public static extern bool GetCursorPos(out Point pt);

		[DllImport("user32")]
		[Obsolete("请使用 Win32.User32.SetCursorPos")]
		public static extern void SetCursorPos(int x, int y);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.ReleaseCapture")]
		public static extern bool ReleaseCapture();

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[Obsolete("请使用 Win32.User32.Keyboard_Event")]
		public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		[Obsolete("请使用 Win32.User32.GetKeyState")]
		public static extern short GetKeyState(int nVirtKey);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.AllowSetForegroundWindow")]
		public static extern bool AllowSetForegroundWindow(int processId);

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.PostMessage")]
		public static extern int PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		[DllImport("user32.dll", SetLastError = true)]
		[Obsolete("请使用 Win32.User32.SetParent")]
		internal static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("kernel32.dll")]
		[Obsolete("请使用 Kernel32.GetCurrentThreadId")]
		public static extern uint GetCurrentThreadId();

		[DllImport("user32.dll")]
		[Obsolete("请使用 Win32.User32.AttachThreadInput")]
		public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		[DllImport("User32.dll")]
		[Obsolete("请使用 Win32.User32.SetFocus")]
		public static extern IntPtr SetFocus(IntPtr hWnd);

		public static void ForceWindowToForeground(IntPtr hWnd)
		{
			uint windowThreadProcessId = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
			uint currentThreadId = GetCurrentThreadId();
			if (windowThreadProcessId != currentThreadId)
			{
				AttachThreadInput(windowThreadProcessId, currentThreadId, true);
				BringWindowToTop(hWnd);
				ShowWindow(hWnd, 5u);
				AttachThreadInput(windowThreadProcessId, currentThreadId, false);
			}
			else
			{
				BringWindowToTop(hWnd);
				ShowWindow(hWnd, 5u);
			}
		}
	}
}
