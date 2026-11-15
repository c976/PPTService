using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal static class TextUtilities
	{
		internal static bool IsChineseSymbolNeedRotation(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException();
			}
			string pattern = "([\\u0028-\\u0029]|[\\u005b]|[\\u2026]|[\\u005d]|[\\u007b]|[\\u2014]|[\\u007d]|[\\u0025-\\u0026]|[\\uff08-\\uff09]|[\\u3008-\\u3011]|[\\u3014-\\u301b]|[\\ufe59-\\ufe5e])";
			return Regex.Match(text, pattern).Value.Equals(text);
		}

		internal static bool IsChineseSymbol(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException();
			}
			string pattern = "([\\u2014-\\u2026]|[\\u3000-\\u303F]|[\\uff01-\\uff0f]|[\\uff1a-\\uff20]|[\\uff3b-\\uff40]|[\\uff5b-\\uff5e])";
			return Regex.Match(text, pattern).Value.Equals(text);
		}

		internal static bool IsAsciiPunctuation(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}
			return RegexPatterns.AsciiPunctuationPattern.AreAllInRange(text);
		}

		internal static bool IsMongolian(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException();
			}
			return RegexPatterns.MongolianPattern.AreAllInRange(text);
		}

		internal static bool IsArabic(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException();
			}
			if (!RegexPatterns.ArabicPattern.AreAllInRange(text) && !RegexPatterns.ArabicSupplementPattern.AreAllInRange(text))
			{
				return RegexPatterns.ArabicExtendedAPattern.AreAllInRange(text);
			}
			return true;
		}

		internal static bool IsLigature(string text)
		{
			if (!IsMongolian(text) && !IsTibetan(text))
			{
				return IsArabic(text);
			}
			return true;
		}

		internal static bool IsRightToLeftLigature(string text)
		{
			return IsArabic(text);
		}

		internal static bool IsHebrew(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException();
			}
			return RegexPatterns.HebrewPattern.AreAllInRange(text);
		}

		internal static bool IsTibetan(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException();
			}
			return RegexPatterns.TibetanPattern.AreAllInRange(text);
		}

		internal static bool IsHanzi(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException();
			}
			return RegexPatterns.HanziPattern.AreAllInRange(text);
		}

		internal static CharFlowDirection GetFlowDirection(TextChar textChar)
		{
			string textCharObject = textChar.TextCharObject;
			if (string.IsNullOrEmpty(textCharObject))
			{
				throw new ArgumentNullException();
			}
			if (string.IsNullOrWhiteSpace(textCharObject))
			{
				return CharFlowDirection.Neutral;
			}
			if (RegexPatterns.ArabicPattern.AreAllInRange(textCharObject) || RegexPatterns.HebrewPattern.AreAllInRange(textCharObject))
			{
				return CharFlowDirection.RightToLeft;
			}
			if (IsAsciiPunctuation(textCharObject))
			{
				return CharFlowDirection.Neutral;
			}
			return CharFlowDirection.LeftToRight;
		}

		internal static bool IsBlankSpace(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException();
			}
			return text.Equals(" ");
		}

		internal static bool IsValidInputByRegexString(string regex, string newText)
		{
			if (string.IsNullOrEmpty(newText))
			{
				return true;
			}
			return Regex.Match(newText, regex).Value.Equals(newText);
		}

		internal static bool IsValidInputByRegex(Regex regex, string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return true;
			}
			return regex.Match(text).Value.Equals(text);
		}

		internal static bool IsValidInputByAcceptingInputType(AcceptingInputTypes acceptingInputType, string newText)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (acceptingInputType.HasFlag(AcceptingInputTypes.All) || string.IsNullOrEmpty(newText))
			{
				return true;
			}
			if (acceptingInputType.HasFlag(AcceptingInputTypes.Letters))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append("[a-zA-Z]");
			}
			if (acceptingInputType.HasFlag(AcceptingInputTypes.LowerLetters))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append("[a-z]");
			}
			if (acceptingInputType.HasFlag(AcceptingInputTypes.UpperLetters))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append("[A-Z]");
			}
			if (acceptingInputType.HasFlag(AcceptingInputTypes.Hanzi))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append("[\\u4e00-\\u9fbb]");
			}
			if (acceptingInputType.HasFlag(AcceptingInputTypes.Number))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append("[0-9]");
			}
			if (acceptingInputType.HasFlag(AcceptingInputTypes.NewLine))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append("\\r\\n");
			}
			if (acceptingInputType.HasFlag(AcceptingInputTypes.Blank))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append("\\s");
			}
			if (acceptingInputType.HasFlag(AcceptingInputTypes.UpperLetters) || acceptingInputType.HasFlag(AcceptingInputTypes.UpperLetters))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append("[-,.?:;'\"!`]|(-{2})|(/.{3})|(/(/))|(/[/])|({})");
			}
			stringBuilder.Insert(0, "^(");
			stringBuilder.Append(")+");
			return Regex.Match(newText, stringBuilder.ToString()).Value.Equals(newText);
		}

		internal static bool NeedRotation(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException();
			}
			string pattern = "([\0-ÿ]|[0-9]|[\\u1800-\\u18AF]|[\\u0028-\\u0029]|[\\u005b]|[\\u2026]|[\\u005d]|[\\u007b]|[\\u2014]|[\\u007d]|[\\u0025-\\u0026]|[\\uff08-\\uff09]|[\\u3008-\\u3011]|[\\u3014-\\u301b]|[\\ufe59-\\ufe5e])+";
			return Regex.Match(text, pattern).Value.Equals(text);
		}

		internal static bool IsMarkerStringNeedRotation(string markerString)
		{
			if (string.IsNullOrEmpty(markerString))
			{
				throw new ArgumentNullException();
			}
			string pattern = "([▶]|[\\u221a]|[\\u0030-\\u0039]|[\\u0041-\\u005A]|[\\u0061-\\u007A]|[\\u2460-\\u249b]|[\\u3251-\\u3289]|[\\u32b1-\\u32cb]|[\\u2160-\\u217b])";
			return Regex.Match(markerString, pattern).Value.Equals(markerString);
		}

		internal static List<ICharObject> GetCharsFromText(string text)
		{
			List<ICharObject> list = new List<ICharObject>();
			if (string.IsNullOrEmpty(text))
			{
				return list;
			}
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '\r' || text[i] == '\n')
				{
					if (i + 1 < text.Length && text[i + 1] == '\n')
					{
						i++;
					}
					list.Add(new TextChar("\r"));
					list.Add(new TextChar("\n"));
				}
				else if (RegexPatterns.Utf16SurrogatesPattern.IsInRange(text[i]))
				{
					if (i + 1 < text.Length && RegexPatterns.Utf16SurrogatesPattern.IsInRange(text[i + 1]))
					{
						list.Add(new TextChar(new string(new char[2]
						{
							text[i],
							text[i + 1]
						})));
						i++;
					}
					else
					{
						list.Add(new TextChar(text[i].ToString()));
					}
				}
				else
				{
					list.Add(new TextChar(text[i].ToString()));
				}
			}
			return list;
		}

		internal static SimpleSegment NextNewParagraph([NotNull] IReadOnlyList<ICharObject> chars, int startOffset)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars cannot be null.");
			}
			if (startOffset < 0)
			{
				throw new InvalidOperationException("offset cannot less than 0.");
			}
			if (startOffset > chars.Count)
			{
				throw new InvalidOperationException("offset cannot larger than TextLength.");
			}
			int num = -1;
			for (int i = startOffset; i < chars.Count; i++)
			{
				if (chars[i].ToText().Equals("\r") || chars[i].ToText().Equals("\n"))
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				return SimpleSegment.Invalid;
			}
			if (chars[num].ToText().Equals("\r") && num + 1 < chars.Count && chars[num + 1].ToText().Equals("\n"))
			{
				return new SimpleSegment(num, 2);
			}
			return new SimpleSegment(num, 1);
		}
	}
}
