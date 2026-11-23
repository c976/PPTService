using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.Threading
{
	
	
	public static class CompletedTask
	{
		private static readonly Task s_completedTask = Task.FromResult(true);

		public static Task Value => s_completedTask;
	}
}
