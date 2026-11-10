using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using _0006;
using _000F;
using Cvte.Pipe.Http;
using Newtonsoft.Json;

namespace Cvte.WebService.Account
{
	public class AccountService : IAccountService
	{
		private class SA_C3
		{
			private string m__0001;

			private string m__0002;

			private string m__0003;

			[SpecialName]
			public dynamic _0001;
			public string _0001_M1()
			{
				return this.m__0001;
			}

			[SpecialName]
			public void _0001_M2(string P_0)
			{
				this.m__0001 = P_0;
			}

			[SpecialName]
			public string _0002()
			{
				return this.m__0002;
			}

			[SpecialName]
			public void _0002(string P_0)
			{
				this.m__0002 = P_0;
			}

			[SpecialName]
			public string _0003()
			{
				return this.m__0003;
			}

			[SpecialName]
			public void _0003(string P_0)
			{
				this.m__0003 = P_0;
			}
		}

		public dynamic m__0001;




		public dynamic m__0002;

		private string _0003;

		private string _0004;

		private string _0005;

		private string _0006;

		private string _0007;

		private string _0008;

		private string _000E;

		private string _000F;

		public string this[string key]
		{
			get
			{
				return this.m__0001[key];
			}
			set
			{
				this.m__0001[key] = value;
			}
		}

		public string CurrentUserResourceId
		{
			get
			{
				_0002();
				return this.m__0002;
			}
			private set
			{
				this.m__0002 = value;
				this.m__0001._0001(value);
			}
		}

		public string CurrentUserId
		{
			get
			{
				_0002();
				return _0003;
			}
			private set
			{
				_0003 = value;
				this.m__0001._0002(value);
			}
		}

		public string CurrentUserName
		{
			get
			{
				_0002();
				return _0004;
			}
			private set
			{
				_0004 = value;
				this.m__0001._0003(value);
			}
		}

		public string CurrentUserToken
		{
			get
			{
				return _0005;
			}
			private set
			{
				_0005 = value;
				this.m__0001._0004(value);
			}
		}

		public string CurrentPhone
		{
			get
			{
				_0002();
				return _0006;
			}
			set
			{
				_0006 = value;
				this.m__0001._0005(value);
			}
		}

		public string CurrentStage
		{
			get
			{
				_0002();
				return _0007;
			}
			set
			{
				_0007 = value;
				this.m__0001._0006(value);
			}
		}

		public string CurrentSubject
		{
			get
			{
				_0002();
				return _0008;
			}
			set
			{
				_0008 = value;
				this.m__0001._0007(value);
			}
		}

		public string CurrentStageName
		{
			get
			{
				_0002();
				return _000E;
			}
			set
			{
				_000E = value;
				this.m__0001._0008(value);
			}
		}

		public string CurrentSubjectName
		{
			get
			{
				_0002();
				return _000F;
			}
			set
			{
				_000F = value;
				this.m__0001._000E(value);
			}
		}

		public IAccountCache Cache => this.m__0001;

		public event EventHandler UserLogined
		{
			add
			{
				EventHandler eventHandler = this.m__0001;
				while (true)
				{
					EventHandler eventHandler2 = eventHandler;
					while (true)
					{
						EventHandler eventHandler3 = (EventHandler)SA_Proxy._009B___0088(eventHandler2, value);
						EventHandler value2;
						if (4u != 0)
						{
							value2 = eventHandler3;
						}
						eventHandler = (EventHandler)Interlocked.CompareExchange(ref this.m__0001, value2, eventHandler2);
						if ((object)eventHandler != eventHandler2)
						{
							break;
						}
						if (0 == 0 && 0 == 0)
						{
							return;
						}
					}
				}
			}
			remove
			{
				EventHandler eventHandler = this.m__0001;
				while (true)
				{
					EventHandler eventHandler2 = eventHandler;
					while (true)
					{
						EventHandler eventHandler3 = (EventHandler)SA_Proxy._009C___0088(eventHandler2, value);
						EventHandler value2;
						if (4u != 0)
						{
							value2 = eventHandler3;
						}
						eventHandler = (EventHandler)Interlocked.CompareExchange(ref this.m__0001, value2, eventHandler2);
						if ((object)eventHandler != eventHandler2)
						{
							break;
						}
						if (0 == 0 && 0 == 0)
						{
							return;
						}
					}
				}
			}
		}

