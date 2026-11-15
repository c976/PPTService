using System.Windows.Forms;
using System.Windows.Threading;

namespace PPTService.Ink.Core.Windows.Threading
{
	public static class ThreadSwitcher
	{
		public static DispatcherThreadSwitcher ResumeForegroundAsync(this Dispatcher dispatcher)
		{
			return new DispatcherThreadSwitcher(dispatcher);
		}

		public static ControlThreadSwitcher ResumeForegroundAsync(this Control control)
		{
			return new ControlThreadSwitcher(control);
		}

		public static ThreadPoolThreadSwitcher ResumeBackgroundAsync()
		{
			return default(ThreadPoolThreadSwitcher);
		}

		public static ThreadPoolThreadSwitcher ResumeBackgroundAsync(this Dispatcher _)
		{
			return default(ThreadPoolThreadSwitcher);
		}
	}
}
