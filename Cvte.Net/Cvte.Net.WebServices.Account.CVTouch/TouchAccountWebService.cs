using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account.CVTouch
{
	
	
	public class TouchAccountWebService : WebService, ITouchAccountService
	{
		private readonly string _appName;

		private string _currentSessionId;

		private string _currentToken = "";

		private string _currentUid = "";

		public TouchAccountWebService(string appName)
		{
			TouchAccountWebService touchAccountWebService = this;
			_appName = appName;
			AddRequestFilter(new BasicHttpRequestFilter((HttpRequest request) =>
			{
				if (!string.IsNullOrEmpty(touchAccountWebService._currentUid))
				{
					request.CookieCollection = new CookieCollection
					{
						new Cookie("x-auth-app", appName),
						new Cookie("x-auth-token", UrlEncodeUtility.JavaUrlEncode(touchAccountWebService._currentToken, Encoding.UTF8)),
						new Cookie("x-auth-uid", touchAccountWebService._currentUid),
						new Cookie("sid", touchAccountWebService._currentSessionId ?? "")
					};
				}
				else
				{
					request.CookieCollection = new CookieCollection
					{
						new Cookie("x-auth-app", appName)
					};
				}
			}));
		}

		public async Task<GenerateCheckCodeHttpResponse> GenerateCheckCodeAsync()
		{
			GenerateCheckCodeHttpResponse generateCheckCodeHttpResponse = await GetAsync<GenerateCheckCodeHttpResponse>(new GenerateCheckCodeHttpRequest());
			if (generateCheckCodeHttpResponse.Success)
			{
				if (!string.IsNullOrEmpty(generateCheckCodeHttpResponse.SessionId))
				{
					_currentSessionId = generateCheckCodeHttpResponse.SessionId;
				}
				generateCheckCodeHttpResponse.ImageUrl = $"{Host}/api/v2/captcha/show?t={DateTime.UtcNow.Ticks}&sid={_currentSessionId}";
			}
			return generateCheckCodeHttpResponse;
		}

		public async Task<LoginAccountResponse> LoginAsync(string userName, string password)
		{
			LoginAccountResponse loginAccountResponse = await PostAsync<LoginAccountResponse>(new LoginAccountRequest
			{
				AppName = _appName,
				UserName = userName,
				Password = SecurityUtility.SHA256(password)
			});
			if (loginAccountResponse.Success)
			{
				_currentUid = loginAccountResponse.Uid;
				_currentToken = loginAccountResponse.Token;
			}
			return loginAccountResponse;
		}

		public async Task<AccountHttpResponse> SendValidateCodeAsync(string mobile, int actionCode, string captcha)
		{
			return await PostAsync<AccountHttpResponse>(new SendValidateCodeRequest
			{
				AppName = _appName,
				Mobile = mobile,
				ActionType = actionCode,
				Captcha = captcha
			});
		}

		public async Task<AccountHttpResponse> CheckValidateCodeAsync(string mobile, int actionCode, string validateCode)
		{
			return await PostAsync<AccountHttpResponse>(new CheckValidateCodeRequest
			{
				AppName = _appName,
				Mobile = mobile,
				ActionType = actionCode,
				ValidateCode = validateCode
			});
		}

		public async Task<AccountHttpResponse> BindPhoneAsync(string mobile, string validateCode)
		{
			return await PostAsync<AccountHttpResponse>(new BindPhoneRequest
			{
				AppName = _appName,
				Mobile = mobile,
				ValidateCode = validateCode
			});
		}
	}
}
