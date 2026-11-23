using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.ComponentModel;
using PPTServiceUltra.Ink.Core.Windows.Mathematics.Geometries;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class VerticalParagraphBase : ParagraphBase
	{
		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass20_1
		{
			public Rect lastCharVisualBounds;

			public double maxRunFontSize;

			public double underlineWidth;

			public Rect firstCharVisualBounds;

			public double underlineHeight;

			public List<UnderlineRenderData> underlineRenderDataList;

			public Brush foreground;
		}

		internal VerticalParagraphBase(CharsManager charsManager, PropertyManager propertyManager, ParagraphNode paragraphNode, ITextGeometryRender textGeometryRender)
			: base(charsManager, propertyManager, paragraphNode, textGeometryRender)
		{
		}

		protected override void FillLineInfosDefaultBounds(List<LineInfo> lines)
		{
			for (int i = 0; i < lines.Count; i++)
			{
				RunProperty runProperty = null;
				runProperty = ((!lines[i].Equals(Lines.First())) ? PropertyManager.GetRunProperty(ParagraphNode.StartOffset + lines[i].LineProperty.ParagraphOffset).Clone() : TargetParagraphProperty.DefaultRunProperty.Clone());
				Size item = TextHelperInner.BuildDefaultCharGeometry(runProperty).Item2;
				lines[i].DefaultRunBounds = new Rect(default(Point), new Size(item.Height, 1.0));
			}
		}

		protected override List<LineRenderData> ConvertLineInfoToLineRenderData(List<LineInfo> lines, double renderWidth)
		{
			List<LineRenderData> list = new List<LineRenderData>();
			for (int i = 0; i < lines.Count; i++)
			{
				LineInfo lineInfo = lines[i];
				double renderLineSpace = lineInfo.RenderLineSpace;
				LineRenderData lineRenderData = new LineRenderData
				{
					SamePropIndexes = lineInfo.SamePropIndexes
				};
				int startOffset = ParagraphNode.StartOffset + lineInfo.LineProperty.ParagraphOffset;
				if (lineInfo.CharVisuals.Any())
				{
					lineRenderData.CharVisuals = lineInfo.CharVisuals;
					ValueTuple<double, double, double, double> maxTopAndMinBottom = GetMaxTopAndMinBottom(lineInfo, startOffset);
					double item = maxTopAndMinBottom.Item1;
					double item2 = maxTopAndMinBottom.Item2;
					double item3 = maxTopAndMinBottom.Item3;
					double item4 = maxTopAndMinBottom.Item4;
					lineRenderData.Height = item2 - item + renderLineSpace * 0.8;
					lineRenderData.OutlineHeight = item4 - item3 + renderLineSpace * 0.8;
					lineRenderData.Top = item2;
					lineRenderData.Bottom = item;
					lineRenderData.OutlineTop = item4;
					lineRenderData.OutlineBottom = item3;
					lineRenderData.Underlines = lineInfo.UnderLines.Values.SelectMany((List<UnderlineRenderData> t) => t).ToList();
				}
				else if (i == 0 && IsFirstLineShrink)
				{
					lineRenderData.Height = lineInfo.DefaultRunBounds.Width + renderLineSpace * 0.8;
				}
				else
				{
					lineRenderData.Height = lineInfo.DefaultRunBounds.Width + renderLineSpace;
				}
				lineRenderData.OutlineHeight = Math.Max(lineRenderData.OutlineHeight, lineRenderData.Height);
				lineRenderData.TextWidth = lineRenderData.CharVisuals.Sum((ICharVisual x) => x.Bounds.Height) + (lineInfo.Equals(lines[0]) ? ParagraphTextBeforeData.FirstLineTextIndent : ParagraphTextBeforeData.NotFirstLineTextIndent);
				lineRenderData.Width = ((Math.Abs(32000.0 - renderWidth) < 0.001) ? lineRenderData.TextWidth : renderWidth);
				list.Add(lineRenderData);
			}
			return list;
		}

		protected override void GenerateTextEffects(List<LineInfo> newLineInfo, List<LineRenderData> lineRenderData)
		{
			for (int i = 0; i < lineRenderData.Count; i++)
			{
				int num = ParagraphNode.StartOffset + newLineInfo[i].LineProperty.ParagraphOffset;
				List<List<int>> seriesSamePropIndexes = GetSeriesSamePropIndexes(newLineInfo[i]);
				List<List<int>> seriesSameFontVariants = GetSeriesSameFontVariants(newLineInfo[i].CharVisuals, num);
				foreach (List<int> indexes in seriesSamePropIndexes)
				{
					RunProperty runProperty = PropertyManager.GetRunProperty(num + indexes.First());
					if (!runProperty.EffectsArgs.Any())
					{
						continue;
					}
					RenderRunProperty renderRunProperty = runProperty.RenderRunProperty;
					List<ICharVisual> list = new List<ICharVisual>();
					if (CharsManager.GetCharAt(num + indexes.First()) is TextChar)
					{
						foreach (int item in indexes)
						{
							list.Add(newLineInfo[i].CharVisuals[item]);
						}
					}
					Rect bounds = new Rect(new Point(0.0, 0.0), new Size(list.Max((ICharVisual x) => x.Bounds.Width), list.Sum((ICharVisual x) => x.Bounds.Height)));
					double baseLine = newLineInfo[i].Baseline;
					if (renderRunProperty.FontVariants != FontVariants.Normal)
					{
						List<int> source = seriesSameFontVariants.First((List<int> x) => x.Contains(indexes.First())).ToList();
						baseLine = newLineInfo[i].ScriptBaseLines[source.First()];
					}
					UIElementContainer charView = TextEffectFactory.CreateTextEffectChar(list.OfType<TextCharVisual>().ToList(), renderRunProperty.EffectsArgs, baseLine, ArrangingType.Vertical);
					lineRenderData[i].EffectCharVisuals.Add(new UiElementCharVisual
					{
						CharView = charView,
						Bounds = bounds
					});
				}
				foreach (ISegment indexes2 in GetUnderLineAndArtSamePropIndexes(newLineInfo[i], num))
				{
					RunProperty runProperty2 = PropertyManager.GetRunProperty(num + indexes2.StartOffset);
					if (!runProperty2.EffectsArgs.Any())
					{
						continue;
					}
					RenderRunProperty renderRunProperty2 = runProperty2.RenderRunProperty;
					double baseLine2 = newLineInfo[i].Baseline;
					if (renderRunProperty2.FontVariants != FontVariants.Normal)
					{
						List<int> source2 = seriesSameFontVariants.First((List<int> x) => x.Contains(indexes2.StartOffset)).ToList();
						baseLine2 = newLineInfo[i].ScriptBaseLines[source2.First()];
					}
					List<UnderlineRenderData> list2 = newLineInfo[i].UnderLines[indexes2.StartOffset];
					List<int> list3 = seriesSamePropIndexes.First((List<int> x) => x.Contains(indexes2.StartOffset)).ToList();
					List<TextCharVisual> list4 = new List<TextCharVisual>();
					foreach (int item2 in list3)
					{
						TextCharVisual textCharVisual = newLineInfo[i].CharVisuals[item2] as TextCharVisual;
						if (textCharVisual != null)
						{
							list4.Add(textCharVisual);
						}
					}
					foreach (UnderlineRenderData item3 in list2)
					{
						Rect bounds2 = new Rect(new Point(item3.UnderlineBounds.X, list4.First().Bounds.Top), new Size(item3.UnderlineBounds.Width, list4.First().Bounds.Height));
						UIElementContainer charView2 = TextEffectFactory.CreateEffectChar(list4, renderRunProperty2.EffectsArgs, DrawType.Underline, item3, baseLine2, ArrangingType.Vertical);
						lineRenderData[i].EffectCharVisuals.Add(new UiElementCharVisual
						{
							CharView = charView2,
							Bounds = bounds2
						});
					}
				}
			}
		}

		protected override double GetRenderUsedWidth(LineInfo newLineInfo)
		{
			return newLineInfo.CharVisuals.Last().Bounds.Bottom;
		}

		protected override double CurrentGeometryWidth(CharGeometryInfo charGeometryInfo)
		{
			return charGeometryInfo.Size.Height;
		}

		protected override double GetContinuousPhraseGroupWidth(PhraseSegment phraseSegment)
		{
			return CharsManager.GeometryPool.GetRange(phraseSegment.Index, phraseSegment.Length).Sum((CharGeometryInfo x) => x.Size.Height);
		}

		protected override double ExtensionalCharWidth(ExtensionalChar extensionalChar)
		{
			return extensionalChar.Size.Height;
		}

		protected override Rect GetCharVisualBounds(double position, Size size)
		{
			return new Rect(new Point(0.0, position), size);
		}

		protected override Rect GetMongolianCharBounds(double position, Size size)
		{
			return GetCharVisualBounds(position, new Size(size.Height, size.Width));
		}

		protected override Rect GetExtensionalCharBounds(double position, ExtensionalChar extensionalChar)
		{
			return new Rect(new Point(0.0, position), extensionalChar.Size);
		}

		protected override void TransformTextChar(double offset, TextCharVisual textCharVisual)
		{
			textCharVisual.TransformTextChar(0.0, offset);
		}

		protected override void GenerateTextEditorDecoration(List<LineInfo> newLineInfos)
		{
			if (!DebuggingProperties.IsDebug || ParagraphNode.Length <= 0)
			{
				return;
			}
			int startOffset = ParagraphNode.StartOffset;
			foreach (LineInfo newLineInfo in newLineInfos)
			{
				int num = startOffset + newLineInfo.LineProperty.ParagraphOffset;
				for (int i = 0; i < newLineInfo.CharVisualsCount; i++)
				{
					TextEditorDecorationCollection property = PropertyManager.GetRunProperty(num + i).RunTimeAttachedRunPropertyCollection.GetProperty<TextEditorDecorationCollection>();
					if (property != null && property.Count > 0)
					{
						throw new NotSupportedException("现在还不支持竖排文本的装饰");
					}
				}
			}
		}

		protected override UIElement RenderExtensionalChar(ExtensionalChar extensionalChar, RunProperty runProperty)
		{
			UIElement uIElement = base.RenderExtensionalChar(extensionalChar, runProperty);
			if (extensionalChar.IsNeedRotationUnderVertical)
			{
				extensionalChar.Size = new Size(extensionalChar.Size.Height, extensionalChar.Size.Width);
				TransformGroup transformGroup = new TransformGroup();
				transformGroup.Children.Add(new RotateTransform(90.0));
				transformGroup.Children.Add(new TranslateTransform(extensionalChar.Size.Width, 0.0));
				uIElement.RenderTransform = transformGroup;
			}
			return uIElement;
		}

		protected override ValueTuple<double, int, List<ICharVisual>, bool> BuildMongolian(int startOffset, double renderWidthInner, double offset, double notFirstRenderInner)
		{
			return TextHelperInner.BuildMongolian(CharsManager, PropertyManager, startOffset, renderWidthInner, offset, notFirstRenderInner, true);
		}

		protected override void AdaptBaseLine(LineInfo newLineInfo)
		{
			int num = ParagraphNode.StartOffset + newLineInfo.LineProperty.ParagraphOffset;
			int maxCharIndexWithoutScript = GetMaxCharIndexWithoutScript(newLineInfo);
			ValueTuple<Geometry, Size> valueTuple = TextHelperInner.BuildDefaultCharGeometry(PropertyManager.GetRunProperty(num + maxCharIndexWithoutScript));
			Geometry item = valueTuple.Item1;
			Size item2 = valueTuple.Item2;
			item = TextHelperInner.CorrectToVerticalGeometry(item, item2);
			double num2 = (newLineInfo.Baseline = item.Bounds.Left);
			List<List<int>> list = (newLineInfo.SamePropIndexes = GetSamePropIndexes(newLineInfo));
			foreach (List<int> item7 in list)
			{
				RunProperty runProperty = PropertyManager.GetRunProperty(item7.First() + num);
				if (runProperty.FontVariants != FontVariants.Normal)
				{
					continue;
				}
				ValueTuple<Geometry, Size> valueTuple2 = TextHelperInner.BuildDefaultCharGeometry(runProperty);
				Geometry item3 = valueTuple2.Item1;
				Size item4 = valueTuple2.Item2;
				item3 = TextHelperInner.CorrectToVerticalGeometry(item3, item4);
				double xOffset = num2 - item3.Bounds.Left;
				foreach (int item8 in item7)
				{
					TextCharVisual textCharVisual = (TextCharVisual)newLineInfo.CharVisuals[item8];
					textCharVisual.TransformTextChar(xOffset, 0.0);
					textCharVisual.Bounds = textCharVisual.Bounds.OffsetRect(xOffset, 0.0);
				}
			}
			ValueTuple<Geometry, Size> valueTuple3 = TextHelperInner.BuildDefaultCharGeometry(newLineInfo.Equals(Lines.First()) ? TargetParagraphProperty.DefaultRunProperty : PropertyManager.GetRunProperty(num).Clone());
			Geometry item5 = valueTuple3.Item1;
			Size item6 = valueTuple3.Item2;
			item5 = TextHelperInner.CorrectToVerticalGeometry(item5, item6);
			newLineInfo.DefaultRunBounds = newLineInfo.DefaultRunBounds.OffsetRect(num2 - item5.Bounds.Left, 0.0);
		}

		protected override void AdaptLogicalCenter(LineInfo lineInfo)
		{
			for (int i = 0; i < lineInfo.CharVisualsCount; i++)
			{
				ExtensionCharVisual extensionCharVisual = lineInfo.CharVisuals[i] as ExtensionCharVisual;
				if (extensionCharVisual != null)
				{
					int lastTextCharIndex = GetLastTextCharIndex(lineInfo, i);
					double xOffset = GetNormalBounds(lineInfo, lastTextCharIndex).Center().X - extensionCharVisual.LogicalCenter.Y;
					extensionCharVisual.Bounds = extensionCharVisual.Bounds.OffsetRect(xOffset, 0.0);
				}
			}
		}

		protected override void AdaptScript(LineInfo newLineInfo)
		{
			int num = ParagraphNode.StartOffset + newLineInfo.LineProperty.ParagraphOffset;
			foreach (List<int> seriesSameFontVariant in GetSeriesSameFontVariants(newLineInfo.CharVisuals, num))
			{
				Rect normalRunBounds = GetNormalRunBounds(seriesSameFontVariant.First(), newLineInfo, num);
				foreach (List<int> seriesSameScriptPropIndex in GetSeriesSameScriptPropIndexes(num, seriesSameFontVariant))
				{
					foreach (int item5 in seriesSameScriptPropIndex)
					{
						RunProperty runProperty = PropertyManager.GetRunProperty(item5 + num).Clone();
						runProperty.FontSize /= 2.0;
						ValueTuple<Geometry, Size> valueTuple = TextHelperInner.BuildDefaultCharGeometry(runProperty);
						Geometry item = valueTuple.Item1;
						Size item2 = valueTuple.Item2;
						item = TextHelperInner.CorrectToVerticalGeometry(item, item2);
						double left = item.Bounds.Left;
						double xOffset;
						if (Converter.AreEquals(runProperty.FontVariants, FontVariants.Superscript))
						{
							double num2 = normalRunBounds.Width / 3.0 * 2.0;
							xOffset = num2 - left + normalRunBounds.Left;
							if (!newLineInfo.ScriptBaseLines.ContainsKey(seriesSameScriptPropIndex.First()))
							{
								newLineInfo.ScriptBaseLines.Add(seriesSameScriptPropIndex.First(), num2);
							}
						}
						else
						{
							RunProperty runProperty2 = PropertyManager.GetRunProperty(seriesSameFontVariant.First() + num).Clone();
							runProperty2.FontSize /= 2.0;
							ValueTuple<Geometry, Size> valueTuple2 = TextHelperInner.BuildDefaultCharGeometry(runProperty2);
							Geometry item3 = valueTuple2.Item1;
							Size item4 = valueTuple2.Item2;
							item3 = TextHelperInner.CorrectToVerticalGeometry(item3, item4);
							double left2 = item3.Bounds.Left;
							double num3 = normalRunBounds.Width / 3.0;
							xOffset = left2 - left + num3 - item3.Bounds.Right + normalRunBounds.Left;
							if (!newLineInfo.ScriptBaseLines.ContainsKey(seriesSameScriptPropIndex.First()))
							{
								newLineInfo.ScriptBaseLines.Add(seriesSameScriptPropIndex.First(), left2 + num3 - item3.Bounds.Right);
							}
						}
						TextCharVisual textCharVisual = (TextCharVisual)newLineInfo.CharVisuals[item5];
						textCharVisual.TransformTextChar(xOffset, 0.0);
						textCharVisual.Bounds = textCharVisual.Bounds.OffsetRect(xOffset, 0.0);
					}
				}
			}
		}

		protected override void AdaptLineSpace(LineInfo newLineInfo, bool firstLineExpand)
		{
			double num = 0.0;
			int num2 = ParagraphNode.StartOffset + newLineInfo.LineProperty.ParagraphOffset;
			double num3 = 0.0;
			RunProperty maxFontSizeRunProperty = (newLineInfo.CharVisuals.Any() ? PropertyManager.GetRunProperty(num2) : TargetParagraphProperty.DefaultRunProperty);
			for (int i = 0; i < newLineInfo.CharVisualsCount; i++)
			{
				RunProperty runProperty = PropertyManager.GetRunProperty(num2 + i);
				if (runProperty.FontVariants == FontVariants.Normal && num > newLineInfo.CharVisuals[i].Bounds.Right)
				{
					num = newLineInfo.CharVisuals[i].Bounds.Right;
				}
				if (runProperty.FontSize > num3)
				{
					maxFontSizeRunProperty = runProperty;
					num3 = runProperty.FontSize;
				}
			}
			double num4 = (newLineInfo.RenderLineSpace = GetRenderLineSpace(maxFontSizeRunProperty));
			double yOffset = 0.0;
			if (newLineInfo.CharVisuals.Any() && num < 0.0)
			{
				yOffset = 0.0 - num;
			}
			if (firstLineExpand)
			{
				yOffset += num4 * 0.19999999999999996;
			}
			newLineInfo.Baseline += yOffset;
			newLineInfo.ScriptBaseLines = newLineInfo.ScriptBaseLines.ToDictionary((KeyValuePair<int, double> x) => x.Key, (KeyValuePair<int, double> y) => y.Value + yOffset);
			TransformCharAndBounds(newLineInfo, yOffset, 0.0);
		}

		protected override void AdaptHorizontalAlignment(LineInfo newLineInfo)
		{
			double num = (newLineInfo.CharVisuals.Any() ? newLineInfo.CharVisuals.Max((ICharVisual visual) => visual.Bounds.Bottom) : newLineInfo.DefaultRunBounds.Height);
			double renderWidthInner = GetRenderWidthInner(LastRenderWidth, newLineInfo.LineProperty.ParagraphOffset + ParagraphNode.StartOffset);
			double yOffset = 0.0;
			switch (TargetParagraphProperty.TextAlignment)
			{
			case TextAlignment.Center:
				yOffset = (renderWidthInner - num) / 2.0;
				break;
			case TextAlignment.Right:
				yOffset = renderWidthInner - num;
				break;
			case TextAlignment.Justify:
				HandleJustify(newLineInfo);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case TextAlignment.Left:
				break;
			}
			TransformCharAndBounds(newLineInfo, 0.0, yOffset);
		}

		protected override void GenerateUnderline(LineInfo newLineInfo)
		{
			int num = ParagraphNode.StartOffset + newLineInfo.LineProperty.ParagraphOffset;
			int maxCharIndexWithoutScript = GetMaxCharIndexWithoutScript(newLineInfo);
			RunProperty runProperty = PropertyManager.GetRunProperty(num + maxCharIndexWithoutScript);
			List<List<int>> seriesSameFontVariants = GetSeriesSameFontVariants(newLineInfo.CharVisuals, num);
			_003C_003Ec__DisplayClass20_1 obj = default(_003C_003Ec__DisplayClass20_1);
			foreach (ISegment underLineAndArtSamePropIndex in GetUnderLineAndArtSamePropIndexes(newLineInfo, num))
			{
				int indexFirst = underLineAndArtSamePropIndex.StartOffset;
				RunProperty renderRunProperty = PropertyManager.GetRenderRunProperty(num + indexFirst);
				bool flag = renderRunProperty.Hyperlink != null;
				if (!flag && !renderRunProperty.TextDecoration.Any())
				{
					continue;
				}
				int num2;
				if (renderRunProperty.FontVariants == FontVariants.Normal)
				{
					obj.maxRunFontSize = runProperty.FontSize;
					num2 = maxCharIndexWithoutScript;
				}
				else
				{
					List<int> indexes = seriesSameFontVariants.First((List<int> x) => x.Contains(indexFirst)).ToList();
					num2 = GetCurrentMaxFontSizeRunIndex(indexes, num);
					obj.maxRunFontSize = PropertyManager.GetRunProperty(num2 + num).FontSize / 2.0;
				}
				obj.underlineWidth = obj.maxRunFontSize * 0.06;
				obj.underlineHeight = newLineInfo.CharVisuals.GetRange(indexFirst, underLineAndArtSamePropIndex.Length).Sum((ICharVisual x) => x.Bounds.Height);
				ICharVisual charVisual = newLineInfo.CharVisuals[indexFirst];
				ICharVisual charVisual2 = newLineInfo.CharVisuals[num2];
				obj.firstCharVisualBounds = charVisual.Bounds;
				obj.lastCharVisualBounds = charVisual2.Bounds;
				Brush brush = (obj.foreground = renderRunProperty.UnderlineBrush ?? renderRunProperty.Foreground);
				obj.underlineRenderDataList = new List<UnderlineRenderData>(renderRunProperty.TextDecoration.Count + (flag ? 1 : 0));
				newLineInfo.UnderLines.Add(indexFirst, obj.underlineRenderDataList);
				if (flag)
				{
					_003CGenerateUnderline_003Eg__DrawTextDecorationLine_007C20_1(TextDecorationLocation.Underline, ref obj);
				}
				foreach (TextDecoration item in renderRunProperty.TextDecoration.OfType<TextDecoration>())
				{
					if (!flag || item.Location != TextDecorationLocation.Underline)
					{
						obj.foreground = item.Pen?.Brush ?? brush;
						_003CGenerateUnderline_003Eg__DrawTextDecorationLine_007C20_1(item.Location, ref obj);
					}
				}
			}
		}

		protected override ValueTuple<Geometry, TextCharGlyphData> BuildMongolianTextGeometry(RunProperty runProperty, string text)
		{
			Geometry textGeometry;
			Size textGeometrySize;
			TextCharGlyphData textCharGlyphData;
			TextGeometryRender.BuildTextGeometry(runProperty, text).Deconstruct(out textGeometry, out textGeometrySize, out textCharGlyphData);
			Geometry geometry = textGeometry;
			Size size = textGeometrySize;
			return new ValueTuple<Geometry, TextCharGlyphData>(item2: textCharGlyphData, item1: TextHelperInner.CorrectToVerticalGeometry(geometry, size));
		}

		protected void HandleJustify(LineInfo newLineInfo)
		{
			double space = GetRenderWidthInner(LastRenderWidth, newLineInfo.LineProperty.ParagraphOffset + ParagraphNode.StartOffset) - newLineInfo.CharVisuals.Sum((ICharVisual x) => x.Bounds.Height);
			int num = ParagraphNode.StartOffset + newLineInfo.LineProperty.ParagraphOffset;
			if (TextHelperInner.IsContainMongolian(CharsManager, num, num + newLineInfo.LineProperty.Length))
			{
				HandleMongolianJustify(newLineInfo, space);
			}
			else
			{
				HandleNormalJustify(newLineInfo, space);
			}
		}

		private void HandleNormalJustify(LineInfo newLineInfo, double space)
		{
			if (space > 1E-05 && newLineInfo.CharVisualsCount > 1)
			{
				double num = space / (double)(newLineInfo.CharVisualsCount - 1);
				for (int i = 0; i < newLineInfo.CharVisualsCount; i++)
				{
					(newLineInfo.CharVisuals[i] as TextCharVisual)?.TransformTextChar(0.0, (double)i * num);
					Size size = new Size(newLineInfo.CharVisuals[i].Bounds.Size.Width, newLineInfo.CharVisuals[i].Bounds.Size.Height + ((i == newLineInfo.CharVisualsCount - 1) ? 0.0 : num));
					Point location = new Point(newLineInfo.CharVisuals[i].Bounds.X, newLineInfo.CharVisuals[i].Bounds.Y + (double)i * num);
					newLineInfo.CharVisuals[i].Bounds = new Rect(location, size);
				}
			}
		}

		private void HandleMongolianJustify(LineInfo newLineInfo, double space)
		{
			int start = ParagraphNode.StartOffset + newLineInfo.LineProperty.ParagraphOffset;
			List<Tuple<int, int>> list = TextHelperInner.HandleMongolianLine(CharsManager, start, newLineInfo.LineProperty.Length);
			if (space > 1E-05 && list.Count > 1)
			{
				double num = space / (double)(list.Count - 1);
				for (int i = 0; i < newLineInfo.CharVisualsCount; i++)
				{
					int item;
					bool item2;
					TupleExtensions.Deconstruct(TextHelperInner.GetMongolianIndex(i, list), out item, out item2);
					int num2 = item;
					bool flag = item2;
					(newLineInfo.CharVisuals[i] as TextCharVisual)?.TransformTextChar(0.0, (double)i * num);
					Size size = new Size(newLineInfo.CharVisuals[i].Bounds.Width, newLineInfo.CharVisuals[i].Bounds.Height + ((i == newLineInfo.CharVisualsCount - 1 || !flag) ? 0.0 : num));
					Point location = new Point(newLineInfo.CharVisuals[i].Bounds.X, newLineInfo.CharVisuals[i].Bounds.Y + (double)num2 * num);
					newLineInfo.CharVisuals[i].Bounds = new Rect(location, size);
				}
			}
		}

		
		private ValueTuple<double, double, double, double> GetMaxTopAndMinBottom(LineInfo lineInfo, int startOffset)
		{
			double num = 0.0;
			double num2 = 0.0;
			double val = 0.0;
			double val2 = 0.0;
			List<Rect> list = new List<Rect>();
			int num3 = 0;
			for (int i = 0; i < lineInfo.CharVisualsCount; i++)
			{
				RunProperty runProperty = PropertyManager.GetRunProperty(startOffset + i);
				Rect bounds = lineInfo.CharVisuals[i].Bounds;
				if (runProperty.FontVariants != FontVariants.Normal && lineInfo.CharVisuals[i] is TextCharVisual)
				{
					val2 = Math.Max(val2, bounds.Right);
					val = Math.Min(val, bounds.Left);
					num3++;
				}
				else
				{
					list.Add(bounds);
				}
			}
			if (num3 == lineInfo.CharVisualsCount)
			{
				list.Add(lineInfo.DefaultRunBounds);
			}
			if (list.Any())
			{
				num = list.Min((Rect item) => item.Left);
				num = Math.Min(0.0, num);
				num2 = list.Max((Rect x) => x.Right);
			}
			if (num < 0.0)
			{
				num = 0.0;
			}
			val2 = Math.Max(val2, num2);
			val = Math.Min(val, num);
			return new ValueTuple<double, double, double, double>(num, num2, val, val2);
		}

		protected override Rect GetNormalBoundsInner(LineInfo lineInfo, RunProperty runProp, int index)
		{
			ValueTuple<Geometry, Size> valueTuple = TextHelperInner.BuildDefaultCharGeometry(runProp);
			Geometry item = valueTuple.Item1;
			Size item2 = valueTuple.Item2;
			item = TextHelperInner.CorrectToVerticalGeometry(item, item2);
			double xOffset = lineInfo.Baseline - item.Bounds.Left;
			Geometry textGeometry;
			Size textGeometrySize;
			TextCharGlyphData textCharGlyphData;
			TextGeometryRender.BuildCharGeometry(runProp, CharsManager.GetCharAt(index)).Deconstruct(out textGeometry, out textGeometrySize, out textCharGlyphData);
			Geometry geometry = textGeometry;
			Size size = textGeometrySize;
			return TextHelperInner.CorrectToVerticalGeometry(geometry, size).Bounds.OffsetRect(xOffset, 0.0);
		}

		protected override void ArrangeBlocks(List<Block> blocks)
		{
			FlowDirection direction = TargetParagraphProperty.Direction;
			double num = 0.0;
			if (direction == FlowDirection.RightToLeft)
			{
				for (int num2 = blocks.Count - 1; num2 >= 0; num2--)
				{
					Rect bounds = blocks[num2].Bounds;
					bounds.Offset(0.0, num);
					blocks[num2].Bounds = bounds;
					num += bounds.Height;
				}
			}
			else
			{
				for (int i = 0; i < blocks.Count; i++)
				{
					Rect bounds2 = blocks[i].Bounds;
					bounds2.Offset(0.0, num);
					blocks[i].Bounds = bounds2;
					num += bounds2.Height;
				}
			}
			blocks.ForEach(ArrangeChars);
		}

		private void ArrangeChars(Block block)
		{
			double num = block.Bounds.Top;
			List<ICharVisual> charVisuals = block.CharVisuals;
			if (block.Direction == FlowDirection.RightToLeft)
			{
				for (int num2 = charVisuals.Count - 1; num2 >= 0; num2--)
				{
					Rect bounds = charVisuals[num2].Bounds;
					charVisuals[num2].Bounds = new Rect(new Point(bounds.Left, num), bounds.Size);
					(charVisuals[num2] as TextCharVisual)?.TransformTextChar(0.0, num - bounds.Top);
					num += bounds.Height;
					HandleLigature(charVisuals[num2]);
				}
			}
			else
			{
				for (int i = 0; i < charVisuals.Count; i++)
				{
					Rect bounds2 = charVisuals[i].Bounds;
					charVisuals[i].Bounds = new Rect(new Point(bounds2.Left, num), bounds2.Size);
					(charVisuals[i] as TextCharVisual)?.TransformTextChar(0.0, num - bounds2.Top);
					num += bounds2.Height;
				}
			}
		}

		private void HandleLigature(ICharVisual visual)
		{
			TextCharVisual textCharVisual = visual as TextCharVisual;
			if (textCharVisual != null && textCharVisual.IsIsLigature && textCharVisual.Direction == CharFlowDirection.RightToLeft && textCharVisual.TextGeometry != null && !textCharVisual.TextGeometry.IsEmpty())
			{
				double height = textCharVisual.TextGeometry.Bounds.Height;
				textCharVisual.TransformTextChar(0.0, textCharVisual.Bounds.Height - height);
			}
		}

		protected override Block BuildBlock(List<ICharVisual> charVisuals, FlowDirection direction)
		{
			Block block = new Block
			{
				CharVisuals = charVisuals.ToList(),
				Direction = direction
			};
			double height = charVisuals.Sum((ICharVisual visual) => visual.Bounds.Height);
			block.Bounds = new Rect(new Size(0.0, height));
			charVisuals.ForEach((ICharVisual visual) =>
			{
				visual.ActualDirection = direction;
			});
			return block;
		}

		internal override DocumentOffset GetDocumentOffsetByPositionFromRunBounds(Point position)
		{
			if (ParagraphNode.Length == 0)
			{
				return DocumentOffset.DefaultDocumentOffset;
			}
			Point point = new Point(position.X - VisualTop, position.Y);
			if (point.X < 0.0 || point.X > VisualHeight)
			{
				return DocumentOffset.DefaultDocumentOffset;
			}
			ValueTuple<LineRenderData, LineProperty> lineDataByParagraphXOffset = GetLineDataByParagraphXOffset(point.X);
			LineRenderData item = lineDataByParagraphXOffset.Item1;
			LineProperty item2 = lineDataByParagraphXOffset.Item2;
			double num = position.Y - (item.Equals(LineRenderData[0]) ? ParagraphTextBeforeData.FirstLineTextIndent : ParagraphTextBeforeData.NotFirstLineTextIndent);
			if (num < 0.0 || num > item.CharVisuals.Last().Bounds.Bottom)
			{
				return DocumentOffset.DefaultDocumentOffset;
			}
			for (int i = 0; i < item.CharVisualsCount; i++)
			{
				ICharVisual charVisual = item.CharVisuals[i];
				if (charVisual.Bounds.Y <= num && charVisual.Bounds.Bottom > num)
				{
					return ParagraphNode.StartOffset + item2.ParagraphOffset + i;
				}
			}
			return DocumentOffset.DefaultDocumentOffset;
		}

		internal override Rect GetRunBoundsByCaretOffset(CaretOffset caretOffset)
		{
			throw new NotSupportedException("暂时不支持在竖排文本相对于文档的光标偏移量获取字符相对于段落的bounds方法");
		}

		internal override CaretOffset GetCaretOffsetByParagraphPosition(Point paragraphPosition)
		{
			ValueTuple<LineRenderData, LineProperty> lineDataByParagraphXOffset = GetLineDataByParagraphXOffset(paragraphPosition.X);
			LineRenderData item = lineDataByParagraphXOffset.Item1;
			LineProperty item2 = lineDataByParagraphXOffset.Item2;
			double num = (item.Equals(LineRenderData.First()) ? ParagraphTextBeforeData.FirstLineTextIndent : ParagraphTextBeforeData.NotFirstLineTextIndent);
			int lineOffsetByParagraphYPosition = GetLineOffsetByParagraphYPosition(paragraphPosition.Y - num, item);
			CaretOffset result = new CaretOffset
			{
				Offset = ParagraphNode.StartOffset + item2.ParagraphOffset + lineOffsetByParagraphYPosition,
				IsAtLineStart = (lineOffsetByParagraphYPosition == -1)
			};
			result.IsAtParagraphStart = result.Offset == ParagraphNode.StartOffset - 1;
			return result;
		}

		private int GetLineOffsetByParagraphYPosition(double yOffset, LineRenderData lineRenderData)
		{
			if (!lineRenderData.CharVisuals.Any())
			{
				return -1;
			}
			int num = 0;
			int num2 = 0;
			double num3 = double.NegativeInfinity;
			double num4 = double.PositiveInfinity;
			for (int i = 0; i < lineRenderData.CharVisualsCount; i++)
			{
				ICharVisual charVisual = lineRenderData.CharVisuals[i];
				Rect bounds = charVisual.Bounds;
				if (yOffset >= bounds.Top && yOffset <= bounds.Bottom)
				{
					if (charVisual.ActualDirection == FlowDirection.LeftToRight)
					{
						if (!(yOffset < bounds.Center().Y))
						{
							return i;
						}
						return i - 1;
					}
					if (!(yOffset < bounds.Center().Y))
					{
						return i - 1;
					}
					return i;
				}
				if (bounds.Bottom > num3)
				{
					num3 = bounds.Bottom;
					num2 = i;
				}
				if (bounds.Top < num4)
				{
					num4 = bounds.Top;
					num = i;
				}
			}
			if (yOffset > num3)
			{
				if (lineRenderData.CharVisuals[num2].ActualDirection != FlowDirection.LeftToRight)
				{
					return num2 - 1;
				}
				return num2;
			}
			if (yOffset < num4)
			{
				if (lineRenderData.CharVisuals[num2].ActualDirection != FlowDirection.LeftToRight)
				{
					return num;
				}
				return num - 1;
			}
			throw new InvalidOperationException();
		}

		
		private ValueTuple<LineRenderData, LineProperty> GetLineDataByParagraphXOffset(double xOffset)
		{
			if (xOffset < 0.0)
			{
				return new ValueTuple<LineRenderData, LineProperty>(LineRenderData[0], Lines[0].LineProperty);
			}
			double num = xOffset;
			for (int i = 0; i < LineRenderData.Count; i++)
			{
				LineRenderData lineRenderData = LineRenderData[i];
				if (lineRenderData.Height > num)
				{
					return new ValueTuple<LineRenderData, LineProperty>(lineRenderData, Lines[i].LineProperty);
				}
				num -= lineRenderData.Height;
			}
			return new ValueTuple<LineRenderData, LineProperty>(LineRenderData.Last(), Lines.Last().LineProperty);
		}

		[CompilerGenerated]
		private static void _003CGenerateUnderline_003Eg__DrawTextDecorationLine_007C20_1(TextDecorationLocation location, ref _003C_003Ec__DisplayClass20_1 P_1)
		{
			double x;
			switch (location)
			{
			case TextDecorationLocation.Underline:
				x = P_1.lastCharVisualBounds.Left + P_1.maxRunFontSize / 8.0;
				break;
			case TextDecorationLocation.Strikethrough:
				x = P_1.lastCharVisualBounds.Left + P_1.lastCharVisualBounds.Width / 2.0 - P_1.underlineWidth / 2.0;
				break;
			default:
				x = P_1.lastCharVisualBounds.Left;
				break;
			}
			double y = P_1.firstCharVisualBounds.Y;
			Rect underlineBounds = new Rect(new Point(x, y), new Size(P_1.underlineWidth, P_1.underlineHeight));
			P_1.underlineRenderDataList.Add(new UnderlineRenderData
			{
				Foreground = P_1.foreground,
				UnderlineBounds = underlineBounds
			});
		}
	}
}
