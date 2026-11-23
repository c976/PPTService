using System;
using System.Globalization;
using System.Windows.Markup;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Glyph
{
	public static class XmlLanguageExtension
	{
		private const int MaxCultureDepth = 32;

		public static bool MatchCulture(this XmlLanguage language, CultureInfo culture)
		{
			if (language == null)
			{
				return true;
			}
			if (culture != null)
			{
				return language.RangeIncludes(culture);
			}
			return false;
		}

		private static bool IsPrefixOf(this XmlLanguage language, string longTag)
		{
			string ietfLanguageTag = language.IetfLanguageTag;
			if (!longTag.StartsWith(ietfLanguageTag, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (ietfLanguageTag.Length != 0 && ietfLanguageTag.Length != longTag.Length)
			{
				return longTag[ietfLanguageTag.Length] == '-';
			}
			return true;
		}

		internal static bool RangeIncludes(this XmlLanguage language, CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			for (int i = 0; i < 32; i++)
			{
				if (language.IsPrefixOf(culture.IetfLanguageTag))
				{
					return true;
				}
				CultureInfo parent = culture.Parent;
				if (parent == null || parent.Equals(CultureInfo.InvariantCulture) || parent == culture)
				{
					break;
				}
				culture = parent;
			}
			return false;
		}
	}
}
