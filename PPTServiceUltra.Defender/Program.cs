using System;
using PPTServiceUltra.Shared;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace PPTServiceUltra.Defender
{
	/// <summary>
	/// PPTServiceUltra.Defender —— PPT 小工具的守护（看门狗）进程。
	///
	/// 互守护约定（与主程序 PPTServiceUltra.DefenderGuardian 对应）：
	///  - 守护进程每 0.5s 巡检主程序，主程序崩溃 / 消失后 1s 内重新拉起；
	///  - 主程序每 1s 巡检守护进程，守护进程消失后 2s 内重新拉起。
	///  - 调试模式不启用（由主程序侧控制是否拉起本进程）。
	///
	/// 注意：这里刻意不持有 Process 对象等待 HasExited，而是每轮重新枚举进程。
	/// 旧版在 try 里 return p、finally 里 p.Dispose()，返回的是已释放对象，
	/// Run() 首次访问 HasExited 即抛异常导致守护进程静默退出 —— 即「运行没有反应」。
	///
	/// 停止：正常退出 / 卸载 / 升级时直接结束掉两个进程（不设计进程间优雅退出握手），
	/// 安装脚本会在同一轮里同时结束主程序与守护进程，避免互相拉起。
	/// </summary>
	internal static class Program
	{
		private const string WatchedExe = "PPTServiceUltra.exe";
		private const string WatchedProcessName = "PPTServiceUltra";

		private const string MutexName = "PPTServiceUltra.Defender.Mutex";

		// 启动标记：仅在「初始化阶段」存在，初始化成功后立刻删除。
		// 因此残留标记 == 上次启动没走到初始化完成（首发崩溃），被强杀不会误判。
		private const string BootMarker = "Defender.boot";
		private const string BootCrashCountFile = "BootCrashCount";
		private const int MaxBootCrashes = 3;

		/// <summary>
		/// 守护进程巡检主程序的间隔（毫秒）。设为 500ms：主程序崩溃后最坏 500ms 被发现，
		/// 加上进程启动耗时仍可保证「1s 内拉起」。
		/// </summary>
		private const int CheckIntervalMs = 500;

		private const int MaxRestartStorm = 5;
		private const int RestartStormWindowSec = 60;

		/// <summary>
		/// 拉起后主程序存活超过该秒数视为「启动成功」，清空连续崩溃计数。
		/// 巡检间隔缩短到 0.5s 后，若只按「60s 内拉起次数」统计，
		/// 主程序启动慢 / 单实例互斥体抢占导致连续几次快速退出会在 2.5s 内
		/// 触发风暴保护而永久放弃守护；用存活时间区分「反复崩溃」与「正常运行后崩溃」。
		/// </summary>
		private const int HealthyAfterRestartSec = 3;

		// 与主程序 AppPath.LocalRoot() 保持一致：%APPDATA%\PPTServiceUltra\Defender（Roaming）
		private static readonly string DataDir = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"PPTServiceUltra", "Defender");

		/// <summary>守护程序日志目录：与主程序统一放在 %APPDATA%\PPTServiceUltra\Logs 下，按程序分目录。</summary>
		private static string LogDirectory()
		{
			return Path.Combine(Path.GetDirectoryName(DataDir) ?? DataDir, "Logs", "Defender");
		}

		private static string BaseDirectory
		{
			get { return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "."; }
		}

		private static string BootMarkerPath
		{
			get { return Path.Combine(DataDir, BootMarker); }
		}

		private static string BootCrashCountPath
		{
			get { return Path.Combine(DataDir, BootCrashCountFile); }
		}

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
				if (!SelfCheck())
				{
					return;
				}
				try
				{
					string exePath = ResolveWatchedPath();
					Log("Defender started.");
					// 初始化完成：删除启动标记。此后被强杀不再算「首发崩溃」。
					TryDelete(BootMarkerPath);
					Run(exePath);
				}
				catch (Exception ex)
				{
					Log("Loop error: " + ex.Message);
				}
			}
		}

		/// <summary>自检「守护进程首发崩溃」：残留启动标记说明上次初始化未完成。</summary>
		private static bool SelfCheck()
		{
			try
			{
				Directory.CreateDirectory(DataDir);
				if (File.Exists(BootMarkerPath))
				{
					int crashes = ReadInt(BootCrashCountPath) + 1;
					Log("Boot crash detected (#" + crashes + ").");
					if (crashes >= MaxBootCrashes)
					{
						Log("Stop after " + MaxBootCrashes + " boot crashes.");
						TryDelete(BootCrashCountPath);
						return false;
					}
					WriteInt(BootCrashCountPath, crashes);
				}
				else
				{
					WriteInt(BootCrashCountPath, 0);
				}
				File.WriteAllText(BootMarkerPath, DateTime.Now.ToString("o"));
				return true;
			}
			catch (Exception ex)
			{
				Log("Self-check error: " + ex.Message);
				return true;
			}
		}

		private static void Run(string exePath)
		{
			int restartCount = 0;
			DateTime stormWindowStart = DateTime.MinValue;
			DateTime lastRestart = DateTime.MinValue;

			while (true)
			{
				Thread.Sleep(CheckIntervalMs);
				try
				{
					if (IsMainRunning())
					{
						// 拉起后能稳定存活一段时间 = 启动成功，清空连续崩溃计数。
						if (lastRestart != DateTime.MinValue &&
							(DateTime.Now - lastRestart).TotalSeconds >= HealthyAfterRestartSec)
						{
							restartCount = 0;
							lastRestart = DateTime.MinValue;
						}
						continue;
					}
					if (!File.Exists(exePath))
					{
						Log("Target not found.");
						continue;
					}

					if ((DateTime.Now - stormWindowStart).TotalSeconds > RestartStormWindowSec)
					{
						stormWindowStart = DateTime.Now;
						restartCount = 0;
					}
					restartCount++;
					if (restartCount > MaxRestartStorm)
					{
						Log("Restart storm limit reached.");
						return;
					}
					Log("Restarting main (#" + restartCount + ").");
					// 不带 Restart 参数：主程序单实例互斥体会自动去掉重复实例，
					// 避免与 PostErrorAndShutdown 里的 Restart.PPTServiceUltra() 抢出双份。
					Process.Start(new ProcessStartInfo(exePath)
					{
						UseShellExecute = false,
						WorkingDirectory = BaseDirectory
					});
					lastRestart = DateTime.Now;
				}
				catch (Exception ex)
				{
					Log("Check error: " + ex.Message);
				}
			}
		}

		/// <summary>
		/// 每轮重新枚举进程名，不缓存也不跨轮持有 Process 对象。
		/// 精确比对进程名，避免把 PPTServiceUltra.Defender 自己算成主程序。
		/// </summary>
		private static bool IsMainRunning()
		{
			Process[] processes = Process.GetProcessesByName(WatchedProcessName);
			try
			{
				foreach (Process p in processes)
				{
					try
					{
						if (string.Equals(p.ProcessName, WatchedProcessName, StringComparison.OrdinalIgnoreCase))
						{
							return true;
						}
					}
					catch
					{
					}
				}
				return false;
			}
			finally
			{
				foreach (Process p in processes)
				{
					p.Dispose();
				}
			}
		}

		private static string ResolveWatchedPath()
		{
			// 默认与守护进程同目录
			var candidate = Path.Combine(BaseDirectory, WatchedExe);
			if (File.Exists(candidate))
			{
				return candidate;
			}
			// 回退：注册表 ExePath（与 PPTServiceUltra.Restart 一致）
			try
			{
				using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\PPTServiceUltra"))
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

		private static void TryDelete(string file)
		{
			try
			{
				File.Delete(file);
			}
			catch
			{
			}
		}

		private static void Log(string msg)
		{
			try
			{
				string directory = LogDirectory();
				Directory.CreateDirectory(directory);
				File.AppendAllText(LogCleaner.FilePath(directory),
					$"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}\r\n");
				LogCleaner.Clean(directory);
			}
			catch
			{
			}
		}
	}
}
