using System;

namespace PPTService.Ink.Core.Threading
{
	public sealed class PartialRetryContext
	{
		private int _stepCount = 1;

		public int StepCount
		{
			get
			{
				return _stepCount;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("次数必须大于或等于 1。", "value");
				}
				_stepCount = value;
			}
		}
	}
}
