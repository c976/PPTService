using System;

namespace PPTServiceUltra.Ink.Core.Windows.Controls
{
	internal class BackgroundTask
	{
		public string Name { get; }

		public Action Action { get; }

		public TimeSpan DelayTime { get; set; }

		public DateTime LastStartTime { get; set; }

		public BackgroundTask(string name, Action action, TimeSpan delayTime)
		{
			Name = name;
			Action = action;
			DelayTime = delayTime;
			LastStartTime = DateTime.Now;
		}
	}
}
