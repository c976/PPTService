using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PPTService.Ink.Core.Configurations.Core
{
	
	
	public static class CoinConfigurationSerializer
	{
		private static string _splitString = ">";

		private static string _escapeString = "?";

		private static string EscapeString(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			string splitString = _splitString;
			string escapeString = _escapeString;
			if (str.StartsWith(splitString, StringComparison.Ordinal))
			{
				return _escapeString + str;
			}
			if (str.StartsWith(escapeString, StringComparison.Ordinal))
			{
				return _escapeString + str;
			}
			return str;
		}

		public static string Serialize( IReadOnlyDictionary<string, string> keyValue)
		{
			if (keyValue == null)
			{
				throw new ArgumentNullException("keyValue");
			}
			return Serialize(from p in keyValue.ToArray()
				orderby p.Key
				select p);
		}

		public static string Serialize( Dictionary<string, string> keyValue)
		{
			if (keyValue == null)
			{
				throw new ArgumentNullException("keyValue");
			}
			return Serialize(from p in keyValue.ToArray()
				orderby p.Key
				select p);
		}

		private static string Serialize( IOrderedEnumerable<KeyValuePair<string, string>> keyValuePairList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("> 配置文件\n");
			stringBuilder.Append("> 版本 1.0\n");
			foreach (KeyValuePair<string, string> keyValuePair in keyValuePairList)
			{
				stringBuilder.Append(EscapeString(keyValuePair.Key ?? ""));
				stringBuilder.Append("\n");
				stringBuilder.Append(EscapeString(keyValuePair.Value ?? ""));
				stringBuilder.Append("\n>\n");
			}
			stringBuilder.Append("> 配置文件结束");
			return stringBuilder.ToString();
		}

		public static Dictionary<string, string> Deserialize(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			string[] source = str.Split('\n');
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
			string text = null;
			string splitString = _splitString;
			foreach (string item in source.Select( (string temp) => temp.Trim()))
			{
				if (item.StartsWith(splitString, StringComparison.Ordinal))
				{
					text = null;
					continue;
				}
				string text2 = UnescapeString(item);
				if (text == null)
				{
					text = text2;
					if (dictionary.ContainsKey(text))
					{
						dictionary.Remove(text);
					}
				}
				else if (dictionary.ContainsKey(text))
				{
					dictionary[text] = dictionary[text] + "\n" + text2;
				}
				else
				{
					dictionary.Add(text, text2);
				}
			}
			return dictionary;
		}

		private static string UnescapeString(string str)
		{
			string escapeString = _escapeString;
			if (str.StartsWith(escapeString, StringComparison.Ordinal))
			{
				return str.Substring(1);
			}
			return str;
		}
	}
}
