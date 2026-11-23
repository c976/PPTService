using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.Threading.Reentrancy
{
	
	
	public abstract class ReentrancyTask< TParameter,  TReturn>
	{
		protected Func<TParameter, Task<TReturn>> WorkingTask { get; }

		protected ReentrancyTask(Func<TParameter, Task<TReturn>> task)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			WorkingTask = task;
		}

		public abstract Task<TReturn> InvokeAsync(TParameter arg);

		protected Task<TReturn> RunCore(TParameter arg)
		{
			return WorkingTask(arg);
		}
	}
}
