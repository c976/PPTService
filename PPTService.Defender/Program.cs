using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace PPTService.Defender
{
	/// <summary>
	/// PPTService.Defender —— PPT 小工具的守护（看门狗）进程。
	///
	/// 互守护约定（与主程序 PPTService.DefenderGuardian 对应）：
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
		private const string WatchedExe = "PPTService.exe";
		private const string WatchedProcessName = "PPTService";

		private const string MutexName = "PPTService.Defender.Mutex";

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

		private static readonly string DataDir = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"Seewo", "PPTService", "Defender");

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
					Log($"守护进程已启动（每 {CheckIntervalMs}ms 巡检主程序，1s 内拉起），目标：{exePath}");
					// 初始化完成：删除启动标记。此后被强杀不再算「首发崩溃」。
					TryDelete(BootMarkerPath);
					Run(exePath);
				}
				catch (Exception ex)
				{
					Log("守护循环异常：" + ex);
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
					Log($"检测到守护进程上次启动未完成即崩溃（第 {crashes} 次）。");
					if (crashes >= MaxBootCrashes)
					{
						Log($"连续 {MaxBootCrashes} 次启动崩溃，停止自启以避免死循环。");
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
				Log("自检阶段异常：" + ex);
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
						Log("目标程序不存在，跳过本次拉起。");
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
						Log($"主程序连续 {MaxRestartStorm} 次拉起后快速退出，停止拉起以避免重启风暴。");
						return;
					}
					Log($"主程序不在运行，尝试重新启动（第 {restartCount} 次）……");
					// 不带 Restart 参数：主程序单实例互斥体会自动去掉重复实例，
					// 避免与 PostErrorAndShutdown 里的 Restart.PPTService() 抢出双份。
					Process.Start(new ProcessStartInfo(exePath)
					{
						UseShellExecute = false,
						WorkingDirectory = BaseDirectory
					});
					lastRestart = DateTime.Now;
					Log("已发出启动请求。");
				}
				catch (Exception ex)
				{
					Log("巡检异常：" + ex.Message);
				}
			}
		}

		/// <summary>
		/// 每轮重新枚举进程名，不缓存也不跨轮持有 Process 对象。
		/// 精确比对进程名，避免把 PPTService.Defender 自己算成主程序。
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
