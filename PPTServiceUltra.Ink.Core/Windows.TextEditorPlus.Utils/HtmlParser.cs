using System;
using System.Globalization;
using System.Text.RegularExpressions;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	[Obsolete("还没写好")]
	internal class HtmlParser
	{
		private class ClipboardHtmlOutput
		{
			private int _startHTML;

			private int _endHTML;

			private int _startFragment;

			private int _endFragment;

			public double Version { get; private set; }

			public string Input { get; private set; }

			public string Html => Input.Substring(_startHTML, Math.Min(_endHTML - _startHTML, Input.Length - _startHTML));

			public string Fragment => Input.Substring(_startFragment, _endFragment - _startFragment);

			public static ClipboardHtmlOutput ParseString(string s)
			{
				string input = s.Replace("\r\n", "\n");
				ClipboardHtmlOutput clipboardHtmlOutput = new ClipboardHtmlOutput();
				Match match = new Regex("Version:(?<version>[0-9]+(?:\\.[0-9]*)?)[.\\n]StartHTML:(?<startH>\\d*)[.\\n]EndHTML:(?<endH>\\d*)[.\\n]StartFragment:(?<startF>\\d+)[.\\n]EndFragment:(?<endF>\\d*)[.\\n]", RegexOptions.Multiline).Match(input);
				if (match.Success)
				{
					try
					{
						clipboardHtmlOutput.Input = s;
						clipboardHtmlOutput.Version = double.Parse(match.Groups["version"].Value, CultureInfo.InvariantCulture);
						clipboardHtmlOutput._startHTML = int.Parse(match.Groups["startH"].Value);
						clipboardHtmlOutput._endHTML = int.Parse(match.Groups["endH"].Value);
						clipboardHtmlOutput._startFragment = int.Parse(match.Groups["startF"].Value);
						clipboardHtmlOutput._endFragment = int.Parse(match.Groups["endF"].Value);
						return clipboardHtmlOutput;
					}
					catch (Exception exception)
					{
						TextTracer.Error(exception, "粘贴html内容");
						return null;
					}
				}
				return null;
			}
		}

		public static RichTextProperty ParseDataObject(string html)
		{
			ClipboardHtmlOutput.ParseString(html);
			return null;
		}
	}
}
