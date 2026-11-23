using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal static class TextDecorationCollectionExtensions
	{
		internal static PPTServiceUltra.Ink.Core.Storage.Standard.TextDecorationCollection ToStorageTextDecorationCollection(this System.Windows.TextDecorationCollection textDecorationCollection)
		{
			if (textDecorationCollection.Count == 0)
			{
				return new PPTServiceUltra.Ink.Core.Storage.Standard.TextDecorationCollection();
			}
			if (textDecorationCollection.Count == 1)
			{
				string value = ConvertToStorageText(textDecorationCollection[0]);
				PPTServiceUltra.Ink.Core.Storage.Standard.TextDecorationCollection textDecorationCollection2 = new PPTServiceUltra.Ink.Core.Storage.Standard.TextDecorationCollection();
				textDecorationCollection2.TryDecode(value);
				return textDecorationCollection2;
			}
			List<string> list = new List<string>(textDecorationCollection.Count);
			if (Converter.Contains(textDecorationCollection, InnerTextContext.Underline))
			{
				list.Add("Underline");
			}
			if (Converter.Contains(textDecorationCollection, InnerTextContext.Strikethrough))
			{
				list.Add("Strikethrough");
			}
			foreach (TextDecoration item2 in textDecorationCollection)
			{
				string item = ConvertToStorageText(item2);
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			PPTServiceUltra.Ink.Core.Storage.Standard.TextDecorationCollection textDecorationCollection3 = new PPTServiceUltra.Ink.Core.Storage.Standard.TextDecorationCollection();
			textDecorationCollection3.TryDecode(string.Join(",", list));
			return textDecorationCollection3;
		}

		private static string ConvertToStorageText(TextDecoration textDecoration)
		{
			if (Converter.AreEquals(InnerTextContext.Underline, textDecoration))
			{
				return "Underline";
			}
			if (Converter.AreEquals(InnerTextContext.Strikethrough, textDecoration))
			{
				return "Strikethrough";
			}
			if (Converter.AreEquals(InnerTextContext.Baseline, textDecoration))
			{
				return "Baseline";
			}
			if (Converter.AreEquals(InnerTextContext.OverLine, textDecoration))
			{
				return "OverLine";
			}
			return string.Empty;
		}

		public static TextDecoration ConvertFromStorageText(string text)
		{
			if (string.Equals(text, "Underline"))
			{
				return InnerTextContext.Underline;
			}
			if (string.Equals(text, "Strikethrough"))
			{
				return InnerTextContext.Strikethrough;
			}
			if (string.Equals(text, "Baseline"))
			{
				return InnerTextContext.Baseline;
			}
			if (string.Equals(text, "OverLine"))
			{
				return InnerTextContext.OverLine;
			}
			return null;
		}
	}
}
