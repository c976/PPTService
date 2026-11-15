using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils.Glyph
{
	public class FallBackFontFamily
	{
		private const string FallBackFontFamilyName = "#GLOBAL USER INTERFACE";

		private static readonly Lazy<FallBackFontFamily> _lazy = new Lazy<FallBackFontFamily>(() => new FallBackFontFamily(CultureInfo.CurrentCulture));

		private FontFamily FallBack { get; } = new FontFamily("#GLOBAL USER INTERFACE");

		private IEnumerable<FontFamilyMapItem> FontFamilyItems { get; }

		public static FallBackFontFamily Instance => _lazy.Value;

		private FallBackFontFamily(CultureInfo culture)
		{
			FontFamilyItems = (from map in FallBack.FamilyMaps
				where map.Language == null || map.Language.MatchCulture(culture)
				select new FontFamilyMapItem(map)).ToList();
		}

		public bool TryGetFallBackFontFamily(char unicodeChar, out string familyName)
		{
			FontFamilyMapItem fontFamilyMapItem = FontFamilyItems.FirstOrDefault((FontFamilyMapItem item) => item.InRange(unicodeChar));
			familyName = null;
			if (fontFamilyMapItem != null)
			{
				familyName = fontFamilyMapItem.Target;
				return true;
			}
			return false;
		}
	}
}
