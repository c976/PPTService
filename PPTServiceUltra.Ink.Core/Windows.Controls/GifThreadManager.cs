using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PPTServiceUltra.Ink.Core.Windows.Controls
{
	internal class GifThreadManager
	{
		private readonly object _obj = new object();

		private Thread _thread;

		public static GifThreadManager Instance { get; } = new GifThreadManager();

		private bool BackgroundTaskListChanged { get; set; }

		private TimeSpan MaxDelayTime { get; } = TimeSpan.FromSeconds(0.5);

		private TimeSpan MinDelayTime { get; } = TimeSpan.FromMilliseconds(5.0);

		private TimeSpan[] WaitDelayTimeList { get; } = new TimeSpan[12]
		{
			TimeSpan.FromMilliseconds(5.0),
			TimeSpan.FromMilliseconds(10.0),
			TimeSpan.FromMilliseconds(20.0),
			TimeSpan.FromMilliseconds(50.0),
			TimeSpan.FromMilliseconds(100.0),
			TimeSpan.FromMilliseconds(100.0),
			TimeSpan.FromMilliseconds(100.0),
			TimeSpan.FromMilliseconds(200.0),
			TimeSpan.FromMilliseconds(200.0),
			TimeSpan.FromMilliseconds(200.0),
			TimeSpan.FromMilliseconds(200.0),
			TimeSpan.FromMilliseconds(200.0)
		};

		private List<BackgroundTask> BackgroundTaskList { get; } = new List<BackgroundTask>();

		public bool ContainsBackgroundTask(BackgroundTask task)
		{
			lock (_obj)
			{
				return BackgroundTaskList.Contains(task);
			}
		}

		public void RunBackgroundTask(BackgroundTask task)
		{
			lock (_obj)
			{
				if (BackgroundTaskList.Contains(task))
				{
					throw new ArgumentException("不能传入重复的任务");
				}
				BackgroundTaskList.Add(task);
				BackgroundTaskListChanged = true;
				Start();
			}
		}

		public void RemoveBackgroundTask(BackgroundTask task)
		{
			lock (_obj)
			{
				BackgroundTaskList.Remove(task);
				BackgroundTaskListChanged = true;
			}
		}

		[Conditional("DEBUG_取消注释这句话调试gif线程")]
		private void Log(string str)
		{
		}

		private void Start()
		{
			lock (_obj)
			{
				if (_thread == null)
				{
					_thread = new Thread(Run)
					{
						IsBackground = true,
						Name = "gif thread"
					};
					_thread.Start();
				}
			}
		}

		private void Run()
		{
			try
			{
				int num = 0;
				List<BackgroundTask> list = new List<BackgroundTask>();
				while (true)
				{
					lock (_obj)
					{
						if (BackgroundTaskListChanged)
						{
							list = BackgroundTaskList.ToList();
							BackgroundTaskListChanged = false;
						}
						else if (num >= 15)
						{
							break;
						}
					}
					if (list.Count == 0)
					{
						if (num < WaitDelayTimeList.Length)
						{
							Thread.Sleep(WaitDelayTimeList[num]);
							num++;
						}
						else
						{
							Thread.Sleep(MaxDelayTime);
							num++;
						}
						continue;
					}
					num = 0;
					TimeSpan timeSpan = TimeSpan.MaxValue;
					foreach (BackgroundTask item in list)
					{
						if (DateTime.Now - item.LastStartTime >= item.DelayTime)
						{
							try
							{
								item.Action();
							}
							catch (Exception)
							{
							}
							item.LastStartTime = DateTime.Now;
							TimeSpan delayTime = item.DelayTime;
							if (delayTime < timeSpan)
							{
								timeSpan = delayTime;
							}
						}
						else
						{
							TimeSpan timeSpan2 = item.DelayTime - (DateTime.Now - item.LastStartTime);
							if (timeSpan2 < timeSpan)
							{
								timeSpan = timeSpan2;
							}
						}
					}
					if (timeSpan > MaxDelayTime)
					{
						timeSpan = MaxDelayTime;
					}
					else if (timeSpan < MinDelayTime)
					{
						timeSpan = MinDelayTime;
					}
					Thread.Sleep(timeSpan);
				}
			}
			finally
			{
				lock (_obj)
				{
					_thread = null;
				}
			}
		}
	}
}