		public event EventHandler UserLogout
		{
			add
			{
				EventHandler eventHandler = this.m__0002;
				while (true)
				{
					EventHandler eventHandler2 = eventHandler;
					while (true)
					{
						EventHandler eventHandler3 = (EventHandler)SA_Proxy._009B___0088(eventHandler2, value);
						EventHandler value2;
						if (4u != 0)
						{
							value2 = eventHandler3;
						}
						eventHandler = (EventHandler)Interlocked.CompareExchange(ref this.m__0002, value2, eventHandler2);
						if ((object)eventHandler != eventHandler2)
						{
							break;
						}
						if (0 == 0 && 0 == 0)
						{
							return;
						}
					}
				}
			}
			remove
			{
				EventHandler eventHandler = this.m__0002;
				while (true)
				{
					EventHandler eventHandler2 = eventHandler;
					while (true)
					{
						EventHandler eventHandler3 = (EventHandler)SA_Proxy._009C___0088(eventHandler2, value);
						EventHandler value2;
						if (4u != 0)
						{
							value2 = eventHandler3;
						}
						eventHandler = (EventHandler)Interlocked.CompareExchange(ref this.m__0002, value2, eventHandler2);
						if ((object)eventHandler != eventHandler2)
						{
							break;
						}
						if (0 == 0 && 0 == 0)
						{
							return;
						}
					}
				}
			}
		}

		public AccountService(IHttpPipe accountPipe, string defaultCacheDir)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Seewo", "Users");
			text = _0001(text);
			this.m__0001 = new global::_000F._0002(Path.Combine(text, "User.cache"), "Login");
			this.m__0001 = new AccountCache(text, defaultCacheDir);
			this.m__0001 = accountPipe;
		}

		public bool LoginWithCache()
		{
			do
			{
				_0001();
				int num = (SA_Proxy._001D___0016(CurrentUserId) ? 1 : 0);
				while (num != 0)
				{
					num = 0;
					if (num == 0)
					{
						return (byte)num != 0;
					}
				}
				OnUserLogining(CurrentUserId, CurrentUserName);
				this.m__0001?.Invoke(null, EventArgs.Empty);
			}
			while (2 == 0);
			return true;
		}

		public void Init(string appName)
		{
			this.m__0001 = appName;
		}

		public bool ApplyForToken()
		{
			string empty = string.Empty;
			if (0 == 0)
			{
				CurrentUserToken = empty;
			}
			try
			{
				string text;
				LoginResponse loginResponse;
				do
				{
					text = this.m__0001._0001(CurrentUserId)?.Password;
					LoginReqeust request = new LoginReqeust(CurrentUserId, text, this.m__0001);
					loginResponse = this.m__0001.Post<LoginReqeust, LoginResponse>(request);
				}
				while (6 == 0);
				if (!SA_Proxy._007E_0095___0083(loginResponse))
				{
					throw new AccountResponseException(loginResponse);
				}
				_0001(CurrentUserId, text, loginResponse);
			}
			catch (AccountResponseException)
			{
				return false;
			}
			return !SA_Proxy._001D___0016(CurrentUserToken);
		}

		public IList<Stage> GetStages()
		{
			if (false)
			{
				goto IL_0022;
			}
			GetSubjectsResponse getSubjectsResponse = this.m__0001.Get<GetSubjectsRequest, GetSubjectsResponse>(new GetSubjectsRequest(CurrentUserToken));
			GetSubjectsResponse getSubjectsResponse2;
			if (7u != 0)
			{
				getSubjectsResponse2 = getSubjectsResponse;
			}
			goto IL_004f;
			IL_004f:
			while (getSubjectsResponse2.Succeed)
			{
				if (8 == 0)
				{
					continue;
				}
				return getSubjectsResponse2.Data;
			}
			IList<Stage> result = new List<Stage>();
			goto IL_0022;
			IL_0022:
			if (false)
			{
				goto IL_004f;
			}
			return result;
		}

		public UserSettingInfo GetUserSetting()
		{
			object obj;
			if (0 == 0)
			{
				UserInfo userInfo = this.m__0001._0001(CurrentUserId);
				if (userInfo != null)
				{
					obj = userInfo.Key;
					goto IL_00cf;
				}
			}
			obj = null;
			goto IL_00cf;
			IL_00cf:
			string resourceId = (string)obj;
			GetUserSettingResponse getUserSettingResponse = this.m__0001.Get<GetUserSettingRequest, GetUserSettingResponse>(new GetUserSettingRequest(resourceId, CurrentUserToken));
			if (!SA_Proxy._007E_0095___0083(getUserSettingResponse))
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
			string resourceId = this.m__0001._0001(CurrentUserId)?.Key;
			if (!SA_Proxy._007E_0095___0083(this.m__0001.Post<UpdateUserSettingRequest, UpdateUserSettingResponse>(new UpdateUserSettingRequest(resourceId, CurrentUserToken, userName, userSettingInfo.Stage, userSettingInfo.Subject))))
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
			RegisterResponse registerResponse = this.m__0001.Post<RegisterRequest, RegisterResponse>(new RegisterRequest(userId, password, name, this.m__0001));
			if (!SA_Proxy._007E_0095___0083(registerResponse))
			{
				throw new AccountResponseException(registerResponse);
			}
		}

