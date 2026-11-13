using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace PPTService.Defender
{
	/// <summary>
	/// PPTService.Defender —— PPT 小工具的守护（看门狗）进程。
	///
	/// 职责：
	///  1. 监控 PPTService.exe：发现其退出且退出码非 0（崩溃 / 异常终止）时，自动重新启动它；
	///     正常退出（退出码 0）时守护进程也一并退出。
	///  2. 单实例：通过命名互斥体保证同一时刻只有一个守护进程。
	///  3. 自检「首发崩溃」：启动时若发现上次启动未完成即崩溃的标记，说明守护进程自身在初始化阶段崩溃，
	///     连续超过阈值则停止自启，避免无限崩溃循环。
	///  4. 防「重启风暴」：被监控程序在短时间窗口内反复崩溃超过阈值则停止重启。
	///
	/// 说明：
	///  - 该进程由 PPTService 在启动时拉起（调试模式不拉起），自身不再依赖 PPTService 存活。
	///  - 重新拉起 PPTService 时带 "Restart" 参数，绕过其单实例互斥体。
	/// </summary>
	internal static class Program
	{
		private const string WatchedExe = "PPTService.exe";

		private const string MutexName = "PPTService.Defender.Mutex";

		// 启动标记：进入监控循环并初始化成功后即删除；若存在说明上次启动未完成即崩溃
		private const string BootMarker = "Defender.boot";

		// 守护进程自身首发崩溃计数
		private const int MaxBootCrashes = 3;

		// 重启风暴阈值：窗口内崩溃次数上限
		private const int MaxRestartStorm = 5;

		private const int RestartStormWindowSec = 60;

		private static readonly string DataDir = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"Seewo", "PPTService", "Defender");

		[STAThread]
		private static void Main()
		{
			// 单实例
			bool createdNew;
			using (var mutex = new Mutex(true, MutexName, out createdNew))
			{
				if (!createdNew)
				{
					return;
				}

				// 自检：检测守护进程自身首发崩溃
				try
				{
					Directory.CreateDirectory(DataDir);
					string bootMarker = Path.Combine(DataDir, BootMarker);
					string crashCountFile = Path.Combine(DataDir, "BootCrashCount");
					if (File.Exists(bootMarker))
					{
						int bootCrashes = ReadInt(crashCountFile) + 1;
						Log($"检测到守护进程上次启动未完成即崩溃（第 {bootCrashes} 次）。");
						if (bootCrashes >= MaxBootCrashes)
						{
							Log($"守护进程连续 {MaxBootCrashes} 次启动崩溃，停止自启以避免死循环。");
							File.Delete(bootMarker);
							File.Delete(crashCountFile);
							return;
						}
						WriteInt(crashCountFile, bootCrashes);
					}
					else
					{
						WriteInt(crashCountFile, 0);
					}
					// 写入启动标记：进入监控循环并成功初始化后删除
					File.WriteAllText(bootMarker, DateTime.Now.ToString("o"));
				}
				catch (Exception ex)
				{
					Log("自检阶段异常：" + ex);
				}

				try
				{
					Run();
				}
				finally
				{
					try
					{
						File.Delete(Path.Combine(DataDir, BootMarker));
					}
					catch
					{
					}
				}
			}
		}

		private static void Run()
		{
			string exePath = ResolveWatchedPath();
			Log($"守护进程启动，监控目标：{exePath}");

			int restartCount = 0;
			var stormWindowStart = DateTime.MinValue;

			while (true)
			{
				Process proc = FindOrStart(exePath);
				if (proc == null)
				{
					Log("无法定位或启动被监控程序，守护进程退出。");
					return;
				}

				Log($"被监控程序已启动（PID={proc.Id}），等待其退出……");
				while (!proc.HasExited)
				{
					Thread.Sleep(1000);
				}

				int exitCode = 0;
				try
				{
					exitCode = proc.ExitCode;
				}
				catch
				{
				}
				Log($"被监控程序已退出，退出码={exitCode}。");

				if (exitCode == 0)
				{
					// 正常退出：守护进程一同退出
					Log("检测到正常退出，守护进程终止。");
					return;
				}

				// 异常退出（崩溃）：重启，并做重启风暴保护
				if ((DateTime.Now - stormWindowStart).TotalSeconds > RestartStormWindowSec)
				{
					stormWindowStart = DateTime.Now;
					restartCount = 0;
				}
				restartCount++;
				if (restartCount > MaxRestartStorm)
				{
					Log($"被监控程序在 {RestartStormWindowSec}s 内崩溃超过 {MaxRestartStorm} 次，停止重启。");
					return;
				}
				Log($"尝试重启被监控程序（第 {restartCount} 次）……");
				Thread.Sleep(2000);
			}
		}

		private static Process FindOrStart(string exePath)
		{
			foreach (var p in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(WatchedExe)))
			{
				try
				{
					if (!p.HasExited)
					{
						// 仅当路径匹配时才视为目标（避免同名其它进程）
						if (string.Equals(p.MainModule.FileName, exePath, StringComparison.OrdinalIgnoreCase))
						{
							return p;
						}
					}
				}
				catch
				{
				}
				finally
				{
					p.Dispose();
				}
			}

			if (!File.Exists(exePath))
			{
				return null;
			}
			var psi = new ProcessStartInfo(exePath, "Restart")
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			};
			return Process.Start(psi);
		}

		private static string ResolveWatchedPath()
		{
			// 默认与守护进程同目录
			var myDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".";
			var candidate = Path.Combine(myDir, WatchedExe);
			if (File.Exists(candidate))
			{
				return candidate;
			}

			// 回退：注册表 ExePath（与 PPTService.Restart 一致）
			try
			{
				using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Seewo\PPTService"))
				{
					if (key != null)
					{
						var v = key.GetValue("ExePath") as string;
						if (!string.IsNullOrEmpty(v) && File.Exists(v))
						{
							return v;
						}
					}
				}
			}
			catch
			{
			}
			return candidate;
		}

		private static int ReadInt(string file)
		{
			try
			{
				return int.Parse(File.ReadAllText(file));
			}
			catch
			{
				return 0;
			}
		}

		private static void WriteInt(string file, int n)
		{
			try
			{
				File.WriteAllText(file, n.ToString());
			}
			catch
			{
			}
		}

		private static void Log(string msg)
		{
			try
			{
				Directory.CreateDirectory(DataDir);
				File.AppendAllText(
					Path.Combine(DataDir, "Defender.log"),
					$"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}\r\n");
			}
			catch
			{
			}
		}
	}
}
