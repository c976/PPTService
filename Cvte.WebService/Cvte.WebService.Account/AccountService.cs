using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Cvte.Pipe.Http;
using Newtonsoft.Json;
using Cvte.WebService.Internal;

namespace Cvte.WebService.Account
{
	public class AccountService : IAccountService
	{
		private class Class22
		{
			private string string_0;

			private string string_1;

			private string string_2;

			[SpecialName]
			public string method_0()
			{
				return string_0;
			}

			[SpecialName]
			public void method_1(string string_3)
			{
				string_0 = string_3;
			}

			[SpecialName]
			public string method_2()
			{
				return string_1;
			}

			[SpecialName]
			public void method_3(string string_3)
			{
				string_1 = string_3;
			}

			[SpecialName]
			public string method_4()
			{
				return string_2;
			}

			[SpecialName]
			public void method_5(string string_3)
			{
				string_2 = string_3;
			}
		}

		private readonly AccountSettings class14_0;

		private readonly AccountCache accountCache_0;

		private readonly IHttpPipe ihttpPipe_0;

		private string string_0;

		private string string_1;

		private string string_2;

		private string string_3;

		private string string_4;

		private string string_5;

		private string string_6;

		private string string_7;

		private string string_8;

		private string string_9;

		private EventHandler eventHandler_0;

		private EventHandler eventHandler_1;

		private bool bool_0;

		public string this[string key]
		{
			get
			{
				return class14_0[key];
			}
			set
			{
				class14_0[key] = value;
			}
		}

		public string CurrentUserResourceId
		{
			get
			{
				method_5();
				return string_1;
			}
			private set
			{
				string_1 = value;
				class14_0.method_7(value);
			}
		}

		public string CurrentUserId
		{
			get
			{
				method_5();
				return string_2;
			}
			private set
			{
				string_2 = value;
				class14_0.method_9(value);
			}
		}

		public string CurrentUserName
		{
			get
			{
				method_5();
				return string_3;
			}
			private set
			{
				string_3 = value;
				class14_0.method_11(value);
			}
		}

		public string CurrentUserToken
		{
			get
			{
				return string_4;
			}
			private set
			{
				string_4 = value;
				class14_0.method_13(value);
			}
		}

		public string CurrentPhone
		{
			get
			{
				method_5();
				return string_5;
			}
			set
			{
				string_5 = value;
				class14_0.method_15(value);
			}
		}

		public string CurrentStage
		{
			get
			{
				method_5();
				return string_6;
			}
			set
			{
				string_6 = value;
				class14_0.method_17(value);
			}
		}

		public string CurrentSubject
		{
			get
			{
				method_5();
				return string_7;
			}
			set
			{
				string_7 = value;
				class14_0.method_19(value);
			}
		}

		public string CurrentStageName
		{
			get
			{
				method_5();
				return string_8;
			}
			set
			{
				string_8 = value;
				class14_0.method_21(value);
			}
		}

		public string CurrentSubjectName
		{
			get
			{
				method_5();
				return string_9;
			}
			set
			{
				string_9 = value;
				class14_0.method_23(value);
			}
		}

		public IAccountCache Cache => accountCache_0;

