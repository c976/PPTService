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
				Result = _func(Argument);
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
				lock (_actions)
				{
					return _isExecuting || _actions.Any();
				}
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
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (_dispatcherOfCallBack == null)
			{
				action();
			}
			else
			{
				_dispatcherOfCallBack.BeginInvoke(action);
			}
		}

		public void InvokeOnCallBackThread([NotNull] Action action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (_dispatcherOfCallBack == null)
			{
				action();
			}
			else
			{
				_dispatcherOfCallBack.Invoke(action);
			}
		}

		private void QueueKeyTask(TaskItem task)
		{
			lock (_actions)
			{
				_actions.Clear();
				_actions.Enqueue(task);
				TriggerBackgroundThread();
			}
		}

		private void Queue(TaskItem task)
		{
			lock (_actions)
			{
				_actions.Enqueue(task);
				TriggerBackgroundThread();
			}
		}

		private void TriggerBackgroundThread()
		{
			lock (_actions)
			{
				if (!_actions.Any() || _isExecuting)
				{
					return;
				}
				_isExecuting = true;
				ThreadPool.QueueUserWorkItem((object obj) =>
				{
					TaskItem task;
					lock (_actions)
					{
						task = _actions.Dequeue();
					}
					task.Execute();
					_isExecuting = false;
					BeginInvokeOnCallBackThread(() =>
					{
						OnTaskExcecuted(new TaskExcecutedEventArgs(task.Result, task.Argument, task.Error));
					});
					TriggerBackgroundThread();
				});
			}
		}

		private void OnTaskExcecuted(TaskExcecutedEventArgs e)
		{
			TaskExcecuted?.Invoke(this, e);
		}
	}
}
