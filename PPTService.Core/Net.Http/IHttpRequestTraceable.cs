using System.Runtime.CompilerServices;

namespace PPTService.Core.Net.Http
{
	
	public interface IHttpRequestTraceable
	{
		string TraceId { get; }
	}
}
