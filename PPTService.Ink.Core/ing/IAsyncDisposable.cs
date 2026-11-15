using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace dotnetCampus.Threading
{
	
	internal interface IAsyncDisposable
	{
		Task DisposeAsync();
	}
}
