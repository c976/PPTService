using System;
using System.Security;
using System.Windows;
using System.Windows.Threading;

namespace Cvte.Paint.Inks_.System_.Windows_.Input.StylusPlugIns
{
	internal sealed class DispatcherShutdownStartedEventManager : WeakEventManager
	{
		private static DispatcherShutdownStartedEventManager CurrentManager
		{
			[SecurityCritical]
			get
			{
				Type typeFromHandle = typeof(DispatcherShutdownStartedEventManager);
				DispatcherShutdownStartedEventManager dispatcherShutdownStartedEventManager = (DispatcherShutdownStartedEventManager)WeakEventManager.GetCurrentManager(typeFromHandle);
				if (dispatcherShutdownStartedEventManager == null)
				{
					dispatcherShutdownStartedEventManager = new DispatcherShutdownStartedEventManager();
					WeakEventManager.SetCurrentManager(typeFromHandle, dispatcherShutdownStartedEventManager);
				}
				return dispatcherShutdownStartedEventManager;
			}
		}

		private DispatcherShutdownStartedEventManager()
		{
		}

		[SecurityCritical]
		public static void AddListener(Dispatcher source, IWeakEventListener listener)
		{
			CurrentManager.ProtectedAddListener(source, listener);
		}

		protected override void StartListening(object source)
		{
			((Dispatcher)source).ShutdownStarted += OnShutdownStarted;
		}

		protected override void StopListening(object source)
		{
			((Dispatcher)source).ShutdownStarted -= OnShutdownStarted;
		}

		private void OnShutdownStarted(object sender, EventArgs args)
		{
			DeliverEvent(sender, args);
		}
	}
}
