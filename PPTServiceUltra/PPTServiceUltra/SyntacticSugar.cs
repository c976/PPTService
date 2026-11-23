using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace PPTServiceUltra
{
	public static class SyntacticSugar
	{
		// PowerPoint 忙 / 放映窗口正在销毁时 COM 会返回这些瞬时错误。
		// 300ms 的界面定时器会反复打到它们，过去每次都写 error.txt，
		// 既淹没真正的错误，也让 UI 线程被同步文件 IO 拖慢（启动变慢）。
		private const int RpcECallRejected = unchecked((int)0x80010001);
		private const int RpcEServerCallRetryLater = unchecked((int)0x8001010A);
		private const int RpcEUnavailable = unchecked((int)0x800706BA);
		private const int PptObjectDoesNotExist = unchecked((int)0x80048010);

		private static DateTime _comBusyUntil = DateTime.MinValue;

		/// <summary>最近一次 COM 调用被 PPT 拒绝，短时间内定时器应跳过 COM 探测。</summary>
		public static bool IsComBusy
		{
			get { return DateTime.Now < _comBusyUntil; }
		}

		public static bool IsTransientComException(Exception ex)
		{
			COMException com = ex as COMException;
			if (com == null)
			{
				return false;
			}
			int hr = com.HResult;
			return hr == RpcECallRejected || hr == RpcEServerCallRetryLater ||
				hr == RpcEUnavailable || hr == PptObjectDoesNotExist;
		}

		/// <summary>周期性 COM 探测专用：瞬时错误不写日志，并短暂退避。</summary>
		public static void TryComAction(Action tryAction, Action finallyAction = null)
		{
			try
			{
				tryAction();
			}
			catch (Exception ex)
			{
				if (IsTransientComException(ex))
				{
					_comBusyUntil = DateTime.Now.AddMilliseconds(1500.0);
				}
				else
				{
					Log.WriteError(ex.ToString());
				}
			}
			finally
			{
				finallyAction?.Invoke();
			}
		}

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
