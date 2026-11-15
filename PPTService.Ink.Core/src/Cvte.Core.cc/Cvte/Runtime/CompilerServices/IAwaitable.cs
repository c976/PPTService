using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Runtime.CompilerServices
{
	
	public interface IAwaitable< out TAwaiter> where TAwaiter : IAwaiter
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		TAwaiter GetAwaiter();
	}
	
	public interface IAwaitable< out TAwaiter,  out TResult> where TAwaiter : IAwaiter<TResult>
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		TAwaiter GetAwaiter();
	}
}
