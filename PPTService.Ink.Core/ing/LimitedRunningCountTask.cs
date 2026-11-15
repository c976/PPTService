using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace dotnetCampus.Threading
{
	
	
	internal class LimitedRunningCountTask
	{
		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass25_0
		{
			
			public List<Task> runningTaskList;

			
			public LimitedRunningCountTask _003C_003E4__this;
		}

		private bool _isRunning;

		private int _runningCount;

		
		private TaskCompletionSource<bool> _runningBreakTask;

		
		private TaskCompletionSource<bool> _waitForFreeTask;

		public int RunningCount
		{
			get
			{
				lock (Locker)
				{
					return _runningCount;
				}
			}
			set
			{
				lock (Locker)
				{
					_runningCount = value;
				}
			}
		}

		public uint MaxRunningCount { get; }

		
		private TaskCompletionSource<bool> RunningBreakTask
		{
			
			get
			{
				lock (Locker)
				{
					return _runningBreakTask;
				}
			}
			
			set
			{
				lock (Locker)
				{
					_runningBreakTask = value;
				}
			}
		}

		
		private TaskCompletionSource<bool> WaitForFreeTask
		{
			
			get
			{
				lock (Locker)
				{
					return _waitForFreeTask;
				}
			}
			
			set
			{
				lock (Locker)
				{
					_waitForFreeTask = value;
				}
			}
		}

		private List<Task> Buffer { get; } = new List<Task>();

		private object Locker => Buffer;

		public LimitedRunningCountTask(uint maxRunningCount)
		{
			MaxRunningCount = maxRunningCount;
		}

		public void Add(Task task)
		{
			RunningCount++;
			lock (Locker)
			{
				Buffer.Add(task);
				RunningBreakTask?.TrySetResult(true);
			}
			RunningInner();
		}

		public async Task AddAsync(Task task)
		{
			Add(task);
			await WaitForFree().ConfigureAwait(false);
		}

		public async Task WaitForFree()
		{
			if (WaitForFreeTask != null)
			{
				await WaitForFreeTask.Task.ConfigureAwait(false);
			}
		}

		private async void RunningInner()
		{
			_003C_003Ec__DisplayClass25_0 obj = default(_003C_003Ec__DisplayClass25_0);
			obj._003C_003E4__this = this;
			if (_isRunning)
			{
				return;
			}
			lock (Locker)
			{
				if (_isRunning)
				{
					return;
				}
				_isRunning = true;
			}
			lock (Locker)
			{
				obj.runningTaskList = Buffer.ToList();
				Buffer.Clear();
				RunningBreakTask = new TaskCompletionSource<bool>();
				obj.runningTaskList.Add(RunningBreakTask.Task);
				_003CRunningInner_003Eg__SetWaitForFreeTask_007C25_1(ref obj);
			}
			while (obj.runningTaskList.Count > 0)
			{
				await Task.WhenAny(obj.runningTaskList).ConfigureAwait(false);
				obj.runningTaskList.RemoveAll((Task task) => task.IsCompleted);
				lock (Locker)
				{
					obj.runningTaskList.AddRange(Buffer);
					Buffer.Clear();
					RunningCount = obj.runningTaskList.Count;
					if (!RunningBreakTask.Task.IsCompleted)
					{
						obj.runningTaskList.Add(RunningBreakTask.Task);
					}
					else
					{
						RunningBreakTask = new TaskCompletionSource<bool>();
						obj.runningTaskList.Add(RunningBreakTask.Task);
					}
					if (obj.runningTaskList.Count < MaxRunningCount)
					{
						WaitForFreeTask?.TrySetResult(true);
					}
					else
					{
						_003CRunningInner_003Eg__SetWaitForFreeTask_007C25_1(ref obj);
					}
				}
			}
			lock (Locker)
			{
				_isRunning = false;
			}
		}

		[CompilerGenerated]
		private void _003CRunningInner_003Eg__SetWaitForFreeTask_007C25_1(ref _003C_003Ec__DisplayClass25_0 P_0)
		{
			if (P_0.runningTaskList.Count > MaxRunningCount && (WaitForFreeTask?.Task.IsCompleted ?? true))
			{
				WaitForFreeTask = new TaskCompletionSource<bool>();
			}
		}
	}
}
