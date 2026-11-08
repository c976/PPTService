using System;
using System.Windows;
using System.Windows.Threading;

namespace PPTService
{
	public static class SyntacticSugar
	{
		public static void TryCatchAction(Action tryAction, Action finallyAction = null)
		{
			try
			{
				tryAction();
			}
			catch (Exception ex)
			{
				Log.WriteError(ex.ToString());
			}
			finally
			{
				finallyAction?.Invoke();
			}
		}

		public static void CurrentDispatcherInvoke(Action action)
		{
			Application.Current.Dispatcher.Invoke(action.Invoke);
		}

		public static void CurrentDispatcherBeginInvoke(Action action)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(action.Invoke));
		}

		public static void CurrentDispatcherBeginInvoke(DispatcherPriority dispatcherPriority, Action action)
		{
			Application.Current.Dispatcher.BeginInvoke(dispatcherPriority, new Action(action.Invoke));
		}
	}
}
