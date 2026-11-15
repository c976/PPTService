using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.IO
{
	
	public interface ILoggerDisposer
	{
		Task NotifyAndWaitLoggerDisposedAsync();
	}
}