		public void Login(string userId, string password)
		{
			while (true)
			{
				string text = password;
				if (text == null)
				{
					goto IL_0005;
				}
				goto IL_0029;
				IL_0029:
				password = SA_Helpers.Str(text).ToLower();
				if (0 == 0)
				{
					if (false)
					{
						goto IL_0005;
					}
					string text2 = password;
					if (5u != 0)
					{
						_0001(userId, text2);
					}
				}
				if (0 == 0)
				{
					break;
				}
				goto IL_0005;
				IL_0005:
				if (false)
				{
					continue;
				}
				text = string.Empty;
				goto IL_0029;
			}
		}

		public bool ResetPassowrd(string userId, string dynamic, string password)
		{
			ResetPasswordResponse resetPasswordResponse = default(ResetPasswordResponse);
			int num;
			while (true)
			{
				string text = password;
				if (text == null)
				{
					goto IL_0005;
				}
				goto IL_0049;
				IL_0049:
				password = SA_Helpers.Str(text).ToLower();
				if (-1 != 0)
				{
					resetPasswordResponse = this.m__0001.Post<ResetPasswordRequest, ResetPasswordResponse>(new ResetPasswordRequest(userId, dynamic, password));
				}
				num = (SA_Proxy._007E_0094___0083(resetPasswordResponse) ? 1 : 0);
				if (1 == 0)
				{
					break;
				}
				if (num == 0)
				{
					if (0 == 0)
					{
						return SA_Proxy._007E_0095___0083(resetPasswordResponse);
					}
					continue;
				}
				if (4 != 0)
				{
					num = 0;
					break;
				}
				goto IL_0005;
				IL_0005:
				text = string.Empty;
				goto IL_0049;
			}
			return (byte)num != 0;
		}

		public void Logout()
		{
			while (true)
			{
				CurrentUserResourceId = string.Empty;
				if (4 != 0)
				{
					if (false)
					{
						goto IL_004f;
					}
					CurrentUserId = string.Empty;
				}
				CurrentUserName = string.Empty;
				if (0 == 0)
				{
					CurrentUserToken = string.Empty;
				}
				CurrentPhone = string.Empty;
				goto IL_0039;
				IL_004f:
				if (false)
				{
					continue;
				}
				if (0 == 0)
				{
					break;
				}
				goto IL_0039;
				IL_0039:
				EventHandler eventHandler = this.m__0002;
				if (eventHandler == null)
				{
					break;
				}
				eventHandler(null, EventArgs.Empty);
				goto IL_004f;
			}
		}

		public bool ExistUser(string userId)
		{
			return this.m__0001.Post<CheckUserRequest, CheckUserResponse>(new CheckUserRequest(userId)).StatusCode == AccountResponseCode.UserIdAlreadyExists;
		}

		public bool SendValidateCode(string target, string action)
		{
			SendValidateCodeResponse sendValidateCodeResponse = this.m__0001.Post<SendValidateCodeRequest, SendValidateCodeResponse>(new SendValidateCodeRequest(target, action));
			int num = (SA_Proxy._007E_0094___0083(sendValidateCodeResponse) ? 1 : 0);
			while (true)
			{
				if (num == 0)
				{
					num = (SA_Proxy._007E_0095___0083(sendValidateCodeResponse) ? 1 : 0);
					if (8 != 0)
					{
						return (byte)num != 0;
					}
					continue;
				}
				num = 0;
				break;
			}
			return (byte)num != 0;
		}

		public bool CheckValidateCode(string target, string code)
		{
			CheckValidateCodeResponse checkValidateCodeResponse = this.m__0001.Post<CheckValidateCodeRequest, CheckValidateCodeResponse>(new CheckValidateCodeRequest(target, code));
			int num = (SA_Proxy._007E_0094___0083(checkValidateCodeResponse) ? 1 : 0);
			while (true)
			{
				if (num == 0)
				{
					num = (SA_Proxy._007E_0095___0083(checkValidateCodeResponse) ? 1 : 0);
					if (8 != 0)
					{
						return (byte)num != 0;
					}
					continue;
				}
				num = 0;
				break;
			}
			return (byte)num != 0;
		}

		public string GenerateSeewoToken()
		{
			Func<object, string> obj = (object x) => SA_Helpers.Str(x);
			SA_C3 obj2 = new SA_C3();
			obj2._0001(CurrentUserId);
			obj2._0002(CurrentUserName);
			obj2._0003(this.m__0001._0001(CurrentUserId).Password);
			return obj(obj2);
		}

