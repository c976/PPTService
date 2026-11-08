using System.Runtime.CompilerServices;
using Cvte.Net.WebServices.Account.Seewo.UserCenter.Models;

namespace Cvte.Net.WebServices.Account.Seewo.QrCode
{
	
	
	public class QrLoginResult
	{
		public QrState State { get; }

		public QrLoginUserInfo UserInfo { get; }

		public QrLoginResult(QrState state)
			: this(state, null)
		{
		}

		
		public QrLoginResult(QrState state, QrLoginUserInfo userInfo)
		{
			State = state;
			UserInfo = userInfo;
		}
	}
}
