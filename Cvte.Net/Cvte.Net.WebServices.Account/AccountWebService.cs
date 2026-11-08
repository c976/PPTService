using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	
	
	public class AccountWebService : WebService, IAccountService
	{
		public AccountWebService(string appName, WebServiceSettings settings)
			: this(appName, "", settings)
		{
		}

		public AccountWebService(string appName, string appBrand, WebServiceSettings settings)
			: base(settings)
		{
			if (string.IsNullOrWhiteSpace(appName))
			{
				throw new ArgumentNullException("appName");
			}
			Host = "http://account.seewo.com/";
			AddRequestFilter(new BasicHttpRequestFilter((HttpRequest request) =>
			{
				request.CookieCollection.Add(new CookieCollection
				{
					new Cookie("x-auth-app", appName)
				});
				if (!string.IsNullOrWhiteSpace(appBrand))
				{
					request.CookieCollection.Add(new Cookie("x-auth-brand", appBrand));
				}
			}));
		}

		public async Task<RegisterAccountResponse> RegisterAsync(string userName, string password, string nickName, string validateCode)
		{
			bool flag = RegexUtilities.IsValidPhoneNumber(userName);
			return await PostAsync<RegisterAccountResponse>(new RegisterAccountRequest
			{
				User = new UserDetails
				{
					Phone = (flag ? userName : ""),
					Email = (flag ? "" : userName),
					Password = SecurityUtility.Encrypt32(password ?? string.Empty).ToLower(),
					Nickname = nickName
				},
				ValidateCode = validateCode
			}).ConfigureAwait(false);
		}

		public async Task<LoginAccountResponse> LoginAsync(string userName, string password,  Captcha captcha = null)
		{
			return await LoginWithEncryptedPasswordAsync(userName, SecurityUtility.Encrypt32(password), captcha);
		}

		public async Task<LoginAccountResponse> LoginWithEncryptedPasswordAsync(string userName, string password,  Captcha captcha = null)
		{
			return await LoginAsync(new LoginAccountRequest
			{
				UserName = userName,
				Password = password,
				Captcha = captcha
			});
		}

		internal async Task<LoginAccountResponse> LoginAsync(LoginAccountRequest loginAccountRequest)
		{
			if (loginAccountRequest == null)
			{
				throw new ArgumentNullException("loginAccountRequest");
			}
			if (!string.IsNullOrWhiteSpace(AccountWebServiceExtension.ClientPackageChannelName))
			{
				loginAccountRequest.CookieCollection.Add(new Cookie("client_channel", AccountWebServiceExtension.ClientPackageChannelName));
			}
			return await PostAsync<LoginAccountResponse>(loginAccountRequest);
		}

		public async Task<AccountHttpResponse> ResetPasswordAsync(string userName, string validateCode, string password)
		{
			return await PostAsync<AccountHttpResponse>(new ResetPasswordAccountRequest
			{
				UserName = userName,
				Password = SecurityUtility.Encrypt32(password ?? string.Empty).ToLower(),
				ValidateCode = validateCode
			});
		}

		public async Task<AccountHttpResponse> SendValidateCodeAsync(string userName, int type, Captcha captcha = null)
		{
			return await PostAsync<AccountHttpResponse>(new SendValidateCodeAccountRequest
			{
				UserName = userName,
				Type = type.ToString(),
				Captcha = captcha
			});
		}

		public async Task<AccountHttpResponse> CheckValidateCodeAsync(string userName, string validateCode)
		{
			return await PostAsync<AccountHttpResponse>(new CheckValidateCodeAccountRequest
			{
				UserName = userName,
				ValidateCode = validateCode
			});
		}

		public async Task<AccountHttpResponse> BindPhoneAsync(string userId, string phone, string validateCode)
		{
			return await PostAsync<AccountHttpResponse>(new BindPhoneOrEmailAccountRequest
			{
				Phone = phone,
				UserId = userId,
				ValidateCode = validateCode
			});
		}

		public async Task<GetStageAndSubjectAccountResponse> GetStageAndSubjectAsync(bool useExtendSs = false)
		{
			return await GetAsync<GetStageAndSubjectAccountResponse>(new GetStageAndSubjectAccountRequest(useExtendSs));
		}

		public async Task<GetUserDetailsAccountResponse> GetUserDetailsAsync(bool useExtendSs = false)
		{
			return await GetAsync<GetUserDetailsAccountResponse>(new GetUserDetailsAccountRequest(useExtendSs));
		}

		public async Task<AccountHttpResponse> UpdateUserDetailsAsync(UserDetails userDetails)
		{
			return await PostAsync<AccountHttpResponse>(new UpdateUserDetailsAccountRequest
			{
				Nickname = userDetails.Nickname,
				StageId = userDetails.StageId,
				SubjectId = userDetails.SubjectId,
				UserPhoto = (string.IsNullOrEmpty(userDetails.UserPhoto) ? null : userDetails.UserPhoto)
			});
		}

		public async Task<CheckPhoneExistenceResponse> CheckPhoneExistenceAsync(string phone)
		{
			return await GetAsync<CheckPhoneExistenceResponse>(new CheckPhoneExistenceRequest
			{
				Phone = phone
			});
		}

		public async Task<CheckEmailExistenceResponse> CheckEmailExistenceAsync(string email)
		{
			return await GetAsync<CheckEmailExistenceResponse>(new CheckEmailExistenceRequest
			{
				Email = email
			});
		}

		public async Task<GetCaptchaResponse> GetCaptchaAsync()
		{
			return await PostAsync<GetCaptchaResponse>(new GetCaptchaRequest());
		}
	}
}
