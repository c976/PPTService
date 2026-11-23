using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core
{
	
	
	internal static class RuntimeSystemExtensions
	{
		public static async Task AsTask([NotNull] this IAwaitable<IAwaiter> awaitable)
		{
			if (awaitable == null)
			{
				throw new ArgumentNullException("awaitable");
			}
			await awaitable;
		}

		public static async Task<T> AsTask< T>([NotNull] this IAwaitable<IAwaiter<T>, T> awaitable)
		{
			if (awaitable == null)
			{
				throw new ArgumentNullException("awaitable");
			}
			return await awaitable;
		}
	}
}
