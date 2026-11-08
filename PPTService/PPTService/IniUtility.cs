using System.Runtime.InteropServices;
using System.Text;

namespace PPTService
{
	public static class IniUtility
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

		public static void Write(string section, string key, string value, string filePath)
		{
			WritePrivateProfileString(section, key, value, filePath);
		}

		public static string Read(string section, string key, string filePath)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			GetPrivateProfileString(section, key, "", stringBuilder, 1024, filePath);
			return stringBuilder.ToString();
		}
	}
}
