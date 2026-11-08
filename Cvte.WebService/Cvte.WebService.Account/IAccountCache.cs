using System.Collections.Generic;

namespace Cvte.WebService.Account
{
	public interface IAccountCache
	{
		IList<UserInfo> GetAllUserInfo();

		void Remove(string userId);
	}
}
