using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.Threading
{
	
	internal interface IAsyncDisposable
	{
		Task DisposeAsync();
	}
}
