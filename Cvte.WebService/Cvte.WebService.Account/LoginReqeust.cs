using System;
using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Account
{
	[Reqeust("auth", ReqeustType.Post, ApiPrefix = "/api/v1/")]
	[DataContract]
	internal class LoginReqeust : AccountRequestBase<LoginReqeustBody>
	{
		private static readonly string DefaultExpireTime = 2592000000L.ToString();

		public LoginReqeust(string userId, string password, string appName)
		{
			Body = new LoginReqeustBody
			{
				UserId = userId,
				Password = password,
				ExpireTime = DefaultExpireTime,
				TimeStamp = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString(),
				AppName = appName
			};
		}
	}
}
