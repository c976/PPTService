using System;
using System.Collections.Generic;

namespace Cvte.WebService.Account
{
	public interface IAccountService
	{
		IAccountCache Cache { get; }

		string CurrentUserResourceId { get; }

		string CurrentUserId { get; }

		string CurrentUserName { get; }

		string CurrentUserToken { get; }

		string CurrentPhone { get; }

		string CurrentStage { get; }

		string CurrentSubject { get; }

		string CurrentStageName { get; set; }

		string CurrentSubjectName { get; set; }

		event EventHandler UserLogined;

		event EventHandler UserLogout;

		void Init(string appName);

		void Register(string userId, string password, string name);

		void Login(string userId, string password);

		void LoginWithSeewoToken(string seewoToken);

		bool LoginWithCache();

		bool ApplyForToken();

		IList<Stage> GetStages();

		UserSettingInfo GetUserSetting();

		bool UpdateUserSetting(string userName, UserSettingInfo userSettingInfo);

		bool ResetPassowrd(string userId, string dynamic, string password);

		void Logout();

		bool ExistUser(string userId);

		bool SendValidateCode(string target, string action);

		bool CheckValidateCode(string target, string code);

		string GenerateSeewoToken();

		string GenerateSeewoToken(string userId, string password, out string userName);

		bool CheckSeewoToken(string seewoToken, out string userId, out string userName);

		bool BindPhone(string phone, string validateCode);
	}
}
