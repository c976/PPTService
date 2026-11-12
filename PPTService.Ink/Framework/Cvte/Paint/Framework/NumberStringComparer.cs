using System.Text.RegularExpressions;

namespace Cvte.Paint.Framework
{
	internal static class NumberStringComparer
	{
		public static int CompareNumber(string content, string otherContent)
		{
			string pattern = "\\d+";
			double num;
			if (TryGetDouble(content, out num, pattern) && TryGetDouble(otherContent, out num, pattern))
			{
				int num2 = ((content.Length > otherContent.Length) ? otherContent.Length : content.Length);
				pattern = "^-?\\d+(\\.\\d+)?";
				for (int i = 0; i < num2; i++)
				{
					string content2 = content.Substring(i, content.Length - i);
					string content3 = otherContent.Substring(i, otherContent.Length - i);
					double num3;
					double num4;
					if (TryGetDouble(content2, out num3, pattern) && TryGetDouble(content3, out num4, pattern))
					{
						if (num3 > num4)
						{
							return 1;
						}
						if (num3 < num4)
						{
							return -1;
						}
					}
					else if (content[i] != otherContent[i])
					{
						return 0;
					}
				}
			}
			return 0;
		}

		private static bool TryGetDouble(string content, out double num, string pattern)
		{
			num = 0.0;
			MatchCollection matchCollection = Regex.Matches(content, pattern);
			if (matchCollection.Count <= 0)
			{
				return false;
			}
			if (!matchCollection[0].Success)
			{
				return false;
			}
			if (!double.TryParse(matchCollection[0].Value, out num))
			{
				return false;
			}
			return true;
		}
	}
}
