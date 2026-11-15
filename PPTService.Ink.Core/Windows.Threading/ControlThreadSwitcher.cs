using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace PPTService.Ink.Core.Windows.Threading
{
	
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct ControlThreadSwitcher : INotifyCompletion
	{
		private readonly Control _control;

		public bool IsCompleted => !_control.InvokeRequired;

		internal ControlThreadSwitcher(Control control)
		{
			_control = control;
		}

		public ControlThreadSwitcher GetAwaiter()
		{
			return this;
		}

		public void GetResult()
		{
		}

		public void OnCompleted(Action continuation)
		{
			_control.BeginInvoke(continuation);
		}
	}
}
