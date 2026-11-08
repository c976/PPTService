using System.Runtime.CompilerServices;

namespace Cvte.Net.Http
{
	
	public interface IHttpRequestTraceable
	{
		string TraceId { get; }
	}
}
