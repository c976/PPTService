using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;
using Cvte.Net.WebServices.Monitors;
using Newtonsoft.Json;

namespace Cvte.Net.WebServices.Lychee
{
	
	
	[DataContract]
	public class LycheeHttpResponse : JsonResponse, IApiMonitorResponse
	{
		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "msg")]
		public string ErrorMsg { get; set; }

		[DataMember(Name = "content")]
		public string ContentValue { get; set; }

		public override bool IsSuccessBusiness => Code == "ok";
	}
	
	
	[DataContract]
	internal class LycheeHttpResponse< SA_T0> : LycheeHttpResponse
	{
		public SA_T0 Content { get; private set; }

		protected override void ParseHtml(string html)
		{
			base.ParseHtml(html);
			if (typeof(SA_T0) == typeof(string))
			{
				Content = (SA_T0)(object)ContentValue;
			}
			else
			{
				Content = JsonConvert.DeserializeObject<SA_T0>(ContentValue);
			}
		}
	}
}
