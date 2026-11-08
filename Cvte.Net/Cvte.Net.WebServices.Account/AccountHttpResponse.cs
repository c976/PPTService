using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;
using Cvte.Net.WebServices.Monitors;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class AccountHttpResponse : JsonResponse, IApiMonitorResponse
	{
		[DataMember(Name = "error_code")]
		public int Code { get; internal set; }

		public AccountResponseCodes ResponseCode
		{
			get
			{
				AccountResponseCodes result;
				if (!Enum.TryParse<AccountResponseCodes>(Code.ToString(), out result))
				{
					return AccountResponseCodes.Unknow;
				}
				return result;
			}
		}

		[DataMember(Name = "message")]
		public string Message { get; set; }

		public override bool IsSuccessBusiness
		{
			get
			{
				if (ResponseCode == AccountResponseCodes.Ok)
				{
					return Error == null;
				}
				return false;
			}
		}
	}
}
