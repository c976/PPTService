using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils.Paste
{
	internal class RtfDigRichDataFilter : DigRichDataFilter
	{
		public override bool Handle(DataObjectContext dataObject)
		{
			string rtf = dataObject.Rtf;
			if (string.IsNullOrEmpty(rtf))
			{
				return false;
			}
			rtf = EnSureRtfStringValid(rtf);
			RichTextProperty richTextProperty = rtf.ToFlowDocument().ToRichTextInfo().ToRichTextProperty();
			if (CheckProperty(richTextProperty, dataObject))
			{
				dataObject.RichTextProperty = richTextProperty;
				return true;
			}
			TextTracer.Debug("判断粘贴的文本和纯文本不相同，将跳过带格式文本粘贴为纯文本", "文本粘贴");
			return false;
		}

		private bool CheckProperty(RichTextProperty property, DataObjectContext dataObject)
		{
			string text = dataObject.UnicodeText;
			if (string.IsNullOrEmpty(text))
			{
				text = dataObject.Text;
				if (string.IsNullOrEmpty(text))
				{
					return true;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (property.CharObjects.Count == 0 && text.Length > 0)
			{
				return false;
			}
			foreach (ICharObject charObject in property.CharObjects)
			{
				stringBuilder.Append(charObject.ToText());
			}
			string str = stringBuilder.ToString();
			str = CleanFormat(str);
			text = CleanFormat(text);
			if (!str.Contains(text))
			{
				return text.Contains(str);
			}
			return true;
		}

		private string CleanFormat(string str)
		{
			str = CleanWPSMarkerString(str);
			return new string[4] { "\r", "\n", "\t", " " }.Aggregate(str, (string current, string temp) => current.Replace(temp, string.Empty));
		}

		private static string CleanWPSMarkerString(string str)
		{
			return GetWPSMarkerString().Aggregate(str, (string current, string temp) => current.Replace(temp, string.Empty));
		}

		private static IEnumerable<string> GetWPSMarkerString()
		{
			for (char i = '\uf03c'; i < '\uf104'; i = (char)(i + 1))
			{
				yield return i.ToString();
			}
		}

		private static string EnSureRtfStringValid(string rtfString)
		{
			int num = rtfString.LastIndexOf("}", StringComparison.Ordinal);
			if (num <= 0)
			{
				return rtfString;
			}
			return rtfString.Remove(num);
		}
	}
}
