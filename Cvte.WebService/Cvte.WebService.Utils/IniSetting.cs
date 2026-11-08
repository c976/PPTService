using System;
using System.IO;
using _0002;

namespace Cvte.WebService.Utils
{
	internal abstract class IniSetting
	{
		private readonly string m__0001;

		private readonly string _0002;

		protected IniSetting(string P_0, string P_1)
		{
			_0002 = P_0;
			this.m__0001 = P_1;
		}

		public dynamic _0001;
		public _0001 _0001_M1<_0001>(string P_0, _0001 P_1, Func<string, _0001, _0001> P_2, string P_3, string P_4)
		{
			string text = SA_Helpers.Str(P_3, P_0, _0002);
			if (!string.IsNullOrEmpty(text))
			{
				return P_2(text, P_1);
			}
			return P_1;
		}

		public string _0001_M2(string P_0, string P_1, string P_2, string P_3)
		{
			return _0001(P_0, P_1, (Func<string, string, string>)((Func<string, string, string>)((string text, string text2) => (!SA_Proxy._001D___0016(text)) ? text : text2)), P_2, P_3);
		}

		public string _0001_M3(string P_0, string P_1)
		{
			return this._0001(P_0, P_1, this.m__0001, _0002);
		}

		public void _0001_M4<_0001>(string P_0, _0001 P_1, Func<_0001, string> P_2, string P_3, string P_4)
		{
			DirectoryInfo parent = new DirectoryInfo(_0002).Parent;
			if (false || parent != null)
			{
				while (!parent.Exists)
				{
					if (1 == 0)
					{
						continue;
					}
					Directory.CreateDirectory(parent.FullName);
					break;
				}
			}
			do
			{
				SA_Helpers.Str(P_3, P_0, P_2(P_1), _0002);
			}
			while (2 == 0);
		}

		public void _0001_M5(string P_0, string P_1, string P_2, string P_3)
		{
			do
			{
				if (-1 != 0)
				{
					_0001(P_0, P_1, (Func<string, string>)((Func<string, string>)((string result) => result)), P_2, P_3);
				}
			}
			while (false || false || 5 == 0);
		}

		public void _0001_M6(string P_0, string P_1)
		{
			this._0001(P_0, P_1, this.m__0001, _0002);
		}
	}
}
