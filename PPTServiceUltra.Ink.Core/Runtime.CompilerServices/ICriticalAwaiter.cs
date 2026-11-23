using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Runtime.CompilerServices
{
	public interface ICriticalAwaiter : IAwaiter, INotifyCompletion, ICriticalNotifyCompletion
	{
	}
	public interface ICriticalAwaiter< out TResult> : IAwaiter<TResult>, INotifyCompletion, ICriticalNotifyCompletion
	{
	}
}
