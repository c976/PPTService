using System;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Windows.Media;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils.Glyph
{
	internal class FontFamilyMapItem
	{
		internal class Range
		{
			internal int First { get; }

			internal int Last => First + (int)Delta;

			internal uint Delta { get; }

			internal Range(int first, int last)
			{
				First = first;
				Delta = (uint)(last - First);
			}

			internal bool InRange(int ch)
			{
				return (uint)(ch - First) <= Delta;
			}
		}

		internal const int LastUnicodeScalar = 1114111;

		private Range[] _ranges;

		private FontFamilyMap Map { get; }

		public XmlLanguage Language
		{
			get
			{
				return Map.Language;
			}
			set
			{
				Map.Language = value;
			}
		}

		public double Scale
		{
			get
			{
				return Map.Scale;
			}
			set
			{
				Map.Scale = value;
			}
		}

		public string Unicode
		{
			get
			{
				return Map.Unicode;
			}
			set
			{
				Map.Unicode = value;
				_ranges = ParseUnicodeRanges(Map.Unicode);
			}
		}

		public string Target
		{
			get
			{
				return Map.Target;
			}
			set
			{
				Map.Target = value;
			}
		}

		internal Range[] Ranges => _ranges;

		public FontFamilyMapItem(FontFamilyMap map)
		{
			Map = map;
			_ranges = ParseUnicodeRanges(Map.Unicode);
		}

		private static void ThrowInvalidUnicodeRange()
		{
			throw new InvalidOperationException("无效的Unicode字符范围");
		}

		private static Range[] ParseUnicodeRanges(string unicodeRanges)
		{
			List<Range> list = new List<Range>(3);
			for (int i = 0; i < unicodeRanges.Length; i++)
			{
				int number;
				if (!ParseHexNumber(unicodeRanges, ref i, out number))
				{
					ThrowInvalidUnicodeRange();
				}
				int number2 = number;
				if (i < unicodeRanges.Length)
				{
					if (unicodeRanges[i] == '?')
					{
						do
						{
							number *= 16;
							number2 = number2 * 16 + 15;
							i++;
						}
						while (i < unicodeRanges.Length && unicodeRanges[i] == '?' && number2 <= 1114111);
					}
					else if (unicodeRanges[i] == '-')
					{
						i++;
						if (!ParseHexNumber(unicodeRanges, ref i, out number2))
						{
							ThrowInvalidUnicodeRange();
						}
					}
				}
				if (number > number2 || number2 > 1114111 || (i < unicodeRanges.Length && unicodeRanges[i] != ','))
				{
					ThrowInvalidUnicodeRange();
				}
				list.Add(new Range(number, number2));
			}
			return list.ToArray();
		}

		internal static bool ParseHexNumber(string numString, ref int index, out int number)
		{
			while (index < numString.Length && numString[index] == ' ')
			{
				index++;
			}
			int num = index;
			number = 0;
			while (index < numString.Length)
			{
				int num2 = numString[index];
				if (num2 >= 48 && num2 <= 57)
				{
					number = number * 16 + (num2 - 48);
					index++;
					continue;
				}
				num2 |= 0x20;
				if (num2 < 97 || num2 > 102)
				{
					break;
				}
				number = number * 16 + (num2 - 87);
				index++;
			}
			bool result = index > num;
			while (index < numString.Length && numString[index] == ' ')
			{
				index++;
			}
			return result;
		}

		internal bool InRange(int ch)
		{
			Range[] ranges = _ranges;
			for (int i = 0; i < ranges.Length; i++)
			{
				if (ranges[i].InRange(ch))
				{
					return true;
				}
			}
			return false;
		}
	}
}
