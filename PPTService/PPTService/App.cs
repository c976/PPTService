using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using PPTService.Collection;

namespace PPTService
{
	public class App : Application
	{
		public static string Version;

		private static string AppRestart = "Restart";

		public static readonly string AppName = "PPTService";

		private static string AppMutex = "#P_#P_#T_&S_&e_&r_*v_*i_*c_$e";

		private PptOperator _pptOperator;

		private static Mutex _mutex;

		// 崩溃退出标记：OnExit 据此保留守护进程，由它拉起唯一的新实例。
		private static volatile bool _crashExiting;

		protected override void OnStartup(StartupEventArgs args)
		{
			// /debug：无需 Office/WPS，直接模拟放映状态下的界面（工具栏 + 翻页 + 批注）。
			DebugMode.Enable(args.Args);
			// /settingmode：通过命令行调用已有实例打开设置窗口（见 SettingMode）。
			SettingMode.Enable(args.Args);
			// 单实例判定尽早执行：原来在 StartProcess() 之后才创建互斥体，
			// 重复启动的实例会先跑完主题/配置/COM 初始化才退出；提前后直接退出，
			// 也不会与守护进程拉起的实例交错形成两个 PPTService.exe。
			bool createdNew;
			_mutex = new Mutex(true, AppMutex, out createdNew);
			if (!createdNew)
			{
				// 已有实例：设置模式把请求转交给它（命名管道），其余重复启动直接退出。
				// 注意必须放在 allowSecondInstance 之前，否则 /settingmode 会被当成普通第二实例。
				if (SettingMode.IsEnabled)
				{
					SettingMode.NotifyRunningInstance();
					Environment.Exit(0);
					return;
				}
				// 调试模式允许与正式实例并存，方便对照测试；
				// Restart 参数用于取消加载后的主动重启（旧实例随后 Environment.Exit）。
				bool allowSecondInstance = DebugMode.IsEnabled ||
					(args.Args.Any() && args.Args.First() == AppRestart);
				if (!allowSecondInstance)
				{
					Environment.Exit(0);
					return;
				}
			}
			base.OnStartup(args);
			// 主实例立即开启设置调用通道：第二实例可能在 StartProcess() 完成前就发起通知，
			// 尽早监听可避免这段窗口期连接失败（回调由 Dispatcher 投递，仍会等 UI 就绪）。
			SettingMode.StartServer();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			StartProcess();
			stopwatch.Stop();
			// 冷启动（此前没有实例）时：初始化完成后按参数打开设置窗口。
			if (SettingMode.IsEnabled)
			{
				SyntacticSugar.CurrentDispatcherBeginInvoke(new Action(ViewHelper.OpenSettingsWindow));
			}
			DispatcherTask.Start(6000.0, () =>
			{
				SyntacticSugar.TryCatchAction(() =>
				{
					string eventValue = stopwatch.ElapsedMilliseconds.ConvertMilliseconds();
					FridayReporter.ReportEventDelayed(EventID.StartTime, eventValue);
				});
			});
			// 启动互守护：主程序每 1s 看护守护进程（2s 内拉起），
			// 守护进程每 0.5s 看护主程序（1s 内拉起）。调试模式不启用。
			// 卸载/升级时由安装脚本同时结束两个进程。
			DefenderGuardian.Start();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			// 正常退出：不做优雅握手，直接把守护进程一并结束掉，
			// 否则它会在 0.5s 内把主程序重新拉起。
			// 崩溃退出例外：保留守护进程，由它拉起唯一的新实例。
			if (!_crashExiting)
			{
				DefenderGuardian.StopAndKill();
			}
			base.OnExit(e);
		}

		private void StartProcess()
		{
			AppPath.ClearTempFolder();
			// 在任何窗口 XAML 解析之前建立主题画刷，保证 DynamicResource 首次就能取到配色。
			// 主题只是界面增强，且此处尚未挂载全局异常处理：任何异常都必须降级为
			// 原版浅色外观，绝不能让主程序连主界面都出不来。
			try
			{
				ThemeManager.Apply(UserConfigManager.Load());
			}
			catch (Exception ex)
			{
				Debug.WriteLine("ThemeManager.Apply failed: " + ex);
			}
			ConfigHelper.Instance = new ConfigHelper();
			_pptOperator = new PptOperator();
			PptOperator.Instance = _pptOperator;
			if (DebugMode.IsEnabled)
			{
				DebugMode.CreateSlideWindow();
			}
			_pptOperator.Initialize();
			MouseHelper.Instance = new MouseHelper();
			new ViewHelper().Initialize();
			Version = GetAttribute<AssemblyFileVersionAttribute>().Version;
			if (!DebugMode.IsEnabled)
			{
				FridayReporter.Initialize(AppName, Version);
			}
			CatchUnhandledException();
		}

		private void CatchUnhandledException()
		{
			DispatcherUnhandledException += (object s, DispatcherUnhandledExceptionEventArgs e) =>
			{
				// 记录真正的异常（原来误把 EventArgs 传进去，日志里只剩类型名）。
				PostErrorAndShutdown(new UnhandledExceptionEventArgs(e.Exception, false));
			};
			AppDomain.CurrentDomain.UnhandledException += (object s, UnhandledExceptionEventArgs e) =>
			{
				PostErrorAndShutdown(e);
			};
			Dispatcher.CurrentDispatcher.UnhandledException += (object s, DispatcherUnhandledExceptionEventArgs e) =>
			{
				PostErrorAndShutdown(new UnhandledExceptionEventArgs(e, false));
			};
		}

		private void PostErrorAndShutdown(UnhandledExceptionEventArgs args)
		{
			Log.WriteError(args.ExceptionObject.ToString());
			// 崩溃后不再自己 Process.Start 新实例：由守护进程（每 0.5s 巡检）
			// 拉起唯一的新实例，避免与自己拉起的实例竞争出现两个 PPTService.exe。
			_crashExiting = true;
			Environment.Exit(0);
		}

		private static T GetAttribute<T>() where T : Attribute
		{
			return (T)Assembly.GetAssembly(Application.Current.GetType()).GetCustomAttributes(typeof(T), false)[0];
		}

		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public static void Main()
		{
			// 本程序是常驻后台服务：平时没有任何窗口，工具栏/设置窗口只是临时 UI。
			// 若不显式指定，WPF 默认 OnLastWindowClose，关闭「唯一窗口」（如 /settingmode
			// 冷启动时打开的设置窗口）就会触发 Shutdown → OnExit → StopAndKill 杀掉守护进程
			// 并退出整个服务。改为 OnExplicitShutdown：窗口关闭不再结束进程，退出只走
			// Environment.Exit / 看门狗 / 安装器等显式路径（OnExit 仍会在真正 Shutdown 时触发）。
			App app = new App();
			app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			app.Run();
		}
	}
}
