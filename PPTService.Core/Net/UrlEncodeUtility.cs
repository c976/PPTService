using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace PPTService.Core.Net
{
	internal static class UrlEncodeUtility
	{
		
		public static string JavaUrlEncode(string value, Encoding encoding)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				string text = value[i].ToString();
				string text2 = System.Web.HttpUtility.UrlEncode(text, encoding);
				switch (text)
				{
				case "+":
					text2 = "%2b";
					break;
				case " ":
					text2 = "%20";
					break;
				case "~":
					text2 = "~";
					break;
				case "'":
					text2 = "'";
					break;
				}
				stringBuilder.Append((text == text2) ? text : text2?.ToUpper());
			}
			return stringBuilder.ToString();
		}
	}
}
