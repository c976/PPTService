using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.Account
{
	
	public interface IAccountService
	{
		Task<RegisterAccountResponse> RegisterAsync(string userName, string password, string nickName, string validateCode);

		Task<LoginAccountResponse> LoginAsync(string userName, string password,  Captcha captcha = null);

		Task<LoginAccountResponse> LoginWithEncryptedPasswordAsync(string userName, string password, Captcha captcha = null);

		Task<AccountHttpResponse> ResetPasswordAsync(string userName, string validateCode, string password);

		Task<AccountHttpResponse> SendValidateCodeAsync(string userName, int type,  Captcha captcha = null);

		Task<AccountHttpResponse> CheckValidateCodeAsync(string userName, string validateCode);

		Task<AccountHttpResponse> BindPhoneAsync(string userId, string phone, string validateCode);

		Task<GetStageAndSubjectAccountResponse> GetStageAndSubjectAsync(bool useExtendSs = false);

		Task<GetUserDetailsAccountResponse> GetUserDetailsAsync(bool useExtendSs = false);

		Task<AccountHttpResponse> UpdateUserDetailsAsync(UserDetails userDetails);

		Task<CheckPhoneExistenceResponse> CheckPhoneExistenceAsync(string phone);

		[Obsolete("此方法后台没有进行正确的实现，可直接使用 CheckPhoneExistenceAsync 进行邮箱的检查，已经做了兼容处理。")]
		Task<CheckEmailExistenceResponse> CheckEmailExistenceAsync(string email);

		Task<GetCaptchaResponse> GetCaptchaAsync();
	}
}
