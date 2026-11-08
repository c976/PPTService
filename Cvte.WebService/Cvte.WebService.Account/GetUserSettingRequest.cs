using System;
using System.Globalization;
using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Account
{
	[Reqeust("users/subject", ReqeustType.Get, ApiPrefix = "/api/v1/")]
	[DataContract]
	internal class GetUserSettingRequest : AccountGetRequestBase<GetUserSettingBody>
	{
		public GetUserSettingRequest(string resourceId, string tokenId)
		{
			Body = new GetUserSettingBody
			{
				ResourceId = resourceId,
				TokenId = tokenId,
				TimeSpan = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString(CultureInfo.InvariantCulture)
			};
		}
	}
}
