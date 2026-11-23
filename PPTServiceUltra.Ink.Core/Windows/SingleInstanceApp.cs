using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public class SingleInstanceApp : Application
	{
		private NamedPipeServerStream _server;

		private readonly string _uniqueAppName;

		private const string CommandLineArgsSeperator = " O(∩_∩)O~ ";

		private SingleInstanceOptions Options { get; }

		public bool ActivatedOnOtherInstanceStartingUp { get; set; }

		internal bool HasToExit { get; private set; }

		public event EventHandler<OtherInstanceStartedUpEventArgs> OtherInstanceStartedUp;

		public SingleInstanceApp(string uniqueAppName)
			: this(uniqueAppName, false, SingleInstanceOptions.None)
		{
		}

		public SingleInstanceApp(string uniqueAppName, SingleInstanceOptions options)
			: this(uniqueAppName, false, options)
		{
		}

		internal SingleInstanceApp(string uniqueAppName, bool supressExit, SingleInstanceOptions options)
		{
			Options = options;
			if (uniqueAppName == null)
			{
				throw new ArgumentNullException("uniqueAppName");
			}
			if (string.IsNullOrWhiteSpace(uniqueAppName))
			{
				throw new ArgumentException("uniqueAppName 不可为空白字符串。");
			}
			_uniqueAppName = uniqueAppName;
			if (!TryStartup())
			{
				try
				{
					HasToExit = true;
					InformTheOtherInstance();
					return;
				}
				catch (Exception)
				{
				}
				finally
				{
					if (!supressExit)
					{
						SafeExit();
					}
				}
			}
			ActivatedOnOtherInstanceStartingUp = true;
			OtherInstanceStartedUp += SingleInstanceApp_OtherInstanceStartedUp;
		}

		private void SafeExit()
		{
			try
			{
				Process.GetCurrentProcess().Kill();
			}
			catch (Exception)
			{
				try
				{
					Environment.Exit(0);
				}
				catch (Exception)
				{
				}
			}
		}

		protected virtual void OnOtherInstanceStartedUp(OtherInstanceStartedUpEventArgs e)
		{
			OtherInstanceStartedUp?.Invoke(this, e);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			_server?.Dispose();
			base.OnExit(e);
		}

		private void SingleInstanceApp_OtherInstanceStartedUp(object sender, OtherInstanceStartedUpEventArgs e)
		{
			if (!ActivatedOnOtherInstanceStartingUp)
			{
				return;
			}
			Window mainWindow = MainWindow;
			if (mainWindow != null)
			{
				if (mainWindow.WindowState == WindowState.Minimized)
				{
					mainWindow.WindowState = WindowState.Maximized;
				}
				bool topmost = mainWindow.Topmost;
				mainWindow.Activate();
				mainWindow.Topmost = true;
				mainWindow.Topmost = topmost;
				mainWindow.Focus();
			}
		}

		private void InformTheOtherInstance()
		{
			NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", _uniqueAppName, PipeDirection.InOut, PipeOptions.Asynchronous);
			using (namedPipeClientStream)
			{
				int timeout = 5000;
				try
				{
					namedPipeClientStream.Connect(timeout);
				}
				catch (TimeoutException)
				{
					return;
				}
				using (StreamReader streamReader = new StreamReader(namedPipeClientStream))
				{
					string s = streamReader.ReadLine();
					int result;
					if ((Options & SingleInstanceOptions.AllowSetForegroundWindow) != 0 && int.TryParse(s, out result))
					{
						AllowSetForegroundWindow(result);
					}
					using (StreamWriter streamWriter = new StreamWriter(namedPipeClientStream))
					{
						string[] commandLineArgs = Environment.GetCommandLineArgs();
						string value = string.Join(" O(∩_∩)O~ ", commandLineArgs);
						try
						{
							streamWriter.WriteLine(value);
						}
						catch (IOException)
						{
						}
					}
				}
			}
		}

		private bool TryStartup()
		{
			try
			{
				SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
				PipeSecurity pipeSecurity = new PipeSecurity();
				if ((object)user == null)
				{
					pipeSecurity.AddAccessRule(new PipeAccessRule("Users", PipeAccessRights.ReadWrite, AccessControlType.Allow));
				}
				else
				{
					pipeSecurity.AddAccessRule(new PipeAccessRule(user, PipeAccessRights.ReadWrite, AccessControlType.Allow));
				}
				NamedPipeServerStream namedPipeServerStream = (_server = new NamedPipeServerStream(_uniqueAppName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 1024, 1024, pipeSecurity));
				namedPipeServerStream.BeginWaitForConnection(OnWaitForConnection, namedPipeServerStream);
				return true;
			}
			catch (IOException)
			{
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				return false;
			}
		}

		private void OnWaitForConnection(IAsyncResult ar)
		{
			string text;
			try
			{
				NamedPipeServerStream namedPipeServerStream = (NamedPipeServerStream)ar.AsyncState;
				namedPipeServerStream.EndWaitForConnection(ar);
				StreamWriter streamWriter = new StreamWriter(namedPipeServerStream);
				streamWriter.WriteLine(Process.GetCurrentProcess().Id.ToString());
				streamWriter.Flush();
				text = new StreamReader(namedPipeServerStream).ReadLine();
				namedPipeServerStream.Disconnect();
				namedPipeServerStream.BeginWaitForConnection(OnWaitForConnection, namedPipeServerStream);
			}
			catch (Exception)
			{
				return;
			}
			string[] args;
			if (text == null)
			{
				args = new string[0];
			}
			else
			{
				args = text.Split(new string[1] { " O(∩_∩)O~ " }, StringSplitOptions.RemoveEmptyEntries);
			}
			Dispatcher.BeginInvoke((Action)(() =>
			{
				OnOtherInstanceStartedUp(new OtherInstanceStartedUpEventArgs(args));
			}));
		}

		[DllImport("user32.dll")]
		private static extern bool AllowSetForegroundWindow(int dwProcessId);
	}
}
