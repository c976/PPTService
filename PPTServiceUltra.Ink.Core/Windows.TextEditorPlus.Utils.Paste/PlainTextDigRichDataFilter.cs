using System.Collections.Generic;
using System.Text.RegularExpressions;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Paste
{
	internal class PlainTextDigRichDataFilter : DigRichDataFilter
	{
		public override bool Handle(DataObjectContext dataObject)
		{
			string text = dataObject.UnicodeText;
			if (string.IsNullOrEmpty(text))
			{
				text = dataObject.Text;
				if (string.IsNullOrEmpty(text))
				{
					return false;
				}
			}
			RichTextProperty richTextProperty = HandlePlainText(text);
			dataObject.RichTextProperty = richTextProperty;
			return true;
		}

		private static RichTextProperty HandlePlainText(string text)
		{
			RichTextProperty richTextProperty = new RichTextProperty();
			if (text != null)
			{
				List<ICharObject> list = new List<ICharObject>();
				for (int i = 0; i < text.Length; i++)
				{
					string text2 = text[i].ToString();
					if (Regex.IsMatch(text2, "[\\ud800-\\udfff]"))
					{
						if (i + 1 < text.Length)
						{
							string text3 = text[i + 1].ToString();
							list.Add(new TextChar(text2 + text3));
							i++;
						}
					}
					else
					{
						list.Add(new TextChar(text2));
					}
				}
				richTextProperty.CharObjects = list;
			}
			return richTextProperty;
		}
	}
}
