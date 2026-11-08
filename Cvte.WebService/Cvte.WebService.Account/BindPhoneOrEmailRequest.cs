using System;
using System.Globalization;
using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Account
{
	[Reqeust("users/PhoneOrEmailBind", ReqeustType.Post, ApiPrefix = "/api/v1/")]
	[DataContract]
	internal class BindPhoneOrEmailRequest : AccountRequestBase<BindPhoneOrEmailBody>
	{
		public BindPhoneOrEmailRequest(string tokenId, string phoneOrEmail, string validateCode, int sendType)
		{
			Body = new BindPhoneOrEmailBody
			{
				TokenId = tokenId,
				PhoneOrEmail = phoneOrEmail,
				SendType = sendType.ToString(),
				ValidateCode = validateCode,
				TimeSpan = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString(CultureInfo.InvariantCulture)
			};
		}
	}
}
