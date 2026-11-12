using System;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Cvte.Paint.Inks_.System_.Windows_.Input.StylusPlugIns
{
	internal class DynamicRendererThreadManager : IWeakEventListener, IDisposable
	{
		private class DynamicRendererThreadManagerWorker
		{
			private Dispatcher _dispatcher;

			private AutoResetEvent _startupCompleted;

			internal DynamicRendererThreadManagerWorker()
			{
			}

			[SecurityCritical]
			internal Dispatcher StartUpAndReturnDispatcher()
			{
				_startupCompleted = new AutoResetEvent(false);
				Thread thread = new Thread(InkingThreadProc);
				thread.SetApartmentState(ApartmentState.STA);
				thread.IsBackground = true;
				thread.Start();
				_startupCompleted.WaitOne();
				_startupCompleted.Close();
				_startupCompleted = null;
				return _dispatcher;
			}

			[SecurityCritical]
			public void InkingThreadProc()
			{
				Thread.CurrentThread.Name = "DynamicRenderer";
				_dispatcher = Dispatcher.CurrentDispatcher;
				_startupCompleted.Set();
				Dispatcher.Run();
			}
		}

		[ThreadStatic]
		private static WeakReference _tsDRTMWeakRef;

		private volatile Dispatcher __inkingDispatcher;

		private bool _disposed;

		internal Dispatcher ThreadDispatcher => __inkingDispatcher;

		internal static DynamicRendererThreadManager GetCurrentThreadInstance()
		{
			if (_tsDRTMWeakRef == null || _tsDRTMWeakRef.Target == null)
			{
				_tsDRTMWeakRef = new WeakReference(new DynamicRendererThreadManager());
			}
			return _tsDRTMWeakRef.Target as DynamicRendererThreadManager;
		}

		public static Dispatcher GetThreadDispatcher(DynamicRendererThreadManager dynamicRendererThread)
		{
			return dynamicRendererThread.__inkingDispatcher;
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		private DynamicRendererThreadManager()
		{
			DynamicRendererThreadManagerWorker dynamicRendererThreadManagerWorker = new DynamicRendererThreadManagerWorker();
			__inkingDispatcher = dynamicRendererThreadManagerWorker.StartUpAndReturnDispatcher();
			DispatcherShutdownStartedEventManager.AddListener(Dispatcher.CurrentDispatcher, this);
		}

		~DynamicRendererThreadManager()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs args)
		{
			if (managerType == typeof(DispatcherShutdownStartedEventManager))
			{
				OnAppDispatcherShutdown(sender, args);
				return true;
			}
			return false;
		}

		private void OnAppDispatcherShutdown(object sender, EventArgs e)
		{
			__inkingDispatcher?.Invoke(DispatcherPriority.Send, (DispatcherOperationCallback)((object unused) =>
			{
				Dispose();
				return (object)null;
			}), null);
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;
				if (__inkingDispatcher != null && !Environment.HasShutdownStarted)
				{
					try
					{
					}
					finally
					{
						__inkingDispatcher = null;
					}
				}
			}
			GC.KeepAlive(this);
		}
	}
}
