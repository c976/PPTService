using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Cvte.Net.Http;
using Newtonsoft.Json;

namespace Cvte.Net.WebServices.Myou
{
	
	
	[Request("http://myou.cvte.com/api/v1/analyze")]
	[DataContract]
	internal class PostExceptionReqeust : JsonRequest
	{
		public IDictionary<string, string> Headers { get; set; }

		public IList<IDictionary<string, string>> Exceptions { get; set; }

		public override byte[] GetRequestData()
		{
			string s = JsonConvert.SerializeObject(new
			{
				header = Headers,
				body = new
				{
					error = Exceptions
				}
			});
			return Encoding.UTF8.GetBytes(s);
		}
	}
}
