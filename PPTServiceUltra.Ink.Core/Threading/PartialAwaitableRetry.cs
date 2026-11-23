using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.Threading
{
	
	
	public class PartialAwaitableRetry
	{
		private readonly object _locker = new object();

		private readonly Func<PartialRetryContext, Task<OperationResult>> _loopItem;

		private readonly List<CountLimitOperationToken> _tokens = new List<CountLimitOperationToken>();

		private volatile bool _isLooping;

		public PartialAwaitableRetry(Func<PartialRetryContext, Task<OperationResult>> loopItem)
		{
			if (loopItem == null)
			{
				throw new ArgumentNullException("loopItem");
			}
			_loopItem = loopItem;
		}

		public ContinuousPartOperation JoinAsync(int countLimit)
		{
			CountLimitOperationToken countLimitOperationToken = new CountLimitOperationToken(countLimit);
			lock (_locker)
			{
				_tokens.Add(countLimitOperationToken);
				if (!_isLooping)
				{
					Loop();
				}
			}
			return countLimitOperationToken.Operation;
		}

		private async Task Loop()
		{
			_isLooping = true;
			PartialRetryContext context = new PartialRetryContext();
			bool shouldContinue = true;
			try
			{
				while (shouldContinue)
				{
					List<CountLimitOperationToken> snapshot;
					lock (_locker)
					{
						snapshot = _tokens.ToList();
					}
					Exception ex;
					bool flag;
					try
					{
						OperationResult operationResult = await _loopItem(context).ConfigureAwait(false);
						ex = operationResult.Exception;
						flag = operationResult.Success;
					}
					catch (Exception ex2)
					{
						ex = ex2;
						flag = false;
					}
					if (ex != null)
					{
						foreach (CountLimitOperationToken item in snapshot)
						{
							item.UpdateException(ex);
						}
					}
					if (flag)
					{
						foreach (CountLimitOperationToken item2 in snapshot)
						{
							item2.Complete();
						}
						lock (_locker)
						{
							_tokens.RemoveAll((CountLimitOperationToken token) => snapshot.Contains(token));
							shouldContinue = _tokens.Count > 0;
						}
						continue;
					}
					foreach (CountLimitOperationToken item3 in snapshot)
					{
						item3.Pass(context.StepCount);
					}
				}
			}
			finally
			{
				_isLooping = false;
			}
		}
	}
}
