using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace PPTServiceUltra.Ink.Core.Cli.Utils
{
	
	
	internal static class NamingHelper
	{
		internal static bool CheckIsPascalCase(string value)
		{
			if (char.IsLower(value[0]))
			{
				return false;
			}
			string b = MakePascalCase(value);
			return string.Equals(value, b, StringComparison.Ordinal);
		}

		internal static string MakePascalCase(string oldName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			bool flag2 = true;
			foreach (char c in oldName)
			{
				if (!char.IsLetterOrDigit(c))
				{
					flag2 = true;
				}
				else if (flag)
				{
					if (char.IsDigit(c))
					{
						flag2 = true;
					}
					else if (char.IsLower(c))
					{
						flag = false;
						flag2 = false;
						stringBuilder.Append(char.ToUpperInvariant(c));
					}
					else if (char.IsUpper(c))
					{
						flag = false;
						flag2 = false;
						stringBuilder.Append(c);
					}
					else
					{
						flag = false;
						flag2 = true;
						stringBuilder.Append(c);
					}
				}
				else if (char.IsDigit(c))
				{
					flag2 = true;
					stringBuilder.Append(c);
				}
				else if (char.IsLower(c))
				{
					stringBuilder.Append(flag2 ? char.ToUpperInvariant(c) : c);
					flag2 = false;
				}
				else if (char.IsUpper(c))
				{
					flag2 = false;
					stringBuilder.Append(c);
				}
				else
				{
					flag2 = true;
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string MakeKebabCase(string oldName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			bool flag2 = false;
			foreach (char c in oldName)
			{
				if (!char.IsLetterOrDigit(c))
				{
					flag2 = false;
				}
				else if (flag)
				{
					if (char.IsDigit(c))
					{
						flag2 = false;
					}
					else if (char.IsUpper(c))
					{
						flag = false;
						flag2 = true;
						stringBuilder.Append(char.ToLowerInvariant(c));
					}
					else if (char.IsLower(c))
					{
						flag = false;
						flag2 = true;
						stringBuilder.Append(c);
					}
					else
					{
						flag = false;
						flag2 = false;
						stringBuilder.Append(c);
					}
				}
				else if (char.IsDigit(c))
				{
					flag2 = false;
					stringBuilder.Append(c);
				}
				else if (char.IsUpper(c))
				{
					stringBuilder.Append('-');
					stringBuilder.Append(char.ToLowerInvariant(c));
					flag2 = true;
				}
				else if (char.IsLower(c))
				{
					stringBuilder.Append(c);
					flag2 = true;
				}
				else
				{
					if (flag2)
					{
						stringBuilder.Append('-');
					}
					stringBuilder.Append(c);
					flag2 = false;
				}
			}
			return stringBuilder.ToString();
		}
	}
}
