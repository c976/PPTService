using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.WebServices.Account.Seewo.QrCode;

namespace Cvte.Net.WebServices.Account
{
	
	public interface IQRLogin
	{
		string CachePath { get; set; }

		
		Task<QrCodeImageObject> GetLoginQrAsync();

		void WaitingLoginAsync(Action<QrLoginResult> callback, string barCode, CancellationTokenSource cancellationTokenSource, ManualResetEvent manualResetEvent);
	}
}
