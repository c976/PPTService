using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Cvte.WebService.Utils;
using LinqToDB;
using Cvte.WebService.Internal;

namespace Cvte.WebService.Account
{
	internal class AccountCache : IAccountCache
	{
		private sealed class Class23
		{
			public string string_0;

			internal UserInfo method_0(UserDB userDB_0)
			{
				return (from u in (IQueryable<UserEntity>)userDB_0.method_0()
					where u.method_2() == string_0
					select new UserInfo
					{
						Name = u.method_8(),
						Id = u.method_2(),
						Key = u.method_0(),
						Password = DesSecurityUtility.smethod_4(u.method_4(), "EasiUser"),
						Photo = u.method_6(),
						TokenId = u.method_10(),
						LastTime = (u.method_12() ?? 0L)
					}).FirstOrDefault();
			}
		}

		private sealed class Class24
		{
			public UserInfo userInfo_0;

			internal void method_0(UserDB userDB_0)
			{
				UserEntity @class = new UserEntity();
				@class.method_1(userInfo_0.Key);
				@class.method_9(userInfo_0.Name);
				@class.method_3(userInfo_0.Id);
				@class.method_5(DesSecurityUtility.smethod_1(userInfo_0.Password, "EasiUser"));
				@class.method_7(userInfo_0.Photo);
				@class.method_11(userInfo_0.TokenId);
				@class.method_13(userInfo_0.LastTime);
				DataExtensions.InsertOrReplace<UserEntity>((IDataContext)(object)userDB_0, @class, (string)null, (string)null, (string)null, (string)null, (TableOptions)0);
			}
		}

		private sealed class Class25
		{
			public string string_0;

			internal void method_0(UserDB userDB_0)
			{
				LinqExtensions.Delete<UserEntity>((IQueryable<UserEntity>)userDB_0.method_0(), (Expression<Func<UserEntity, bool>>)((UserEntity u) => u.method_2() == string_0));
			}
		}

		private readonly DBCache<UserDB> dbcache_0;

		public AccountCache(string string_0, string string_1)
		{
			string text = string_0 + "\\User.db";
			if (!File.Exists(text))
			{
				File.Copy(Path.Combine(string_1, "User.db"), text);
			}
			dbcache_0 = new DBCache<UserDB>("Data Source=" + text);
		}

		public UserInfo method_0(string string_0)
		{
			return dbcache_0.Execute((UserDB userDB_0) => (from u in (IQueryable<UserEntity>)userDB_0.method_0()
				where u.method_2() == string_0
				select new UserInfo
				{
					Name = u.method_8(),
					Id = u.method_2(),
					Key = u.method_0(),
					Password = DesSecurityUtility.smethod_4(u.method_4(), "EasiUser"),
					Photo = u.method_6(),
					TokenId = u.method_10(),
					LastTime = (u.method_12() ?? 0L)
				}).FirstOrDefault());
		}

		public IList<UserInfo> GetAllUserInfo()
		{
			return dbcache_0.Execute((UserDB userDB_0) => (from u in (IQueryable<UserEntity>)userDB_0.method_0()
				select (u) into u
				where u.method_12() != (long?)null
				orderby u.method_12() descending
				select new UserInfo
				{
					Id = u.method_2(),
					Name = u.method_8(),
					Photo = u.method_6()
				}).ToList());
		}

		public void method_1(UserInfo userInfo_0)
		{
			dbcache_0.Execute((UserDB userDB_0) =>
			{
				UserEntity @class = new UserEntity();
				@class.method_1(userInfo_0.Key);
				@class.method_9(userInfo_0.Name);
				@class.method_3(userInfo_0.Id);
				@class.method_5(DesSecurityUtility.smethod_1(userInfo_0.Password, "EasiUser"));
				@class.method_7(userInfo_0.Photo);
				@class.method_11(userInfo_0.TokenId);
				@class.method_13(userInfo_0.LastTime);
				DataExtensions.InsertOrReplace<UserEntity>((IDataContext)(object)userDB_0, @class, (string)null, (string)null, (string)null, (string)null, (TableOptions)0);
			});
		}

		public void Remove(string userId)
		{
			dbcache_0.Execute((UserDB userDB_0) =>
			{
				LinqExtensions.Delete<UserEntity>((IQueryable<UserEntity>)userDB_0.method_0(), (Expression<Func<UserEntity, bool>>)((UserEntity u) => u.method_2() == userId));
			});
		}
	}
}
