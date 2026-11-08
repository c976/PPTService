using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Cvte.Net
{
	internal class RegexUtilities
	{
		
		public static bool IsValidPhoneNumber(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return false;
			}
			return Regex.IsMatch(str, "^[0-9]{11,11}$");
		}
	}
}
