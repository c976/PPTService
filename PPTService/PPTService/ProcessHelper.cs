using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PPTService
{
	internal static class ProcessHelper
	{
		[Flags]
		private enum SnapshotFlags : uint
		{
			HeapList = 1u,
			Process = 2u,
			Thread = 4u,
			Module = 8u,
			Module32 = 0x10u,
			Inherit = 0x80000000u,
			All = HeapList | Process | Thread | Module | Module32,
			NoHeaps = 0x40000000u
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct PROCESSENTRY32
		{
			private const int MAX_PATH = 260;

			internal uint dwSize;

			internal uint cntUsage;

			internal uint th32ProcessID;

			internal IntPtr th32DefaultHeapID;

			internal uint th32ModuleID;

			internal uint cntThreads;

			internal uint th32ParentProcessID;

			internal int pcPriClassBase;

			internal uint dwFlags;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			internal string szExeFile;
		}

		private const string PptName = "powerpnt.exe";

		private const string WppName = "wpp.exe";

		private const string WppSimplifyName = "wpp";

		public static bool HasPowerPointProcess()
		{
			bool result = false;
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				PROCESSENTRY32 lppe = new PROCESSENTRY32
				{
					dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32))
				};
				intPtr = CreateToolhelp32Snapshot(2u, 0u);
				if (!Process32First(intPtr, ref lppe))
				{
					throw new ApplicationException($"Failed with win32 error code {Marshal.GetLastWin32Error()}");
				}
				do
				{
					string text = lppe.szExeFile.ToLower();
					if (text == "powerpnt.exe" || text == "wpp.exe")
					{
						result = true;
						break;
					}
				}
				while (Process32Next(intPtr, ref lppe));
			}
			catch (Exception innerException)
			{
				throw new ApplicationException("Can't get the process.", innerException);
			}
			finally
			{
				CloseHandle(intPtr);
			}
			return result;
		}

		public static void TryKillWppProcess()
		{
			Process[] processesByName = Process.GetProcessesByName("wpp");
			if (processesByName.Length == 1)
			{
				Process[] array = processesByName;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Kill();
				}
			}
		}

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CreateToolhelp32Snapshot([In] uint dwFlags, [In] uint th32ProcessID);

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool Process32First([In] IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool Process32Next([In] IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

		[DllImport("kernel32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle([In] IntPtr hObject);
	}
}
