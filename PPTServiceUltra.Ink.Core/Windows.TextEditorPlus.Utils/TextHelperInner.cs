using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Storage.Standard;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Editing;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Glyph;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal static class TextHelperInner
	{
		public static class TextGeometry
		{
			private static readonly Dictionary<string, List<TextGeometryCache>> _textGeometryCache = new Dictionary<string, List<TextGeometryCache>>();

			public static bool UsingCache { get; set; }

			internal static TextGeometryRenderResult GetOrBuildTextGeometry(RunProperty runProperty, string text)
			{
				List<TextGeometryCache> value;
				if (_textGeometryCache.TryGetValue(text, out value))
				{
					foreach (TextGeometryCache item in value)
					{
						if (item.FontStyle.Equals(runProperty.FontStyle) && Nearly.Equals(item.FontSize, runProperty.FontSize) && runProperty.FontFamily.Equals(item.FontFamily) && runProperty.FontWeight.Equals(item.FontWeight))
						{
							return new ValueTuple<System.Windows.Media.Geometry, System.Windows.Size, TextCharGlyphData>(item.Geometry.Clone(), item.Size, item.Data);
						}
					}
				}
				else
				{
					value = new List<TextGeometryCache>();
					_textGeometryCache[text] = value;
				}
				System.Windows.Media.Geometry textGeometry;
				System.Windows.Size textGeometrySize;
				TextCharGlyphData textCharGlyphData;
				BuildGeometry(runProperty, text).Deconstruct(out textGeometry, out textGeometrySize, out textCharGlyphData);
				System.Windows.Media.Geometry geometry = textGeometry;
				System.Windows.Size size = textGeometrySize;
				TextCharGlyphData textCharGlyphData2 = textCharGlyphData;
				value.Add(new TextGeometryCache(text, size, runProperty.FontSize, runProperty.FontStyle, runProperty.FontFamily, geometry.Clone(), runProperty.FontWeight)
				{
					Data = textCharGlyphData2
				});
				return new TextGeometryRenderResult(geometry, size, textCharGlyphData2);
			}

			internal static TextGeometryRenderResult BuildTextGeometry(RunProperty runProperty, string text)
			{
				if (!UsingCache)
				{
					return BuildGeometry(runProperty, text);
				}
				return GetOrBuildTextGeometry(runProperty, text);
			}

			private static TextGeometryRenderResult BuildGeometry(RunProperty runProperty, string text)
			{
				Typeface typeface = new Typeface(runProperty.FontFamily, runProperty.FontStyle, runProperty.FontWeight, FontStretches.Normal);
				GlyphInfo info;
				if (!TextUtilities.IsLigature(text) && text.Length == 1 && text[0] != '\r' && text[0] != '\n' && text[0] != '\t' && GlyphRunCreator.TryGetGlyphInfo(typeface, text[0], out info))
				{
					TextCharGlyphData textCharGlyphData = new TextCharGlyphData
					{
						FontSize = runProperty.FontSize,
						GlyphInfo = info
					};
					GlyphRun run = (textCharGlyphData.GlyphRun = GlyphRunCreator.BuildSingleGlyphRun(info, runProperty.FontSize));
					System.Windows.Rect bounds = run.GetBounds();
					if (!bounds.IsEmpty)
					{
						RectangleGeometry textGeometry = new RectangleGeometry(bounds)
						{
							Transform = new TranslateTransform(0.0, runProperty.FontFamily.GetBaseline(runProperty.FontSize))
						};
						System.Windows.Size size = run.GetSize(runProperty.FontFamily.LineSpacing);
						return new TextGeometryRenderResult(textGeometry, size, textCharGlyphData);
					}
				}
				FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, runProperty.FontSize, Brushes.Transparent);
				System.Windows.Media.Geometry textGeometry2 = formattedText.BuildGeometry(default(System.Windows.Point));
				System.Windows.Size textGeometrySize = new System.Windows.Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
				return new TextGeometryRenderResult(textGeometry2, textGeometrySize, null);
			}

			private static System.Windows.Size GetTextSize(string text, string fontFamily, System.Windows.FontStyle fontStyle, System.Windows.FontWeight fontWeight, double fontSize)
			{
				return TextContext.TextCharacterSizeBuilderProvider?.GetTextSize(text, fontFamily, fontStyle, fontWeight, fontSize) ?? new System.Windows.Size(fontSize, fontSize * 1.14066666666667);
			}
		}

		private class TextGeometryCache
		{
			public System.Windows.FontWeight FontWeight { get; }

			public System.Windows.Size Size { get; }

			public double FontSize { get; }

			public System.Windows.FontStyle FontStyle { get; }

			[NotNull]
			public System.Windows.Media.FontFamily FontFamily { get; }

			[NotNull]
			public string Text { get; }

			[NotNull]
			public System.Windows.Media.Geometry Geometry { get; }

			public TextCharGlyphData Data { get; set; }

			public TextGeometryCache([NotNull] string text, System.Windows.Size size, double fontSize, System.Windows.FontStyle fontStyle, [NotNull] System.Windows.Media.FontFamily fontFamily, [NotNull] System.Windows.Media.Geometry geometry, System.Windows.FontWeight fontWeight)
			{
				if (text == null)
				{
					throw new ArgumentNullException("text");
				}
				Text = text;
				Size = size;
				FontSize = fontSize;
				FontStyle = fontStyle;
				if (fontFamily == null)
				{
					throw new ArgumentNullException("fontFamily");
				}
				FontFamily = fontFamily;
				if (geometry == null)
				{
					throw new ArgumentNullException("geometry");
				}
				Geometry = geometry;
				FontWeight = fontWeight;
			}
		}

		private static readonly TextFormatter Formatter = TextFormatter.Create();

		public static Dictionary<int, List<double>> MarkerMarginLeftAndIndentMaps = new Dictionary<int, List<double>>
		{
			{
				0,
				new List<double> { 18.0, 18.0 }
			},
			{
				1,
				new List<double> { 30.0, 36.0 }
			},
			{
				2,
				new List<double> { 36.0, 48.0 }
			},
			{
				3,
				new List<double> { 48.0, 54.0 }
			},
			{
				4,
				new List<double> { 60.0, 78.0 }
			},
			{
				5,
				new List<double> { 72.0, 96.0 }
			},
			{
				6,
				new List<double> { 90.0, 120.0 }
			},
			{
				7,
				new List<double> { 120.0, 144.0 }
			}
		};

		internal static System.Windows.Point CorrectPosition(RoutedEventArgs e, UIElement element)
		{
			System.Windows.Point result = default(System.Windows.Point);
			MouseEventArgs e2 = e as MouseEventArgs;
			if (e2 == null)
			{
				DragEventArgs e3 = e as DragEventArgs;
				if (e3 == null)
				{
					StylusEventArgs e4 = e as StylusEventArgs;
					if (e4 != null)
					{
						result = e4.GetPosition(element);
					}
				}
				else
				{
					result = e3.GetPosition(element);
				}
			}
			else
			{
				result = e2.GetPosition(element);
			}
			result.X += 4.0 / GetScalingRatioToDevice(element).Width;
			result.Y += 8.0 / GetScalingRatioToDevice(element).Height;
			return result;
		}

		internal static ValueTuple<double, double> CalculateIndent(ParagraphProperty paragraphProperty, double markerWidth = 0.0)
		{
			bool flag = paragraphProperty.Indent < 0.0;
			double num = 0.0;
			double item = 0.0;
			bool flag2 = paragraphProperty.MarkerProperty != null && !paragraphProperty.MarkerProperty.IsEmpty();
			double num2 = Math.Abs(paragraphProperty.Indent);
			double marginLeft = paragraphProperty.MarginLeft;
			if (flag)
			{
				double num3 = marginLeft - num2;
				if (num3 < 0.0)
				{
					num3 = 0.0;
				}
				if (flag2)
				{
					num = ((!(num2 > markerWidth)) ? (num3 + markerWidth) : (num2 + num3));
					item = num3;
				}
				else
				{
					num = num3;
				}
			}
			else if (flag2)
			{
				num = ((!(num2 < markerWidth)) ? (marginLeft + num2) : (marginLeft + markerWidth));
				item = marginLeft;
			}
			else
			{
				num = marginLeft + num2;
			}
			return new ValueTuple<double, double>(num, item);
		}

		internal static DrawingVisual CreateDrawingVisual(System.Windows.Media.Geometry geometry, System.Windows.Media.Brush brush)
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			DrawingContext drawingContext = drawingVisual.RenderOpen();
			drawingContext.DrawGeometry(brush, null, geometry);
			drawingContext.Close();
			return drawingVisual;
		}

		internal static DrawingVisual CreateDrawingVisual(TextCharVisual textCharVisual, System.Windows.Media.Brush brush)
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			DrawingContext drawingContext = drawingVisual.RenderOpen();
			drawingContext.DrawingTextCharVisuals(new TextCharVisual[1] { textCharVisual }, brush);
			drawingContext.Close();
			return drawingVisual;
		}

		internal static ValueTuple<System.Windows.Media.Geometry, System.Windows.Size> BuildDefaultCharGeometry(RunProperty runProperty)
		{
			System.Windows.Media.Geometry textGeometry;
			System.Windows.Size textGeometrySize;
			TextCharGlyphData textCharGlyphData;
			TextGeometry.GetOrBuildTextGeometry(runProperty, "1").Deconstruct(out textGeometry, out textGeometrySize, out textCharGlyphData);
			System.Windows.Media.Geometry item = textGeometry;
			System.Windows.Size item2 = textGeometrySize;
			return new ValueTuple<System.Windows.Media.Geometry, System.Windows.Size>(item, item2);
		}

		internal static double BuildDefaultCharBaseline(RunProperty runProperty)
		{
			Typeface typeface = new Typeface(runProperty.FontFamily, runProperty.FontStyle, runProperty.FontWeight, FontStretches.Normal);
			return new FormattedText(InnerTextContext.DefaultTextChar.ToText(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, runProperty.FontSize, Brushes.Transparent).Baseline;
		}

		internal static System.Windows.Size BuildDefaultCharSize(RunProperty runProperty)
		{
			Typeface typeface = new Typeface(runProperty.FontFamily, runProperty.FontStyle, runProperty.FontWeight, FontStretches.Normal);
			FormattedText formattedText = new FormattedText(InnerTextContext.DefaultTextChar.ToText(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, runProperty.FontSize, Brushes.Transparent);
			return new System.Windows.Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
		}

		internal static ValueTuple<System.Windows.Media.Geometry, System.Windows.Size, TextCharGlyphData> BuildCharGeometry(RunProperty runProperty, ICharObject textChar)
		{
			if (!(textChar is TextChar))
			{
				throw new ArgumentException("Only TextChar Could be Build Geometry");
			}
			return BuildTextGeometry(runProperty, textChar.ToText());
		}

		internal static ValueTuple<System.Windows.Media.Geometry, System.Windows.Size, TextCharGlyphData> BuildTextGeometry(RunProperty runProperty, string text)
		{
			return TextGeometry.BuildTextGeometry(runProperty, text);
		}

		internal static System.Windows.Media.Geometry CorrectToVerticalGeometry(System.Windows.Media.Geometry geometry, System.Windows.Size size, bool isNeedRotated = true, double angle = 90.0)
		{
			if (isNeedRotated)
			{
				geometry.TransformRotate(angle, size.Width / 2.0, size.Height / 2.0);
			}
			geometry.TransformTranslate(size.Height / 2.0 - size.Width / 2.0, 0.0 - (size.Height / 2.0 - size.Width / 2.0));
			return geometry;
		}

		internal static System.Windows.Media.Geometry CorrectToHorizontalGeometry(System.Windows.Media.Geometry geometry, System.Windows.Size size)
		{
			geometry.TransformRotate(-90.0, size.Width / 2.0, size.Height / 2.0);
			geometry.TransformTranslate(size.Height / 2.0 - size.Width / 2.0, 0.0 - (size.Height / 2.0 - size.Width / 2.0));
			return geometry;
		}

		internal static void CorrectToVerticalChar(CharsManager charsManager, List<CharGeometryInfo> charGeometriesInfos)
		{
			for (int i = 0; i < charsManager.CharCount; i++)
			{
				ICharObject charAt = charsManager.GetCharAt(i);
				TextChar textChar = charAt as TextChar;
				if (textChar != null)
				{
					CharGeometryInfo charGeometryInfo = charGeometriesInfos[i];
					if (!TextUtilities.IsLigature(textChar.ToText()))
					{
						if (TextUtilities.NeedRotation(textChar.ToText()))
						{
							charGeometryInfo.Geometry = CorrectToVerticalGeometry(charGeometryInfo.Geometry, charGeometryInfo.Size);
							charGeometryInfo.Size = new System.Windows.Size(charGeometryInfo.Size.Height, charGeometryInfo.Size.Width);
							continue;
						}
						charGeometryInfo.Size = new System.Windows.Size(charGeometryInfo.Size.Height, charGeometryInfo.Size.Width);
						System.Windows.Rect bounds = charGeometryInfo.Geometry.Bounds;
						double x = (charGeometryInfo.Size.Width - bounds.Width) / 2.0 - charGeometryInfo.Geometry.Bounds.Left;
						double y = (charGeometryInfo.Size.Height - bounds.Height) / 2.0 - bounds.Top;
						charGeometryInfo.Geometry.TransformTranslate(x, y);
					}
				}
				else
				{
					ExtensionalChar extensionalChar = charAt as ExtensionalChar;
					if (extensionalChar != null && extensionalChar.IsNeedRotationUnderVertical)
					{
						extensionalChar.Size = new System.Windows.Size(extensionalChar.Size.Height, extensionalChar.Size.Width);
					}
				}
			}
		}

		internal static void CorrectToHorizontalChar(CharsManager charsManager, List<CharGeometryInfo> charGeometriesInfos)
		{
			for (int i = 0; i < charsManager.CharCount; i++)
			{
				ICharObject charAt = charsManager.GetCharAt(i);
				TextChar textChar = charAt as TextChar;
				if (textChar != null)
				{
					CharGeometryInfo charGeometryInfo = charGeometriesInfos[i];
					if (!TextUtilities.IsLigature(textChar.ToText()))
					{
						if (TextUtilities.NeedRotation(textChar.ToText()))
						{
							charGeometryInfo.Geometry = CorrectToHorizontalGeometry(charGeometryInfo.Geometry, charGeometryInfo.Size);
							charGeometryInfo.Size = new System.Windows.Size(charGeometryInfo.Size.Height, charGeometryInfo.Size.Width);
							continue;
						}
						charGeometryInfo.Size = new System.Windows.Size(charGeometryInfo.Size.Height, charGeometryInfo.Size.Width);
						System.Windows.Rect bounds = charGeometryInfo.Geometry.Bounds;
						double x = charGeometryInfo.HorizontalBounds.X - bounds.X;
						double y = charGeometryInfo.HorizontalBounds.Y - bounds.Y;
						charGeometryInfo.Geometry.TransformTranslate(x, y);
					}
				}
				else
				{
					ExtensionalChar extensionalChar = charAt as ExtensionalChar;
					if (extensionalChar != null && extensionalChar.IsNeedRotationUnderVertical)
					{
						extensionalChar.Size = new System.Windows.Size(extensionalChar.Size.Height, extensionalChar.Size.Width);
					}
				}
			}
		}

		internal static void TransformTextChar(this TextCharVisual textCharVisual, double xOffset, double yOffset)
		{
			System.Windows.Media.Matrix matrix = new System.Windows.Media.Matrix(1.0, 0.0, 0.0, 1.0, xOffset, yOffset);
			textCharVisual.Matrix *= matrix;
			textCharVisual.UpdateMatrix();
		}

		internal static System.Windows.Media.Geometry BuildSelectionGeometry(DocumentManager documentManager, RenderManager renderManager)
		{
			GeometryGroup g = new GeometryGroup
			{
				FillRule = FillRule.Nonzero
			};
			if (documentManager.Selection.IsEmpty)
			{
				return g;
			}
			TextEditorHelper.OptimizeRects(renderManager.GetBoundsBySegment(documentManager.Selection.ToSegment())).ForEach((System.Windows.Rect x) =>
			{
				g.Children.Add(new RectangleGeometry(x));
			});
			return g;
		}

		internal static List<System.Windows.Rect> GetRunBoundsBySegmentFromParagraph(ParagraphBase paragraph, ISegment segment, double renderWidth, bool isLastParagraph = false)
		{
			List<System.Windows.Rect> list = new List<System.Windows.Rect>();
			ISegment segment2 = paragraph.ParagraphNode.Intersect(segment);
			if (!segment2.Equals(SimpleSegment.Invalid))
			{
				int num = 0;
				List<System.Windows.Rect> list2 = new List<System.Windows.Rect>();
				for (int i = 0; i < paragraph.LineRenderData.Count; i++)
				{
					if (num >= segment2.StartOffset - paragraph.ParagraphNode.StartOffset + segment2.Length)
					{
						continue;
					}
					if (num < paragraph.Lines[i].LineProperty.ParagraphOffset)
					{
						for (int j = 0; j < paragraph.LineRenderData[i].CharVisualsCount; j++)
						{
							list2.Add(System.Windows.Rect.Empty);
						}
					}
					else
					{
						IEnumerable<System.Windows.Rect> lineRunBoundsToDocument = GetLineRunBoundsToDocument(paragraph, i, renderWidth);
						list2.AddRange(lineRunBoundsToDocument);
					}
					num += paragraph.LineRenderData[i].CharVisualsCount;
				}
				list.AddRange(list2.GetRange(segment2.StartOffset - paragraph.ParagraphNode.StartOffset, segment2.Length));
			}
			return list;
		}

		private static IEnumerable<System.Windows.Rect> GetLineRunBoundsToDocument(ParagraphBase paragraph, int lineIndex, double renderWidth)
		{
			List<ICharVisual> charVisuals = paragraph.LineRenderData[lineIndex].CharVisuals;
			if (!(paragraph is HorizontalParagraph))
			{
				if (!(paragraph is VerticalParagraph))
				{
					if (paragraph is MongolianParagraph)
					{
						return charVisuals.Select((ICharVisual x) => CorrectMogolianParagraphRunBoundsToDocument(paragraph, x.Bounds, lineIndex));
					}
					throw new ArgumentException();
				}
				return charVisuals.Select((ICharVisual x) => CorrectVerticalParagraphRunBoundsToDocument(paragraph, x.Bounds, lineIndex, renderWidth));
			}
			return charVisuals.Select((ICharVisual x) => CorrectHorizontalParagraphRunBoundsToDocument(paragraph, x.Bounds, lineIndex));
		}

		internal static System.Windows.Rect CreateTailRect(ArrangingType arrangingType, ParagraphBase paragraph, double renderWidth)
		{
			int lineIndex = paragraph.LineRenderData.Count - 1;
			System.Windows.Rect runBounds = (paragraph.LineRenderData.Last().CharVisuals.Any() ? paragraph.LineRenderData.Last().CharVisuals.Last().Bounds : paragraph.Lines.Last().DefaultRunBounds);
			switch (arrangingType)
			{
			case ArrangingType.Horizontal:
				return CorrectHorizontalParagraphRunBoundsToDocument(paragraph, runBounds, lineIndex);
			case ArrangingType.Vertical:
				return CorrectVerticalParagraphRunBoundsToDocument(paragraph, runBounds, lineIndex, renderWidth);
			case ArrangingType.Mongolian:
				return CorrectMogolianParagraphRunBoundsToDocument(paragraph, runBounds, lineIndex);
			default:
				throw new ArgumentOutOfRangeException("arrangingType", "the text arrangingtype is wrong");
			}
		}

		internal static System.Windows.Rect CorrectHorizontalParagraphRunBoundsToDocument(ParagraphBase paragraph, System.Windows.Rect runBounds, int lineIndex)
		{
			double num = 0.0;
			double renderParagraphBefore = paragraph.GetRenderParagraphBefore();
			double renderParagraphAfter = paragraph.GetRenderParagraphAfter();
			for (int i = 0; i < lineIndex; i++)
			{
				num += paragraph.LineRenderData[i].Height;
			}
			double num2 = ((lineIndex == 0) ? paragraph.ParagraphTextBeforeData.FirstLineTextIndent : paragraph.ParagraphTextBeforeData.NotFirstLineTextIndent);
			System.Windows.Rect result = new System.Windows.Rect(runBounds.X + num2, paragraph.VisualTop + num + renderParagraphBefore, runBounds.Width, paragraph.LineRenderData[lineIndex].Height);
			if (lineIndex == 0)
			{
				result = new System.Windows.Rect(result.X, result.Y - renderParagraphBefore, result.Width, result.Height + renderParagraphBefore);
			}
			if (lineIndex == paragraph.LineRenderData.Count - 1)
			{
				result = new System.Windows.Rect(result.X, result.Y, result.Width, result.Height + renderParagraphAfter);
			}
			return result;
		}

		internal static System.Windows.Rect CorrectVerticalParagraphRunBoundsToDocument(ParagraphBase paragraph, System.Windows.Rect runBounds, int lineIndex, double renderWidth)
		{
			double num = 0.0;
			double renderParagraphBefore = paragraph.GetRenderParagraphBefore();
			double renderParagraphAfter = paragraph.GetRenderParagraphAfter();
			double num2 = renderWidth - paragraph.VisualTop;
			for (int i = 0; i <= lineIndex; i++)
			{
				num += paragraph.LineRenderData[i].Height;
			}
			double num3 = ((lineIndex == 0) ? paragraph.ParagraphTextBeforeData.FirstLineTextIndent : paragraph.ParagraphTextBeforeData.NotFirstLineTextIndent);
			System.Windows.Rect result = new System.Windows.Rect(num2 - num, runBounds.Y + num3, paragraph.LineRenderData[lineIndex].Height, runBounds.Height);
			if (lineIndex == 0)
			{
				result = new System.Windows.Rect(result.X, result.Y, result.Width + renderParagraphBefore, result.Height);
			}
			if (lineIndex == paragraph.LineRenderData.Count - 1)
			{
				result = new System.Windows.Rect(result.X - renderParagraphAfter, result.Y, result.Width + renderParagraphAfter, result.Height);
			}
			return result;
		}

		internal static System.Windows.Rect CorrectMogolianParagraphRunBoundsToDocument(ParagraphBase paragraph, System.Windows.Rect runBounds, int lineIndex)
		{
			double num = 0.0;
			double renderParagraphBefore = paragraph.GetRenderParagraphBefore();
			double renderParagraphAfter = paragraph.GetRenderParagraphAfter();
			for (int i = 0; i < lineIndex; i++)
			{
				num += paragraph.LineRenderData[i].Height;
			}
			double num2 = ((lineIndex == 0) ? paragraph.ParagraphTextBeforeData.FirstLineTextIndent : paragraph.ParagraphTextBeforeData.NotFirstLineTextIndent);
			System.Windows.Rect result = new System.Windows.Rect(paragraph.VisualTop + num, runBounds.Y + num2, paragraph.LineRenderData[lineIndex].Height, runBounds.Height);
			if (lineIndex == 0)
			{
				result = new System.Windows.Rect(result.X - renderParagraphBefore, result.Y, result.Width + renderParagraphBefore, result.Height);
			}
			if (lineIndex == paragraph.LineRenderData.Count - 1)
			{
				result = new System.Windows.Rect(result.X, result.Y, result.Width + renderParagraphAfter, result.Height);
			}
			return result;
		}

		internal static bool IsEditorSupport(DragEventArgs dataObject)
		{
			if (dataObject.Data.GetData("source") is TextArea)
			{
				return true;
			}
			if (dataObject.Data.GetData("TextEditor") != null)
			{
				return true;
			}
			string[] formats = dataObject.Data.GetFormats();
			if (!formats.Contains(DataFormats.Rtf) && !formats.Contains(DataFormats.StringFormat) && !formats.Contains(DataFormats.Text) && !formats.Contains(DataFormats.UnicodeText) && !formats.Contains(DataFormats.FileDrop) && !formats.Contains(DataFormats.Bitmap))
			{
				return formats.Contains(DataFormats.Dib);
			}
			return true;
		}

		internal static ISegment Intersect(this ISegment segment0, ISegment segment1)
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			for (int i = segment0.FrontOffset; i < segment0.BehindOffset; i++)
			{
				list.Add(i);
			}
			for (int j = segment1.FrontOffset; j < segment1.BehindOffset; j++)
			{
				list2.Add(j);
			}
			List<int> list3 = list.Intersect(list2).ToList();
			if (list3.Count != 0)
			{
				return new SimpleSegment(list3.First(), list3.Count);
			}
			return SimpleSegment.Invalid;
		}

		internal static bool IsSegmentInvalid(this ISegment segment0, ISegment segment1)
		{
			if (segment0 == null || segment1 == null)
			{
				throw new ArgumentNullException("segment0 and segment1 can not be null");
			}
			if (segment0.StartOffset > segment1.EndOffset || segment0.EndOffset < segment1.StartOffset)
			{
				return false;
			}
			return true;
		}

		internal static ISegment Union(List<ISegment> segments)
		{
			int num = segments.Min((ISegment x) => x.FrontOffset);
			int num2 = segments.Max((ISegment x) => x.BehindOffset);
			return new SimpleSegment(num, num2 - num);
		}

		internal static System.Windows.Rect OffsetRect(this System.Windows.Rect rect1, double xOffset, double yOffset)
		{
			return new System.Windows.Rect(new System.Windows.Point(rect1.X + xOffset, rect1.Y + yOffset), rect1.Size);
		}

		internal static List<int> GetParagraphIndexBySegment(CharsManager charsManager, ISegment segment, bool includeFirstParagraph = true)
		{
			List<int> list = new List<int>();
			ParagraphNode paragraphNodeByDocumentOffset = charsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(segment.FrontOffset);
			ParagraphNode paragraphNodeByDocumentOffset2 = charsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(segment.BehindOffset);
			int num = paragraphNodeByDocumentOffset.ParagraphNumber - 1;
			int num2 = paragraphNodeByDocumentOffset2.ParagraphNumber - 1;
			if (segment.FrontOffset == paragraphNodeByDocumentOffset.StartOffset)
			{
				includeFirstParagraph = true;
			}
			for (int i = (includeFirstParagraph ? num : (num + 1)); i <= num2; i++)
			{
				list.Add(i);
			}
			return list;
		}

		internal static double GetImageWidthWithExtractHeight(string path, double height)
		{
			BitmapImage bitmapImage = new BitmapImage(new System.Uri(path));
			if (bitmapImage.PixelHeight == 0)
			{
				return height;
			}
			double num = height / (double)bitmapImage.PixelHeight;
			return Math.Max((double)bitmapImage.PixelWidth * num, height);
		}

		internal static int PasteText(TextArea textArea, DataObjectAdapter dataObject, bool isRichText)
		{
			DocumentManager documentManager = textArea.DocumentManager;
			ISegment segment;
			if (!documentManager.Selection.IsEmpty)
			{
				segment = documentManager.Selection.ToSegment();
			}
			else
			{
				ISegment segment2 = new SimpleSegment(documentManager.CaretInfo.CurrentCaretOffset.Offset + 1, 0);
				segment = segment2;
			}
			ISegment segment3 = segment;
			if (isRichText)
			{
				RichTextProperty richTextProperty = DataObjectHelper.DigRichDataInner(dataObject, documentManager.IsExtCharEditSupported, documentManager.PropertyManager.CurrentRunProperty);
				if (richTextProperty.RunProperties.Any())
				{
					documentManager.ReplaceSelectionWithRichTextProperty(segment3, richTextProperty);
					return richTextProperty.CharObjects.Count;
				}
				documentManager.EditChars(segment3, richTextProperty.CharObjects);
				return richTextProperty.CharObjects.Count;
			}
			string charString = DataObjectHelper.DigPlainData(dataObject);
			documentManager.EditChar(segment3, charString);
			return 0;
		}

		internal static ParagraphSaveInfo GetParagraphSaveInfo(DocumentManager documentManager, int paragraphIndex)
		{
			ParagraphNode paragraphNodeByNumber = documentManager.CharsManager.ParagraphTree.GetParagraphNodeByNumber(paragraphIndex + 1);
			return GetParagraphSaveInfo(documentManager, paragraphIndex, 0, paragraphNodeByNumber.TotalLength);
		}

		internal static ParagraphSaveInfo GetParagraphSaveInfo(DocumentManager documentManager, int paragraphIndex, int startOffset, int totalLength)
		{
			ParagraphNode paragraphNodeByNumber = documentManager.CharsManager.ParagraphTree.GetParagraphNodeByNumber(paragraphIndex + 1);
			ParagraphProperty paragraphProperty = documentManager.PropertyManager.GetParagraphProperty(paragraphIndex);
			int startOffset2 = paragraphNodeByNumber.StartOffset;
			ParagraphSaveInfo paragraphSaveInfo = paragraphProperty.ToParagraphSaveInfo();
			for (int i = startOffset; i < totalLength; i++)
			{
				RunProperty runProperty = documentManager.PropertyManager.GetRunProperty(startOffset2 + i);
				RunSaveInfo runSaveInfo = runProperty.ToRunSaveInfo();
				if (documentManager.CharsManager.Chars[startOffset2 + i] is TextChar)
				{
					List<ICharObject> list = new List<ICharObject> { documentManager.CharsManager.Chars[startOffset2 + i] };
					for (int j = i + 1; j < totalLength; j++)
					{
						ICharObject charObject = documentManager.CharsManager.Chars[startOffset2 + j];
						if (!(charObject is TextChar))
						{
							break;
						}
						RunProperty runProperty2 = documentManager.PropertyManager.GetRunProperty(startOffset2 + j);
						if (!runProperty.Equals(runProperty2))
						{
							break;
						}
						list.Add(charObject);
					}
					i = list.Count - 1 + i;
					StringBuilder text = new StringBuilder();
					list.ForEach((ICharObject x) =>
					{
						text.Append(x.ToText());
					});
					runSaveInfo.Text = text.ToString();
				}
				else
				{
					ExtensionalChar extensionalChar = (ExtensionalChar)documentManager.CharsManager.Chars[startOffset2 + i];
					ExtCharSaveInfo saveInfo = extensionalChar.GetSaveInfo();
					if (documentManager.ArrangingType != ArrangingType.Horizontal && extensionalChar.IsNeedRotationUnderVertical)
					{
						PPTServiceUltra.Ink.Core.Storage.Standard.Size size = new PPTServiceUltra.Ink.Core.Storage.Standard.Size
						{
							Width = saveInfo.Size.Height,
							Height = extensionalChar.Size.Width
						};
						saveInfo.Size = size;
					}
					runSaveInfo.Extensions.Add(saveInfo);
				}
				paragraphSaveInfo.TextRuns.Add(runSaveInfo);
			}
			double num = paragraphSaveInfo.Indent;
			paragraphSaveInfo.IndentType = ((!(num < 0.0)) ? IndentType.FirstLine : IndentType.Hanging);
			if (paragraphProperty.MarkerProperty != null)
			{
				if (!paragraphProperty.MarkerProperty.IsEmpty() && paragraphProperty.MarkerProperty.AutoNumber != null)
				{
					paragraphSaveInfo.TextMarkerSaveInfo.Char = string.Empty;
				}
				if (paragraphProperty.MarkerProperty.IsEmpty())
				{
					num = 0.0;
				}
			}
			paragraphSaveInfo.Indent = Math.Abs(num);
			return paragraphSaveInfo;
		}

		internal static int GetNextNotMongolianCharDocumentIndexInParagraph(CharsManager charsManager, int currentIndex, int endIndex)
		{
			int result = currentIndex + 1;
			for (int i = currentIndex + 1; i < endIndex; i++)
			{
				TextChar textChar = charsManager.GetCharAt(i) as TextChar;
				if (textChar == null || !TextUtilities.IsLigature(textChar.ToText()))
				{
					break;
				}
				result = i + 1;
			}
			return result;
		}

		internal static CaretOffset UpdateCaretOffsetIsAtParagraphStart(DocumentManager documentManager, CaretOffset caretOffset)
		{
			CaretOffset result = new CaretOffset
			{
				Offset = caretOffset.Offset,
				IsAtParagraphStart = caretOffset.IsAtParagraphStart,
				IsAtLineStart = caretOffset.IsAtLineStart
			};
			if (documentManager.CharsManager.ParagraphTree.Any((ParagraphNode x) => x.StartOffset == caretOffset.Offset + 1))
			{
				result.IsAtParagraphStart = true;
			}
			return result;
		}

		internal static bool IsHittingSelection(TextArea textArea, System.Windows.Point point)
		{
			if (textArea.DocumentManager.Selection.IsEmpty)
			{
				return false;
			}
			CaretOffset caretOffsetByPosition = textArea.RenderManager.GetCaretOffsetByPosition(point);
			return IsHittingSelection(textArea, caretOffsetByPosition);
		}

		internal static bool IsHittingSelection(TextArea textArea, CaretOffset offset)
		{
			ISelection selection = textArea.DocumentManager.Selection;
			if (selection.IsEmpty)
			{
				return false;
			}
			if (selection.FrontOffset.Offset <= offset.Offset)
			{
				return selection.BehindOffset.Offset >= offset.Offset;
			}
			return false;
		}

		internal static System.Windows.Size GetScalingRatioToDevice(Visual visual)
		{
			Visual ancestor = VisualRoot(visual);
			System.Windows.Media.Matrix value = ((MatrixTransform)visual.TransformToAncestor(ancestor)).Value;
			CompositionTarget compositionTarget = PresentationSource.FromVisual(visual)?.CompositionTarget;
			if (compositionTarget != null)
			{
				value.Append(compositionTarget.TransformToDevice);
			}
			System.Windows.Vector vector = new System.Windows.Vector(1.0, 0.0);
			System.Windows.Vector vector2 = value.Transform(vector);
			double num = System.Windows.Vector.AngleBetween(vector, vector2);
			value.Rotate(0.0 - num);
			System.Windows.Rect rect = new System.Windows.Rect(new System.Windows.Size(1.0, 1.0));
			rect.Transform(value);
			return rect.Size;
		}

		private static Visual VisualRoot(Visual visual)
		{
			Visual result = visual;
			for (DependencyObject parent = VisualTreeHelper.GetParent(visual); parent != null; parent = VisualTreeHelper.GetParent(parent))
			{
				Visual visual2 = parent as Visual;
				if (visual2 != null)
				{
					result = visual2;
				}
			}
			return result;
		}

		internal static bool IsWidthEnoughToIndent(TextArea textArea)
		{
			if (textArea.DocumentManager.SizeToContent == SizeToContent.Width || textArea.DocumentManager.SizeToContent == SizeToContent.WidthAndHeight)
			{
				return true;
			}
			double num = textArea.RenderManager.CalculateDocumentMinWidth() + 33.599999999999994;
			if (textArea.DocumentManager.ArrangingType == ArrangingType.Horizontal)
			{
				return textArea.DocumentManager.EditorWidth > num;
			}
			return textArea.DocumentManager.EditorHeight > num;
		}

		internal static bool IsMaxIndent(TextArea textArea)
		{
			return GetParagraphSelectedIndexes(textArea).Any((int x) => textArea.DocumentManager.PropertyManager.GetParagraphProperty(x).IndentLevel >= 8);
		}

		internal static bool IsMinIndent(TextArea textArea)
		{
			return GetParagraphSelectedIndexes(textArea).Any((int x) => textArea.DocumentManager.PropertyManager.GetParagraphProperty(x).IndentLevel <= 0);
		}

		private static List<int> GetParagraphSelectedIndexes(TextArea textArea)
		{
			ISegment paragraphSelectedSegment = textArea.DocumentManager.GetParagraphSelectedSegment();
			return GetParagraphIndexBySegment(textArea.DocumentManager.CharsManager, paragraphSelectedSegment);
		}

		internal static List<TextLine> BuildTextLines(string text, RunProperty runProperty, ParagraphProperty paragraphProperty, double renderWidth)
		{
			RunProperty runProperty2;
			if (runProperty.FontVariants == FontVariants.Normal)
			{
				runProperty2 = runProperty;
			}
			else
			{
				runProperty2 = runProperty.Clone();
				runProperty2.FontVariants = FontVariants.Normal;
			}
			MongolianTextSource textSource = new MongolianTextSource
			{
				Text = text,
				FontRendering = new MongolianTextRunProperties(runProperty2)
			};
			RunProperty runProperty3 = paragraphProperty.DefaultRunProperty.Clone();
			runProperty3.FontVariants = FontVariants.Normal;
			paragraphProperty.DefaultRunProperty = runProperty3;
			MongolianTextLineProperties paragraphProperties = new MongolianTextLineProperties
			{
				SetDefaultTextRunProperties = new MongolianTextRunProperties(runProperty3)
			};
			List<TextLine> list = new List<TextLine>();
			TextLineBreak previousLineBreak = null;
			int num = 0;
			while (num <= text.Length)
			{
				TextLine textLine = Formatter.FormatLine(textSource, num, renderWidth, paragraphProperties, previousLineBreak);
				num += textLine.Length;
				list.Add(textLine);
				previousLineBreak = textLine.GetTextLineBreak();
			}
			return list;
		}

		
		internal static ValueTuple<double, int, List<ICharVisual>, bool> BuildMongolian(CharsManager charsManager, PropertyManager propertyManager, int startOffset, double renderWidth, double offset, double notFirstRenderInner, bool isNeedRotation = false)
		{
			List<ICharVisual> list = new List<ICharVisual>();
			bool item = false;
			ParagraphNode paragraphNodeByDocumentOffset = charsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(startOffset);
			int nextNotMongolianCharDocumentIndexInParagraph = GetNextNotMongolianCharDocumentIndexInParagraph(charsManager, startOffset, paragraphNodeByDocumentOffset.EndOffset);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = startOffset; i < nextNotMongolianCharDocumentIndexInParagraph; i++)
			{
				stringBuilder.Append(charsManager.GetCharAt(i).ToText());
			}
			ParagraphProperty paragraphProperty = propertyManager.GetParagraphProperty(paragraphNodeByDocumentOffset.ParagraphNumber - 1);
			RunProperty renderRunProperty = propertyManager.GetRenderRunProperty(startOffset);
			string text = stringBuilder.ToString();
			List<TextLine> list2 = BuildTextLines(text, renderRunProperty, paragraphProperty, Math.Max(renderWidth - offset, 1.0));
			double num = (isNeedRotation ? 0.0 : offset);
			double num2 = (isNeedRotation ? offset : 0.0);
			if (list2.Count > stringBuilder.Length / 2)
			{
				text = stringBuilder.ToString(0, 1);
				System.Windows.Rect rect = (from x in (from y in BuildTextLines(text, renderRunProperty, paragraphProperty, 32000.0).First().GetTextBounds(0, 1)
						where y.TextRunBounds != null
						select y).SelectMany((TextBounds x) => x.TextRunBounds).ToList()
					select x.Rectangle).ToList().First();
				if (renderWidth - offset < rect.Width)
				{
					renderWidth = notFirstRenderInner;
					item = true;
					if (offset > 0.0)
					{
						startOffset--;
					}
					else
					{
						System.Windows.Size size = (isNeedRotation ? new System.Windows.Size(rect.Size.Height, rect.Size.Width) : rect.Size);
						ValueTuple<System.Windows.Media.Geometry, System.Windows.Size, TextCharGlyphData> valueTuple = BuildTextGeometry(renderRunProperty, text);
						System.Windows.Media.Geometry geometry = valueTuple.Item1;
						System.Windows.Size item2 = valueTuple.Item2;
						if (isNeedRotation)
						{
							geometry = CorrectToVerticalGeometry(geometry, item2);
						}
						CharFlowDirection direction = ((!TextUtilities.IsRightToLeftLigature(text)) ? CharFlowDirection.LeftToRight : CharFlowDirection.RightToLeft);
						TextCharVisual textCharVisual = new TextCharVisual
						{
							Bounds = new System.Windows.Rect(new System.Windows.Point(num, num2), size),
							Foreground = renderRunProperty.Foreground,
							TextGeometry = geometry,
							Matrix = geometry.Transform.Value,
							Direction = direction,
							IsIsLigature = true
						};
						textCharVisual.TransformTextChar(num, num2);
						list.Add(textCharVisual);
					}
					return new ValueTuple<double, int, List<ICharVisual>, bool>(renderWidth, startOffset, list, item);
				}
			}
			TextLine textLine = list2.First();
			if (list2.Count > 1)
			{
				text = stringBuilder.ToString(0, textLine.Length - textLine.NewlineLength);
				textLine = BuildTextLines(text, renderRunProperty, paragraphProperty, 32000.0).First();
			}
			ValueTuple<System.Windows.Media.Geometry, System.Windows.Size, TextCharGlyphData> valueTuple2 = BuildTextGeometry(renderRunProperty, text);
			System.Windows.Media.Geometry geometry2 = valueTuple2.Item1;
			System.Windows.Size item3 = valueTuple2.Item2;
			TextCharGlyphData item4 = valueTuple2.Item3;
			if (isNeedRotation)
			{
				geometry2 = CorrectToVerticalGeometry(geometry2, item3);
			}
			List<System.Windows.Rect> list3 = (from x in (from y in textLine.GetTextBounds(0, textLine.Length)
					where y.TextRunBounds != null
					select y).SelectMany((TextBounds x) => x.TextRunBounds).ToList()
				select x.Rectangle).ToList();
			CharFlowDirection direction2 = ((!TextUtilities.IsRightToLeftLigature(text)) ? CharFlowDirection.LeftToRight : CharFlowDirection.RightToLeft);
			for (int num3 = 0; num3 < list3.Count; num3++)
			{
				System.Windows.Size size2 = (isNeedRotation ? new System.Windows.Size(list3[num3].Size.Height, list3[num3].Size.Width) : list3[num3].Size);
				TextCharVisual textCharVisual2 = new TextCharVisual
				{
					Bounds = new System.Windows.Rect(new System.Windows.Point(num, num2), size2),
					Foreground = renderRunProperty.Foreground,
					TextGeometry = ((num3 == 0) ? geometry2 : System.Windows.Media.Geometry.Empty),
					Matrix = ((num3 == 0) ? geometry2.Transform.Value : System.Windows.Media.Matrix.Identity),
					GlyphData = item4,
					Direction = direction2,
					IsIsLigature = true
				};
				textCharVisual2.TransformTextChar(num, num2);
				list.Add(textCharVisual2);
				if (isNeedRotation)
				{
					num2 += list3[num3].Size.Width;
				}
				else
				{
					num += list3[num3].Size.Width;
				}
			}
			if (list2.Count > 1)
			{
				renderWidth = notFirstRenderInner;
				item = true;
			}
			startOffset += text.Length - 1;
			return new ValueTuple<double, int, List<ICharVisual>, bool>(renderWidth, startOffset, list, item);
		}

		internal static bool IsContainMongolian(CharsManager charsManager, int startOffset, int endOffset)
		{
			bool result = false;
			for (int i = startOffset; i < endOffset; i++)
			{
				if (TextUtilities.IsLigature(charsManager.GetCharAt(i).ToText()))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		internal static List<Tuple<int, int>> HandleMongolianLine(CharsManager charsManager, int start, int length)
		{
			List<Tuple<int, int>> list = new List<Tuple<int, int>>();
			int item = 0;
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				if (TextUtilities.IsLigature(charsManager.GetCharAt(i + start).ToText()))
				{
					num++;
					continue;
				}
				if (num > 0)
				{
					list.Add(new Tuple<int, int>(item, num));
				}
				list.Add(new Tuple<int, int>(i, 1));
				item = i + 1;
				num = 0;
			}
			if (num > 0)
			{
				list.Add(new Tuple<int, int>(item, num));
			}
			return list;
		}

		internal static Tuple<int, bool> GetMongolianIndex(int start, List<Tuple<int, int>> charExs)
		{
			if (start > charExs.Last().Item1 + charExs.Last().Item2)
			{
				return new Tuple<int, bool>(charExs.Count - 1, false);
			}
			int item = 0;
			bool item2 = false;
			for (int i = 0; i < charExs.Count; i++)
			{
				if (start < charExs[i].Item1 + charExs[i].Item2 - 1)
				{
					item = i;
					break;
				}
				if (start == charExs[i].Item1 + charExs[i].Item2 - 1)
				{
					item = i;
					item2 = true;
					break;
				}
			}
			return new Tuple<int, bool>(item, item2);
		}

		public static void HandlerMarginLeftAndIndent(ParagraphProperty newProp, ParagraphProperty oldProp)
		{
			double num = 0.0;
			double num2 = 0.0;
			double fontSize = newProp.DefaultRunProperty.FontSize * 3.0 / 4.0;
			if (oldProp.MarkerProperty != null && !oldProp.MarkerProperty.IsEmpty())
			{
				newProp.MarginLeft = (double)newProp.IndentLevel * 24.0 * 1.4;
				newProp.Indent = 0.0;
			}
			if (newProp.MarkerProperty != null && !newProp.MarkerProperty.IsEmpty())
			{
				if (newProp.MarkerProperty.MarkerChar != null && newProp.MarkerProperty.AutoNumber == null)
				{
					ValueTuple<double, double> marginAndIndentChar = GetMarginAndIndentChar(fontSize);
					num = marginAndIndentChar.Item1;
					num2 = marginAndIndentChar.Item2;
				}
				else
				{
					ValueTuple<double, double> marginAndIndentNumber = GetMarginAndIndentNumber(fontSize);
					num = marginAndIndentNumber.Item1;
					num2 = marginAndIndentNumber.Item2;
				}
				newProp.MarginLeft += num;
				newProp.Indent -= num2;
			}
		}

		internal static ValueTuple<double, double> GetMarginAndIndentChar(double fontSize)
		{
			double item = 0.0;
			double item2 = 0.0;
			if (fontSize <= 6.0)
			{
				item = MarkerMarginLeftAndIndentMaps[0][0];
				item2 = MarkerMarginLeftAndIndentMaps[0][0];
			}
			else if (fontSize > 6.0 && fontSize <= 18.0)
			{
				item = MarkerMarginLeftAndIndentMaps[1][0];
				item2 = MarkerMarginLeftAndIndentMaps[1][0];
			}
			else if (fontSize > 18.0 && fontSize <= 25.0)
			{
				item = MarkerMarginLeftAndIndentMaps[2][0];
				item2 = MarkerMarginLeftAndIndentMaps[2][0];
			}
			else if (fontSize > 25.0 && fontSize <= 35.0)
			{
				item = MarkerMarginLeftAndIndentMaps[3][0];
				item2 = MarkerMarginLeftAndIndentMaps[3][0];
			}
			else if (fontSize > 35.0 && fontSize <= 44.0)
			{
				item = MarkerMarginLeftAndIndentMaps[4][0];
				item2 = MarkerMarginLeftAndIndentMaps[4][0];
			}
			else if (fontSize > 44.0 && fontSize <= 56.0)
			{
				item = MarkerMarginLeftAndIndentMaps[5][0];
				item2 = MarkerMarginLeftAndIndentMaps[5][0];
			}
			else if (fontSize > 56.0 && fontSize <= 72.0)
			{
				item = MarkerMarginLeftAndIndentMaps[6][0];
				item2 = MarkerMarginLeftAndIndentMaps[6][0];
			}
			else if (fontSize > 72.0)
			{
				item = MarkerMarginLeftAndIndentMaps[7][0];
				item2 = MarkerMarginLeftAndIndentMaps[7][0];
			}
			return new ValueTuple<double, double>(item, item2);
		}

		internal static ValueTuple<double, double> GetMarginAndIndentNumber(double fontSize)
		{
			double item = 0.0;
			double item2 = 0.0;
			if (fontSize <= 12.0)
			{
				item = MarkerMarginLeftAndIndentMaps[0][1];
				item2 = MarkerMarginLeftAndIndentMaps[0][1];
			}
			else if (fontSize > 12.0 && fontSize <= 18.0)
			{
				item = MarkerMarginLeftAndIndentMaps[1][1];
				item2 = MarkerMarginLeftAndIndentMaps[1][1];
			}
			else if (fontSize > 18.0 && fontSize <= 25.0)
			{
				item = MarkerMarginLeftAndIndentMaps[2][1];
				item2 = MarkerMarginLeftAndIndentMaps[2][1];
			}
			else if (fontSize > 25.0 && fontSize <= 35.0)
			{
				item = MarkerMarginLeftAndIndentMaps[3][1];
				item2 = MarkerMarginLeftAndIndentMaps[3][1];
			}
			else if (fontSize > 35.0 && fontSize <= 44.0)
			{
				item = MarkerMarginLeftAndIndentMaps[4][1];
				item2 = MarkerMarginLeftAndIndentMaps[4][1];
			}
			else if (fontSize > 44.0 && fontSize <= 56.0)
			{
				item = MarkerMarginLeftAndIndentMaps[5][1];
				item2 = MarkerMarginLeftAndIndentMaps[5][1];
			}
			else if (fontSize > 56.0 && fontSize <= 72.0)
			{
				item = MarkerMarginLeftAndIndentMaps[6][1];
				item2 = MarkerMarginLeftAndIndentMaps[6][1];
			}
			else if (fontSize > 72.0)
			{
				item = MarkerMarginLeftAndIndentMaps[7][1];
				item2 = MarkerMarginLeftAndIndentMaps[7][1];
			}
			return new ValueTuple<double, double>(item, item2);
		}

		internal static List<ICharObject> PreHandleNewLine(List<ICharObject> charObjects)
		{
			List<ICharObject> list = new List<ICharObject>();
			for (int i = 0; i < charObjects.Count; i++)
			{
				TextChar textChar = charObjects[i] as TextChar;
				if (textChar != null)
				{
					if (textChar.ToText().Equals("\r"))
					{
						if (i + 1 < charObjects.Count)
						{
							TextChar textChar2 = charObjects[i + 1] as TextChar;
							if (textChar2 != null)
							{
								if (!textChar2.ToText().Equals("\n"))
								{
									list.AddRange(GetEnvironmentNewLineCharObjects());
									continue;
								}
								list.Add(textChar);
								list.Add(textChar2);
								i++;
								continue;
							}
						}
						list.AddRange(GetEnvironmentNewLineCharObjects());
					}
					else if (textChar.ToText().Equals("\n"))
					{
						list.AddRange(GetEnvironmentNewLineCharObjects());
					}
					else
					{
						list.Add(textChar);
					}
				}
				else
				{
					list.Add(charObjects[i]);
				}
			}
			return list;
		}

		private static List<ICharObject> GetEnvironmentNewLineCharObjects()
		{
			List<ICharObject> list = new List<ICharObject>();
			string text = "\r\n";
			for (int i = 0; i < text.Length; i++)
			{
				list.Add(new TextChar(text[i].ToString()));
			}
			return list;
		}

		internal static string PreHandleNewLine(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			string[] value = text.Split(TextContext.NewLines, StringSplitOptions.None);
			return string.Join("\r\n", value);
		}
	}
}
