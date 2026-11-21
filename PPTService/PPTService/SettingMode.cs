using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Windows.Threading;

namespace PPTService
{
	/// <summary>
	/// 设置调用：以 /settingmode（或 --settingmode / -settingmode）启动本程序时，
	/// 若已有实例在运行，通过本机命名管道通知它打开设置窗口，本实例随即退出；
	/// 若没有实例，则由主实例在初始化完成后自行打开设置窗口。
	///
	/// 通道独立于任何可选插件，使用 Local\ 命名管道并限制为当前用户，避免被其他会话访问。
	/// </summary>
	public static class SettingMode
	{
		/// <summary>是否已按设置模式启动。</summary>
		public static bool IsEnabled { get; private set; }

		/// <summary>命名管道名（Local\ 前缀限定在当前会话）。</summary>
		private const string PipeName = @"Local\PPTServiceUltra.SettingMode";

		/// <summary>客户端发给服务端的固定消息。</summary>
		private const string OpenSettingsMessage = "open-settings";

		private const int ClientConnectTimeoutMs = 3000;

		private static NamedPipeServerStream _server;

		private static readonly object ServerLock = new object();

		public static bool IsSettingModeArg(string arg)
		{
			if (string.IsNullOrEmpty(arg))
			{
				return false;
			}
			string text = arg.Trim();
			return text.StartsWith("/settingmode", StringComparison.OrdinalIgnoreCase)
				|| text.StartsWith("--settingmode", StringComparison.OrdinalIgnoreCase)
				|| text.StartsWith("-settingmode", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>解析命令行，与 DebugMode.Enable 风格一致。</summary>
		public static void Enable(string[] args)
		{
			IsEnabled = args != null && args.Any(IsSettingModeArg);
		}

		/// <summary>
		/// 主实例启动时调用：开一个命名管道服务端，收到消息后在 UI 线程打开设置窗口。
		/// 异常一律吞掉：设置调用失败不应影响主程序正常运行。
		/// </summary>
		public static void StartServer()
		{
			lock (ServerLock)
			{
				if (_server != null)
				{
					return;
				}
				try
				{
					_server = CreateServer();
					_server.BeginWaitForConnection(OnClientConnected, _server);
				}
				catch
				{
					_server = null;
				}
			}
		}

		private static NamedPipeServerStream CreateServer()
		{
			// 仅当前用户可读写，防止同机其他用户伪造请求。
			PipeSecurity security = new PipeSecurity();
			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			if (identity != null && identity.User != null)
			{
				security.AddAccessRule(new PipeAccessRule(identity.User,
					PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow));
			}
			return new NamedPipeServerStream(PipeName, PipeDirection.In, 1,
				PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 1024, 1024, security);
		}

		private static void OnClientConnected(IAsyncResult ar)
		{
			bool openSettings = false;
			NamedPipeServerStream server = ar.AsyncState as NamedPipeServerStream;
			try
			{
				if (server != null)
				{
					server.EndWaitForConnection(ar);
					using (StreamReader reader = new StreamReader(server))
					{
						string message = reader.ReadLine();
						openSettings = string.Equals(message, OpenSettingsMessage, StringComparison.Ordinal);
					}
				}
			}
			catch
			{
			}
			finally
			{
				// 管道是一次性的，处理后立刻重建，等待下一次调用。
				lock (ServerLock)
				{
					_server = null;
				}
				try
				{
					if (server != null)
					{
						server.Close();
						server.Dispose();
					}
				}
				catch
				{
				}
				StartServer();
			}

			if (openSettings)
			{
				ApplicationDispatcherBeginInvoke(new Action(ViewHelper.OpenSettingsWindow));
			}
		}

		/// <summary>
		/// settingmode 第二实例调用：通知已运行的实例打开设置窗口。
		/// 返回是否成功送达（失败时主实例会按配置自行打开或用户可手动打开）。
		/// </summary>
		public static bool NotifyRunningInstance()
		{
			try
			{
				using (NamedPipeClientStream client = new NamedPipeClientStream(".",
					PipeName, PipeDirection.Out))
				{
					client.Connect(ClientConnectTimeoutMs);
					using (StreamWriter writer = new StreamWriter(client))
					{
						writer.WriteLine(OpenSettingsMessage);
						writer.Flush();
					}
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		/// <summary>用 Dispatcher.BeginInvoke 投递到 UI 线程，避免跨线程操作窗口。</summary>
		private static void ApplicationDispatcherBeginInvoke(Action action)
		{
			Dispatcher dispatcher = System.Windows.Application.Current != null
				? System.Windows.Application.Current.Dispatcher
				: null;
			if (dispatcher != null)
			{
				dispatcher.BeginInvoke(action);
			}
		}
	}
}
