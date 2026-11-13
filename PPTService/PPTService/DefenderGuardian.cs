using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;

namespace PPTService
{
	/// <summary>
	/// 主程序侧的守护进程「看护」：
	///  - 启动时立即拉起 PPTService.Defender（调试模式不启用）；
	///  - 每 2s 巡检一次，发现守护进程不在就重新拉起（主程序守护守护进程）；
	///  - 守护进程侧每 4s 巡检主程序，主程序崩溃时由它重新拉起（见 PPTService.Defender）。
	///
	/// 互守护不设计进程间优雅退出握手：正常退出时直接结束掉两个进程
	/// （StopAndKill），卸载/升级时由安装脚本同时结束两个进程（见 setup/PPTService.nsi）。
	/// </summary>
	public static class DefenderGuardian
	{
		private const string DefenderExeName = "PPTService.Defender.exe";

		private const string DefenderMutexName = "PPTService.Defender.Mutex";

		/// <summary>主程序巡检守护进程的间隔（毫秒）。</summary>
		private const int CheckIntervalMs = 2000;

		private static DispatcherTimer _timer;
		private static int _checking;

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
			if (DebugMode.IsEnabled || _timer != null)
			{
				return;
			}
			EnsureDefenderRunning();
			_timer = new DispatcherTimer(DispatcherPriority.Background)
			{
				Interval = TimeSpan.FromMilliseconds(CheckIntervalMs)
			};
			_timer.Tick += delegate { EnsureDefenderRunning(); };
			_timer.Start();
		}

		/// <summary>
		/// 程序正常退出：停止巡检并直接结束守护进程，避免它 4s 后把主程序拉回来。
		/// 调试模式直接返回，防止误杀同时运行的正式实例。
		/// </summary>
		public static void StopAndKill()
		{
			if (DebugMode.IsEnabled)
			{
				return;
			}
			DispatcherTimer timer = _timer;
			_timer = null;
			if (timer != null)
			{
				timer.Stop();
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
				if (DebugMode.IsEnabled || !File.Exists(DefenderPath) || IsDefenderRunning())
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
