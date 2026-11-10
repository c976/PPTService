using System.Text;
using System.Web;

namespace Cvte.WebService.Internal
{
	internal static class SignatureUrlEncoder
	{
		public static string smethod_0(string string_0, Encoding encoding_0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < string_0.Length; i++)
			{
				string text = string_0[i].ToString();
				string text2 = HttpUtility.UrlEncode(text, encoding_0);
				switch (text)
				{
				case "'":
					text2 = "'";
					break;
				case "~":
					text2 = "~";
					break;
				case " ":
					text2 = "%20";
					break;
				case "+":
					text2 = "%2b";
					break;
				}
				stringBuilder.Append((text == text2) ? text : text2?.ToUpper());
			}
			return stringBuilder.ToString();
		}
	}
}
