using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Cvte.Windows
{
	internal static class ScreenInterrogatory
	{
		public enum QUERY_DEVICE_CONFIG_FLAGS : uint
		{
			QDC_ALL_PATHS = 1u,
			QDC_ONLY_ACTIVE_PATHS = 2u,
			QDC_DATABASE_CURRENT = 4u
		}

		public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint
		{
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = uint.MaxValue,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = 2147483648u,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER
		}

		public enum DISPLAYCONFIG_SCANLINE_ORDERING : uint
		{
			DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0u,
			DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1u,
			DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2u,
			DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
			DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3u,
			DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = uint.MaxValue
		}

		public enum DISPLAYCONFIG_ROTATION : uint
		{
			DISPLAYCONFIG_ROTATION_IDENTITY = 1u,
			DISPLAYCONFIG_ROTATION_ROTATE90 = 2u,
			DISPLAYCONFIG_ROTATION_ROTATE180 = 3u,
			DISPLAYCONFIG_ROTATION_ROTATE270 = 4u,
			DISPLAYCONFIG_ROTATION_FORCE_UINT32 = uint.MaxValue
		}

		public enum DISPLAYCONFIG_SCALING : uint
		{
			DISPLAYCONFIG_SCALING_IDENTITY = 1u,
			DISPLAYCONFIG_SCALING_CENTERED = 2u,
			DISPLAYCONFIG_SCALING_STRETCHED = 3u,
			DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4u,
			DISPLAYCONFIG_SCALING_CUSTOM = 5u,
			DISPLAYCONFIG_SCALING_PREFERRED = 128u,
			DISPLAYCONFIG_SCALING_FORCE_UINT32 = uint.MaxValue
		}

		public enum DISPLAYCONFIG_PIXELFORMAT : uint
		{
			DISPLAYCONFIG_PIXELFORMAT_8BPP = 1u,
			DISPLAYCONFIG_PIXELFORMAT_16BPP = 2u,
			DISPLAYCONFIG_PIXELFORMAT_24BPP = 3u,
			DISPLAYCONFIG_PIXELFORMAT_32BPP = 4u,
			DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5u,
			DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = uint.MaxValue
		}

		public enum DISPLAYCONFIG_MODE_INFO_TYPE : uint
		{
			DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1u,
			DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2u,
			DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = uint.MaxValue
		}

		public enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint
		{
			DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1u,
			DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2u,
			DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3u,
			DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4u,
			DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5u,
			DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6u,
			DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32 = uint.MaxValue
		}

		public struct LUID
		{
			public uint LowPart;

			public int HighPart;
		}

		public struct DISPLAYCONFIG_PATH_SOURCE_INFO
		{
			public LUID adapterId;

			public uint id;

			public uint modeInfoIdx;

			public uint statusFlags;
		}

		public struct DISPLAYCONFIG_PATH_TARGET_INFO
		{
			public LUID adapterId;

			public uint id;

			public uint modeInfoIdx;

			private DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;

			private DISPLAYCONFIG_ROTATION rotation;

			private DISPLAYCONFIG_SCALING scaling;

			private DISPLAYCONFIG_RATIONAL refreshRate;

			private DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;

			public bool targetAvailable;

			public uint statusFlags;
		}

		public struct DISPLAYCONFIG_RATIONAL
		{
			public uint Numerator;

			public uint Denominator;
		}

		public struct DISPLAYCONFIG_PATH_INFO
		{
			public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;

			public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;

			public uint flags;
		}

		public struct DISPLAYCONFIG_2DREGION
		{
			public uint cx;

			public uint cy;
		}

		public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
		{
			public ulong pixelRate;

			public DISPLAYCONFIG_RATIONAL hSyncFreq;

			public DISPLAYCONFIG_RATIONAL vSyncFreq;

			public DISPLAYCONFIG_2DREGION activeSize;

			public DISPLAYCONFIG_2DREGION totalSize;

			public uint videoStandard;

			public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
		}

		public struct DISPLAYCONFIG_TARGET_MODE
		{
			public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
		}

		public struct POINTL
		{
			private int x;

			private int y;
		}

		public struct DISPLAYCONFIG_SOURCE_MODE
		{
			public uint width;

			public uint height;

			public DISPLAYCONFIG_PIXELFORMAT pixelFormat;

			public POINTL position;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct DISPLAYCONFIG_MODE_INFO_UNION
		{
			[FieldOffset(0)]
			public DISPLAYCONFIG_TARGET_MODE targetMode;

			[FieldOffset(0)]
			public DISPLAYCONFIG_SOURCE_MODE sourceMode;
		}

		public struct DISPLAYCONFIG_MODE_INFO
		{
			public DISPLAYCONFIG_MODE_INFO_TYPE infoType;

			public uint id;

			public LUID adapterId;

			public DISPLAYCONFIG_MODE_INFO_UNION modeInfo;
		}

		public struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
		{
			public uint value;
		}

		public struct DISPLAYCONFIG_DEVICE_INFO_HEADER
		{
			public DISPLAYCONFIG_DEVICE_INFO_TYPE type;

			public uint size;

			public LUID adapterId;

			public uint id;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DISPLAYCONFIG_TARGET_DEVICE_NAME
		{
			public DISPLAYCONFIG_DEVICE_INFO_HEADER header;

			public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;

			public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;

			public ushort edidManufactureId;

			public ushort edidProductCodeId;

			public uint connectorInstance;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string monitorFriendlyDeviceName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string monitorDevicePath;
		}

		public const int ERROR_SUCCESS = 0;

		[DllImport("user32.dll")]
		public static extern int GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

		[DllImport("user32.dll")]
		public static extern int QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS flags, ref uint numPathArrayElements, [Out] DISPLAYCONFIG_PATH_INFO[] PathInfoArray, ref uint numModeInfoArrayElements, [Out] DISPLAYCONFIG_MODE_INFO[] ModeInfoArray, IntPtr currentTopologyId);

		[DllImport("user32.dll")]
		public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName);

		private static string MonitorFriendlyName(LUID adapterId, uint targetId)
		{
			DISPLAYCONFIG_TARGET_DEVICE_NAME dISPLAYCONFIG_TARGET_DEVICE_NAME = new DISPLAYCONFIG_TARGET_DEVICE_NAME
			{
				header = 
				{
					size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_TARGET_DEVICE_NAME)),
					adapterId = adapterId,
					id = targetId
				}
			};
			int num = default(int);
			if (-1 != 0)
			{
				dISPLAYCONFIG_TARGET_DEVICE_NAME.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
				DISPLAYCONFIG_TARGET_DEVICE_NAME dISPLAYCONFIG_TARGET_DEVICE_NAME2 = dISPLAYCONFIG_TARGET_DEVICE_NAME;
				DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName = default(DISPLAYCONFIG_TARGET_DEVICE_NAME);
				if (0 == 0)
				{
					deviceName = dISPLAYCONFIG_TARGET_DEVICE_NAME2;
				}
				if (0 == 0)
				{
					num = DisplayConfigGetDeviceInfo(ref deviceName);
					if (num != 0)
					{
						goto IL_005c;
					}
				}
				return deviceName.monitorFriendlyDeviceName;
			}
			goto IL_005c;
			IL_005c:
			throw new Win32Exception(num);
		}

		private static IEnumerable<string> GetAllMonitorsFriendlyNames()
		{
			uint pathCount;
			uint modeCount;
			int error = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
			DISPLAYCONFIG_PATH_INFO[] displayPaths = default(DISPLAYCONFIG_PATH_INFO[]);
			DISPLAYCONFIG_MODE_INFO[] displayModes = default(DISPLAYCONFIG_MODE_INFO[]);
			if (error != 0)
			{
				if (-1 != 0)
				{
					throw new Win32Exception(error);
				}
			}
			else
			{
				displayPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
				displayModes = new DISPLAYCONFIG_MODE_INFO[modeCount];
			}
			error = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, ref pathCount, displayPaths, ref modeCount, displayModes, IntPtr.Zero);
			if (error != 0)
			{
				throw new Win32Exception(error);
			}
			int i = 0;
			while (true)
			{
				if (0 == 0)
				{
					if (i >= modeCount)
					{
						break;
					}
					if (displayModes[i].infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET)
					{
						yield return MonitorFriendlyName(displayModes[i].adapterId, displayModes[i].id);
					}
				}
				i++;
			}
		}

		public unsafe static string GetDeviceFriendlyName(this Screen screen)
		{
			IEnumerable<string> allMonitorsFriendlyNames;
			int num;
			do
			{
				allMonitorsFriendlyNames = GetAllMonitorsFriendlyNames();
				num = 0;
			}
			while (2 == 0);
			if (false)
			{
				goto IL_001a;
			}
			goto IL_002b;
			IL_002b:
			while (true)
			{
				int num2 = num;
				IntPtr intPtr = (IntPtr)(void*)Screen.AllScreens.LongLength;
				if (0 == 0)
				{
					intPtr = (IntPtr)(int)(long)intPtr;
				}
				if ((long)num2 < (long)intPtr)
				{
					if (object.Equals(screen, Screen.AllScreens[num]))
					{
						break;
					}
					int num3 = num;
					do
					{
						num3++;
					}
					while (false);
					num = num3;
					continue;
				}
				return null;
			}
			goto IL_001a;
			IL_001a:
			if (0 == 0)
			{
				return allMonitorsFriendlyNames.ToArray()[num];
			}
			goto IL_002b;
		}
	}
}
