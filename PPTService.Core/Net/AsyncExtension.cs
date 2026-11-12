using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PPTService.Core.Net
{
	internal static class AsyncExtension
	{
		
		public static async Task<TResult> WithTimeout< TResult>(this Task<TResult> task, TimeSpan timeout)
		{
			using (CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource())
			{
				Task task2 = Task.Delay(timeout, timeoutCancellationTokenSource.Token);
				if (await Task.WhenAny(task, task2).ConfigureAwait(false) == task)
				{
					timeoutCancellationTokenSource.Cancel();
					return await task.ConfigureAwait(false);
				}
				throw new WebException("The operation has timed out", WebExceptionStatus.Timeout);
			}
		}
	}
}
