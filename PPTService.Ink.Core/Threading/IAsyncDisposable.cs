using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.Threading
{
	
	internal interface IAsyncDisposable
	{
		Task DisposeAsync();
	}
}
