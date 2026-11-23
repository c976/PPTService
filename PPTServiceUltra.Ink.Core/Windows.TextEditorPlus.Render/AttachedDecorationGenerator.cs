using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal sealed class AttachedDecorationGenerator
	{
		private class DecorationSegmentGenerator
		{
			private AttachedDecorationGenerator AttachedDecorationGenerator { get; }

			private LineInfo NewLineInfo { get; }

			private int StartOffset { get; }

			private PropertyManager PropertyManager => AttachedDecorationGenerator.PropertyManager;

			public DecorationSegmentGenerator(AttachedDecorationGenerator attachedDecorationGenerator, LineInfo newLineInfo, int startOffset)
			{
				AttachedDecorationGenerator = attachedDecorationGenerator;
				NewLineInfo = newLineInfo;
				StartOffset = startOffset;
			}

			
			public IReadOnlyList<ValueTuple<ISegment, TextEditorDecoration>> GetDecorationAndArtSamePropertySegmentList()
			{
				LineInfo newLineInfo = NewLineInfo;
				int startOffset = StartOffset;
				PropertyManager propertyManager = PropertyManager;
				List<ValueTuple<ISegment, TextEditorDecoration>> list = new List<ValueTuple<ISegment, TextEditorDecoration>>();
				CharDecorationPropertySegment charDecorationPropertySegment = null;
				TextEditorDecoration textEditorDecoration = null;
				for (int i = 0; i < newLineInfo.CharVisualsCount; i++)
				{
					if (!(newLineInfo.CharVisuals[i] is TextCharVisual))
					{
						continue;
					}
					int num = startOffset + i;
					TextEditorDecorationCollection property = propertyManager.GetRunProperty(num).RunTimeAttachedRunPropertyCollection.GetProperty<TextEditorDecorationCollection>();
					if (property == null || property.Count == 0)
					{
						charDecorationPropertySegment = null;
						textEditorDecoration = null;
						continue;
					}
					if (property.Count == 1)
					{
						TextEditorDecoration textEditorDecoration2 = property[0];
						if (charDecorationPropertySegment == null && textEditorDecoration == null)
						{
							charDecorationPropertySegment = new CharDecorationPropertySegment(num);
							textEditorDecoration = textEditorDecoration2;
							list.Add(new ValueTuple<ISegment, TextEditorDecoration>(charDecorationPropertySegment, textEditorDecoration));
						}
						else if (textEditorDecoration.Equals(textEditorDecoration2))
						{
							charDecorationPropertySegment.Length++;
						}
						else
						{
							charDecorationPropertySegment = new CharDecorationPropertySegment(num);
							textEditorDecoration = textEditorDecoration2;
							list.Add(new ValueTuple<ISegment, TextEditorDecoration>(charDecorationPropertySegment, textEditorDecoration));
						}
						continue;
					}
					throw new NotImplementedException("还没实现支持多个装饰");
				}
				return list;
			}
		}

		private class RunTimeTextEditorDecorationGenerator
		{
			private ParagraphBase Paragraph => AttachedDecorationGenerator.Paragraph;

			private PropertyManager PropertyManager => AttachedDecorationGenerator.PropertyManager;

			private ParagraphNode ParagraphNode => AttachedDecorationGenerator.ParagraphNode;

			private int ParagraphNodeStartOffset { get; }

			private int MaxCharIndexInLine { get; }

			private int StartOffset { get; }

			private RunProperty MaxRunPropertyInLine { get; }

			private List<ICharVisual> CharVisuals { get; }

			private List<List<int>> SameFontVariants { get; }

			private AttachedDecorationGenerator AttachedDecorationGenerator { get; }

			private LineInfo NewLineInfo { get; }

			public RunTimeTextEditorDecorationGenerator(AttachedDecorationGenerator attachedDecorationGenerator, LineInfo newLineInfo)
			{
				AttachedDecorationGenerator = attachedDecorationGenerator;
				NewLineInfo = newLineInfo;
				MaxCharIndexInLine = Paragraph.GetMaxCharIndexWithoutScript(newLineInfo);
				ParagraphNodeStartOffset = ParagraphNode.StartOffset;
				StartOffset = ParagraphNodeStartOffset + newLineInfo.LineProperty.ParagraphOffset;
				MaxRunPropertyInLine = PropertyManager.GetRunProperty(StartOffset + MaxCharIndexInLine);
				CharVisuals = newLineInfo.CharVisuals;
				SameFontVariants = Paragraph.GetSeriesSameFontVariants(CharVisuals, StartOffset);
			}

			public void GenerateRunTimeTextEditorDecoration()
			{
				LineInfo newLineInfo = NewLineInfo;
				foreach (var decorationAndArtSamePropertySegment in new DecorationSegmentGenerator(AttachedDecorationGenerator, NewLineInfo, StartOffset).GetDecorationAndArtSamePropertySegmentList())
				{
					ISegment item = decorationAndArtSamePropertySegment.Item1;
					TextEditorDecoration item2 = decorationAndArtSamePropertySegment.Item2;
					int startOffset = item.StartOffset;
					RunProperty renderRunProperty = PropertyManager.GetRenderRunProperty(startOffset);
					Brush foreground = renderRunProperty.Foreground;
					if (item2.TextDecorationLocation == TextDecorationLocation.Underline)
					{
						Rect underlineBounds = GetUnderlineBounds(item, renderRunProperty);
						newLineInfo.AttachedDecorationDataList.Add(new AttachedDecorationData
						{
							TextEditorDecoration = item2,
							Bounds = underlineBounds,
							Foreground = foreground
						});
						continue;
					}
					throw new NotImplementedException("还没实现计算不是下标的方法");
				}
			}

			private int DocumentOffsetToLineOffset(DocumentOffset documentOffset)
			{
				return (int)documentOffset - StartOffset;
			}

			private Rect GetUnderlineBounds(ISegment segment, RunProperty runProp)
			{
				double num;
				int num2;
				if (runProp.FontVariants == FontVariants.Normal)
				{
					num = MaxRunPropertyInLine.FontSize;
					num2 = MaxCharIndexInLine;
				}
				else
				{
					int segmentParagraphStartOffset = DocumentOffsetToLineOffset(segment.StartOffset);
					List<int> indexes = SameFontVariants.First((List<int> list) => list.Contains(segmentParagraphStartOffset)).ToList();
					num2 = Paragraph.GetCurrentMaxFontSizeRunIndex(indexes, StartOffset);
					num = PropertyManager.GetRunProperty(num2 + StartOffset).FontSize / 2.0;
				}
				double height = num * 0.06;
				int paragraphOffset = NewLineInfo.LineProperty.ParagraphOffset;
				int index = segment.StartOffset - ParagraphNodeStartOffset - paragraphOffset;
				double width = CharVisuals.GetRange(index, segment.Length).Sum((ICharVisual charVisual3) => charVisual3.Bounds.Width);
				ICharVisual charVisual = CharVisuals[index];
				ICharVisual charVisual2 = CharVisuals[num2];
				Rect bounds = charVisual.Bounds;
				Rect bounds2 = charVisual2.Bounds;
				double x = bounds.X;
				double y = bounds2.Bottom - num / 8.0;
				return new Rect(new Point(x, y), new Size(width, height));
			}
		}

		private PropertyManager PropertyManager { get; }

		private ParagraphNode ParagraphNode { get; }

		private ParagraphBase Paragraph { get; }

		public AttachedDecorationGenerator(PropertyManager propertyManager, ParagraphNode paragraphNode, ParagraphBase paragraph)
		{
			PropertyManager = propertyManager;
			ParagraphNode = paragraphNode;
			Paragraph = paragraph;
		}

		public void GenerateRunTimeTextEditorDecoration(LineInfo newLineInfo)
		{
			new RunTimeTextEditorDecorationGenerator(this, newLineInfo).GenerateRunTimeTextEditorDecoration();
		}
	}
}
