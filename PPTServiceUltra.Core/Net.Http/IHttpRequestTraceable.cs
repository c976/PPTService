using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Core.Net.Http
{
	
	public interface IHttpRequestTraceable
	{
		string TraceId { get; }
	}
}
