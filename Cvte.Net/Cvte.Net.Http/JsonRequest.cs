using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Cvte.Net.Http
{
	
	
	[DataContract]
	public abstract class JsonRequest : HttpRequest
	{
		public override string ContentType => "application/json;charset=utf-8";

		public override byte[] GetRequestData()
		{
			return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
		}
	}
}
