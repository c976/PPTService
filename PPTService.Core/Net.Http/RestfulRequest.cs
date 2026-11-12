using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace PPTService.Core.Net.Http
{
	
	
	[DataContract]
	public class RestfulRequest : HttpRequest
	{
		public override string ContentType => "application/json;charset=utf-8";

		public override byte[] GetRequestData()
		{
			return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
		}
	}
}
