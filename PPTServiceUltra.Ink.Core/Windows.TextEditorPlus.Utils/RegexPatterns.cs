namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class RegexPatterns
	{
		public const string Tibetan = "[\\u0f00-\\u0ff0]";

		private const char TibetanMinChar = 'ༀ';

		private const char TibetanMaxChar = '\u0ff0';

		public static readonly TextRangePattern TibetanPattern = new TextRangePattern('ༀ', '\u0ff0');

		public const string Mongolian = "[\\u1800-\\u18AF]";

		public static readonly TextRangePattern MongolianPattern = new TextRangePattern('᠀', '\u18af');

		public const string Number = "[0-9]";

		public static readonly TextRangePattern NumberPattern = new TextRangePattern('0', '9');

		public const string ChineseSymbol = "[\\u2014-\\u2026]|[\\u3000-\\u303F]|[\\uff01-\\uff0f]|[\\uff1a-\\uff20]|[\\uff3b-\\uff40]|[\\uff5b-\\uff5e]";

		public static readonly string ContinuousPhrase = $"[^一-龻\\s{'\ufffd'}{'\ufffe'}]+";

		public static readonly string IndependentPhrase = $"[一-龻\\s{'\ufffd'}]+";

		public static readonly char[] LeftSurroundInterpunction = new char[7] { '"', '\'', '“', '‘', '《', '[', '【' };

		public const string Interpunction = "[\\p{P}]+";

		public const string Utf16Char = "([\\ud800-\\udbff][\\udc00-\\udfff])";

		public const string Utf16Surrogates = "[\\ud800-\\udfff]";

		public static readonly TextRangePattern Utf16SurrogatesPattern = new TextRangePattern('\ud800', '\udfff');

		public const string OneCodeEmojiParts = "[\\u2600-\\u27ff]";

		public const string ChineseSymbolNeedRotation = "[\\u0028-\\u0029]|[\\u005b]|[\\u2026]|[\\u005d]|[\\u007b]|[\\u2014]|[\\u007d]|[\\u0025-\\u0026]|[\\uff08-\\uff09]|[\\u3008-\\u3011]|[\\u3014-\\u301b]|[\\ufe59-\\ufe5e]";

		public const string MarkerStringNeedRotation = "[▶]|[\\u221a]|[\\u0030-\\u0039]|[\\u0041-\\u005A]|[\\u0061-\\u007A]|[\\u2460-\\u249b]|[\\u3251-\\u3289]|[\\u32b1-\\u32cb]|[\\u2160-\\u217b]";

		public const string EnglishSymbol = "[-,.?:;'\"!`]|(-{2})|(/.{3})|(/(/))|(/[/])|({})";

		public const string Hanzi = "[\\u4e00-\\u9fbb]";

		public static readonly TextRangePattern HanziPattern = new TextRangePattern('一', '龻');

		public static readonly TextRangePattern ArabicPattern = new TextRangePattern('\u0600', 'ۿ');

		public static readonly TextRangePattern ArabicSupplementPattern = new TextRangePattern('ݐ', 'ݿ');

		public static readonly TextRangePattern ArabicExtendedAPattern = new TextRangePattern('ࢠ', '\u08ff');

		public static readonly TextRangePattern HebrewPattern = new TextRangePattern('\u0590', '\u05ff');

		public const string AsciiPunctuation = "[!-/:-@[-`{-~]";

		internal static readonly MultiTextRangePattern AsciiPunctuationPattern = new MultiTextRangePattern('!', '/', ':', '@', '[', '`', '{', '~');

		public const string English = "[\0-ÿ]";

		public const string Letters = "[a-zA-Z]";

		public const string LowerLetters = "[a-z]";

		public static readonly TextRangePattern LowerLettersPattern = new TextRangePattern('a', 'z');

		public const string UpperLetters = "[A-Z]";

		public static readonly TextRangePattern UpperLettersPattern = new TextRangePattern('A', 'Z');

		public const string NewLine = "\\r\\n";

		public const string Blank = "\\s";

		public const string BlankSpace = " ";
	}
}
