using System;
using System.Globalization;
using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Account
{
	[Reqeust("subjects", ReqeustType.Get, ApiPrefix = "/api/v1/")]
	[DataContract]
	internal class GetSubjectsRequest : AccountGetRequestBase<GetSubjectsBody>
	{
		public GetSubjectsRequest(string tokenId)
		{
			Body = new GetSubjectsBody
			{
				TokenId = tokenId,
				TimeSpan = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString(CultureInfo.InvariantCulture)
			};
		}
	}
}
