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
			if (!createdNew && (!args.Args.Any() || args.Args.First() != AppRestart))
			{
				Environment.Exit(0);
			}
		}

		private void StartProcess()
		{
			AppPath.ClearTempFolder();
			ConfigHelper.Instance = new ConfigHelper();
			_pptOperator = new PptOperator();
			PptOperator.Instance = _pptOperator;
			_pptOperator.Initialize();
			MouseHelper.Instance = new MouseHelper();
			new ViewHelper().Initialize();
			Version = GetAttribute<AssemblyFileVersionAttribute>().Version;
			FridayReporter.Initialize(AppName, Version);
			CatchUnhandledException();
		}

		private void CatchUnhandledException()
		{
			DispatcherUnhandledException += (object s, DispatcherUnhandledExceptionEventArgs e) =>
			{
				PostErrorAndShutdown(new UnhandledExceptionEventArgs(e, false));
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
