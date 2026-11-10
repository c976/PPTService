using Cvte.Pipe.Http.Response;

namespace Cvte.WebService.Account
{
	public class AccountResponseException : AccountException
	{
		private readonly AccountResponse accountResponse_0;

		public int Code => (int)accountResponse_0.StatusCode;

		public override string Message
		{
			get
			{
				if (!accountResponse_0.IsBadRequest && accountResponse_0.Html != null)
				{
					switch (accountResponse_0.StatusCode)
					{
					default:
						return "未知错误";
					case AccountResponseCode.UserIdAlreadyExists:
						return "账号已存在";
					case AccountResponseCode.UserNameOrPasswordIncorrect:
						return "账号或密码错误";
					case AccountResponseCode.PasswordIncorrect:
						return "密码不正确";
					case AccountResponseCode.UserNameNotExist:
					case AccountResponseCode.UserNameNotExist2:
						return "账号不存在";
					}
				}
				switch (accountResponse_0.ErrorCode)
				{
				case ErrorCode.TimeOut:
					return "网络繁忙中";
				default:
					return "网络不畅通";
				case ErrorCode.WebError:
					return "连接超时，请检查本地网络";
				}
			}
		}

		public AccountResponseException(AccountResponse response)
		{
			accountResponse_0 = response;
		}
	}
}
