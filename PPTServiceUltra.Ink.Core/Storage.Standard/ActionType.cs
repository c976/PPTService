using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public abstract class ActionType : IStorableType
	{
		public string Encode()
		{
			Dictionary<string, string> dictionary = (from p in GetType().GetProperties()
				where p.IsDefined(typeof(ActionParameterAttribute), false)
				select p).ToDictionary( (PropertyInfo p) => p.GetCustomAttribute<ActionParameterAttribute>().Name,  (PropertyInfo p) => Convert.ToString(p.GetValue(this, null), CultureInfo.InvariantCulture));
			string key = GetType().GetCustomAttribute<ActionTypeAttribute>().Key;
			string text = "swaction://" + key;
			if (!dictionary.Any())
			{
				return text;
			}
			string text2 = "";
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				text2 = ((!string.IsNullOrEmpty(text2)) ? (text2 + "&" + item.Key + "=" + item.Value) : (text2 + item.Key + "=" + item.Value));
			}
			return text + "?" + text2;
		}

		public static string DecodeName(string value)
		{
			Match match = new Regex("swaction://(?'Name'[^\\?]+)").Match(value);
			if (match.Success)
			{
				return match.Groups["Name"].Value;
			}
			return string.Empty;
		}

		public StorableNode Deparse()
		{
			return new StorableNode
			{
				Value = Encode()
			};
		}

		public void Parse(StorableNode node)
		{
			foreach (Match match in new Regex("(?'Key'[a-zA-Z0-9]+?)=(?'Value'[^&]+)").Matches(node.StringValue))
			{
				GetType().GetProperties().FirstOrDefault( (PropertyInfo p) =>
				{
					ActionParameterAttribute customAttribute = p.GetCustomAttribute<ActionParameterAttribute>();
					return customAttribute != null && customAttribute.Name == match.Groups["Key"].Value;
				})?.SetValue(this, match.Groups["Value"].Value);
			}
		}
	}
}
