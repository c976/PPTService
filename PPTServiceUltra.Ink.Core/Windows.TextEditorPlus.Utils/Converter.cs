using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Linq;
using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class Converter
	{
		public static MarkerStyle ToMarkerStyle(this MarkerProperty markerProperty)
		{
			MarkerStyle result = MarkerStyle.None;
			if (markerProperty != null && !markerProperty.IsEmpty())
			{
				if (markerProperty.AutoNumber != null)
				{
					result = GetMarkerStyleByAutoNumber(markerProperty.AutoNumber.AutoNumberType);
				}
				if (MarkerStyleExtension.MarkerCharDic.ContainsValue(markerProperty.MarkerChar?.ToText()))
				{
					result = MarkerStyleExtension.MarkerCharDic.FirstOrDefault((KeyValuePair<MarkerStyle, string> x) => x.Value.Equals(markerProperty.MarkerChar.ToText())).Key;
				}
			}
			return result;
		}

		private static MarkerStyle GetMarkerStyleByAutoNumber(AutoNumberType autoNumberType)
		{
			MarkerStyle result = MarkerStyle.None;
			switch (autoNumberType)
			{
			case AutoNumberType.AlphaLcPeriod:
				result = MarkerStyle.LowerLatin;
				break;
			case AutoNumberType.AlphaUcPeriod:
				result = MarkerStyle.UpperLatin;
				break;
			case AutoNumberType.ArabicPeriod:
				result = MarkerStyle.Decimal;
				break;
			case AutoNumberType.ArabicParenBoth:
				result = MarkerStyle.Decimal1;
				break;
			case AutoNumberType.CircleNumDbPlain:
				result = MarkerStyle.Decimal2;
				break;
			}
			return result;
		}

		public static RunSaveInfo ToRunSaveInfo(this RunProperty prop)
		{
			return new RunSaveInfo
			{
				FontSize = prop.FontSize,
				FontStyle = prop.FontStyle,
				FontWeight = prop.FontWeight,
				FontFamily = prop.FontFamily,
				FontVariants = prop.FontVariants,
				Background = prop.Background,
				Foreground = prop.Foreground,
				Opacity = prop.Opacity,
				Hyperlink = prop.Hyperlink,
				Hyerlink = prop.Hyperlink,
				UnderlineDashStyle = prop.UnderlineDashStyle,
				UnderlineBrush = prop.UnderlineBrush,
				Decoration = prop.TextDecoration.ToStorageTextDecorationCollection(),
				TextEffects = prop.EffectsArgs.ToEffectSaveInfoList()
			}.FixNullValue();
		}

		public static RunProperty ToRunProperty(this RunSaveInfo saveInfo)
		{
			string text = saveInfo.Hyperlink;
			if (string.IsNullOrWhiteSpace(text))
			{
				text = saveInfo.Hyerlink;
			}
			return new RunProperty
			{
				FontSize = saveInfo.FontSize,
				FontStyle = saveInfo.FontStyle,
				FontWeight = saveInfo.FontWeight,
				FontFamily = saveInfo.FontFamily,
				FontVariants = saveInfo.FontVariants,
				Background = saveInfo.Background,
				Foreground = saveInfo.Foreground,
				Opacity = saveInfo.Opacity,
				UnderlineDashStyle = saveInfo.UnderlineDashStyle,
				UnderlineBrush = saveInfo.UnderlineBrush,
				Hyperlink = text,
				TextDecoration = saveInfo.Decoration,
				EffectsArgs = saveInfo.TextEffects.ToEffectArgsCollection()
			};
		}

		private static List<EffectSaveInfo> ToEffectSaveInfoList(this EffectArgsCollection argsCollection)
		{
			if (argsCollection == null)
			{
				return new List<EffectSaveInfo>();
			}
			List<EffectSaveInfo> newObj = new List<EffectSaveInfo>();
			argsCollection.ForEach((ITextEffectArgs x) =>
			{
				newObj.Add(x.ToSaveInfo());
			});
			return newObj;
		}

		private static EffectArgsCollection ToEffectArgsCollection(this List<EffectSaveInfo> effectSaveInfos)
		{
			if (effectSaveInfos == null)
			{
				return new EffectArgsCollection();
			}
			EffectArgsCollection newObj = new EffectArgsCollection();
			effectSaveInfos.ForEach((EffectSaveInfo x) =>
			{
				newObj.Add(x.ToEffectArgs());
			});
			return newObj;
		}

		public static ParagraphSaveInfo ToParagraphSaveInfo(this ParagraphProperty prop)
		{
			return new ParagraphSaveInfo
			{
				MarginLeft = prop.MarginLeft,
				Indent = prop.Indent,
				TextMarkerSaveInfo = prop.MarkerProperty.ToMarkerSaveInfo(prop.DefaultRunProperty),
				LineSpacing = prop.LineSpacing,
				FixedLineSpacing = prop.FixedLineSpacing,
				TextAlignment = prop.TextAlignment,
				DefaultRunProperty = prop.DefaultRunProperty.ToRunSaveInfo(),
				SpaceBefore = prop.ParagraphBefore,
				SpaceAfter = prop.ParagraphAfter,
				TextMarkerStyle = prop.MarkerProperty.ToMarkerStyle(),
				IndentLevel = prop.IndentLevel,
				Direction = prop.Direction
			}.FixNullValue();
		}

		public static ParagraphProperty ToParagraphProperty(this ParagraphSaveInfo saveInfo)
		{
			MarkerSaveInfo textMarkerSaveInfo = saveInfo.TextMarkerSaveInfo;
			MarkerProperty markerProperty = ((textMarkerSaveInfo != null) ? textMarkerSaveInfo.ToMarkerProperty() : null);
			if ((markerProperty == null || markerProperty.IsEmpty()) && saveInfo.TextMarkerStyle != MarkerStyle.None)
			{
				markerProperty = MarkerFactory.GetMarkerProperty(saveInfo.DefaultRunProperty.ToRunProperty(), saveInfo.TextMarkerStyle);
			}
			ParagraphProperty paragraphProperty = new ParagraphProperty
			{
				MarginLeft = saveInfo.MarginLeft,
				Indent = saveInfo.Indent,
				MarkerProperty = markerProperty,
				LineSpacing = saveInfo.LineSpacing,
				FixedLineSpacing = saveInfo.FixedLineSpacing,
				TextAlignment = saveInfo.TextAlignment,
				DefaultRunProperty = saveInfo.DefaultRunProperty.ToRunProperty(),
				ParagraphBefore = saveInfo.SpaceBefore,
				ParagraphAfter = saveInfo.SpaceAfter,
				IndentLevel = saveInfo.IndentLevel,
				Direction = saveInfo.Direction
			};
			if (saveInfo.IndentType == IndentType.Hanging)
			{
				paragraphProperty.Indent = 0.0 - paragraphProperty.Indent;
			}
			return paragraphProperty;
		}

		public static RichTextProperty ToRichTextProperty(this RichTextSaveInfo richTextSaveInfo)
		{
			List<ParagraphProperty> list = new List<ParagraphProperty>();
			List<RunProperty> list2 = new List<RunProperty>();
			List<ICharObject> list3 = new List<ICharObject>();
			Dictionary<int, List<LineProperty>> dictionary = new Dictionary<int, List<LineProperty>>();
			richTextSaveInfo.Text = TextHelperInner.PreHandleNewLine(richTextSaveInfo.Text);
			for (int i = 0; i < richTextSaveInfo.Paragraphs.Count; i++)
			{
				ParagraphSaveInfo paragraphSaveInfo = richTextSaveInfo.Paragraphs[i];
				list.Add(paragraphSaveInfo.ToParagraphProperty());
				if (paragraphSaveInfo.Lines.Any())
				{
					dictionary.Add(i, paragraphSaveInfo.Lines.Select((LineSaveInfo x) => x.ToLineProperty()).ToList());
				}
				if (paragraphSaveInfo.TextRuns != null)
				{
					ValueTuple<List<RunProperty>, List<ICharObject>> valueTuple = ParseRichText(paragraphSaveInfo);
					list2.AddRange(valueTuple.Item1);
					list3.AddRange(valueTuple.Item2);
				}
			}
			return new RichTextProperty
			{
				ArrangingType = richTextSaveInfo.ArrangingType,
				SizeToContent = richTextSaveInfo.SizeToContent,
				VerticalTextAlignment = richTextSaveInfo.VerticalTextAlignment,
				ParagraphProperties = list,
				RunProperties = list2,
				CharObjects = list3,
				ParagraphLineInfo = dictionary
			};
		}

		
		internal static ValueTuple<List<RunProperty>, List<ICharObject>> ParseRichText(ParagraphSaveInfo richTextSaveInfoParagraphSaveInfo)
		{
			List<RunProperty> list = new List<RunProperty>();
			List<ICharObject> list2 = new List<ICharObject>();
			for (int i = 0; i < richTextSaveInfoParagraphSaveInfo.TextRuns.Count; i++)
			{
				RunSaveInfo runSaveInfo = richTextSaveInfoParagraphSaveInfo.TextRuns[i];
				if (runSaveInfo.Extensions.Any())
				{
					foreach (ExtCharSaveInfo item2 in runSaveInfo.Extensions.OfType<ExtCharSaveInfo>())
					{
						list.Add(runSaveInfo.ToRunProperty());
						ExtensionalChar item = TextEditorExtManager.CreateExtChar(item2);
						list2.Add(item);
					}
				}
				else if (!string.IsNullOrEmpty(runSaveInfo.Text))
				{
					if (runSaveInfo.Text == "\r" && richTextSaveInfoParagraphSaveInfo.TextRuns[i + 1].Text == "\n")
					{
						richTextSaveInfoParagraphSaveInfo.TextRuns.RemoveAt(i + 1);
					}
					RunProperty prop = runSaveInfo.ToRunProperty();
					runSaveInfo.Text = TextHelperInner.PreHandleNewLine(runSaveInfo.Text);
					ParseText(list, list2, runSaveInfo, prop);
				}
				else
				{
					list.Add(runSaveInfo.ToRunProperty());
					list2.Add(new TextChar('\ufffd'.ToString()));
				}
			}
			return new ValueTuple<List<RunProperty>, List<ICharObject>>(list, list2);
		}

		private static void ParseText(ICollection<RunProperty> runProps, ICollection<ICharObject> charObjects, RunSaveInfo runSaveInfo, RunProperty prop)
		{
			for (int i = 0; i < runSaveInfo.Text.Length; i++)
			{
				string text = runSaveInfo.Text[i].ToString();
				if (Regex.IsMatch(text, "[\\ud800-\\udfff]") && i + 1 < runSaveInfo.Text.Length)
				{
					string text2 = runSaveInfo.Text[i + 1].ToString();
					if (Regex.IsMatch(text2, "[\\ud800-\\udfff]"))
					{
						text += text2;
					}
					i++;
				}
				runProps.Add(prop);
				charObjects.Add(new TextChar(text));
			}
		}

		public static LineProperty ToLineProperty(this LineSaveInfo saveInfo)
		{
			return new LineProperty
			{
				Length = saveInfo.Length,
				ParagraphOffset = saveInfo.StartOffset
			};
		}

		internal static MarkerSaveInfo ToMarkerSaveInfo(this MarkerProperty prop, RunProperty runProperty)
		{
			if (prop != null)
			{
				if (prop.AutoNumber == null && (prop.MarkerChar == null || (prop.MarkerChar != null && string.IsNullOrEmpty(prop.MarkerChar.ToText()))))
				{
					return null;
				}
				MarkerSaveInfo markerSaveInfo = new MarkerSaveInfo();
				AutoNumberProperty autoNumber = prop.AutoNumber;
				markerSaveInfo.AutoNumber = ((autoNumber != null) ? autoNumber.ToAutoNumberSaveInfo() : null);
				markerSaveInfo.Char = prop.MarkerChar?.ToText();
				markerSaveInfo.FontFamily = prop.FontFamily;
				markerSaveInfo.FontSize = runProperty.FontSize;
				markerSaveInfo.Foreground = prop.Foreground;
				return markerSaveInfo;
			}
			return null;
		}

		private static AutoNumberSaveInfo ToAutoNumberSaveInfo(this AutoNumberProperty autoNumberProperty)
		{
			return new AutoNumberSaveInfo
			{
				AutoNumberType = autoNumberProperty.AutoNumberType,
				StartAt = autoNumberProperty.StartAt
			};
		}

		private static MarkerProperty ToMarkerProperty(this MarkerSaveInfo saveInfo)
		{
			MarkerProperty markerProperty = new MarkerProperty();
			if (!saveInfo.IsEmpty())
			{
				AutoNumberSaveInfo autoNumber = saveInfo.AutoNumber;
				markerProperty.AutoNumber = ((autoNumber != null) ? autoNumber.ToAutoNumberProp() : null);
				markerProperty.MarkerChar = (string.IsNullOrEmpty(saveInfo.Char) ? null : new TextChar(saveInfo.Char));
				markerProperty.FontFamily = saveInfo.FontFamily;
				markerProperty.Foreground = saveInfo.Foreground;
			}
			return markerProperty;
		}

		private static AutoNumberProperty ToAutoNumberProp(this AutoNumberSaveInfo saveInfo)
		{
			return new AutoNumberProperty
			{
				AutoNumberType = saveInfo.AutoNumberType,
				StartAt = saveInfo.StartAt
			};
		}

		internal static LineSaveInfo ToLineSaveInfo(this LineProperty prop)
		{
			return new LineSaveInfo
			{
				StartOffset = prop.ParagraphOffset,
				Length = prop.Length
			};
		}

		public static bool AreEquals(TextDecorationCollection value1, TextDecorationCollection value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			if (value1 == null || value2 == null)
			{
				return false;
			}
			if (value1.Count != value2.Count)
			{
				return false;
			}
			foreach (TextDecoration item in value1)
			{
				if (!Contains(value2, item))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool Contains(TextDecorationCollection textDecorationCollection, TextDecoration textDecoration)
		{
			return textDecorationCollection.Any((TextDecoration textDecoration2) => AreEquals(textDecoration, textDecoration2));
		}

		internal static bool AreEquals(TextDecoration textDecoration1, TextDecoration textDecoration2)
		{
			if (textDecoration1 == textDecoration2)
			{
				return true;
			}
			TextDecorationLocation location = textDecoration1.Location;
			TextDecorationLocation location2 = textDecoration2.Location;
			if (location != location2)
			{
				return false;
			}
			Pen pen = textDecoration1.Pen;
			Pen pen2 = textDecoration2.Pen;
			if (!object.Equals(pen, pen2))
			{
				return false;
			}
			TextDecorationUnit penThicknessUnit = textDecoration1.PenThicknessUnit;
			TextDecorationUnit penThicknessUnit2 = textDecoration2.PenThicknessUnit;
			if (penThicknessUnit != penThicknessUnit2)
			{
				return false;
			}
			double penOffset = textDecoration1.PenOffset;
			double penOffset2 = textDecoration2.PenOffset;
			if (!Nearly.Equals(penOffset, penOffset2, 0.1))
			{
				return false;
			}
			return true;
		}

		internal static bool AreEquals(MarkerProperty value1, MarkerProperty value2)
		{
			if (value1 == null && value2 == null)
			{
				return true;
			}
			if (value1 == null || value2 == null)
			{
				return false;
			}
			return value1.Equals(value2);
		}

		public static bool AreEquals(AutoNumberProperty value1, AutoNumberProperty value2)
		{
			if (value1 == null && value2 == null)
			{
				return true;
			}
			if (value1 == null || value2 == null)
			{
				return false;
			}
			return value1.Equals(value2);
		}

		public static bool AreEquals(double value1, double value2)
		{
			return Nearly.Equals(value1, value2);
		}

		public static bool AreEquals(Brush value1, Brush value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			if (value1 == null || value2 == null)
			{
				return false;
			}
			if (value1.GetType() != value2.GetType())
			{
				return false;
			}
			SolidColorBrush solidColorBrush = value1 as SolidColorBrush;
			if (solidColorBrush != null)
			{
				SolidColorBrush solidColorBrush2 = value2 as SolidColorBrush;
				if (solidColorBrush2 != null)
				{
					if (solidColorBrush.Color == solidColorBrush2.Color)
					{
						return solidColorBrush.Opacity == solidColorBrush2.Opacity;
					}
					return false;
				}
			}
			ImageBrush imageBrush = value1 as ImageBrush;
			if (imageBrush != null)
			{
				ImageBrush imageBrush2 = value2 as ImageBrush;
				if (imageBrush2 != null)
				{
					if (imageBrush.ImageSource.ToString().Equals(imageBrush2.ImageSource.ToString()) && imageBrush.Viewbox.Equals(imageBrush2.Viewbox) && imageBrush.Stretch == imageBrush2.Stretch && imageBrush.Opacity == imageBrush2.Opacity)
					{
						return imageBrush.TileMode == imageBrush2.TileMode;
					}
					return false;
				}
			}
			LinearGradientBrush linearGradientBrush = value1 as LinearGradientBrush;
			if (linearGradientBrush != null)
			{
				LinearGradientBrush linearGradientBrush2 = value2 as LinearGradientBrush;
				if (linearGradientBrush2 != null)
				{
					return AreEquals(linearGradientBrush, linearGradientBrush2);
				}
			}
			return object.Equals(value1, value2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Pure]
		public static bool AreEquals(LinearGradientBrush linearGradientBrush1, LinearGradientBrush linearGradientBrush2)
		{
			if (linearGradientBrush1.ColorInterpolationMode != linearGradientBrush2.ColorInterpolationMode || linearGradientBrush1.EndPoint != linearGradientBrush2.EndPoint || linearGradientBrush1.MappingMode != linearGradientBrush2.MappingMode || linearGradientBrush1.Opacity != linearGradientBrush2.Opacity || linearGradientBrush1.StartPoint != linearGradientBrush2.StartPoint || linearGradientBrush1.SpreadMethod != linearGradientBrush2.SpreadMethod || linearGradientBrush1.GradientStops.Count != linearGradientBrush2.GradientStops.Count)
			{
				return false;
			}
			for (int i = 0; i < linearGradientBrush1.GradientStops.Count; i++)
			{
				if (linearGradientBrush1.GradientStops[i].Color != linearGradientBrush2.GradientStops[i].Color || linearGradientBrush1.GradientStops[i].Offset != linearGradientBrush2.GradientStops[i].Offset)
				{
					return false;
				}
			}
			return true;
		}

		public static bool AreEquals(FontFamily value1, FontFamily value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			if (value1 == null || value2 == null)
			{
				return false;
			}
			if (value1.Source == value2.Source && value1.BaseUri == value2.BaseUri)
			{
				return true;
			}
			return false;
		}

		public static bool AreEquals(DashStyle value1, DashStyle value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			if (value1 == null || value2 == null)
			{
				return false;
			}
			if (value1.Dashes.SequenceEqual(value2.Dashes))
			{
				return true;
			}
			return object.Equals(value1, value2);
		}

		public static bool AreEquals(EffectArgsCollection value1, EffectArgsCollection value2)
		{
			if (value1 == null && value2 == null)
			{
				return true;
			}
			if (value1 == null || value2 == null)
			{
				return false;
			}
			return value1.Equals(value2);
		}

		public static bool AreEquals(FontVariants fontVariants1, FontVariants fontVariants2)
		{
			return fontVariants1.Equals(fontVariants2);
		}

		public static bool AreEquals(FontStyle fontStyle1, FontStyle fontStyle2)
		{
			return fontStyle1.Equals(fontStyle2);
		}

		public static bool AreEquals(FontWeight value1, FontWeight value2)
		{
			return value1.Equals(value2);
		}

		public static bool AreEquals(BaselineAlignment value1, BaselineAlignment value2)
		{
			return value1.Equals(value2);
		}

		public static bool AreEquals(object value1, object value2)
		{
			return object.Equals(value2, value1);
		}

		[Obsolete("不要使用，因为此方法性能太差。建议单独实现相等判断。")]
		private static bool StorableTypeAreEquals(IStorableType value1, IStorableType value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			if (value1 == null || value2 == null)
			{
				return false;
			}
			StorableNode node = value1.Deparse();
			StorableNode node2 = value2.Deparse();
			return AreEquals(node, node2);
		}

		[Obsolete("不要使用，因为此方法性能太差。建议单独实现相等判断。")]
		private static bool AreEquals(StorableNode node1, StorableNode node2)
		{
			if (node1 == null && node2 == null)
			{
				return true;
			}
			if (node1 == null || node2 == null || node1.Children.Count != node2.Children.Count)
			{
				return false;
			}
			string obj = node1.Value?.ToString() ?? string.Empty;
			string value = node2.Value?.ToString() ?? string.Empty;
			if (!obj.Equals(value))
			{
				return false;
			}
			for (int i = 0; i < node1.Children.Count; i++)
			{
				if (!AreEquals(node1.Children[i], node2.Children[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
