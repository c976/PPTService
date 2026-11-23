using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal static class MarkerFactory
	{
		private const int NumberOneChar = 9312;

		private const int BracketNumberOneChar = 9332;

		private static readonly string[][] LowerRoman = new string[4][]
		{
			new string[10] { "", "i", "ii", "iii", "iv", "v", "vi", "vii", "viii", "ix" },
			new string[10] { "", "x", "xx", "xxx", "xl", "l", "lx", "lxx", "lxxx", "xc" },
			new string[10] { "", "c", "cc", "ccc", "cd", "d", "dc", "dcc", "dccc", "cm" },
			new string[4] { "", "m", "mm", "mmm" }
		};

		private static readonly string[][] UpperRoman = new string[4][]
		{
			new string[10] { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" },
			new string[10] { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" },
			new string[10] { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" },
			new string[4] { "", "M", "MM", "MMM" }
		};

		private static readonly string[] Hanzi = new string[11]
		{
			"", "一", "二", "三", "四", "五", "六", "七", "八", "九",
			"〇"
		};

		internal static double OriginalMarkerOpacity { get; } = 1.0;

		internal static double HalfMarkerOpacity { get; } = 0.5;

		internal static double HiddenMarkerOpacity { get; } = 0.0;

		internal static MarkerProperty GetMarkerProperty(RunProperty runProp, MarkerStyle markerStyle)
		{
			MarkerProperty markerProperty = new MarkerProperty();
			switch (markerStyle)
			{
			case MarkerStyle.None:
				return markerProperty;
			case MarkerStyle.Circle:
			case MarkerStyle.Rect:
			case MarkerStyle.Box:
			case MarkerStyle.Square:
			case MarkerStyle.Tick:
				markerProperty.MarkerChar = new TextChar(MarkerStyleExtension.MarkerCharDic[markerStyle]);
				markerProperty.FontFamily = ((markerStyle == MarkerStyle.Box) ? new FontFamily("Wingdings 3") : new FontFamily("Wingdings"));
				break;
			default:
				markerProperty.AutoNumber = GetAutoNumberProperty(markerStyle);
				markerProperty.MarkerChar = new TextChar(GetSortedBulletString(markerProperty.AutoNumber.AutoNumberType, 1));
				markerProperty.FontFamily = runProp.FontFamily;
				break;
			}
			return markerProperty;
		}

		private static AutoNumberProperty GetAutoNumberProperty(MarkerStyle markerStyle)
		{
			AutoNumberProperty autoNumberProperty = new AutoNumberProperty
			{
				StartAt = 1
			};
			switch (markerStyle)
			{
			case MarkerStyle.LowerLatin:
				autoNumberProperty.AutoNumberType = AutoNumberType.AlphaLcPeriod;
				break;
			case MarkerStyle.UpperLatin:
				autoNumberProperty.AutoNumberType = AutoNumberType.AlphaUcPeriod;
				break;
			case MarkerStyle.Decimal:
				autoNumberProperty.AutoNumberType = AutoNumberType.ArabicPeriod;
				break;
			case MarkerStyle.Decimal1:
				autoNumberProperty.AutoNumberType = AutoNumberType.ArabicParenBoth;
				break;
			case MarkerStyle.Decimal2:
				autoNumberProperty.AutoNumberType = AutoNumberType.CircleNumDbPlain;
				break;
			default:
				autoNumberProperty.AutoNumberType = AutoNumberType.Nil;
				break;
			}
			return autoNumberProperty;
		}

		internal static DrawingVisualContainer CreateMarkerCharVisualContainer(TextCharVisual textCharVisual, RunProperty runProperty)
		{
			DrawingVisualContainer drawingVisualContainer = new DrawingVisualContainer();
			if (textCharVisual.GlyphData != null)
			{
				drawingVisualContainer.AddChild(TextHelperInner.CreateDrawingVisual(textCharVisual, runProperty.Foreground));
			}
			else
			{
				drawingVisualContainer.AddChild(TextHelperInner.CreateDrawingVisual(textCharVisual.TextGeometry, runProperty.Foreground));
			}
			return drawingVisualContainer;
		}

		internal static UiElementCharVisual BuildMarkerCharVisual(RunProperty runProperty, ICharObject charObject, double height, bool isNeedRotated = false)
		{
			UiElementCharVisual uiElementCharVisual = new UiElementCharVisual();
			bool needCached = false;
			if (!runProperty.EffectsArgs.Any())
			{
				needCached = false;
			}
			UIElementContainer uIElementContainer = new UIElementContainer(needCached);
			TextChar textChar = charObject as TextChar;
			if (textChar != null)
			{
				TextCharVisual textCharVisual = BuildMarkerTextCharVisual(runProperty, textChar, height, isNeedRotated);
				uIElementContainer.AddChild(CreateMarkerCharVisualContainer(textCharVisual, runProperty));
				uiElementCharVisual.Bounds = new Rect(default(Point), textCharVisual.Bounds.Size);
			}
			uiElementCharVisual.CharView = uIElementContainer;
			return uiElementCharVisual;
		}

		internal static TextCharVisual BuildMarkerTextCharVisual(RunProperty runProperty, TextChar textChar, double height, bool isNeedRotated = false)
		{
			_003CBuildMarkerTextCharVisual_003Eg__ChangeOpacity_007C4_0(runProperty);
			ValueTuple<Geometry, Size, TextCharGlyphData> valueTuple = TextHelperInner.BuildCharGeometry(runProperty, textChar);
			Geometry geometry = valueTuple.Item1;
			Size size = valueTuple.Item2;
			TextCharGlyphData item = valueTuple.Item3;
			double height2 = size.Height;
			bool flag = true;
			MarkerStyle key = MarkerStyleExtension.MarkerCharDic.FirstOrDefault((KeyValuePair<MarkerStyle, string> x) => x.Value.Equals(textChar.ToText())).Key;
			if (TextContext.MarkerStringDic.ContainsKey(key))
			{
				flag = TextUtilities.IsMarkerStringNeedRotation(TextContext.MarkerStringDic[key]);
			}
			if (isNeedRotated & flag)
			{
				geometry = TextHelperInner.CorrectToVerticalGeometry(geometry, size);
				size = new Size(size.Height, size.Width);
			}
			else if (isNeedRotated)
			{
				geometry = TextHelperInner.CorrectToVerticalGeometry(geometry, size, false);
				size = new Size(size.Height, size.Width);
			}
			double num = height / 2.0 - height2 / 2.0;
			if (isNeedRotated)
			{
				geometry.TransformTranslate(num, 0.0);
			}
			else
			{
				geometry.TransformTranslate(0.0, num);
			}
			return new TextCharVisual
			{
				TextGeometry = geometry,
				Foreground = runProperty.Foreground,
				Matrix = geometry.Transform.Value,
				Bounds = new Rect(default(Point), size),
				GlyphData = item,
				Direction = CharFlowDirection.LeftToRight
			};
		}

		internal static ICharObject GetCurrentAutoNumber(CharsManager charsManager, PropertyManager propertyManager, int paragraphNumber)
		{
			int num = 1;
			ParagraphProperty paragraphProperty = propertyManager.GetParagraphProperty(paragraphNumber);
			AutoNumberType autoNumberType = paragraphProperty.MarkerProperty.AutoNumber.AutoNumberType;
			for (int num2 = paragraphNumber - 1; num2 >= 0; num2--)
			{
				ParagraphProperty paragraphProperty2 = propertyManager.GetParagraphProperty(num2);
				if (paragraphProperty2.MarkerProperty != null)
				{
					if (paragraphProperty2.MarkerProperty.AutoNumber == null || !Converter.AreEquals(paragraphProperty2.MarkerProperty.AutoNumber.AutoNumberType, autoNumberType) || paragraphProperty2.IndentLevel != paragraphProperty.IndentLevel)
					{
						if (paragraphProperty2.IndentLevel < paragraphProperty.IndentLevel)
						{
							break;
						}
					}
					else if (charsManager.ParagraphTree.GetParagraphNodeByNumber(num2 + 1).Length != 0)
					{
						num++;
					}
				}
			}
			return new TextChar(GetSortedBulletString(autoNumberType, num));
		}

		internal static string GetSortedBulletString(AutoNumberType style, int num)
		{
			string result = string.Empty;
			switch (style)
			{
			case AutoNumberType.AlphaLcPeriod:
				result = GetLowerLatin(num);
				break;
			case AutoNumberType.AlphaUcPeriod:
				result = GetUpperLatin(num);
				break;
			case AutoNumberType.ArabicParenBoth:
				result = GetDecimal1(num);
				break;
			case AutoNumberType.ArabicPeriod:
				result = num + ".";
				break;
			case AutoNumberType.CircleNumDbPlain:
				result = GetDecimal2(num);
				break;
			case AutoNumberType.AlphaLcParenR:
				result = GetAlphaLcParenR(num);
				break;
			case AutoNumberType.ArabicParenR:
				result = GetArabicParenR(num);
				break;
			case AutoNumberType.RomanLcPeriod:
				result = GetRomanLcPeriod(num);
				break;
			case AutoNumberType.RomanUcPeriod:
				result = GetRomanUcPeriod(num);
				break;
			case AutoNumberType.Ea1JpnChsDbPeriod:
				result = GetHanzi(num);
				break;
			default:
				throw new ArgumentOutOfRangeException("marker style must can be sort");
			case AutoNumberType.Nil:
				break;
			}
			return result;
		}

		private static string GetLowerLatin(int num)
		{
			int num2 = num / 27;
			int num3 = Math.Max(num % 27, 1);
			return new string((char)(97 + num3 - 1), num2 + 1) + ".";
		}

		private static string GetDecimal1(int num)
		{
			if (num < 21)
			{
				return ((char)(9332 + num - 1)).ToString();
			}
			return num.ToString();
		}

		private static string GetDecimal2(int num)
		{
			if (num < 21)
			{
				return ((char)(9312 + num - 1)).ToString();
			}
			return num.ToString();
		}

		private static string GetUpperLatin(int num)
		{
			int num2 = num / 27;
			int num3 = Math.Max(num % 27, 1);
			return new string((char)(65 + num3 - 1), num2 + 1) + ".";
		}

		private static string GetAlphaLcParenR(int num)
		{
			int num2 = num / 27;
			int num3 = Math.Max(num % 27, 1);
			return new string((char)(97 + num3 - 1), num2 + 1) + ")";
		}

		private static string GetArabicParenR(int num)
		{
			return num + ")";
		}

		private static string GetRomanLcPeriod(int num)
		{
			string text = LowerRoman[3][num / 1000 % 10];
			string text2 = "" + text;
			text = LowerRoman[2][num / 100 % 10];
			string text3 = text2 + text;
			text = LowerRoman[1][num / 10 % 10];
			string text4 = text3 + text;
			text = LowerRoman[0][num % 10];
			return string.Concat(text4 + text, ".");
		}

		private static string GetRomanUcPeriod(int num)
		{
			string text = UpperRoman[3][num / 1000 % 10];
			string text2 = "" + text;
			text = UpperRoman[2][num / 100 % 10];
			string text3 = text2 + text;
			text = UpperRoman[1][num / 10 % 10];
			string text4 = text3 + text;
			text = UpperRoman[0][num % 10];
			return string.Concat(text4 + text, ".");
		}

		private static string GetHanzi(int num)
		{
			string text = "";
			while (num > 0)
			{
				int num2 = num % 10;
				text = ((num2 != 0 || num / 10 <= 0) ? (text + Hanzi[num2]) : (text + Hanzi[10]));
				num /= 10;
			}
			char[] array = text.ToCharArray();
			Array.Reverse(array);
			text = new string(array);
			return text + ".";
		}

		internal static double GetMarkerBaseLine(ParagraphProperty paragraphProperty, TextChar textChar)
		{
			ValueTuple<Geometry, Size> valueTuple = TextHelperInner.BuildDefaultCharGeometry(paragraphProperty.DefaultRunProperty);
			Geometry item = valueTuple.Item1;
			Size item2 = valueTuple.Item2;
			double height = item2.Height;
			RunProperty runProperty = paragraphProperty.DefaultRunProperty.Clone();
			runProperty.FontFamily = ((paragraphProperty.MarkerProperty.AutoNumber != null) ? paragraphProperty.DefaultRunProperty.FontFamily : paragraphProperty.MarkerProperty.FontFamily);
			Size item3 = TextHelperInner.BuildCharGeometry(runProperty, textChar).Item2;
			Geometry geometry = TextHelperInner.CorrectToVerticalGeometry(item, item2);
			double width = new Size(item2.Height, item2.Width).Width;
			double x = height / 2.0 - width / 2.0;
			geometry.TransformTranslate(x, 0.0);
			return geometry.Bounds.Left;
		}

		[CompilerGenerated]
		internal static void _003CBuildMarkerTextCharVisual_003Eg__ChangeOpacity_007C4_0(RunProperty rp)
		{
			if (!Nearly.Equals(rp.Opacity, 1.0) || !Nearly.Equals(rp.Foreground.Opacity, 1.0))
			{
				Brush brush = rp.Foreground.CloneCurrentValue();
				if (!brush.IsFrozen)
				{
					brush.Opacity = rp.Opacity;
				}
				rp.Foreground = brush;
			}
		}
	}
}
