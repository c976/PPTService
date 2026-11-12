using System;
using System.Collections.Generic;
using System.Linq;

namespace PPTService.Core.Mcu
{
	public static class Mcu
	{
		private static readonly Usb Usb;

		private static readonly List<int> Vids;

		private static readonly int MinPid;

		private static readonly int MaxPid;

		private static readonly string Key;

		private static readonly int DataLength;

		static Mcu()
		{
			Vids = new List<int> { 8183, 8746 };
			MinPid = 1;
			MaxPid = 32;
			Key = "col03";
			DataLength = 64;
			Usb = new Usb();
		}

		public static bool FindMcu()
		{
			bool result = false;
			try
			{
				int pid;
				for (pid = MinPid; pid < MaxPid; pid++)
				{
					if (Vids.Any((int t) => Usb.FindUsb(t, pid, Key)))
					{
						result = true;
						break;
					}
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static bool ForbiddenAndroidPen()
		{
			try
			{
				return Write(McuCommand.ForbiddenAndroidPenCommand);
			}
			catch
			{
				return false;
			}
		}

		public static bool OpenAndroidPen()
		{
			try
			{
				return Write(McuCommand.OpenAndroidPenCommand);
			}
			catch
			{
				return false;
			}
		}

		public static int GetCVTouchSize()
		{
			try
			{
				if (!Write(McuCommand.GetMcuVersionCommand))
				{
					return -1;
				}
				for (int i = 0; i < 3; i++)
				{
					byte[] data;
					if (!Read(DataLength, out data))
					{
						return -1;
					}
					if (Analyzer.IsMatchCVTouchVersion(data))
					{
						return Analyzer.GetCVTouchSize(data);
					}
				}
				return -1;
			}
			catch (Exception)
			{
				return -1;
			}
		}

		public static bool Write(byte[] array)
		{
			try
			{
				return Connection() && Usb.WriteUsbData(array);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool Read(int dataLength, out byte[] data)
		{
			data = new byte[dataLength];
			try
			{
				return Usb.ReadUsbData(dataLength, out data);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static void CloseConnection()
		{
			try
			{
				Usb?.CloseConnection();
			}
			catch (Exception)
			{
			}
		}

		private static bool Connection()
		{
			bool result = false;
			try
			{
				int pid;
				for (pid = MinPid; pid < MaxPid; pid++)
				{
					if (Vids.Any((int t) => Usb.OnInitUsb(t, pid, Key)))
					{
						result = true;
						break;
					}
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}
	}
}
