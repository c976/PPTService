using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PPTServiceUltra
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

		// 新版 WPS 可能用 wps.exe 统一承载演示窗口，不能再只认 wpp.exe。
		private const string WpsName = "wps.exe";

		private const string WppSimplifyName = "wpp";

		private const string WpsSimplifyName = "wps";

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
					if (text == "powerpnt.exe" || text == WppName || text == WpsName)
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

		/// <summary>是否有 WPS 演示进程（wpp.exe / 新版统一进程 wps.exe）在运行。用于识别 Office 与 WPS 共存时绑错应用。</summary>
		public static bool HasWppProcess()
		{
			try
			{
				return Process.GetProcessesByName(WppSimplifyName).Length > 0 ||
					Process.GetProcessesByName(WpsSimplifyName).Length > 0;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 是否存在 WPS 放映窗口。新版 WPS 可能用 wps.exe 统一承载演示，
		/// 也可能仍是 wpp.exe；放映窗口的 MainWindowHandle 常为 0，所以枚举所有顶层窗口。
		/// COM 通道收不到 WPS 放映事件时，这是回退到 ROT 的可靠判据。
		/// </summary>
		public static bool HasWpsSlideShowWindow()
		{
			return FindWpsSlideShowWindow() != IntPtr.Zero;
		}

		/// <summary>返回 WPS 放映窗口句柄；没有时返回 IntPtr.Zero。</summary>
		public static IntPtr FindWpsSlideShowWindow()
		{
			try
			{
				HashSet<uint> wpsPids = new HashSet<uint>();
				foreach (string name in new[] { WppSimplifyName, WpsSimplifyName })
				{
					foreach (Process process in Process.GetProcessesByName(name))
					{
						using (process)
						{
							wpsPids.Add((uint)process.Id);
						}
					}
				}
				if (wpsPids.Count == 0)
				{
					return IntPtr.Zero;
				}

				IntPtr found = IntPtr.Zero;
				EnumWindows(delegate(IntPtr handle, IntPtr parameter)
				{
					if (found != IntPtr.Zero || !WindowsApi.IsWindowVisible(handle))
					{
						return true;
					}
					uint pid;
					GetWindowThreadProcessId(handle, out pid);
					if (!wpsPids.Contains(pid))
					{
						return true;
					}
					int length = GetWindowTextLength(handle);
					if (length <= 0)
					{
						return true;
					}
					StringBuilder text = new StringBuilder(length + 1);
					GetWindowText(handle, text, text.Capacity);
					string title = text.ToString();
					if (title.IndexOf("Slide Show", StringComparison.OrdinalIgnoreCase) >= 0 ||
						title.IndexOf("幻灯片放映", StringComparison.Ordinal) >= 0 ||
						title.IndexOf("放映", StringComparison.Ordinal) >= 0)
					{
						found = handle;
					}
					return true;
				}, IntPtr.Zero);
				return found;
			}
			catch
			{
				// 进程窗口枚举失败不影响其它判据。
			}
			return IntPtr.Zero;
		}

		/// <summary>返回 WPS 放映窗口所属进程的可执行文件路径；取不到时返回 null。</summary>
		public static string GetWpsSlideShowProcessPath()
		{
			try
			{
				IntPtr hwnd = FindWpsSlideShowWindow();
				if (hwnd == IntPtr.Zero)
				{
					return null;
				}
				uint pid;
				GetWindowThreadProcessId(hwnd, out pid);
				if (pid == 0)
				{
					return null;
				}
				using (Process process = Process.GetProcessById((int)pid))
				{
					return process.MainModule.FileName;
				}
			}
			catch
			{
				return null;
			}
		}

		/// <summary>前台窗口是否属于 WPS（wpp/wps）。比"进程存在"更精准，避免 WPS 后台常驻时误切通道。</summary>
		public static bool IsWppForeground()
		{
			try
			{
				IntPtr front = WindowsApi.GetForegroundWindow();
				if (front == IntPtr.Zero)
				{
					return false;
				}
				uint pid;
				GetWindowThreadProcessId(front, out pid);
				if (pid == 0)
				{
					return false;
				}
				using (Process proc = Process.GetProcessById((int)pid))
				{
					string name = proc.ProcessName.ToLowerInvariant();
					return name.StartsWith("wpp", StringComparison.Ordinal) ||
						name.StartsWith("wps", StringComparison.Ordinal);
				}
			}
			catch
			{
				return false;
			}
		}

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr handle, out uint processId);

		private delegate bool EnumWindowsProc(IntPtr handle, IntPtr parameter);

		[DllImport("user32.dll")]
		private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr parameter);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetWindowTextLength(IntPtr handle);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetWindowText(IntPtr handle, StringBuilder text, int maxCount);

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
