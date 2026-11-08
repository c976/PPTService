using System.Runtime.CompilerServices;

namespace Cvte.Net.Http
{
	public abstract class DynamicGetRequest : HttpRequest
	{
		
		public override byte[] GetRequestData()
		{
			return new byte[0];
		}
	}
}
