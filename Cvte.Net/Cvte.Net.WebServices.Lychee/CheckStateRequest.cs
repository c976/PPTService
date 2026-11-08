using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Lychee
{
	
	
	[Request("http://doc.lizhi.im/status")]
	[DataContract]
	internal class CheckStateRequest : HttpRequest
	{
		private readonly string _id;

		public CheckStateRequest(string id)
		{
			_id = "id=" + id;
		}

		public override byte[] GetRequestData()
		{
			return Encoding.UTF8.GetBytes(_id);
		}
	}
}
