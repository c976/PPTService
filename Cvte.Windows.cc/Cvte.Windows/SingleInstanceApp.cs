using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows;

namespace Cvte.Windows
{
	public class SingleInstanceApp : Application
	{
		private const string CommandLineArgsSeperator = " O(∩_∩)O~ ";

		private EventHandler<OtherInstanceStartedUpEventArgs> m_OtherInstanceStartedUp;

		private readonly string _uniqueAppName;

		public bool ActivatedOnOtherInstanceStartingUp { get; set; }

		internal bool HasToExit { get; private set; }

		public event EventHandler<OtherInstanceStartedUpEventArgs> OtherInstanceStartedUp
		{
			add
			{
				EventHandler<OtherInstanceStartedUpEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_OtherInstanceStartedUp;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<OtherInstanceStartedUpEventArgs> eventHandler2 = default(EventHandler<OtherInstanceStartedUpEventArgs>);
				EventHandler<OtherInstanceStartedUpEventArgs> value2 = (EventHandler<OtherInstanceStartedUpEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_OtherInstanceStartedUp, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<OtherInstanceStartedUpEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
			remove
			{
				EventHandler<OtherInstanceStartedUpEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_OtherInstanceStartedUp;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<OtherInstanceStartedUpEventArgs> eventHandler2 = default(EventHandler<OtherInstanceStartedUpEventArgs>);
				EventHandler<OtherInstanceStartedUpEventArgs> value2 = (EventHandler<OtherInstanceStartedUpEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_OtherInstanceStartedUp, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<OtherInstanceStartedUpEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
		}

		public SingleInstanceApp(string uniqueAppName)
			: this(uniqueAppName, false)
		{
		}

		internal SingleInstanceApp(string uniqueAppName, bool supressExit)
		{
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
						Environment.Exit(0);
					}
				}
			}
			ActivatedOnOtherInstanceStartingUp = true;
			OtherInstanceStartedUp += SingleInstanceApp_OtherInstanceStartedUp;
		}

		protected virtual void OnOtherInstanceStartedUp(OtherInstanceStartedUpEventArgs e)
		{
			this.m_OtherInstanceStartedUp?.Invoke(this, e);
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
			string serverName = ".";
			NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(serverName, _uniqueAppName, PipeDirection.Out, PipeOptions.Asynchronous);
			int timeout = 5000;
			try
			{
				if (0 == 0)
				{
					namedPipeClientStream.Connect(timeout);
				}
			}
			catch (TimeoutException)
			{
				return;
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

		private bool TryStartup()
		{
			try
			{
				NamedPipeServerStream namedPipeServerStream = new NamedPipeServerStream(_uniqueAppName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
				NamedPipeServerStream namedPipeServerStream2 = default(NamedPipeServerStream);
				if (0 == 0)
				{
					namedPipeServerStream2 = namedPipeServerStream;
				}
				namedPipeServerStream2.BeginWaitForConnection(OnWaitForConnection, namedPipeServerStream2);
				return true;
			}
			catch (IOException)
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
				StreamReader streamReader = new StreamReader(namedPipeServerStream);
				text = streamReader.ReadLine();
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
	}
}
