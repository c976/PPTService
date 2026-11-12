using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PPTService.Core.WebService.Internal
{
	internal class NativeIniFile
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		private static extern long WritePrivateProfileString(string string_0, string string_1, string string_2, string string_3);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetPrivateProfileString(string string_0, string string_1, string string_2, StringBuilder stringBuilder_0, int int_0, string string_3);

		public static void smethod_0(string string_0, string string_1, string string_2, string string_3)
		{
			try
			{
				if (!File.Exists(string_3))
				{
					File.WriteAllText(string_3, "", Encoding.Unicode);
				}
			}
			catch (Exception)
			{
			}
			WritePrivateProfileString(string_0, string_1, string_2, string_3);
		}

		public static string smethod_1(string string_0, string string_1, string string_2)
		{
			StringBuilder stringBuilder = new StringBuilder(2048);
			GetPrivateProfileString(string_0, string_1, "", stringBuilder, 2048, string_2);
			return stringBuilder.ToString();
		}
	}
}
