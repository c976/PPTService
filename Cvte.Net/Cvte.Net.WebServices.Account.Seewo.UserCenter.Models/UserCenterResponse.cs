using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;
using Newtonsoft.Json;

namespace Cvte.Net.WebServices.Account.Seewo.UserCenter.Models
{
	
	
	[DataContract]
	public abstract class UserCenterResponse : JsonResponse
	{
		internal QrAccountResponseCodes QrAccountResponseCode
		{
			get
			{
				QrAccountResponseCodes result;
				if (Enum.TryParse<QrAccountResponseCodes>(ResponseCode.ToString(), out result))
				{
					return result;
				}
				return QrAccountResponseCodes.Unknow;
			}
		}

		[DataMember(Name = "statusCode")]
		internal int ResponseCode { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }

		public override bool IsSuccessBusiness => QrAccountResponseCode == QrAccountResponseCodes.Ok;

		protected override void ParseHtml(string html)
		{
			var anon = JsonConvert.DeserializeAnonymousType(html, new
			{
				data = (object)null
			});
			if (anon != null)
			{
				object obj = JsonConvert.DeserializeObject(anon.data.ToString(), GetType());
				ResponseCode = ((UserCenterResponse)obj).ResponseCode;
				Message = ((UserCenterResponse)obj).Message;
				SelfAssignment(obj);
			}
		}
	}
}
