using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cvte.Net.WebServices.Account;

namespace Cvte.Net
{
	
	
	public static class AccountWebServiceExtension
	{
		internal const string ClientChannelCookieName = "client_channel";

		public static string ClientPackageChannelName { get; set; }

		public static async Task<LoginAccountResponse> LoginWithChannelNameAsync(this AccountWebService accountService, string userName, string password, string channelName, Captcha captcha = null)
		{
			string password2 = SecurityUtility.Encrypt32(password ?? string.Empty).ToLower();
			return await accountService.LoginWithEncryptedPasswordAndChannelNameAsync(userName, password2, channelName, captcha);
		}

		public static async Task<LoginAccountResponse> LoginWithEncryptedPasswordAndChannelNameAsync(this AccountWebService accountService, string userName, string password, string channelName, Captcha captcha = null)
		{
			LoginAccountRequest loginAccountRequest = new LoginAccountRequest
			{
				UserName = userName,
				Password = password,
				Captcha = captcha
			};
			loginAccountRequest.CookieCollection.Add(new Cookie("client_channel", channelName));
			return await accountService.LoginAsync(loginAccountRequest);
		}
	}
}
