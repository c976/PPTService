using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;
using Cvte.Net.WebServices.Monitors;
using Newtonsoft.Json;

namespace Cvte.Net.WebServices.Account.CVTouch
{
	
	
	[DataContract]
	public class AccountHttpResponse : JsonResponse, IApiMonitorResponse
	{
		[DataMember(Name = "error_code")]
		public int Code { get; internal set; }

		public TouchAccountResponseCodes ResponseCode
		{
			get
			{
				TouchAccountResponseCodes result;
				if (!Enum.TryParse<TouchAccountResponseCodes>(Code.ToString(), out result))
				{
					return TouchAccountResponseCodes.Unknow;
				}
				return result;
			}
		}

		[DataMember(Name = "message")]
		public string ErrorMsg { get; set; }

		public override bool IsSuccessBusiness
		{
			get
			{
				if (ResponseCode == TouchAccountResponseCodes.Ok)
				{
					return Error == null;
				}
				return false;
			}
		}

		protected override void ParseHtml(string html)
		{
			var anon = JsonConvert.DeserializeAnonymousType(html, new
			{
				error_code = 0,
				message = "",
				entity = (object)null
			});
			ErrorMsg = anon.message;
			Code = anon.error_code;
			if (anon.entity != null)
			{
				string text = anon.entity.ToString();
				object obj = JsonConvert.DeserializeObject(text, GetType());
				if (obj == null)
				{
					throw new InvalidOperationException("body is null when parsing html! body string is " + text);
				}
				SelfAssignment(obj);
			}
		}
	}
}