		public event EventHandler UserLogined
		{
			add
			{
				EventHandler eventHandler = eventHandler_0;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler eventHandler = eventHandler_0;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event EventHandler UserLogout
		{
			add
			{
				EventHandler eventHandler = eventHandler_1;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref eventHandler_1, value2, eventHandler2);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler eventHandler = eventHandler_1;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref eventHandler_1, value2, eventHandler2);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public AccountService(IHttpPipe accountPipe, string defaultCacheDir)
		{
			string string_ = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Seewo", "Users");
			string_ = smethod_0(string_);
			class14_0 = new AccountSettings(Path.Combine(string_, "User.cache"), "Login");
			accountCache_0 = new AccountCache(string_, defaultCacheDir);
			ihttpPipe_0 = accountPipe;
		}

		public bool LoginWithCache()
		{
			method_4();
			if (string.IsNullOrEmpty(CurrentUserId))
			{
				return false;
			}
			OnUserLogining(CurrentUserId, CurrentUserName);
			eventHandler_0?.Invoke(null, EventArgs.Empty);
			return true;
		}

		public void Init(string appName)
		{
			string_0 = appName;
		}

		public bool ApplyForToken()
		{
			CurrentUserToken = string.Empty;
			try
			{
				string text = accountCache_0.method_0(CurrentUserId)?.Password;
				LoginReqeust request = new LoginReqeust(CurrentUserId, text, string_0);
				LoginResponse loginResponse = ihttpPipe_0.Post<LoginReqeust, LoginResponse>(request);
				if (!loginResponse.Succeed)
				{
					throw new AccountResponseException(loginResponse);
				}
				method_7(CurrentUserId, text, loginResponse);
			}
			catch (AccountResponseException)
			{
				return false;
			}
			return !string.IsNullOrEmpty(CurrentUserToken);
		}

		public IList<Stage> GetStages()
		{
			GetSubjectsResponse getSubjectsResponse = ihttpPipe_0.Get<GetSubjectsRequest, GetSubjectsResponse>(new GetSubjectsRequest(CurrentUserToken));
			if (!getSubjectsResponse.Succeed)
			{
				return new List<Stage>();
			}
			return getSubjectsResponse.Data;
		}

		public UserSettingInfo GetUserSetting()
		{
			string resourceId = accountCache_0.method_0(CurrentUserId)?.Key;
			GetUserSettingResponse getUserSettingResponse = ihttpPipe_0.Get<GetUserSettingRequest, GetUserSettingResponse>(new GetUserSettingRequest(resourceId, CurrentUserToken));
			if (!getUserSettingResponse.Succeed)
			{
				return new UserSettingInfo();
			}
			CurrentStage = getUserSettingResponse.Data.Stage;
			CurrentSubject = getUserSettingResponse.Data.Subject;
			CurrentStageName = getUserSettingResponse.Data.StageName;
			CurrentSubjectName = getUserSettingResponse.Data.SubjectName;
			return getUserSettingResponse.Data;
		}

		public bool UpdateUserSetting(string userName, UserSettingInfo userSettingInfo)
		{
			string resourceId = accountCache_0.method_0(CurrentUserId)?.Key;
			if (!ihttpPipe_0.Post<UpdateUserSettingRequest, UpdateUserSettingResponse>(new UpdateUserSettingRequest(resourceId, CurrentUserToken, userName, userSettingInfo.Stage, userSettingInfo.Subject)).Succeed)
			{
				return false;
			}
			CurrentUserName = userName;
			CurrentStage = userSettingInfo.Stage;
			CurrentSubject = userSettingInfo.Subject;
			CurrentStageName = userSettingInfo.StageName;
			CurrentSubjectName = userSettingInfo.SubjectName;
			return true;
		}

		public void Register(string userId, string password, string name)
		{
			RegisterResponse registerResponse = ihttpPipe_0.Post<RegisterRequest, RegisterResponse>(new RegisterRequest(userId, password, name, string_0));
			if (!registerResponse.Succeed)
			{
				throw new AccountResponseException(registerResponse);
			}
		}

		public void Login(string userId, string password)
		{
			password = Md5Hash.smethod_0(password ?? string.Empty).ToLower();
			method_6(userId, password);
		}

		public bool ResetPassowrd(string userId, string dynamic, string password)
		{
			password = Md5Hash.smethod_0(password ?? string.Empty).ToLower();
			ResetPasswordResponse resetPasswordResponse = ihttpPipe_0.Post<ResetPasswordRequest, ResetPasswordResponse>(new ResetPasswordRequest(userId, dynamic, password));
			if (!resetPasswordResponse.IsBadRequest)
			{
				return resetPasswordResponse.Succeed;
			}
			return false;
		}

		public void Logout()
		{
			CurrentUserResourceId = string.Empty;
			CurrentUserId = string.Empty;
			CurrentUserName = string.Empty;
			CurrentUserToken = string.Empty;
			CurrentPhone = string.Empty;
			eventHandler_1?.Invoke(null, EventArgs.Empty);
		}

		public bool ExistUser(string userId)
		{
			return ihttpPipe_0.Post<CheckUserRequest, CheckUserResponse>(new CheckUserRequest(userId)).StatusCode == AccountResponseCode.UserIdAlreadyExists;
		}

		public bool SendValidateCode(string target, string action)
		{
			SendValidateCodeResponse sendValidateCodeResponse = ihttpPipe_0.Post<SendValidateCodeRequest, SendValidateCodeResponse>(new SendValidateCodeRequest(target, action));
			if (!sendValidateCodeResponse.IsBadRequest)
			{
				return sendValidateCodeResponse.Succeed;
			}
			return false;
		}

		public bool CheckValidateCode(string target, string code)
		{
			CheckValidateCodeResponse checkValidateCodeResponse = ihttpPipe_0.Post<CheckValidateCodeRequest, CheckValidateCodeResponse>(new CheckValidateCodeRequest(target, code));
			if (!checkValidateCodeResponse.IsBadRequest)
			{
				return checkValidateCodeResponse.Succeed;
			}
			return false;
		}

		public string GenerateSeewoToken()
		{
			Class22 @class = new Class22();
			@class.method_1(CurrentUserId);
			@class.method_3(CurrentUserName);
			@class.method_5(accountCache_0.method_0(CurrentUserId).Password);
			return JsonConvert.SerializeObject((object)@class);
		}

		public string GenerateSeewoToken(string userId, string password, out string userName)
		{
			password = Md5Hash.smethod_0(password ?? string.Empty).ToLower();
			LoginReqeust request = new LoginReqeust(userId, password, string_0);
			LoginResponse loginResponse = ihttpPipe_0.Post<LoginReqeust, LoginResponse>(request);
			userName = loginResponse.UserName;
			if (!loginResponse.Succeed)
			{
				throw new AccountResponseException(loginResponse);
			}
			Class22 @class = new Class22();
			@class.method_1(userId);
			@class.method_3(userName);
			@class.method_5(password);
			return JsonConvert.SerializeObject((object)@class);
		}

		public bool CheckSeewoToken(string token, out string userId, out string userName)
		{
			try
			{
				Class22 @class = JsonConvert.DeserializeObject<Class22>(token);
				userId = @class.method_0();
				userName = @class.method_2();
				return true;
			}
			catch (Exception)
			{
				userId = string.Empty;
				userName = string.Empty;
				return false;
			}
		}

		public bool BindPhone(string phone, string validateCode)
		{
			BindPhoneOrEmailResponse bindPhoneOrEmailResponse = ihttpPipe_0.Post<BindPhoneOrEmailRequest, BindPhoneOrEmailResponse>(new BindPhoneOrEmailRequest(CurrentUserToken, phone, validateCode, 1));
			if (bindPhoneOrEmailResponse.Succeed)
			{
				CurrentPhone = phone;
			}
			return bindPhoneOrEmailResponse.Succeed;
		}

		public void LoginWithSeewoToken(string token)
		{
			Class22 @class = JsonConvert.DeserializeObject<Class22>(token);
			method_6(@class.method_0(), @class.method_4());
		}

		private void method_4()
		{
			string_1 = class14_0.method_6();
			string_2 = class14_0.method_8();
			string_3 = class14_0.method_10();
			string_4 = class14_0.method_12();
			string_5 = class14_0.method_14();
			string_6 = class14_0.method_16();
			string_7 = class14_0.method_18();
			string_8 = class14_0.method_20();
			string_9 = class14_0.method_22();
		}

		private void method_5()
		{
			if (!bool_0)
			{
				method_4();
			}
			bool_0 = true;
		}

		protected void OfflineLogin(string userId, string password, LoginResponse response)
		{
			method_7(userId, password, response);
			eventHandler_0?.Invoke(null, EventArgs.Empty);
		}

		private void method_6(string string_10, string string_11)
		{
			LoginReqeust request = new LoginReqeust(string_10, string_11, string_0);
			LoginResponse loginResponse = ihttpPipe_0.Post<LoginReqeust, LoginResponse>(request);
			if (!loginResponse.Succeed)
			{
				throw new AccountResponseException(loginResponse);
			}
			method_7(string_10, string_11, loginResponse);
			eventHandler_0?.Invoke(null, EventArgs.Empty);
		}

		private void method_7(string string_10, string string_11, LoginResponse loginResponse_0)
		{
			UserInfo userInfo = new UserInfo
			{
				Key = loginResponse_0.ResourceId,
				Id = string_10,
				Name = loginResponse_0.UserName,
				Password = string_11,
				Photo = (loginResponse_0.UserPhoto ?? ""),
				TokenId = loginResponse_0.TokenId,
				LastTime = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds
			};
			accountCache_0.method_1(userInfo);
			CurrentUserResourceId = userInfo.Key;
			CurrentUserId = userInfo.Id;
			CurrentUserName = userInfo.Name;
			CurrentUserToken = userInfo.TokenId;
			CurrentPhone = loginResponse_0.Phone;
			UserSettingInfo userSetting = GetUserSetting();
			CurrentStage = userSetting.Stage;
			CurrentSubject = userSetting.Subject;
			CurrentStageName = userSetting.StageName;
			CurrentSubjectName = userSetting.SubjectName;
			bool_0 = true;
			OnUserLogining(userInfo.Id, userInfo.Name);
		}

		protected virtual void OnUserLogining(string userId, string userName)
		{
		}

		private static string smethod_0(string string_10)
		{
			if (Directory.Exists(string_10))
			{
				return string_10;
			}
			try
			{
				Directory.CreateDirectory(string_10);
			}
			catch (IOException)
			{
			}
			return string_10;
		}
	}
}
