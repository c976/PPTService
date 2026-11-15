using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.Threading
{
	
	
	public class AwaitableTask
	{
		
		public struct TaskAwaiter : INotifyCompletion
		{
			private readonly AwaitableTask _awaitableTask;

			public bool IsCompleted => _awaitableTask._task.IsCompleted;

			public TaskAwaiter(AwaitableTask awaitableTask)
			{
				_awaitableTask = awaitableTask;
			}

			public void OnCompleted(Action continuation)
			{
				TaskAwaiter This = this;
				_awaitableTask._task.ContinueWith( (Task t) =>
				{
					if (This._awaitableTask.Executable)
					{
						continuation?.Invoke();
					}
				});
			}

			public void GetResult()
			{
				_awaitableTask._task.Wait();
			}
		}

		private readonly Task _task;

		public bool Executable { get; private set; } = true;

		public bool IsValid { get; private set; } = true;

		public bool IsCompleted => _task.IsCompleted;

		public int TaskId => _task.Id;

		public void SetNotExecutable()
		{
			Executable = false;
		}

		public void MarkTaskInvalid()
		{
			IsValid = false;
		}

		public AwaitableTask(Task task)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			_task = task;
		}

		public void Start()
		{
			_task.Start();
		}

		public void RunSynchronously()
		{
			_task.RunSynchronously();
		}

		public TaskAwaiter GetAwaiter()
		{
			return new TaskAwaiter(this);
		}
	}
	
	
	public class AwaitableTask< TResult> : AwaitableTask
	{
		
		public new struct TaskAwaiter : INotifyCompletion
		{
			private readonly AwaitableTask<TResult> _awaitableTask;

			public bool IsCompleted => _awaitableTask._task.IsCompleted;

			public TaskAwaiter(AwaitableTask<TResult> awaitableTask)
			{
				_awaitableTask = awaitableTask;
			}

			public void OnCompleted(Action continuation)
			{
				TaskAwaiter This = this;
				_awaitableTask._task.ContinueWith(( Task<TResult> t) =>
				{
					if (This._awaitableTask.Executable)
					{
						continuation?.Invoke();
					}
				});
			}

			public TResult GetResult()
			{
				return _awaitableTask._task.Result;
			}
		}

		private readonly Task<TResult> _task;

		public AwaitableTask(Task<TResult> task)
			: base(task)
		{
			_task = task;
		}

		
		public new TaskAwaiter GetAwaiter()
		{
			return new TaskAwaiter(this);
		}
	}
}
