using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Windows.Threading
{
	public class BackgroundTaskQueue
	{
		private class TaskItem
		{
			private readonly Func<object, object> _func;

			public object Result { get; private set; }

			public object Argument { get; private set; }

			public bool IsExecuted { get; private set; }

			public Exception Error { get; private set; }

			public void Execute()
			{
				if (IsExecuted)
				{
					throw new InvalidOperationException("已经执行过此TaskItem。");
				}
				try
				{
					do
					{
						Result = _func(Argument);
					}
					while (false);
				}
				catch (Exception error)
				{
					if (7 != 0)
					{
						Error = error;
					}
				}
				IsExecuted = true;
			}

			[DebuggerStepThrough]
			public TaskItem(Func<object, object> func, object argument)
			{
				if (func == null)
				{
					throw new ArgumentNullException("func");
				}
				Argument = argument;
				_func = func;
			}

			[DebuggerStepThrough]
			public TaskItem(Func<object> func, object argument)
			{
				if (func == null)
				{
					throw new ArgumentNullException("func");
				}
				Argument = argument;
				_func = (object arg) => func();
			}

			[DebuggerStepThrough]
			public TaskItem(Action<object> func)
			{
				if (func == null)
				{
					throw new ArgumentNullException("func");
				}
				_func = (object arg) =>
				{
					func(arg);
					return (object)null;
				};
			}

			[DebuggerStepThrough]
			public TaskItem(Action func)
			{
				if (func == null)
				{
					throw new ArgumentNullException("func");
				}
				_func = (object arg) =>
				{
					func();
					return (object)null;
				};
			}
		}

		public EventHandler<TaskExcecutedEventArgs> TaskExcecuted;

		private readonly Queue<TaskItem> _actions = new Queue<TaskItem>();

		private readonly Dispatcher _dispatcherOfCallBack;

		private bool _isExecuting;

		public bool IsWorking
		{
			get
			{
				int num = 0;
				Queue<TaskItem> actions = default(Queue<TaskItem>);
				do
				{
					bool lockTaken = (byte)num != 0;
					bool flag;
					try
					{
						System.Threading.Monitor.Enter(actions = _actions, ref lockTaken);
						bool num2 = _isExecuting;
						do
						{
							num2 = num2 || _actions.Any();
						}
						while (false);
						flag = num2;
					}
					finally
					{
						do
						{
							if (lockTaken)
							{
								System.Threading.Monitor.Exit(actions);
							}
						}
						while (false);
					}
					num = (flag ? 1 : 0);
				}
				while (false ? true : false);
				return (byte)num != 0;
			}
		}

		public BackgroundTaskQueue(Dispatcher dispatcherOfCallBackOfCallBack)
		{
			_dispatcherOfCallBack = dispatcherOfCallBackOfCallBack;
		}

		public BackgroundTaskQueue()
			: this(Dispatcher.CurrentDispatcher)
		{
		}

		public void QueueKeyTask([NotNull] Func<object, object> taskFunc, object argument)
		{
			QueueKeyTask(new TaskItem(taskFunc, argument));
		}

		public void QueueKeyTask([NotNull] Func<object> taskFunc, object argument)
		{
			QueueKeyTask(new TaskItem(taskFunc, argument));
		}

		public void QueueKeyTask([NotNull] Action<object> taskAction)
		{
			QueueKeyTask(new TaskItem(taskAction));
		}

		public void QueueKeyTask([NotNull] Action taskAction)
		{
			QueueKeyTask(new TaskItem(taskAction));
		}

		public void Queue([NotNull] Func<object, object> taskFunc, object argument)
		{
			Queue(new TaskItem(taskFunc, argument));
		}

		public void Queue([NotNull] Func<object> taskFunc, object argument)
		{
			Queue(new TaskItem(taskFunc, argument));
		}

		public void Queue([NotNull] Action<object> taskAction)
		{
			Queue(new TaskItem(taskAction));
		}

		public void Queue([NotNull] Action taskAction)
		{
			Queue(new TaskItem(taskAction));
		}

		public void BeginInvokeOnCallBackThread([NotNull] Action action)
		{
			if (1 == 0)
			{
				return;
			}
			if (action != null)
			{
				if (0 == 0 && _dispatcherOfCallBack == null)
				{
					action();
					return;
				}
				_dispatcherOfCallBack.BeginInvoke(action);
				if (0 == 0)
				{
					return;
				}
			}
			throw new ArgumentNullException("action");
		}

		public void InvokeOnCallBackThread([NotNull] Action action)
		{
			while (true)
			{
				if (action == null)
				{
					if (5 == 0 || 8 == 0)
					{
						continue;
					}
					throw new ArgumentNullException("action");
				}
				if (_dispatcherOfCallBack == null)
				{
					if (3 != 0)
					{
						if (false)
						{
							continue;
						}
						action();
						return;
					}
					return;
				}
				break;
			}
			_dispatcherOfCallBack.Invoke(action);
		}

		private void QueueKeyTask(TaskItem task)
		{
			bool lockTaken = default(bool);
			Queue<TaskItem> actions = default(Queue<TaskItem>);
			do
			{
				if (false)
				{
					continue;
				}
				if (0 == 0)
				{
					lockTaken = false;
				}
				try
				{
					System.Threading.Monitor.Enter(actions = _actions, ref lockTaken);
					do
					{
						_actions.Clear();
						_actions.Enqueue(task);
					}
					while (1 == 0);
					TriggerBackgroundThread();
				}
				finally
				{
					if ((7 == 0 || lockTaken) && -1 != 0)
					{
						System.Threading.Monitor.Exit(actions);
					}
				}
			}
			while (false);
		}

		private void Queue(TaskItem task)
		{
			bool lockTaken;
			if (4u != 0)
			{
				lockTaken = false;
			}
			Queue<TaskItem> actions = default(Queue<TaskItem>);
			try
			{
				while (true)
				{
					System.Threading.Monitor.Enter(actions = _actions, ref lockTaken);
					if (0 == 0)
					{
						Queue<TaskItem> actions2 = _actions;
						if (0 == 0)
						{
							actions2.Enqueue(task);
						}
						goto IL_0023;
					}
					goto IL_002a;
					IL_002a:
					if (0 == 0 && 0 == 0)
					{
						break;
					}
					goto IL_0023;
					IL_0023:
					if (7 == 0)
					{
						continue;
					}
					TriggerBackgroundThread();
					goto IL_002a;
				}
			}
			finally
			{
				if (lockTaken)
				{
					System.Threading.Monitor.Exit(actions);
				}
			}
		}

		private void TriggerBackgroundThread()
		{
			if (4 == 0)
			{
				return;
			}
			bool lockTaken = false;
			Queue<TaskItem> actions = default(Queue<TaskItem>);
			try
			{
				System.Threading.Monitor.Enter(actions = _actions, ref lockTaken);
				if (!_actions.Any() || _isExecuting)
				{
					return;
				}
				_isExecuting = true;
				ThreadPool.QueueUserWorkItem((object obj) =>
				{
					if (0 == 0)
					{
						bool lockTaken2 = false;
						Queue<TaskItem> actions2 = default(Queue<TaskItem>);
						TaskItem task = default(TaskItem);
						try
						{
							System.Threading.Monitor.Enter(actions2 = _actions, ref lockTaken2);
							if (4 != 0)
							{
								task = _actions.Dequeue();
							}
						}
						finally
						{
							if (lockTaken2 && 0 == 0 && 0 == 0)
							{
								System.Threading.Monitor.Exit(actions2);
							}
						}
						task.Execute();
						_isExecuting = false;
						BeginInvokeOnCallBackThread(() =>
						{
							if (4 != 0)
							{
								OnTaskExcecuted(new TaskExcecutedEventArgs(task.Result, task.Argument, task.Error));
							}
						});
					}
					TriggerBackgroundThread();
				});
			}
			finally
			{
				if (lockTaken && 0 == 0 && 0 == 0)
				{
					System.Threading.Monitor.Exit(actions);
				}
			}
		}

		private void OnTaskExcecuted(TaskExcecutedEventArgs e)
		{
			TaskExcecuted?.Invoke(this, e);
		}
	}
}
