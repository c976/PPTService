using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.Account.CVTouch
{
	
	public interface ITouchAccountService
	{
		Task<GenerateCheckCodeHttpResponse> GenerateCheckCodeAsync();

		Task<LoginAccountResponse> LoginAsync(string userName, string password);

		Task<AccountHttpResponse> SendValidateCodeAsync(string mobile, int actionCode, string captcha);

		Task<AccountHttpResponse> CheckValidateCodeAsync(string mobile, int actionCode, string validateCode);

		Task<AccountHttpResponse> BindPhoneAsync(string mobile, string validateCode);
	}
}
