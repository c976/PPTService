using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Account
{
	[Reqeust("users/dynamic/resetpassword", ReqeustType.Post, ApiPrefix = "/api/v1/")]
	[DataContract]
	internal class ResetPasswordRequest : AccountRequestBase<ResetPasswordRequestBody>
	{
		public ResetPasswordRequest(string userId, string dynamic, string password)
		{
			Body = new ResetPasswordRequestBody
			{
				UserId = userId,
				Dynamic = dynamic,
				Password = password
			};
		}
	}
}
