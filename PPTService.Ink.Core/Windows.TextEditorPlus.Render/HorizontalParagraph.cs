using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.Mathematics.Geometries;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Extensions;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class HorizontalParagraph : ParagraphBase
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass24_0
		{
			public int indexFirst;

			public RunProperty runProp;

			internal bool _003CGenerateUnderline_003Eb__2(List<int> x)
			{
				return x.Contains(indexFirst);
			}

			internal void _003CGenerateUnderline_003Eg__DrawTextDecorationLine_007C1(TextDecorationLocation location, ref _003C_003Ec__DisplayClass24_1 P_1)
			{
				double x = P_1.firstCharVisualBounds.X;
				double y;
				switch (location)
				{
				case TextDecorationLocation.Underline:
					y = P_1.lastCharVisualBounds.Bottom - P_1.maxRunFontSize / 8.0;
					break;
				case TextDecorationLocation.Strikethrough:
					y = P_1.lastCharVisualBounds.Top + P_1.lastCharVisualBounds.Height / 2.0 - P_1.underlineHeight / 2.0;
					break;
				default:
					y = P_1.lastCharVisualBounds.Bottom;
					break;
				}
				Rect underlineBounds = new Rect(new Point(x, y), new Size(P_1.underlineWidth, P_1.underlineHeight));
				P_1.underlineRenderDataList.Add(new UnderlineRenderData
				{
					Foreground = P_1.foreground,
					UnderlineBounds = underlineBounds,
					DashStyle = runProp.UnderlineDashStyle
				});
			}
		}

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass24_1
		{
			public Rect firstCharVisualBounds;

			public Rect lastCharVisualBounds;

			public double maxRunFontSize;

			public double underlineHeight;

			public double underlineWidth;

			public List<UnderlineRenderData> underlineRenderDataList;

			public Brush foreground;
		}

		internal HorizontalParagraph(CharsManager charsManager, PropertyManager propertyManager, ParagraphNode paragraphNode, ITextGeometryRender textGeometryRender)
			: base(charsManager, propertyManager, paragraphNode, textGeometryRender)
		{
		}

		protected override void GenerateTextEditorDecoration(List<LineInfo> newLineInfos)
		{
			if (ParagraphNode.Length <= 0)
			{
				return;
			}
			foreach (LineInfo newLineInfo in newLineInfos)
			{
				GenerateRunTimeTextEditorDecoration(newLineInfo);
			}
		}

		private void GenerateRunTimeTextEditorDecoration(LineInfo newLineInfo)
		{
			new AttachedDecorationGenerator(PropertyManager, ParagraphNode, this).GenerateRunTimeTextEditorDecoration(newLineInfo);
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
					bounds.Offset(num, 0.0);
					blocks[num2].Bounds = bounds;
					num += bounds.Width;
				}
			}
			else
			{
				for (int i = 0; i < blocks.Count; i++)
				{
					Rect bounds2 = blocks[i].Bounds;
					bounds2.Offset(num, 0.0);
					blocks[i].Bounds = bounds2;
					num += bounds2.Width;
				}
			}
			blocks.ForEach(ArrangeChars);
		}

		private void ArrangeChars(Block block)
		{
			double num = block.Bounds.Left;
			List<ICharVisual> charVisuals = block.CharVisuals;
			if (block.Direction == FlowDirection.RightToLeft)
			{
				for (int num2 = charVisuals.Count - 1; num2 >= 0; num2--)
				{
					Rect bounds = charVisuals[num2].Bounds;
					charVisuals[num2].Bounds = new Rect(new Point(num, bounds.Top), bounds.Size);
					(charVisuals[num2] as TextCharVisual)?.TransformTextChar(num - bounds.Left, 0.0);
					num += bounds.Width;
					HandleLigature(charVisuals[num2]);
				}
			}
			else
			{
				for (int i = 0; i < charVisuals.Count; i++)
				{
					Rect bounds2 = charVisuals[i].Bounds;
					charVisuals[i].Bounds = new Rect(new Point(num, bounds2.Top), bounds2.Size);
					(charVisuals[i] as TextCharVisual)?.TransformTextChar(num - bounds2.Left, 0.0);
					num += bounds2.Width;
				}
			}
		}

		private static void HandleLigature(ICharVisual visual)
		{
			TextCharVisual textCharVisual = visual as TextCharVisual;
			if (textCharVisual != null && textCharVisual.IsIsLigature && textCharVisual.Direction == CharFlowDirection.RightToLeft && textCharVisual.TextGeometry != null && !textCharVisual.TextGeometry.IsEmpty())
			{
				double width = textCharVisual.TextGeometry.Bounds.Width;
				textCharVisual.TransformTextChar(textCharVisual.Bounds.Width - width, 0.0);
			}
		}

		protected override Block BuildBlock(List<ICharVisual> charVisuals, FlowDirection direction)
		{
			Block block = new Block
			{
				CharVisuals = charVisuals.ToList(),
				Direction = direction
			};
			double width = charVisuals.Sum((ICharVisual visual) => visual.Bounds.Width);
			block.Bounds = new Rect(new Size(width, 0.0));
			charVisuals.ForEach((ICharVisual visual) =>
			{
				visual.ActualDirection = direction;
			});
			return block;
		}

		protected override double GetRenderUsedWidth(LineInfo newLineInfo)
		{
			return newLineInfo.CharVisuals.Last().Bounds.Right;
		}

		protected override double CurrentGeometryWidth(CharGeometryInfo charGeometryInfo)
		{
			return charGeometryInfo.Size.Width;
		}

		protected override double GetContinuousPhraseGroupWidth(PhraseSegment phraseSegment)
		{
			return CharsManager.GeometryPool.GetRange(phraseSegment.Index, phraseSegment.Length).Sum((CharGeometryInfo x) => x.Size.Width);
		}

		protected override double ExtensionalCharWidth(ExtensionalChar extensionalChar)
		{
			return extensionalChar.Size.Width;
		}

		protected override Rect GetCharVisualBounds(double position, Size size)
		{
			return new Rect(new Point(position, 0.0), size);
		}

		protected override Rect GetMongolianCharBounds(double position, Size size)
		{
			return GetCharVisualBounds(position, size);
		}

		protected override Rect GetExtensionalCharBounds(double position, ExtensionalChar extensionalChar)
		{
			return new Rect(new Point(position, 0.0), extensionalChar.Size);
		}

		protected override void TransformTextChar(double offset, TextCharVisual textCharVisual)
		{
			textCharVisual.TransformTextChar(offset, 0.0);
		}

		protected override ValueTuple<double, int, List<ICharVisual>, bool> BuildMongolian(int startOffset, double renderWidthInner, double offset, double notFirstRenderInner)
		{
			return TextHelperInner.BuildMongolian(CharsManager, PropertyManager, startOffset, renderWidthInner, offset, notFirstRenderInner);
		}

		protected override void FillLineInfosDefaultBounds(List<LineInfo> lines)
		{
			for (int i = 0; i < lines.Count; i++)
			{
				RunProperty runProperty = null;
				runProperty = ((!lines[i].Equals(Lines.First())) ? PropertyManager.GetRunProperty(ParagraphNode.StartOffset + lines[i].LineProperty.ParagraphOffset).Clone() : TargetParagraphProperty.DefaultRunProperty.Clone());
				Size item = TextHelperInner.BuildDefaultCharGeometry(runProperty).Item2;
				lines[i].DefaultRunBounds = new Rect(default(Point), new Size(1.0, item.Height));
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
					ValueTuple<double, double, double, double> minTopAndMaxBottom = GetMinTopAndMaxBottom(lineInfo, startOffset);
					double item = minTopAndMaxBottom.Item1;
					double item2 = minTopAndMaxBottom.Item2;
					double item3 = minTopAndMaxBottom.Item3;
					double item4 = minTopAndMaxBottom.Item4;
					lineRenderData.Height = item2 - item + renderLineSpace * 0.19999999999999996;
					lineRenderData.OutlineHeight = item4 - item3 + renderLineSpace * 0.19999999999999996;
					lineRenderData.Top = item;
					lineRenderData.Bottom = item2;
					lineRenderData.OutlineTop = item3;
					lineRenderData.OutlineBottom = item4;
					lineRenderData.Underlines = lineInfo.UnderLines.Values.SelectMany((List<UnderlineRenderData> t) => t).ToList();
					lineRenderData.AttachedDecorationDataList = lineInfo.AttachedDecorationDataList;
				}
				else if (i == 0 && IsFirstLineShrink)
				{
					lineRenderData.Height = lineInfo.DefaultRunBounds.Height + renderLineSpace * 0.19999999999999996;
				}
				else
				{
					lineRenderData.Height = lineInfo.DefaultRunBounds.Height + renderLineSpace;
				}
				lineRenderData.OutlineHeight = Math.Max(lineRenderData.Height, lineRenderData.OutlineHeight);
				lineRenderData.TextWidth = lineRenderData.CharVisuals.Sum((ICharVisual x) => x.Bounds.Width) + (lineInfo.Equals(lines[0]) ? ParagraphTextBeforeData.FirstLineTextIndent : ParagraphTextBeforeData.NotFirstLineTextIndent);
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
					List<ICharVisual> list = new List<ICharVisual>(indexes.Count);
					foreach (int item in indexes)
					{
						list.Add(newLineInfo[i].CharVisuals[item]);
					}
					Rect bounds = new Rect(new Point(0.0, 0.0), new Size(list.Sum((ICharVisual x) => x.Bounds.Width), list.Max((ICharVisual x) => x.Bounds.Height)));
					double baseLine = newLineInfo[i].Baseline;
					if (renderRunProperty.FontVariants != FontVariants.Normal)
					{
						List<int> source = seriesSameFontVariants.First((List<int> x) => x.Contains(indexes.First())).ToList();
						baseLine = newLineInfo[i].ScriptBaseLines[source.First()];
					}
					UIElementContainer charView = TextEffectFactory.CreateTextEffectChar(list.OfType<TextCharVisual>().ToList(), renderRunProperty.EffectsArgs, baseLine);
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
						UIElementContainer charView2 = TextEffectFactory.CreateEffectChar(list4, renderRunProperty2.EffectsArgs, DrawType.Underline, item3, baseLine2);
						lineRenderData[i].EffectCharVisuals.Add(new UiElementCharVisual
						{
							CharView = charView2,
							Bounds = bounds2
						});
					}
				}
			}
		}

		protected override void AdaptBaseLine(LineInfo newLineInfo)
		{
			int num = ParagraphNode.StartOffset + newLineInfo.LineProperty.ParagraphOffset;
			int maxCharIndexWithoutScript = GetMaxCharIndexWithoutScript(newLineInfo);
			double num2 = (newLineInfo.Baseline = TextHelperInner.BuildDefaultCharBaseline(PropertyManager.GetRunProperty(num + maxCharIndexWithoutScript)));
			List<List<int>> list = (newLineInfo.SamePropIndexes = GetSamePropIndexes(newLineInfo));
			foreach (List<int> item in list)
			{
				RunProperty runProperty = PropertyManager.GetRunProperty(item.First() + num);
				if (runProperty.FontVariants != FontVariants.Normal)
				{
					continue;
				}
				double num4 = TextHelperInner.BuildDefaultCharBaseline(runProperty);
				double yOffset = num2 - num4;
				foreach (int item2 in item)
				{
					TextCharVisual textCharVisual = (TextCharVisual)newLineInfo.CharVisuals[item2];
					textCharVisual.TransformTextChar(0.0, yOffset);
					textCharVisual.Bounds = textCharVisual.Bounds.OffsetRect(0.0, yOffset);
				}
			}
			double num5 = TextHelperInner.BuildDefaultCharBaseline(newLineInfo.Equals(Lines.First()) ? TargetParagraphProperty.DefaultRunProperty : PropertyManager.GetRunProperty(num).Clone());
			newLineInfo.DefaultRunBounds = newLineInfo.DefaultRunBounds.OffsetRect(0.0, num2 - num5);
		}

		protected override void AdaptLogicalCenter(LineInfo lineInfo)
		{
			for (int i = 0; i < lineInfo.CharVisualsCount; i++)
			{
				ExtensionCharVisual extensionCharVisual = lineInfo.CharVisuals[i] as ExtensionCharVisual;
				if (extensionCharVisual != null)
				{
					int lastTextCharIndex = GetLastTextCharIndex(lineInfo, i);
					double yOffset = GetNormalBounds(lineInfo, lastTextCharIndex).Center().Y - extensionCharVisual.LogicalCenter.Y;
					extensionCharVisual.Bounds = extensionCharVisual.Bounds.OffsetRect(0.0, yOffset);
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
					foreach (int item2 in seriesSameScriptPropIndex)
					{
						RunProperty runProperty = PropertyManager.GetRunProperty(item2 + num).Clone();
						runProperty.FontSize /= 2.0;
						double num2 = TextHelperInner.BuildDefaultCharBaseline(runProperty);
						double yOffset;
						if (Converter.AreEquals(runProperty.FontVariants, FontVariants.Superscript))
						{
							double num3 = normalRunBounds.Height / 3.0;
							yOffset = normalRunBounds.Top + num3 - num2;
							if (!newLineInfo.ScriptBaseLines.ContainsKey(seriesSameScriptPropIndex.First()))
							{
								newLineInfo.ScriptBaseLines.Add(seriesSameScriptPropIndex.First(), normalRunBounds.Top + num3);
							}
						}
						else
						{
							RunProperty runProperty2 = PropertyManager.GetRunProperty(seriesSameFontVariant.First() + num).Clone();
							runProperty2.FontSize /= 2.0;
							Geometry item = TextHelperInner.BuildDefaultCharGeometry(runProperty2).Item1;
							double bottom = item.Bounds.Bottom;
							double num4 = normalRunBounds.Height * 2.0 / 3.0;
							yOffset = bottom - num2 + normalRunBounds.Top + num4 - item.Bounds.Top;
							if (!newLineInfo.ScriptBaseLines.ContainsKey(seriesSameScriptPropIndex.First()))
							{
								newLineInfo.ScriptBaseLines.Add(seriesSameScriptPropIndex.First(), bottom + normalRunBounds.Top + num4 - item.Bounds.Top);
							}
						}
						TextCharVisual textCharVisual = (TextCharVisual)newLineInfo.CharVisuals[item2];
						textCharVisual.TransformTextChar(0.0, yOffset);
						textCharVisual.Bounds = textCharVisual.Bounds.OffsetRect(0.0, yOffset);
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
				if (runProperty.FontVariants == FontVariants.Normal)
				{
					double top = newLineInfo.CharVisuals[i].Bounds.Top;
					if (num > top)
					{
						num = top;
					}
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
				yOffset += num4 * 0.8;
			}
			newLineInfo.Baseline += yOffset;
			newLineInfo.ScriptBaseLines = newLineInfo.ScriptBaseLines.ToDictionary((KeyValuePair<int, double> x) => x.Key, (KeyValuePair<int, double> y) => y.Value + yOffset);
			TransformCharAndBounds(newLineInfo, 0.0, yOffset);
		}

		protected override void AdaptHorizontalAlignment(LineInfo newLineInfo)
		{
			double num = (newLineInfo.CharVisuals.Any() ? newLineInfo.CharVisuals.Max((ICharVisual visual) => visual.Bounds.Right) : newLineInfo.DefaultRunBounds.Width);
			double renderWidthInner = GetRenderWidthInner(LastRenderWidth, newLineInfo.LineProperty.ParagraphOffset + ParagraphNode.StartOffset);
			double num2 = 0.0;
			switch (TargetParagraphProperty.TextAlignment)
			{
			case TextAlignment.Center:
				num2 = (renderWidthInner - num) / 2.0;
				break;
			case TextAlignment.Right:
				num2 = renderWidthInner - num;
				break;
			case TextAlignment.Justify:
				HandleJustify(newLineInfo);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case TextAlignment.Left:
				break;
			}
			if (Math.Abs(num2) > 1E-05)
			{
				TransformCharAndBounds(newLineInfo, num2, 0.0);
			}
		}

		protected override void GenerateUnderline(LineInfo newLineInfo)
		{
			int num = ParagraphNode.StartOffset + newLineInfo.LineProperty.ParagraphOffset;
			int maxCharIndexWithoutScript = GetMaxCharIndexWithoutScript(newLineInfo);
			RunProperty runProperty = PropertyManager.GetRunProperty(num + maxCharIndexWithoutScript);
			List<ICharVisual> charVisuals = newLineInfo.CharVisuals;
			List<List<int>> seriesSameFontVariants = GetSeriesSameFontVariants(charVisuals, num);
			_003C_003Ec__DisplayClass24_1 obj = default(_003C_003Ec__DisplayClass24_1);
			foreach (ISegment underLineAndArtSamePropIndex in GetUnderLineAndArtSamePropIndexes(newLineInfo, num))
			{
				_003C_003Ec__DisplayClass24_0 CS_0024_003C_003E8__locals16 = new _003C_003Ec__DisplayClass24_0();
				CS_0024_003C_003E8__locals16.indexFirst = underLineAndArtSamePropIndex.StartOffset;
				CS_0024_003C_003E8__locals16.runProp = PropertyManager.GetRenderRunProperty(num + CS_0024_003C_003E8__locals16.indexFirst);
				bool flag = CS_0024_003C_003E8__locals16.runProp.Hyperlink != null;
				if (!flag && !CS_0024_003C_003E8__locals16.runProp.TextDecoration.Any())
				{
					continue;
				}
				int num2;
				if (CS_0024_003C_003E8__locals16.runProp.FontVariants == FontVariants.Normal)
				{
					obj.maxRunFontSize = runProperty.FontSize;
					num2 = maxCharIndexWithoutScript;
				}
				else
				{
					List<int> indexes = seriesSameFontVariants.First((List<int> x) => x.Contains(CS_0024_003C_003E8__locals16.indexFirst)).ToList();
					num2 = GetCurrentMaxFontSizeRunIndex(indexes, num);
					obj.maxRunFontSize = PropertyManager.GetRunProperty(num2 + num).FontSize / 2.0;
				}
				obj.underlineHeight = obj.maxRunFontSize * 0.06;
				obj.underlineWidth = charVisuals.GetRange(CS_0024_003C_003E8__locals16.indexFirst, underLineAndArtSamePropIndex.Length).Sum((ICharVisual x) => x.Bounds.Width);
				ICharVisual charVisual = charVisuals[CS_0024_003C_003E8__locals16.indexFirst];
				ICharVisual charVisual2 = charVisuals[num2];
				obj.firstCharVisualBounds = charVisual.Bounds;
				obj.lastCharVisualBounds = charVisual2.Bounds;
				Brush brush = (obj.foreground = CS_0024_003C_003E8__locals16.runProp.UnderlineBrush ?? CS_0024_003C_003E8__locals16.runProp.Foreground);
				obj.underlineRenderDataList = new List<UnderlineRenderData>(CS_0024_003C_003E8__locals16.runProp.TextDecoration.Count + (flag ? 1 : 0));
				newLineInfo.UnderLines.Add(CS_0024_003C_003E8__locals16.indexFirst, obj.underlineRenderDataList);
				if (flag)
				{
					CS_0024_003C_003E8__locals16._003CGenerateUnderline_003Eg__DrawTextDecorationLine_007C1(TextDecorationLocation.Underline, ref obj);
				}
				foreach (TextDecoration item in CS_0024_003C_003E8__locals16.runProp.TextDecoration.OfType<TextDecoration>())
				{
					if (!flag || item.Location != TextDecorationLocation.Underline)
					{
						obj.foreground = item.Pen?.Brush ?? brush;
						CS_0024_003C_003E8__locals16._003CGenerateUnderline_003Eg__DrawTextDecorationLine_007C1(item.Location, ref obj);
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
			Geometry item = textGeometry;
			TextCharGlyphData item2 = textCharGlyphData;
			return new ValueTuple<Geometry, TextCharGlyphData>(item, item2);
		}

		
		private ValueTuple<double, double, double, double> GetMinTopAndMaxBottom(LineInfo lineInfo, int startOffset)
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
					val = Math.Min(val, bounds.Top);
					val2 = Math.Max(val2, bounds.Bottom);
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
				num = list.Min((Rect item) => item.Top);
				num = Math.Min(0.0, num);
				num2 = list.Max((Rect x) => x.Bottom);
			}
			if (num < 0.0)
			{
				num = 0.0;
			}
			val = Math.Min(val, num);
			val2 = Math.Max(val2, num2);
			return new ValueTuple<double, double, double, double>(num, num2, val, val2);
		}

		private void HandleJustify(LineInfo newLineInfo)
		{
			double space = GetRenderWidthInner(LastRenderWidth, newLineInfo.LineProperty.ParagraphOffset + ParagraphNode.StartOffset) - newLineInfo.CharVisuals.Sum((ICharVisual x) => x.Bounds.Width);
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
					ICharVisual charVisual = newLineInfo.CharVisuals[i];
					(charVisual as TextCharVisual)?.TransformTextChar((double)i * num, 0.0);
					Rect bounds = charVisual.Bounds;
					Size size = new Size(bounds.Width + ((i == newLineInfo.CharVisualsCount - 1) ? 0.0 : num), bounds.Height);
					Point location = new Point(bounds.X + (double)i * num, bounds.Y);
					charVisual.Bounds = new Rect(location, size);
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
					ICharVisual charVisual = newLineInfo.CharVisuals[i];
					(charVisual as TextCharVisual)?.TransformTextChar((double)num2 * num, 0.0);
					Rect bounds = charVisual.Bounds;
					Size size = new Size(bounds.Width + ((i == newLineInfo.CharVisualsCount - 1 || !flag) ? 0.0 : num), bounds.Height);
					Point location = new Point(bounds.X + (double)num2 * num, bounds.Y);
					charVisual.Bounds = new Rect(location, size);
				}
			}
		}

		
		private ValueTuple<LineRenderData, LineProperty> GetLineDataByParagraphYOffset(double yOffset)
		{
			if (yOffset < 0.0)
			{
				return new ValueTuple<LineRenderData, LineProperty>(LineRenderData[0], Lines[0].LineProperty);
			}
			double num = yOffset;
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

		private int GetLineOffsetByParagraphXPosition(double xOffset, LineRenderData lineRenderData)
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
				if (xOffset >= bounds.Left && xOffset <= bounds.Right)
				{
					if (charVisual.ActualDirection == FlowDirection.LeftToRight)
					{
						if (!(xOffset < bounds.Center().X))
						{
							return i;
						}
						return i - 1;
					}
					if (!(xOffset < bounds.Center().X))
					{
						return i - 1;
					}
					return i;
				}
				if (bounds.Right > num3)
				{
					num3 = bounds.Right;
					num2 = i;
				}
				if (bounds.Left < num4)
				{
					num4 = bounds.Left;
					num = i;
				}
			}
			if (xOffset > num3)
			{
				if (lineRenderData.CharVisuals[num2].ActualDirection != FlowDirection.LeftToRight)
				{
					return num2 - 1;
				}
				return num2;
			}
			if (xOffset < num4)
			{
				if (lineRenderData.CharVisuals[num2].ActualDirection != FlowDirection.LeftToRight)
				{
					return num;
				}
				return num - 1;
			}
			throw new InvalidOperationException();
		}

		protected override Rect GetNormalBoundsInner(LineInfo lineInfo, RunProperty runProp, int index)
		{
			double num = TextHelperInner.BuildDefaultCharBaseline(runProp);
			double yOffset = lineInfo.Baseline - num;
			Geometry textGeometry;
			Size textGeometrySize;
			TextCharGlyphData textCharGlyphData;
			TextGeometryRender.BuildCharGeometry(runProp, CharsManager.GetCharAt(index)).Deconstruct(out textGeometry, out textGeometrySize, out textCharGlyphData);
			return textGeometry.Bounds.OffsetRect(0.0, yOffset);
		}

		internal override CaretOffset GetCaretOffsetByParagraphPosition(Point paragraphPosition)
		{
			ValueTuple<LineRenderData, LineProperty> lineDataByParagraphYOffset = GetLineDataByParagraphYOffset(paragraphPosition.Y);
			LineRenderData item = lineDataByParagraphYOffset.Item1;
			LineProperty item2 = lineDataByParagraphYOffset.Item2;
			double num = (item.Equals(LineRenderData.First()) ? ParagraphTextBeforeData.FirstLineTextIndent : ParagraphTextBeforeData.NotFirstLineTextIndent);
			int lineOffsetByParagraphXPosition = GetLineOffsetByParagraphXPosition(paragraphPosition.X - num, item);
			CaretOffset result = new CaretOffset
			{
				Offset = ParagraphNode.StartOffset + item2.ParagraphOffset + lineOffsetByParagraphXPosition,
				IsAtLineStart = (lineOffsetByParagraphXPosition == -1)
			};
			result.IsAtParagraphStart = result.Offset == ParagraphNode.StartOffset - 1;
			return result;
		}

		internal override DocumentOffset GetDocumentOffsetByPositionFromRunBounds(Point position)
		{
			if (ParagraphNode.Length == 0)
			{
				return DocumentOffset.DefaultDocumentOffset;
			}
			Point point = new Point(position.X, position.Y - VisualTop);
			if (point.Y < 0.0 || point.Y > VisualHeight)
			{
				return DocumentOffset.DefaultDocumentOffset;
			}
			ValueTuple<LineRenderData, LineProperty> lineDataByParagraphYOffset = GetLineDataByParagraphYOffset(point.Y);
			LineRenderData item = lineDataByParagraphYOffset.Item1;
			LineProperty item2 = lineDataByParagraphYOffset.Item2;
			if (item.CharVisuals.Count == 0)
			{
				return DocumentOffset.DefaultDocumentOffset;
			}
			double num = point.X - (item.Equals(LineRenderData[0]) ? ParagraphTextBeforeData.FirstLineTextIndent : ParagraphTextBeforeData.NotFirstLineTextIndent);
			if (num < 0.0 || num > item.CharVisuals.Last().Bounds.Right)
			{
				return DocumentOffset.DefaultDocumentOffset;
			}
			for (int i = 0; i < item.CharVisualsCount; i++)
			{
				ICharVisual charVisual = item.CharVisuals[i];
				if (charVisual.Bounds.X <= num && charVisual.Bounds.Right > num)
				{
					return ParagraphNode.StartOffset + item2.ParagraphOffset + i;
				}
			}
			return DocumentOffset.DefaultDocumentOffset;
		}

		internal override Rect GetRunBoundsByCaretOffset(CaretOffset caretOffset)
		{
			int num = caretOffset.Offset - ParagraphNode.StartOffset;
			LineRenderData lineRenderDataByCaretOffset = GetLineRenderDataByCaretOffset(caretOffset);
			int lineIndex = GetLineIndex(lineRenderDataByCaretOffset);
			double num2 = ((lineIndex == 0) ? ParagraphTextBeforeData.FirstLineTextIndent : ParagraphTextBeforeData.NotFirstLineTextIndent);
			double num3 = num2;
			if (lineRenderDataByCaretOffset.CharVisuals.Count > 0)
			{
				ICharVisual charVisual = lineRenderDataByCaretOffset.CharVisuals[0];
				Rect bounds = charVisual.Bounds;
				num3 += ((charVisual.ActualDirection == FlowDirection.LeftToRight) ? bounds.Left : bounds.Right);
			}
			else
			{
				num3 += Lines[lineIndex].DefaultRunBounds.X;
			}
			if (caretOffset.IsAtParagraphStart)
			{
				Rect defaultRunBounds = Lines[lineIndex].DefaultRunBounds;
				return new Rect(num3, defaultRunBounds.Y, defaultRunBounds.Width, defaultRunBounds.Height);
			}
			double num4 = LineRenderData.Take(lineIndex).Sum((LineRenderData x) => x.Height);
			if (caretOffset.IsAtLineStart)
			{
				Rect defaultRunBounds2 = Lines[lineIndex].DefaultRunBounds;
				return new Rect(num3, defaultRunBounds2.Y + num4, defaultRunBounds2.Width, defaultRunBounds2.Height);
			}
			int index = num - Lines[lineIndex].LineProperty.ParagraphOffset;
			return lineRenderDataByCaretOffset.CharVisuals[index].Bounds.OffsetRect(num2, num4);
		}
	}
}
