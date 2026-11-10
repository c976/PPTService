using System.Net;
using System.Runtime.Serialization;
using Cvte.Pipe.Http.Response;
using Newtonsoft.Json;

namespace Cvte.WebService.Lychee
{
	[DataContract]
	internal class LycheeResponseBase<TContent> : JsonResponse
	{
		public TContent Content { get; private set; }

		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "content")]
		public string ContentValue { get; set; }

		public override bool Succeed => Code == "ok";

		public override void ParseHtml(string html)
		{
			base.ParseHtml(html);
			Content = JsonConvert.DeserializeObject<TContent>(ContentValue);
		}

		public void ThrowIfBadReqeust()
		{
			if (IsBadRequest || !Succeed)
			{
				throw new WebException();
			}
		}
	}
}
