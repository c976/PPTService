using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace PPTService
{
	/// <summary>
	/// 主程序侧的守护进程「看护」：
	///  - 启动时立即拉起 PPTService.Defender（调试模式不启用）；
	///  - 每 1s 巡检一次，发现守护进程不在就重新拉起，保证「2s 内拉起」；
	///  - 守护进程侧每 0.5s 巡检主程序，主程序崩溃后 1s 内由它重新拉起（见 PPTService.Defender）。
	///
	/// 互守护不设计进程间优雅退出握手：正常退出时直接结束掉两个进程
	/// （StopAndKill），卸载/升级时由安装脚本同时结束两个进程（见 setup/PPTService.nsi）。
	/// </summary>
	public static class DefenderGuardian
	{
		private const string DefenderExeName = "PPTService.Defender.exe";

		private const string DefenderMutexName = "PPTService.Defender.Mutex";

		/// <summary>
		/// 主程序巡检守护进程的间隔（毫秒）。设为 1s：守护进程被结束后最坏 1s 被发现，
		/// 加上进程启动耗时仍可保证「2s 内拉起」。
		/// </summary>
		private const int CheckIntervalMs = 1000;

		// 用线程池定时器而非 DispatcherTimer：PPT COM 调用等耗时操作可能长时间占用 UI 线程，
		// DispatcherTimer 会被拖到 2s 之后才触发，无法保证「2s 内拉起」。
		private static Timer _timer;
		private static int _checking;
		private static volatile bool _stopped;

		private static string BaseDirectory
		{
			get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "."; }
		}

		private static string DefenderPath
		{
			get { return Path.Combine(BaseDirectory, DefenderExeName); }
		}

		/// <summary>由 App.OnStartup 调用。调试模式不启用守护进程。</summary>
		public static void Start()
		{
			if (DebugMode.IsEnabled || _timer != null || _stopped)
			{
				return;
			}
			EnsureDefenderRunning();
			// 先启动一次，之后每 1s 巡检；线程池计时不受 UI 线程阻塞影响。
			_timer = new Timer(delegate { EnsureDefenderRunning(); }, null,
				CheckIntervalMs, CheckIntervalMs);
		}

		/// <summary>
		/// 程序正常退出：停止巡检并直接结束守护进程，避免它 0.5s 内把主程序拉回来。
		/// 调试模式直接返回，防止误杀同时运行的正式实例。
		/// </summary>
		public static void StopAndKill()
		{
			if (DebugMode.IsEnabled)
			{
				return;
			}
			// 先置停止标志：线程池回调可能正在飞行，Dispose 不会等待它结束，
			// 否则退出瞬间可能又把守护进程拉起一次。
			_stopped = true;
			Timer timer = Interlocked.Exchange(ref _timer, null);
			if (timer != null)
			{
				timer.Dispose();
			}
			try
			{
				foreach (Process p in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(DefenderExeName)))
				{
					using (p)
					{
						try
						{
							if (!p.HasExited)
							{
								p.Kill();
							}
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
		}

		private static void EnsureDefenderRunning()
		{
			// 防重入：Process.Start / OpenExisting 偶发较慢，避免叠加
			if (Interlocked.Exchange(ref _checking, 1) != 0)
			{
				return;
			}
			try
			{
				if (_stopped || DebugMode.IsEnabled || !File.Exists(DefenderPath) || IsDefenderRunning())
				{
					return;
				}
				Process.Start(new ProcessStartInfo(DefenderPath)
				{
					UseShellExecute = false,
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden,
					WorkingDirectory = BaseDirectory
				});
			}
			catch
			{
				// 守护进程不可用时不影响主程序运行
			}
			finally
			{
				Interlocked.Exchange(ref _checking, 0);
			}
		}

		/// <summary>
		/// 用守护进程持有的命名互斥体判断是否存活：进程死亡后内核对象自动释放，
		/// 比进程名可靠（不受进程名截断影响）。OpenExisting 不可用时回退到进程名。
		/// </summary>
		private static bool IsDefenderRunning()
		{
			try
			{
				using (Mutex.OpenExisting(DefenderMutexName))
				{
					return true;
				}
			}
			catch (WaitHandleCannotBeOpenedException)
			{
			}
			catch (UnauthorizedAccessException)
			{
				// 对象存在但权限不同，视为存活
				return true;
			}
			catch
			{
			}
			Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(DefenderExeName));
			bool alive = processes.Length > 0;
			foreach (Process p in processes)
			{
				p.Dispose();
			}
			return alive;
		}
	}
}
