using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal static class TextFileWriter
	{
		private static readonly Dictionary<string, string> EscapedReplace = new Dictionary<string, string>
		{
			{
				"\v",
				Environment.NewLine
			},
			{ "\a", "" },
			{ "\b", "" },
			{ "\f", "" },
			{ "\0", "" }
		};

		internal static void WriteAllXmlTextSafety(string filePath, string content)
		{
			string contents = SafetyContent(content);
			File.WriteAllText(filePath, contents, Encoding.UTF8);
		}

		internal static void WriteAllTextSafety(string filePath, string content)
		{
			WriteAllTextSafety(filePath, content, Encoding.UTF8);
		}

		internal static void WriteAllTextSafety(string filePath, string content, Encoding encoding)
		{
			File.WriteAllText(filePath, SafetyContent(content), encoding);
		}

		internal static string SafetyContent(string content)
		{
			string text = Regex.Replace(content, "[\\x00-\\x08]|[\\x0B-\\x0C]|[\\x0E-\\x1F]", " ");
			foreach (string key in EscapedReplace.Keys)
			{
				if (text.Contains(key))
				{
					text = text.Replace(key, EscapedReplace[key]);
				}
			}
			return text;
		}
	}
}
