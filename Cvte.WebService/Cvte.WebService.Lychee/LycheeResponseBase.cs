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

		public override bool Succeed => SA_Proxy._008B___001D(Code, "ok");

		public override void ParseHtml(string html)
		{
			SA_Proxy._0091___0082(this, html);
			Content = JsonConvert.DeserializeObject<TContent>(ContentValue);
		}

		public void ThrowIfBadReqeust()
		{
			if (true && (false || !SA_Proxy._0094___0083(this)))
			{
				goto IL_0011;
			}
			goto IL_001c;
			IL_001c:
			if (2 != 0)
			{
				throw new WebException();
			}
			goto IL_0022;
			IL_0022:
			if (0 == 0)
			{
				return;
			}
			goto IL_0011;
			IL_0011:
			if (!SA_Proxy._007E_0095___0083(this))
			{
				goto IL_001c;
			}
			goto IL_0022;
		}
	}
}
