using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.IO
{
	
	public interface ILoggerDisposer
	{
		Task NotifyAndWaitLoggerDisposedAsync();
	}
}
