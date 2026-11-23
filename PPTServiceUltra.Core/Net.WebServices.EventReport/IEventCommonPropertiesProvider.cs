using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Core.Net.WebServices.EventReport
{
	
	public interface IEventCommonPropertiesProvider
	{
		IDictionary<string, object> BuildCommonProperties();

		IDictionary<string, object> FilterCommonProperties(IDictionary<string, object> commonProperties);
	}
}
