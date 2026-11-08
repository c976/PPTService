using Cvte.Pipe.Http.Response;

namespace Cvte.WebService.Account
{
	public class AccountResponseException : AccountException
	{
		private readonly AccountResponse _0001;

		public int Code => (int)_0001.StatusCode;

		public override string Message
		{
			get
			{
				while (true)
				{
					if (SA_Proxy._007E_0094___0083(_0001) || SA_Proxy._007E_0083___0018(_0001) == null)
					{
						ErrorCode errorCode = SA_Proxy._007E_009A___0087(_0001);
						ErrorCode errorCode2;
						if (uint.MaxValue != 0)
						{
							errorCode2 = errorCode;
						}
						if (0 == 0 && errorCode2 == ErrorCode.WebError)
						{
							return "连接超时，请检查本地网络";
						}
						if (false)
						{
							goto IL_00b3;
						}
						if (false)
						{
							continue;
						}
						if (errorCode2 != ErrorCode.TimeOut)
						{
							return "网络不畅通";
						}
					}
					else
					{
						AccountResponseCode statusCode = _0001.StatusCode;
						switch (statusCode)
						{
						case AccountResponseCode.UserNameOrPasswordIncorrect:
							return "账号或密码错误";
						case AccountResponseCode.UserNameNotExist:
						case AccountResponseCode.UserNameNotExist2:
							return "账号不存在";
						case AccountResponseCode.PasswordIncorrect:
							goto IL_00b3;
						}
						if (2 != 0)
						{
							if (statusCode != AccountResponseCode.UserIdAlreadyExists)
							{
								break;
							}
							return "账号已存在";
						}
					}
					return "网络繁忙中";
					IL_00b3:
					return "密码不正确";
				}
				return "未知错误";
			}
		}

		public AccountResponseException(AccountResponse response)
		{
			_0001 = response;
		}
	}
}
