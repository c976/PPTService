using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using _0002;
using Cvte.WebService.Utils;
using LinqToDB;

namespace Cvte.WebService.Account
{
	internal class AccountCache : IAccountCache
	{
		[CompilerGenerated]
		private sealed class SA_C2
		{
			public dynamic _0001;

			internal UserInfo _0001_M2(UserDB P_0)
			{
				ITable<global::_0002._0002> source = P_0._0001();
				ParameterExpression parameterExpression = SA_Proxy._000E_0002___0094(Type.GetTypeFromHandle(typeof(global::_0002._0002).TypeHandle), "u");
				IQueryable<global::_0002._0002> source2 = source.Where(Expression.Lambda<Func<global::_0002._0002, bool>>(SA_Proxy._0014_0002___009A(SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null)), SA_Proxy._0013_0002___0099(SA_Proxy._0011_0002___0097(this, Type.GetTypeFromHandle(typeof(object).TypeHandle)), SA_Proxy._0012_0002___0098(null))), new ParameterExpression[1] { parameterExpression }));
				parameterExpression = SA_Proxy._000E_0002___0094(Type.GetTypeFromHandle(typeof(global::_0002._0002).TypeHandle), "u");
				return source2.Select(Expression.Lambda<Func<global::_0002._0002, UserInfo>>(SA_Proxy._0019_0002___009E(SA_Proxy._0016_0002___009B(Type.GetTypeFromHandle(typeof(UserInfo).TypeHandle)), new MemberBinding[7]
				{
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null))),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null))),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null))),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0018_0002___009D(null, (MethodInfo)SA_Proxy._000F_0002___0095(null), new Expression[2]
					{
						SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null)),
						SA_Proxy._0011_0002___0097("EasiUser", Type.GetTypeFromHandle(typeof(string).TypeHandle))
					})),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null))),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null))),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0015_0002___009A(SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null)), SA_Proxy._0011_0002___0097(0L, Type.GetTypeFromHandle(typeof(long).TypeHandle))))
				}), new ParameterExpression[1] { parameterExpression })).FirstOrDefault();
			}
		}

		[CompilerGenerated]
		private sealed class _0002
		{
			public dynamic _0001;

			internal void _0001_M2(UserDB P_0)
			{
				global::_0002._0002 obj = new global::_0002._0002();
				obj._0001(this._0001.Key);
				obj._0005(this._0001.Name);
				obj._0002_Set(this._0001.Id);
				obj._0003(DesSecurityUtility._0001(this._0001.Password, "EasiUser"));
				obj._0004(this._0001.Photo);
				obj._0006(this._0001.TokenId);
				obj._0001(this._0001.LastTime);
				P_0.InsertOrReplace(obj);
			}
		}

		[CompilerGenerated]
		private sealed class _0003
		{
			public dynamic _0001;

			internal void _0001_M2(UserDB P_0)
			{
				ITable<global::_0002._0002> source = P_0._0001();
				ParameterExpression parameterExpression = SA_Proxy._000E_0002___0094(Type.GetTypeFromHandle(typeof(global::_0002._0002).TypeHandle), "u");
				source.Delete(Expression.Lambda<Func<global::_0002._0002, bool>>(SA_Proxy._0014_0002___009A(SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null)), SA_Proxy._0013_0002___0099(SA_Proxy._0011_0002___0097(this, Type.GetTypeFromHandle(typeof(object).TypeHandle)), SA_Proxy._0012_0002___0098(null))), new ParameterExpression[1] { parameterExpression }));
			}
		}

		private readonly DBCache<UserDB> m__0001;

		public AccountCache(string P_0, string P_1)
		{
			string text = P_0 + "\\User.db";
			if (!File.Exists(text))
			{
				File.Copy(Path.Combine(P_1, "User.db"), text);
			}
			this.m__0001 = new DBCache<UserDB>("Data Source=" + text);
		}

		public UserInfo _0001(string P_0)
		{
			SA_C2 CS_0024_003C_003E8__locals2 = new SA_C2();
			CS_0024_003C_003E8__locals2._0001 = P_0;
			return this.m__0001.Execute((UserDB userDB) =>
			{
				ITable<global::_0002._0002> source = userDB._0001();
				ParameterExpression parameterExpression = SA_Proxy._000E_0002___0094(Type.GetTypeFromHandle(typeof(global::_0002._0002).TypeHandle), "u");
				IQueryable<global::_0002._0002> source2 = source.Where(Expression.Lambda<Func<global::_0002._0002, bool>>(SA_Proxy._0014_0002___009A(SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null)), SA_Proxy._0013_0002___0099(SA_Proxy._0011_0002___0097(CS_0024_003C_003E8__locals2, Type.GetTypeFromHandle(typeof(object).TypeHandle)), SA_Proxy._0012_0002___0098(null))), new ParameterExpression[1] { parameterExpression }));
				parameterExpression = SA_Proxy._000E_0002___0094(Type.GetTypeFromHandle(typeof(global::_0002._0002).TypeHandle), "u");
				return source2.Select(Expression.Lambda<Func<global::_0002._0002, UserInfo>>(SA_Proxy._0019_0002___009E(SA_Proxy._0016_0002___009B(Type.GetTypeFromHandle(typeof(UserInfo).TypeHandle)), new MemberBinding[7]
				{
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null))),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null))),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null))),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0018_0002___009D(null, (MethodInfo)SA_Proxy._000F_0002___0095(null), new Expression[2]
					{
						SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null)),
						SA_Proxy._0011_0002___0097("EasiUser", Type.GetTypeFromHandle(typeof(string).TypeHandle))
					})),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null))),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null))),
					SA_Proxy._0017_0002___009C((MethodInfo)SA_Proxy._000F_0002___0095(null), SA_Proxy._0015_0002___009A(SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null)), SA_Proxy._0011_0002___0097(0L, Type.GetTypeFromHandle(typeof(long).TypeHandle))))
				}), new ParameterExpression[1] { parameterExpression })).FirstOrDefault();
			});
		}

		public IList<UserInfo> GetAllUserInfo()
		{
			return this.m__0001.Execute((UserDB P_0) =>
			{
				ITable<global::_0002._0002> source = P_0._0001();
				ParameterExpression parameterExpression = Expression.Parameter(typeof(global::_0002._0002), "u");
				ParameterExpression parameterExpression2 = default(ParameterExpression);
				if (0 == 0)
				{
					parameterExpression2 = parameterExpression;
				}
				return (from u in source.Select(Expression.Lambda<Func<global::_0002._0002, global::_0002._0002>>(parameterExpression2, new ParameterExpression[1] { parameterExpression2 }))
					where u._0001() != (long?)null
					orderby u._0001() descending
					select new UserInfo
					{
						Id = u._0002_Get(),
						Name = u._0005(),
						Photo = u._0004()
					}).ToList();
			});
		}

		public void _0001(UserInfo P_0)
		{
			if (0 == 0)
			{
			}
			UserInfo userInfo;
			do
			{
				userInfo = P_0;
				this.m__0001.Execute((UserDB dataContext) =>
				{
					global::_0002._0002 obj = new global::_0002._0002();
					obj._0001(userInfo.Key);
					obj._0005(userInfo.Name);
					obj._0002_Set(userInfo.Id);
					obj._0003(DesSecurityUtility._0001(userInfo.Password, "EasiUser"));
					obj._0004(userInfo.Photo);
					obj._0006(userInfo.TokenId);
					obj._0001(userInfo.LastTime);
					dataContext.InsertOrReplace(obj);
				});
			}
			while (false);
		}

		public void Remove(string P_0)
		{
			_0003 CS_0024_003C_003E8__locals2;
			if (0 == 0)
			{
				CS_0024_003C_003E8__locals2 = new _0003();
			}
			do
			{
				CS_0024_003C_003E8__locals2._0001 = P_0;
				this.m__0001.Execute((UserDB userDB) =>
				{
					ITable<global::_0002._0002> source = userDB._0001();
					ParameterExpression parameterExpression = SA_Proxy._000E_0002___0094(Type.GetTypeFromHandle(typeof(global::_0002._0002).TypeHandle), "u");
					source.Delete(Expression.Lambda<Func<global::_0002._0002, bool>>(SA_Proxy._0014_0002___009A(SA_Proxy._0010_0002___0096(parameterExpression, (MethodInfo)SA_Proxy._000F_0002___0095(null)), SA_Proxy._0013_0002___0099(SA_Proxy._0011_0002___0097(CS_0024_003C_003E8__locals2, Type.GetTypeFromHandle(typeof(object).TypeHandle)), SA_Proxy._0012_0002___0098(null))), new ParameterExpression[1] { parameterExpression }));
				});
			}
			while (false);
		}
	}
}
