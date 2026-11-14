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

		protected override void OnStartup(StartupEventArgs args)
		{
			// /debug：无需 Office/WPS，直接模拟放映状态下的界面（工具栏 + 翻页 + 批注）。
			DebugMode.Enable(args.Args);
			base.OnStartup(args);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			StartProcess();
			stopwatch.Stop();
			DispatcherTask.Start(6000.0, () =>
			{
				SyntacticSugar.TryCatchAction(() =>
				{
					string eventValue = stopwatch.ElapsedMilliseconds.ConvertMilliseconds();
					FridayReporter.ReportEventDelayed(EventID.StartTime, eventValue);
				});
			});
			bool createdNew;
			_mutex = new Mutex(true, AppMutex, out createdNew);
			// 调试模式允许与正式实例并存，方便对照测试。
			bool allowSecondInstance = DebugMode.IsEnabled ||
				(args.Args.Any() && args.Args.First() == AppRestart);
			if (!createdNew && !allowSecondInstance)
			{
				Environment.Exit(0);
			}
			// 启动互守护：主程序每 1s 看护守护进程（2s 内拉起），
			// 守护进程每 0.5s 看护主程序（1s 内拉起）。调试模式不启用。
			// 卸载/升级时由安装脚本同时结束两个进程。
			DefenderGuardian.Start();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			// 正常退出：不做优雅握手，直接把守护进程一并结束掉，
			// 否则它会在 0.5s 内把主程序重新拉起。
			DefenderGuardian.StopAndKill();
			base.OnExit(e);
		}

		private void StartProcess()
		{
			AppPath.ClearTempFolder();
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
			if (ConfigHelper.Instance.IsComOk)
			{
				Restart.PPTService();
			}
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
			new App().Run();
		}
	}
}
