using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Account
{
	[Reqeust("users/validator", ReqeustType.Post, ApiPrefix = "/api/v1/")]
	[DataContract]
	internal class CheckUserRequest : AccountRequestBase<CheckUserRequestBody>
	{
		public CheckUserRequest(string userId)
		{
			Body = new CheckUserRequestBody
			{
				UserId = userId
			};
		}
	}
}