		public string GenerateSeewoToken(string userId, string password, out string userName)
		{
			password = SA_Helpers.Str(password ?? string.Empty).ToLower();
			LoginReqeust request = new LoginReqeust(userId, password, this.m__0001);
			while (true)
			{
				LoginResponse loginResponse = this.m__0001.Post<LoginReqeust, LoginResponse>(request);
				userName = loginResponse.UserName;
				if (false || SA_Proxy._007E_0095___0083(loginResponse))
				{
					break;
				}
				if (false)
				{
					continue;
				}
				throw new AccountResponseException(loginResponse);
			}
			Func<object, string> obj = (object x) => SA_Helpers.Str(x);
			SA_C3 obj2 = new SA_C3();
			obj2._0001(userId);
			obj2._0002(userName);
			obj2._0003(password);
			return obj(obj2);
		}

		public bool CheckSeewoToken(string token, out string userId, out string userName)
		{
			try
			{
				SA_C3 obj = JsonConvert.DeserializeObject<SA_C3>(token);
				userId = obj._0001();
				if (0 == 0)
				{
					userName = obj._0002();
				}
				if (0 == 0 && 8 != 0)
				{
					return true;
				}
			}
			catch (Exception)
			{
				userId = string.Empty;
				userName = string.Empty;
				return false;
			}
			bool result = default(bool);
			return result;
		}

		public bool BindPhone(string phone, string validateCode)
		{
			BindPhoneOrEmailResponse bindPhoneOrEmailResponse = this.m__0001.Post<BindPhoneOrEmailRequest, BindPhoneOrEmailResponse>(new BindPhoneOrEmailRequest(CurrentUserToken, phone, validateCode, 1));
			if (bindPhoneOrEmailResponse.Succeed)
			{
				CurrentPhone = phone;
			}
			return bindPhoneOrEmailResponse.Succeed;
		}

		public void LoginWithSeewoToken(string token)
		{
			SA_C3 obj = JsonConvert.DeserializeObject<SA_C3>(token);
			_0001(obj._0001(), obj._0003());
		}

		private void _0001()
		{
			this.m__0002 = this.m__0001._0001();
			_0003 = this.m__0001._0002();
			_0004 = this.m__0001._0003();
			_0005 = this.m__0001._0004();
			_0006 = this.m__0001._0005();
			_0007 = this.m__0001._0006();
			_0008 = this.m__0001._0007();
			_000E = this.m__0001._0008();
			_000F = this.m__0001._000E();
		}

		private void _0002()
		{
			if (!this.m__0001)
			{
				_0001();
			}
			this.m__0001 = true;
		}

		protected void OfflineLogin(string userId, string password, LoginResponse response)
		{
			_0001(userId, password, response);
			this.m__0001?.Invoke(null, EventArgs.Empty);
		}

		private void _0001(string P_0, string P_1)
		{
			LoginReqeust request = new LoginReqeust(P_0, P_1, this.m__0001);
			LoginResponse loginResponse = this.m__0001.Post<LoginReqeust, LoginResponse>(request);
			if (!SA_Proxy._007E_0095___0083(loginResponse))
			{
				throw new AccountResponseException(loginResponse);
			}
			_0001(P_0, P_1, loginResponse);
			this.m__0001?.Invoke(null, EventArgs.Empty);
		}

		private void _0001(string P_0, string P_1, LoginResponse P_2)
		{
			UserInfo userInfo = new UserInfo
			{
				Key = P_2.ResourceId,
				Id = P_0,
				Name = P_2.UserName,
				Password = P_1,
				Photo = (P_2.UserPhoto ?? ""),
				TokenId = P_2.TokenId
			};
			TimeSpan timeSpan = SA_Proxy._0001_0002___008C(SA_Proxy._009F___008B(), new DateTime(1970, 1, 1));
			TimeSpan timeSpan2 = default(TimeSpan);
			if (0 == 0)
			{
				timeSpan2 = timeSpan;
			}
			userInfo.LastTime = (long)timeSpan2.TotalMilliseconds;
			UserInfo userInfo2 = userInfo;
			this.m__0001._0001(userInfo2);
			CurrentUserResourceId = userInfo2.Key;
			CurrentUserId = userInfo2.Id;
			CurrentUserName = userInfo2.Name;
			CurrentUserToken = userInfo2.TokenId;
			CurrentPhone = P_2.Phone;
			UserSettingInfo userSetting = GetUserSetting();
			CurrentStage = userSetting.Stage;
			CurrentSubject = userSetting.Subject;
			CurrentStageName = userSetting.StageName;
			CurrentSubjectName = userSetting.SubjectName;
			this.m__0001 = true;
			OnUserLogining(userInfo2.Id, userInfo2.Name);
		}

		protected virtual void OnUserLogining(string userId, string userName)
		{
		}

		private static string _0001(string P_0)
		{
			if (!SA_Proxy._001E___0016(P_0))
			{
				try
				{
					SA_Proxy._0002_0002___008D(P_0);
				}
				catch (IOException)
				{
				}
				return P_0;
			}
			return P_0;
		}
	}
}
