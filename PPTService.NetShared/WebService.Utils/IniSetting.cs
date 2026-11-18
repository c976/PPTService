using System;
using System.IO;
using PPTService.Core.WebService.Internal;

namespace PPTService.Core.WebService.Utils
{
	internal abstract class IniSetting
	{
		private readonly string string_0;

		private readonly string string_1;

		protected IniSetting(string string_2, string string_3)
		{
			string_1 = string_2;
			string_0 = string_3;
		}

		public T method_0<T>(string string_2, T gparam_0, Func<string, T, T> func_0, string string_3, string string_4)
		{
			string text = NativeIniFile.smethod_1(string_3, string_2, string_1);
			if (!string.IsNullOrEmpty(text))
			{
				return func_0(text, gparam_0);
			}
			return gparam_0;
		}

		public string method_1(string string_2, string string_3, string string_4, string string_5)
		{
			return method_0(string_2, string_3, (string text, string text2) => (!string.IsNullOrEmpty(text)) ? text : text2, string_4, string_5);
		}

		public string method_2(string string_2, string string_3)
		{
			return method_1(string_2, string_3, string_0, string_1);
		}

		public void method_3<T>(string string_2, T gparam_0, Func<T, string> func_0, string string_3, string string_4)
		{
			DirectoryInfo parent = new DirectoryInfo(string_1).Parent;
			if (parent != null && !parent.Exists)
			{
				Directory.CreateDirectory(parent.FullName);
			}
			NativeIniFile.smethod_0(string_3, string_2, func_0(gparam_0), string_1);
		}

		public void method_4(string string_2, string string_3, string string_4, string string_5)
		{
			method_3(string_2, string_3, (string result) => result, string_4, string_5);
		}

		public void method_5(string string_2, string string_3)
		{
			method_4(string_2, string_3, string_0, string_1);
		}
	}
}
