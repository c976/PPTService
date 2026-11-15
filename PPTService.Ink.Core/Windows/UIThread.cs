using System;
using System.Threading;
using System.Windows.Threading;

namespace PPTService.Ink.Core.Windows
{
	public class UIThread
	{
		private AutoResetEvent _eventObj = new AutoResetEvent(false);

		public Action<UIThread> OnInitialized { get; set; }

		public Action<UIThread, DispatcherUnhandledExceptionEventArgs> OnUnhandledException { get; set; }

		public bool Initialized => Dispatcher != null;

		public Dispatcher Dispatcher { get; private set; }

		public static UIThread Start()
		{
			UIThread uiThreand = new UIThread();
			Thread thread = new Thread(() =>
			{
				try
				{
					uiThreand.Initialize(Dispatcher.CurrentDispatcher);
					Dispatcher.PushFrame(new DispatcherFrame());
				}
				catch (Exception)
				{
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start();
			return uiThreand;
		}

		public void Invoke(Action action)
		{
			CheckInit();
			Dispatcher.Invoke(action);
		}

		public void BeginInvoke(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			CheckInit();
			Dispatcher.BeginInvoke(priority, action);
		}

		public void Shutdown()
		{
			CheckInit();
			Dispatcher.InvokeShutdown();
		}

		private void CheckInit()
		{
			if (!Initialized)
			{
				_eventObj?.WaitOne();
			}
		}

		private UIThread()
		{
		}

		private void Initialize(Dispatcher dispatcher)
		{
			Dispatcher = dispatcher;
			dispatcher.UnhandledException += DispatcherOnUnhandledException;
			dispatcher.BeginInvoke((Action)(() =>
			{
				Action<UIThread> onInitialized = OnInitialized;
				try
				{
					onInitialized?.Invoke(this);
				}
				finally
				{
					_eventObj.Set();
					_eventObj.Dispose();
					_eventObj = null;
				}
			}));
		}

		private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			OnUnhandledException?.Invoke(this, e);
		}
	}
}
