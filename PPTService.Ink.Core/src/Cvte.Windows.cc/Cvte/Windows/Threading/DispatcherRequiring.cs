using System;
using System.Windows.Threading;

namespace PPTService.Ink.Core.Windows.Threading
{
	public class DispatcherRequiring : DispatcherObject
	{
		private bool _isTaskRequired;

		private readonly Action _action;

		private readonly DispatcherPriority _priority = DispatcherPriority.Normal;

		public DispatcherRequiring(Action action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			_action = action;
		}

		public DispatcherRequiring(Action action, DispatcherPriority priority)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			_action = action;
			_priority = priority;
		}

		public void Require()
		{
			if (!_isTaskRequired)
			{
				_isTaskRequired = true;
				Dispatcher.InvokeAsync(InvokeAction, _priority);
			}
		}

		public void Invoke(bool withRequire = false)
		{
			_isTaskRequired |= withRequire;
			InvokeAction();
		}

		public void Cancel()
		{
			_isTaskRequired = false;
		}

		private void InvokeAction()
		{
			if (!_isTaskRequired)
			{
				return;
			}
			try
			{
				_action();
			}
			finally
			{
				_isTaskRequired = false;
			}
		}
	}
}
