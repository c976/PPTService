using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Diagnostics.Trace;
using PPTService.Ink.Core.IO;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Extensions;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal abstract class ParagraphBase
	{
		protected class Block
		{
			public FlowDirection Direction { get; set; }

			public Rect Bounds { get; set; }

			public List<ICharVisual> CharVisuals { get; set; } = new List<ICharVisual>();
		}

		protected double LastRenderWidth { get; set; }

		protected CharsManager CharsManager { get; }

		protected PropertyManager PropertyManager { get; }

		internal ParagraphNode ParagraphNode { get; }

		protected ITextGeometryRender TextGeometryRender { get; }

		internal bool IsAlive { get; set; }

		internal List<LineProperty> ParagraphLineInfoRenderedData { get; }

		public virtual double VisualHeight { get; protected set; }

		public double VisualWidth { get; protected set; }

		public double DeltaOutlineTop { get; protected set; }

		public double DeltaOutlineBottom { get; protected set; }

		internal List<LineRenderData> LineRenderData { get; set; } = new List<LineRenderData>();

		internal List<LineInfo> Lines { get; set; } = new List<LineInfo>();

		public virtual double VisualTop { get; set; }

		public ParagraphProperty TargetParagraphProperty => PropertyManager.GetParagraphProperty(ParagraphNode.ParagraphNumber - 1);

		internal ParagraphTextBeforeData ParagraphTextBeforeData { get; set; }

		private static ILogTracer LayoutCharTracer { get; }

		internal bool IsFirstLineShrink
		{
			get
			{
				if (ParagraphNode.ParagraphNumber == 1)
				{
					return PropertyManager.LineSpacingStrategy == LineSpacingStrategy.FirstLineShrink;
				}
				return false;
			}
		}

		protected ParagraphBase(CharsManager charsManager, PropertyManager propertyManager, ParagraphNode paragraphNode, ITextGeometryRender textGeometryRender)
		{
			CharsManager = charsManager;
			PropertyManager = propertyManager;
			ParagraphNode = paragraphNode;
			TextGeometryRender = textGeometryRender;
			ParagraphLineInfoRenderedData = new List<LineProperty>();
		}

		internal int BuildRenderData(ISegment segment, double renderWidth)
		{
			List<LineRenderData> list = new List<LineRenderData>();
			int result = 0;
			int num;
			if (Math.Abs(renderWidth - LastRenderWidth) < 0.001 && LineRenderData.Count > 1)
			{
				ValueTuple<int, int> valueTuple = OptimizeParagraphRender(segment, list);
				int item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				num = item;
				result = item2;
			}
			else
			{
				num = ParagraphNode.StartOffset;
				Lines.Clear();
			}
			LastRenderWidth = renderWidth;
			int num2 = num;
			int charsCount = ParagraphNode.EndOffset - num2;
			List<LineRenderData> collection = BuildRenderDataInner(num2, charsCount, renderWidth);
			list.AddRange(collection);
			LineRenderData = list;
			BuildVisualHeightAndWidth();
			return result;
		}

		private void BuildVisualHeightAndWidth()
		{
			double renderParagraphBefore = GetRenderParagraphBefore();
			double renderParagraphAfter = GetRenderParagraphAfter();
			double visualHeight = LineRenderData.Sum((LineRenderData x) => x.Height) + renderParagraphBefore + renderParagraphAfter;
			ValueTuple<double, double> deltaOutlineTopAndBottom = GetDeltaOutlineTopAndBottom();
			double item = deltaOutlineTopAndBottom.Item1;
			double item2 = deltaOutlineTopAndBottom.Item2;
			DeltaOutlineTop = item;
			DeltaOutlineBottom = item2;
			VisualHeight = visualHeight;
			VisualWidth = LineRenderData.Max((LineRenderData x) => x.TextWidth);
		}

		
		private ValueTuple<double, double> GetDeltaOutlineTopAndBottom()
		{
			if (LineRenderData.Count == 0)
			{
				return new ValueTuple<double, double>(0.0, 0.0);
			}
			LineRenderData lineRenderData = LineRenderData.First();
			double item = Math.Abs(lineRenderData.Top - lineRenderData.OutlineTop);
			LineRenderData lineRenderData2 = LineRenderData.Last();
			double item2 = Math.Abs(lineRenderData2.Bottom - lineRenderData2.OutlineBottom);
			return new ValueTuple<double, double>(item, item2);
		}

		private ValueTuple<int, int> OptimizeParagraphRender(ISegment segment, List<LineRenderData> newLineRenderData)
		{
			int item;
			int num;
			if (segment.StartOffset < ParagraphNode.StartOffset)
			{
				item = ParagraphNode.StartOffset;
				num = 0;
			}
			else
			{
				LineInfo lineInfoFromParagraphOffset = GetLineInfoFromParagraphOffset(segment.StartOffset - ParagraphNode.StartOffset);
				num = ((lineInfoFromParagraphOffset != null) ? Lines.IndexOf(lineInfoFromParagraphOffset) : (-1));
				if (PropertyManager.TextWrapping != TextWrapping.NoWrap)
				{
					num = Math.Max(0, num - 1);
				}
				item = ((num > 0) ? (Lines.GetRange(0, num).Sum((LineInfo x) => x.CharVisualsCount) + ParagraphNode.StartOffset) : ParagraphNode.StartOffset);
			}
			newLineRenderData.AddRange(LineRenderData.GetRange(0, num));
			Lines.RemoveRange(num, Lines.Count - num);
			return new ValueTuple<int, int>(item, num);
		}

		protected double GetRenderWidthInner(double renderWidth, int startIndex)
		{
			return ((ParagraphNode.StartOffset == startIndex) ? (renderWidth - ParagraphTextBeforeData.FirstLineTextIndent) : (renderWidth - ParagraphTextBeforeData.NotFirstLineTextIndent)) + 0.001;
		}

		protected List<LineRenderData> BuildRenderDataInner(int startIndex, int charsCount, double renderWidth)
		{
			List<LineInfo> list = FillLineInfos(startIndex, charsCount, renderWidth);
			Lines.AddRange(list);
			UpdateLineProperty();
			FillLineInfosDefaultBounds(list);
			CorrectCharBounds(list);
			GenerateUnderlines(list);
			GenerateTextEditorDecoration(list);
			List<LineRenderData> list2 = ConvertLineInfoToLineRenderData(list, renderWidth);
			GenerateTextEffects(list, list2);
			return list2;
		}

		protected abstract void GenerateTextEditorDecoration(List<LineInfo> newLineInfos);

		protected List<LineInfo> FillLineInfos(int startIndex, int charsCount, double renderWidth)
		{
			List<LineInfo> list = new List<LineInfo>();
			LineInfo item = new LineInfo();
			if (startIndex == ParagraphNode.StartOffset && charsCount == 0)
			{
				if (PropertyManager.ParagraphLineInfoRenderingData.Count > 0)
				{
					PropertyManager.ParagraphLineInfoRenderingData.Remove(ParagraphNode.ParagraphNumber - 1);
				}
				list.Add(item);
				return list;
			}
			if (PropertyManager.ParagraphLineInfoRenderingData.Count > 0)
			{
				List<LineProperty> lineProperties = PropertyManager.ParagraphLineInfoRenderingData[ParagraphNode.ParagraphNumber - 1];
				List<LineInfo> newLineInfoFromLineProperties = GetNewLineInfoFromLineProperties(lineProperties);
				list.AddRange(newLineInfoFromLineProperties);
				PropertyManager.ParagraphLineInfoRenderingData.Remove(ParagraphNode.ParagraphNumber - 1);
			}
			else
			{
				List<LineInfo> newLineInfo = GetNewLineInfo(startIndex, charsCount, renderWidth);
				list.AddRange(newLineInfo);
			}
			return list;
		}

		private List<LineInfo> GetNewLineInfoFromLineProperties(List<LineProperty> lineProperties)
		{
			List<LineInfo> list = new List<LineInfo>();
			foreach (LineProperty lineProperty in lineProperties)
			{
				LineInfo lineInfo = new LineInfo();
				int num = ParagraphNode.StartOffset + lineProperty.ParagraphOffset;
				for (int i = 0; i < lineProperty.Length; i++)
				{
					int num2 = num + i;
					ICharObject charAt = CharsManager.GetCharAt(num2);
					double num3 = ((lineInfo.CharVisuals.Count > 0) ? GetRenderUsedWidth(lineInfo) : 0.0);
					RunProperty renderRunProperty = PropertyManager.GetRenderRunProperty(num2);
					TextChar textChar = charAt as TextChar;
					if (textChar != null)
					{
						CharGeometryInfo charGeometriesInfo = CharsManager.GeometryPool.GetCharGeometriesInfo(num2);
						if (TextUtilities.IsLigature(textChar.ToText()))
						{
							int nextNotMongolianCharDocumentIndexInParagraph = TextHelperInner.GetNextNotMongolianCharDocumentIndexInParagraph(CharsManager, num2, num + lineProperty.Length);
							StringBuilder stringBuilder = new StringBuilder();
							for (int j = i + num; j < nextNotMongolianCharDocumentIndexInParagraph; j++)
							{
								stringBuilder.Append(CharsManager.GetCharAt(j).ToText());
							}
							RunProperty renderRunProperty2 = PropertyManager.GetRenderRunProperty(num2);
							string text = stringBuilder.ToString();
							List<Size> list2 = CalculateMongolianTextSizes(text, renderRunProperty2);
							Geometry item = BuildMongolianTextGeometry(renderRunProperty2, text).Item1;
							CharFlowDirection direction = ((!TextUtilities.IsRightToLeftLigature(text)) ? CharFlowDirection.LeftToRight : CharFlowDirection.RightToLeft);
							for (int k = 0; k < list2.Count; k++)
							{
								TextCharVisual textCharVisual = new TextCharVisual
								{
									Bounds = GetMongolianCharBounds(num3, list2[k]),
									Foreground = renderRunProperty.Foreground,
									TextGeometry = ((k == 0) ? item : Geometry.Empty),
									GlyphData = null,
									Direction = direction,
									IsIsLigature = true
								};
								if (k == 0)
								{
									textCharVisual.Matrix = item.Transform.Value;
								}
								TransformTextChar(num3, textCharVisual);
								lineInfo.CharVisuals.Add(textCharVisual);
								num3 += list2[k].Width;
							}
							i += text.Length - 1;
						}
						else
						{
							lineInfo.CharVisuals.Add(CreateTextCharVisual(num3, textChar, charGeometriesInfo, renderRunProperty.Foreground));
						}
					}
					else
					{
						ExtensionalChar extensionalChar = charAt as ExtensionalChar;
						if (extensionalChar != null)
						{
							ExtensionCharVisual extensionCharVisual = CreateExtensionCharVisual(extensionalChar, renderRunProperty);
							extensionCharVisual.Bounds = GetExtensionalCharBounds(num3, extensionalChar);
							lineInfo.CharVisuals.Add(extensionCharVisual);
						}
					}
				}
				list.Add(lineInfo);
			}
			return list;
		}

		private List<Size> CalculateMongolianTextSizes(string mongolianText, RunProperty runProperty)
		{
			TextLine textLine = TextHelperInner.BuildTextLines(mongolianText, runProperty, TargetParagraphProperty, 32000.0).First();
			return (from x in (from y in textLine.GetTextBounds(0, textLine.Length)
					where y.TextRunBounds != null
					select y).SelectMany((TextBounds x) => x.TextRunBounds).ToList()
				select x.Rectangle.Size).ToList();
		}

		private ExtensionCharVisual CreateExtensionCharVisual(ExtensionalChar extensionalChar, RunProperty runProperty)
		{
			return new ExtensionCharVisual
			{
				View = RenderExtensionalChar(extensionalChar, runProperty),
				LogicalCenter = extensionalChar.LogicalCenter,
				IsNeedRotationUnderVertical = extensionalChar.IsNeedRotationUnderVertical,
				IsHitTestVisible = extensionalChar.HitTestEnable
			};
		}

		protected virtual UIElement RenderExtensionalChar(ExtensionalChar extensionalChar, RunProperty runProperty)
		{
			UIElement uIElement = extensionalChar.Render(runProperty);
			uIElement.Opacity = runProperty.Opacity;
			uIElement.RenderTransform = Transform.Identity;
			return uIElement;
		}

		private TextCharVisual CreateTextCharVisual(double position, TextChar textChar, CharGeometryInfo geometriesInfo, Brush foreground)
		{
			TextCharVisual textCharVisual = new TextCharVisual
			{
				Bounds = GetCharVisualBounds(position, geometriesInfo.Size),
				Foreground = foreground,
				TextGeometry = geometriesInfo.Geometry,
				Matrix = geometriesInfo.OriginalMatrix,
				GlyphData = geometriesInfo.Data,
				Direction = TextUtilities.GetFlowDirection(textChar)
			};
			TransformTextChar(position, textCharVisual);
			return textCharVisual;
		}

		protected List<LineInfo> GetNewLineInfo1(int startIndex, int charsCount, double renderWidth)
		{
			List<LineInfo> list = new List<LineInfo>();
			LineInfo lineInfo = new LineInfo();
			double num = GetRenderWidthInner(renderWidth, startIndex);
			double num2 = renderWidth - ParagraphTextBeforeData.NotFirstLineTextIndent;
			for (int i = startIndex; i < startIndex + charsCount; i++)
			{
				ICharObject charAt = CharsManager.GetCharAt(i);
				double num3 = 0.0;
				RunProperty renderRunProperty = PropertyManager.GetRenderRunProperty(i);
				TextChar textChar = charAt as TextChar;
				if (textChar != null)
				{
					CharGeometryInfo charGeometriesInfo = CharsManager.GeometryPool.GetCharGeometriesInfo(i);
					num3 += ((lineInfo.CharVisuals.Count > 0) ? GetRenderUsedWidth(lineInfo) : 0.0);
					if (TextUtilities.IsLigature(textChar.ToText()))
					{
						ValueTuple<double, int, List<ICharVisual>, bool> valueTuple = BuildMongolian(i, num, num3, num2);
						double item = valueTuple.Item1;
						int item2 = valueTuple.Item2;
						List<ICharVisual> item3 = valueTuple.Item3;
						bool item4 = valueTuple.Item4;
						num = item;
						i = item2;
						if (item3.Count > 0)
						{
							lineInfo.CharVisuals.AddRange(item3);
						}
						if (item4)
						{
							list.Add(lineInfo);
							lineInfo = new LineInfo();
						}
					}
					else if (num3 + CurrentGeometryWidth(charGeometriesInfo) <= num)
					{
						lineInfo.CharVisuals.Add(CreateTextCharVisual(num3, textChar, charGeometriesInfo, renderRunProperty.Foreground));
					}
					else
					{
						if (lineInfo.CharVisuals.Count > 0)
						{
							num = num2;
							list.Add(lineInfo);
							lineInfo = new LineInfo();
						}
						lineInfo.CharVisuals.Add(CreateTextCharVisual(0.0, textChar, charGeometriesInfo, renderRunProperty.Foreground));
					}
					continue;
				}
				ExtensionalChar extensionalChar = charAt as ExtensionalChar;
				if (extensionalChar != null)
				{
					num3 += ((lineInfo.CharVisuals.Count > 0) ? GetRenderUsedWidth(lineInfo) : 0.0);
					ExtensionCharVisual extensionCharVisual = CreateExtensionCharVisual(extensionalChar, renderRunProperty);
					extensionCharVisual.Bounds = GetExtensionalCharBounds(0.0, extensionalChar);
					if (num3 + ExtensionalCharWidth(extensionalChar) < num || lineInfo.CharVisuals.Count == 0)
					{
						extensionCharVisual.Bounds = GetExtensionalCharBounds(num3, extensionalChar);
					}
					else
					{
						num = num2;
						list.Add(lineInfo);
						lineInfo = new LineInfo();
					}
					lineInfo.CharVisuals.Add(extensionCharVisual);
				}
			}
			if (lineInfo.CharVisuals.Count > 0)
			{
				list.Add(lineInfo);
			}
			return list;
		}

		protected List<LineInfo> GetNewLineInfo(int startIndex, int charsCount, double renderWidth)
		{
			List<LineInfo> list = new List<LineInfo>();
			LineInfo lineInfo = new LineInfo();
			if (PropertyManager.TextWrapping.Equals(TextWrapping.NoWrap))
			{
				renderWidth = 32000.0;
			}
			double num = GetRenderWidthInner(renderWidth, startIndex);
			double num2 = renderWidth - ParagraphTextBeforeData.NotFirstLineTextIndent;
			for (int i = startIndex; i < startIndex + charsCount; i++)
			{
				double num3 = ((lineInfo.CharVisuals.Count > 0) ? GetRenderUsedWidth(lineInfo) : 0.0);
				PhraseSegment continuousPhraseSegment;
				PhraseSegment independentPhraseSegment;
				if (CharsManager.TryGetContinuousPhraseGroup(i, out continuousPhraseSegment))
				{
					if (num3 + GetContinuousPhraseGroupWidth(continuousPhraseSegment) <= num)
					{
						ValueTuple<int, LineInfo, double> valueTuple = LayoutChars(list, lineInfo, continuousPhraseSegment, num, num2);
						int item = valueTuple.Item1;
						LineInfo item2 = valueTuple.Item2;
						double item3 = valueTuple.Item3;
						i = item;
						lineInfo = item2;
						num = item3;
						continue;
					}
					if (lineInfo.CharVisuals.Count > 0)
					{
						num = num2;
						AddNewLineInfo(list, lineInfo);
						lineInfo = new LineInfo();
						i--;
						continue;
					}
					bool flag = !(GetContinuousPhraseGroupWidth(continuousPhraseSegment) > num) || !PropertyManager.TextWrapping.Equals(TextWrapping.WrapWithOverflow);
					ValueTuple<int, LineInfo, double> valueTuple2 = LayoutChars(list, lineInfo, continuousPhraseSegment, num, num2, flag);
					int item4 = valueTuple2.Item1;
					LineInfo item5 = valueTuple2.Item2;
					double item6 = valueTuple2.Item3;
					i = item4;
					lineInfo = item5;
					if (flag)
					{
						num = item6;
					}
				}
				else if (CharsManager.TryGetIndependentPhraseGroup(i, out independentPhraseSegment))
				{
					ValueTuple<int, LineInfo, double> valueTuple3 = LayoutChars(list, lineInfo, independentPhraseSegment, num, num2);
					int item7 = valueTuple3.Item1;
					LineInfo item8 = valueTuple3.Item2;
					double item9 = valueTuple3.Item3;
					i = item7;
					lineInfo = item8;
					num = item9;
				}
				else
				{
					Log.Error(new InvalidOperationException("invalid char"));
				}
			}
			if (lineInfo.CharVisuals.Count > 0)
			{
				AddNewLineInfo(list, lineInfo);
			}
			return list;
		}

		
		private ValueTuple<int, LineInfo, double> LayoutChars(List<LineInfo> newLineInfos, LineInfo currentLineInfo, PhraseSegment phraseSegment, double renderWidth, double notFirstRenderWidth, bool withWrap = true)
		{
			double num = (withWrap ? renderWidth : 32000.0);
			for (int i = phraseSegment.Index; i < phraseSegment.Index + phraseSegment.Length; i++)
			{
				double num2 = ((currentLineInfo.CharVisuals.Count > 0) ? GetRenderUsedWidth(currentLineInfo) : 0.0);
				RunProperty renderRunProperty = PropertyManager.GetRenderRunProperty(i);
				ICharObject charAt = CharsManager.GetCharAt(i);
				TextChar textChar = charAt as TextChar;
				if (textChar != null)
				{
					if (TextUtilities.IsLigature(textChar.ToText()))
					{
						ValueTuple<double, int, List<ICharVisual>, bool> valueTuple = BuildMongolian(i, num, num2, notFirstRenderWidth);
						double item = valueTuple.Item1;
						int item2 = valueTuple.Item2;
						List<ICharVisual> item3 = valueTuple.Item3;
						bool item4 = valueTuple.Item4;
						num = item;
						i = item2;
						if (item3.Count > 0)
						{
							currentLineInfo.CharVisuals.AddRange(item3);
						}
						if (item4)
						{
							AddNewLineInfo(newLineInfos, currentLineInfo);
							currentLineInfo = new LineInfo();
						}
						if (item2 >= phraseSegment.Index + phraseSegment.Length - 1)
						{
							return new ValueTuple<int, LineInfo, double>(item2, currentLineInfo, num);
						}
						continue;
					}
					CharGeometryInfo charGeometriesInfo = CharsManager.GeometryPool.GetCharGeometriesInfo(i);
					if (newLineInfos.Count > 0 && CharsManager.InterPunctuationIndexes.Contains(i) && !RegexPatterns.LeftSurroundInterpunction.Contains(textChar.TextCharObject[0]) && IsAllWhiteSpace(i, currentLineInfo))
					{
						if (currentLineInfo.CharVisuals.Count == 0 && IsNeedRecall(i, newLineInfos.Last()))
						{
							int lastIndexForInterpunction = GetLastIndexForInterpunction(newLineInfos.Last(), i);
							int num3 = i - lastIndexForInterpunction;
							LineInfo lineInfo = newLineInfos.Last();
							lineInfo.CharVisuals.RemoveRange(lineInfo.CharVisualsCount - num3, num3);
							return new ValueTuple<int, LineInfo, double>(lastIndexForInterpunction - 1, currentLineInfo, num);
						}
						if (newLineInfos.Count > 1 && currentLineInfo.CharVisuals.Count > 0 && IsNeedRecall(i - currentLineInfo.CharVisualsCount, newLineInfos[newLineInfos.Count - 2]))
						{
							LayoutCharTracer?.Debug("符号 " + textChar.TextCharObject + " 前面是空格，拿取上一行最后一个单词填充");
							LineInfo lineInfo2 = newLineInfos[newLineInfos.Count - 2];
							int lastIndexForInterpunction2 = GetLastIndexForInterpunction(lineInfo2, i - currentLineInfo.CharVisualsCount);
							int num4 = i - currentLineInfo.CharVisualsCount - lastIndexForInterpunction2;
							lineInfo2.CharVisuals.RemoveRange(lineInfo2.CharVisualsCount - num4, num4);
							currentLineInfo.CharVisuals.Clear();
							return new ValueTuple<int, LineInfo, double>(lastIndexForInterpunction2 - 1, currentLineInfo, num);
						}
					}
					double num5 = CurrentGeometryWidth(charGeometriesInfo);
					if (num2 + num5 <= num)
					{
						currentLineInfo.CharVisuals.Add(CreateTextCharVisual(num2, textChar, charGeometriesInfo, renderRunProperty.Foreground));
						continue;
					}
					if (currentLineInfo.CharVisuals.Count > 0)
					{
						num = notFirstRenderWidth;
						AddNewLineInfo(newLineInfos, currentLineInfo);
						currentLineInfo = new LineInfo();
					}
					if (newLineInfos.Count > 0 && CharsManager.InterPunctuationIndexes.Contains(i) && !RegexPatterns.LeftSurroundInterpunction.Contains(textChar.TextCharObject[0]) && IsAllWhiteSpace(i, currentLineInfo))
					{
						if (currentLineInfo.CharVisuals.Count == 0 && IsNeedRecall(i, newLineInfos.Last()))
						{
							int lastIndexForInterpunction3 = GetLastIndexForInterpunction(newLineInfos.Last(), i);
							int num6 = i - lastIndexForInterpunction3;
							LineInfo lineInfo3 = newLineInfos.Last();
							lineInfo3.CharVisuals.RemoveRange(lineInfo3.CharVisualsCount - num6, num6);
							return new ValueTuple<int, LineInfo, double>(lastIndexForInterpunction3 - 1, currentLineInfo, num);
						}
						if (newLineInfos.Count > 1 && currentLineInfo.CharVisuals.Count > 0 && IsNeedRecall(i, newLineInfos[newLineInfos.Count - 2]))
						{
							LineInfo lineInfo4 = newLineInfos[newLineInfos.Count - 2];
							int lastIndexForInterpunction4 = GetLastIndexForInterpunction(lineInfo4, i - currentLineInfo.CharVisualsCount);
							int num7 = i - currentLineInfo.CharVisualsCount - lastIndexForInterpunction4;
							lineInfo4.CharVisuals.RemoveRange(lineInfo4.CharVisualsCount - num7, num7);
							currentLineInfo.CharVisuals.Clear();
							return new ValueTuple<int, LineInfo, double>(lastIndexForInterpunction4 - 1, currentLineInfo, num);
						}
					}
					currentLineInfo.CharVisuals.Add(CreateTextCharVisual(0.0, textChar, charGeometriesInfo, renderRunProperty.Foreground));
					continue;
				}
				ExtensionalChar extensionalChar = charAt as ExtensionalChar;
				if (extensionalChar != null)
				{
					num2 = ((currentLineInfo.CharVisuals.Count > 0) ? GetRenderUsedWidth(currentLineInfo) : 0.0);
					ExtensionCharVisual extensionCharVisual = CreateExtensionCharVisual(extensionalChar, renderRunProperty);
					extensionCharVisual.Bounds = GetExtensionalCharBounds(0.0, extensionalChar);
					if (num2 + ExtensionalCharWidth(extensionalChar) < num || currentLineInfo.CharVisuals.Count == 0)
					{
						extensionCharVisual.Bounds = GetExtensionalCharBounds(num2, extensionalChar);
					}
					else
					{
						num = notFirstRenderWidth;
						AddNewLineInfo(newLineInfos, currentLineInfo);
						currentLineInfo = new LineInfo();
					}
					currentLineInfo.CharVisuals.Add(extensionCharVisual);
				}
			}
			if (currentLineInfo.CharVisuals.Count > 0)
			{
				AddNewLineInfo(newLineInfos, currentLineInfo);
			}
			return new ValueTuple<int, LineInfo, double>(phraseSegment.Index + phraseSegment.Length - 1, currentLineInfo, num);
		}

		[Conditional("DEBUG")]
		private void LogDebugMessage(string message)
		{
			LayoutCharTracer?.Debug(message);
		}

		private bool IsAllWhiteSpace(int index, LineInfo lineInfo)
		{
			int charVisualsCount = lineInfo.CharVisualsCount;
			if (charVisualsCount == 0)
			{
				return true;
			}
			int offset = index - charVisualsCount;
			return CharsManager.IsAllWhiteSpace(offset, charVisualsCount);
		}

		private bool IsNeedRecall(int index, LineInfo lineInfo)
		{
			int charVisualsCount = lineInfo.CharVisualsCount;
			if (charVisualsCount == 0)
			{
				return false;
			}
			int offset = index - charVisualsCount;
			string text = CharsManager.GetText(offset, charVisualsCount);
			text = Regex.Replace(text, "([\\ud800-\\udbff][\\udc00-\\udfff])", '\ufffd'.ToString());
			text = text.Substring(1);
			if (string.IsNullOrWhiteSpace(text))
			{
				return false;
			}
			if (IsAllInterpunctionChar(text))
			{
				return false;
			}
			return true;
		}

		private bool IsAllInterpunctionChar(string text)
		{
			return Regex.Match(text.Replace(" ", ","), "[\\p{P}]+").Length == text.Length;
		}

		private int GetLastIndexForInterpunction(LineInfo lastLineInfo, int index, int whiteSpaceCount = 0)
		{
			int num = index - 1;
			if (num < 0)
			{
				return index;
			}
			PhraseSegment independentPhraseSegment;
			if (CharsManager.TryGetIndependentPhraseGroup(num, out independentPhraseSegment))
			{
				TextChar textChar = CharsManager.GetCharAt(num) as TextChar;
				if (textChar != null && string.IsNullOrWhiteSpace(textChar.ToText()))
				{
					return GetLastIndexForInterpunction(lastLineInfo, num, whiteSpaceCount + 1);
				}
				return num;
			}
			PhraseSegment continuousPhraseSegment;
			if (CharsManager.TryGetOriginalContinuousPhraseGroup(num, out continuousPhraseSegment))
			{
				if (lastLineInfo.CharVisualsCount - whiteSpaceCount <= continuousPhraseSegment.Length)
				{
					return num;
				}
				return continuousPhraseSegment.Index;
			}
			throw new InvalidOperationException("a char must be included by IndependentPhraseGroups or ContinuousPhraseGroups");
		}

		private void AddNewLineInfo(List<LineInfo> newLineInfos, LineInfo newLineInfo)
		{
			if (!newLineInfos.Contains(newLineInfo))
			{
				newLineInfos.Add(newLineInfo);
			}
		}

		private void CorrectCharBounds(List<LineInfo> newLineInfos)
		{
			for (int i = 0; i < newLineInfos.Count; i++)
			{
				LineInfo lineInfo = newLineInfos[i];
				if (ParagraphNode.Length > 0)
				{
					AdaptBaseLine(lineInfo);
					AdaptLogicalCenter(lineInfo);
					AdaptScript(lineInfo);
				}
				if (i == 0 && IsFirstLineShrink)
				{
					AdaptLineSpace(lineInfo, false);
				}
				else
				{
					AdaptLineSpace(lineInfo, true);
				}
				AdaptDirection(lineInfo);
				AdaptHorizontalAlignment(lineInfo);
			}
		}

		private void GenerateUnderlines(List<LineInfo> newLineInfos)
		{
			if (ParagraphNode.Length > 0)
			{
				TextTracer.Debug("开始计算下划线布局", "文本渲染");
				newLineInfos.ForEach(GenerateUnderline);
			}
		}

		internal void BuildRenderData(double width)
		{
			BuildRenderData(new SimpleSegment(0, ParagraphNode.Length), width);
		}

		internal double GetRenderParagraphBefore()
		{
			if (ParagraphNode.ParagraphNumber != 1)
			{
				return TargetParagraphProperty.ParagraphBefore;
			}
			return 0.0;
		}

		internal double GetRenderParagraphAfter()
		{
			if (ParagraphNode.ParagraphNumber != CharsManager.ParagraphTree.Count())
			{
				return TargetParagraphProperty.ParagraphAfter;
			}
			return 0.0;
		}

		protected abstract double GetRenderUsedWidth(LineInfo newLineInfo);

		protected abstract double CurrentGeometryWidth(CharGeometryInfo charGeometryInfo);

		protected abstract double GetContinuousPhraseGroupWidth(PhraseSegment phraseSegment);

		protected abstract double ExtensionalCharWidth(ExtensionalChar extensionalChar);

		protected abstract Rect GetCharVisualBounds(double position, Size size);

		protected abstract Rect GetMongolianCharBounds(double position, Size size);

		protected abstract Rect GetExtensionalCharBounds(double position, ExtensionalChar extensionalChar);

		protected abstract void TransformTextChar(double offset, TextCharVisual textCharVisual);

		protected abstract ValueTuple<double, int, List<ICharVisual>, bool> BuildMongolian(int startOffset, double renderWidthInner, double offset, double notFirstRenderInner);

		protected abstract void FillLineInfosDefaultBounds(List<LineInfo> lines);

		protected abstract void AdaptBaseLine(LineInfo newLineInfo);

		protected abstract void AdaptLogicalCenter(LineInfo lineInfo);

		protected abstract void AdaptScript(LineInfo newLineInfo);

		protected abstract void AdaptLineSpace(LineInfo newLineInfo, bool firstLineExpand);

		protected abstract void AdaptHorizontalAlignment(LineInfo newLineInfo);

		protected abstract void GenerateUnderline(LineInfo newLineInfo);

		protected abstract List<LineRenderData> ConvertLineInfoToLineRenderData(List<LineInfo> newLineInfos, double renderWidth);

		protected abstract void GenerateTextEffects(List<LineInfo> newLineInfos, List<LineRenderData> lineRenderData);

		protected abstract ValueTuple<Geometry, TextCharGlyphData> BuildMongolianTextGeometry(RunProperty runProperty, string text);

		[CanBeNull]
		protected LineInfo GetLineInfoFromParagraphOffset(int paragraphOffset)
		{
			if (Lines.Count == 0)
			{
				return null;
			}
			int num = 0;
			foreach (LineInfo line in Lines)
			{
				num += line.CharVisualsCount;
				if (paragraphOffset <= num || (line.CharVisuals.Count == 0 && paragraphOffset == 0))
				{
					return line;
				}
			}
			return Lines.Last();
		}

		internal LineRenderData GetLineRenderDataByCaretOffset(CaretOffset caretOffset)
		{
			int num = caretOffset.Offset + 1 - ParagraphNode.StartOffset;
			if (num <= 0)
			{
				return LineRenderData.First();
			}
			int num2 = 0;
			for (int i = 0; i < LineRenderData.Count; i++)
			{
				num2 += LineRenderData[i].CharVisualsCount;
				if (num <= num2)
				{
					int num3 = ((caretOffset.IsAtLineStart && !caretOffset.IsAtParagraphStart) ? (i + 1) : i);
					if (num3 == LineRenderData.Count)
					{
						return LineRenderData.Last();
					}
					return LineRenderData[num3];
				}
			}
			return LineRenderData.Last();
		}

		internal abstract CaretOffset GetCaretOffsetByParagraphPosition(Point paragraphPosition);

		internal abstract DocumentOffset GetDocumentOffsetByPositionFromRunBounds(Point position);

		internal abstract Rect GetRunBoundsByCaretOffset(CaretOffset caretOffset);

		internal virtual double GetVisualXPositionByCaretOffset(CaretOffset caretOffset)
		{
			return GetRunBoundsByCaretOffset(caretOffset).Right;
		}

		internal virtual double GetVisualYPositionByCaretOffset(CaretOffset caretOffset)
		{
			return GetRunBoundsByCaretOffset(caretOffset).Bottom;
		}

		internal virtual double GetLineParagraphYPosition(LineRenderData line, LineVisualYPosition yPosition = LineVisualYPosition.LineMiddle)
		{
			double num = 0.0;
			foreach (LineRenderData lineRenderDatum in LineRenderData)
			{
				if (line.Equals(lineRenderDatum))
				{
					switch (yPosition)
					{
					case LineVisualYPosition.LineTop:
						return num;
					case LineVisualYPosition.LineBottom:
						return num + lineRenderDatum.Height;
					default:
						return num + lineRenderDatum.Height / 2.0;
					}
				}
				num += line.Height;
			}
			throw new ArgumentException("line is not a valid LineRenderData in this paragraph");
		}

		internal virtual int GetLineIndex(LineRenderData line)
		{
			return LineRenderData.IndexOf(line);
		}

		internal virtual int GetParagraphOffset(LineRenderData line)
		{
			int lineIndex = GetLineIndex(line);
			return Lines[lineIndex].LineProperty.ParagraphOffset;
		}

		internal LineProperty GetLinePropertyByDocumentOffset(int documentOffset)
		{
			int lineIndexByDocumentOffset = GetLineIndexByDocumentOffset(documentOffset);
			return ParagraphLineInfoRenderedData[lineIndexByDocumentOffset];
		}

		internal int GetLineIndexByDocumentOffset(DocumentOffset documentOffset)
		{
			int num = (int)documentOffset - ParagraphNode.StartOffset;
			for (int i = 0; i < ParagraphLineInfoRenderedData.Count; i++)
			{
				LineProperty lineProperty = ParagraphLineInfoRenderedData[i];
				if (lineProperty.ParagraphOffset + lineProperty.Length > num)
				{
					return i;
				}
			}
			if (num >= ParagraphNode.Length && num <= ParagraphNode.TotalLength)
			{
				return ParagraphLineInfoRenderedData.Count - 1;
			}
			throw new ArgumentOutOfRangeException($"传入的文档偏移量{documentOffset.Offset}不在当前段落范围内")
			{
				Data = 
				{
					{
						(object)"paragraphOffset",
						(object)num.ToString()
					},
					{
						(object)"ParagraphNode.TotalLength",
						(object)ParagraphNode.TotalLength.ToString()
					}
				}
			};
		}

		internal LineRenderData GetLineRenderDataByDocumentOffset(DocumentOffset documentOffset)
		{
			return LineRenderData[GetLineIndexByDocumentOffset(documentOffset)];
		}

		private void UpdateLineProperty()
		{
			int num = 0;
			ParagraphLineInfoRenderedData.Clear();
			foreach (LineInfo line in Lines)
			{
				line.LineProperty.ParagraphOffset = num;
				int runPropertiesCount = PropertyManager.RunPropertiesCount;
				line.LineProperty.Length = line.CharVisualsCount;
				num += line.LineProperty.Length;
				ParagraphLineInfoRenderedData.Add(line.LineProperty);
			}
		}

		protected internal int GetMaxCharIndexWithoutScript(LineInfo lineInfo)
		{
			int num = ParagraphNode.StartOffset + lineInfo.LineProperty.ParagraphOffset;
			RunProperty runProperty = PropertyManager.GetRunProperty(num);
			int result = 0;
			for (int i = 0; i < lineInfo.CharVisualsCount; i++)
			{
				RunProperty runProperty2 = PropertyManager.GetRunProperty(i + num);
				if (runProperty2.FontVariants == FontVariants.Normal && (runProperty2.FontSize > runProperty.FontSize || (Math.Abs(runProperty2.FontSize - runProperty.FontSize) < 1E-05 && runProperty.FontVariants != FontVariants.Normal)))
				{
					runProperty = runProperty2;
					result = i;
				}
			}
			return result;
		}

		protected List<List<int>> GetSamePropIndexes(LineInfo lineInfo)
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			int num = ParagraphNode.StartOffset + lineInfo.LineProperty.ParagraphOffset;
			for (int i = 0; i < lineInfo.CharVisualsCount; i++)
			{
				if (lineInfo.CharVisuals[i] is TextCharVisual)
				{
					int key = PropertyManager.RunProperties[i + num];
					if (dictionary.ContainsKey(key))
					{
						dictionary[key].Add(i);
						continue;
					}
					dictionary[key] = new List<int> { i };
				}
			}
			return dictionary.Values.ToList();
		}

		protected internal int GetCurrentMaxFontSizeRunIndex(List<int> indexes, int startOffset)
		{
			RunProperty runProperty = PropertyManager.GetRunProperty(indexes.First() + startOffset);
			int result = indexes.First();
			foreach (int index in indexes)
			{
				RunProperty runProperty2 = PropertyManager.GetRunProperty(index + startOffset);
				if (runProperty2.FontSize > runProperty.FontSize)
				{
					result = index;
					runProperty = runProperty2;
				}
			}
			return result;
		}

		protected int GetLastTextCharIndex(LineInfo lineInfo, int currentIndex)
		{
			for (int num = currentIndex - 1; num >= 0; num--)
			{
				if (!(lineInfo.CharVisuals[num] is ExtensionCharVisual))
				{
					return num;
				}
			}
			return -1;
		}

		protected internal List<List<int>> GetSeriesSameFontVariants(List<ICharVisual> charVisuals, int startOffset)
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			RunProperty runProperty = null;
			for (int i = 0; i < charVisuals.Count; i++)
			{
				RunProperty runProperty2 = PropertyManager.GetRunProperty(i + startOffset);
				if (charVisuals[i] is TextCharVisual)
				{
					if (runProperty2.FontVariants != FontVariants.Normal)
					{
						if (runProperty == null || runProperty.FontVariants != runProperty2.FontVariants)
						{
							dictionary.Add(dictionary.Count + 1, new List<int>());
						}
						dictionary[dictionary.Count].Add(i);
					}
					runProperty = runProperty2;
				}
				else
				{
					runProperty = null;
				}
			}
			return dictionary.Values.ToList();
		}

		protected Rect GetNormalRunBounds(int index, LineInfo newLineInfo, int startOffset)
		{
			Rect result = newLineInfo.DefaultRunBounds;
			for (int num = index - 1; num >= 0; num--)
			{
				if (PropertyManager.GetRunProperty(num + startOffset).FontVariants == FontVariants.Normal && !(newLineInfo.CharVisuals[num] is ExtensionCharVisual))
				{
					result = newLineInfo.CharVisuals[num].Bounds;
					break;
				}
			}
			return result;
		}

		protected List<List<int>> GetSeriesSameScriptPropIndexes(int startOffset, List<int> indexes)
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			int num = -1;
			foreach (int index in indexes)
			{
				int num2 = PropertyManager.RunProperties[index + startOffset];
				if (dictionary.ContainsKey(dictionary.Count) && index - dictionary[dictionary.Count].Last() == 1 && num == num2)
				{
					dictionary[dictionary.Count].Add(index);
				}
				else
				{
					dictionary[dictionary.Count + 1] = new List<int> { index };
				}
				num = num2;
			}
			return dictionary.Values.ToList();
		}

		protected void TransformCharAndBounds(LineInfo lineInfo, double xOffset, double yOffset)
		{
			foreach (ICharVisual charVisual in lineInfo.CharVisuals)
			{
				TextCharVisual textCharVisual = charVisual as TextCharVisual;
				if (textCharVisual != null)
				{
					textCharVisual.Bounds = new Rect(new Point(textCharVisual.Bounds.Location.X + xOffset, textCharVisual.Bounds.Location.Y + yOffset), textCharVisual.Bounds.Size);
					textCharVisual.TransformTextChar(xOffset, yOffset);
					continue;
				}
				ExtensionCharVisual extensionCharVisual = charVisual as ExtensionCharVisual;
				if (extensionCharVisual != null)
				{
					extensionCharVisual.Bounds = new Rect(new Point(extensionCharVisual.Bounds.X + xOffset, extensionCharVisual.Bounds.Y + yOffset), extensionCharVisual.Bounds.Size);
				}
			}
			lineInfo.DefaultRunBounds = new Rect(new Point(lineInfo.DefaultRunBounds.X + xOffset, lineInfo.DefaultRunBounds.Location.Y + yOffset), lineInfo.DefaultRunBounds.Size);
		}

		protected IReadOnlyList<ISegment> GetUnderLineAndArtSamePropIndexes(LineInfo newLineInfo, int startOffset)
		{
			return UnderLineHelper.GetUnderLineAndArtSamePropertySegmentList(newLineInfo, startOffset, PropertyManager);
		}

		protected List<List<int>> GetSeriesSamePropIndexes(LineInfo lineInfo)
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			int num = -1;
			int num2 = ParagraphNode.StartOffset + lineInfo.LineProperty.ParagraphOffset;
			for (int i = 0; i < lineInfo.CharVisualsCount; i++)
			{
				if (lineInfo.CharVisuals[i] is TextCharVisual)
				{
					int num3 = PropertyManager.RunProperties[i + num2];
					if (dictionary.ContainsKey(dictionary.Count) && i - dictionary[dictionary.Count].Last() == 1 && num == num3)
					{
						dictionary[dictionary.Count].Add(i);
					}
					else
					{
						dictionary[dictionary.Count + 1] = new List<int> { i };
					}
					num = num3;
				}
			}
			return dictionary.Values.ToList();
		}

		protected Rect GetNormalBounds(LineInfo lineInfo, int index)
		{
			if (index == -1)
			{
				return lineInfo.DefaultRunBounds;
			}
			int num = lineInfo.LineProperty.ParagraphOffset + ParagraphNode.StartOffset + index;
			RunProperty renderRunProperty = PropertyManager.GetRenderRunProperty(num);
			if (renderRunProperty.FontVariants == FontVariants.Normal)
			{
				return lineInfo.CharVisuals[index].Bounds;
			}
			return GetNormalBoundsInner(lineInfo, renderRunProperty, num);
		}

		protected virtual Rect GetNormalBoundsInner(LineInfo lineInfo, RunProperty runProp, int index)
		{
			throw new NotSupportedException();
		}

		protected virtual void AdaptDirection(LineInfo newLineInfo)
		{
			List<Block> blocks = FillBlocks(newLineInfo);
			ArrangeBlocks(blocks);
		}

		protected abstract void ArrangeBlocks(List<Block> blocks);

		private List<Block> FillBlocks(LineInfo newLineInfo)
		{
			List<Block> list = new List<Block>();
			bool flag = TargetParagraphProperty.Direction == FlowDirection.RightToLeft;
			FlowDirection flowDirection = (flag ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
			FlowDirection flowDirection2 = ((!flag) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
			CharFlowDirection charFlowDirection = ((!flag) ? CharFlowDirection.LeftToRight : CharFlowDirection.RightToLeft);
			List<ICharVisual> list2 = new List<ICharVisual>();
			List<ICharVisual> list3 = new List<ICharVisual>();
			FlowDirection flowDirection3 = flowDirection;
			for (int i = 0; i < newLineInfo.CharVisuals.Count; i++)
			{
				ICharVisual charVisual = newLineInfo.CharVisuals[i];
				if (charVisual.Direction == charFlowDirection)
				{
					if (list3.Count > 0)
					{
						list2.AddRange(list3);
						list3.Clear();
					}
					if (flowDirection3 == flowDirection2 && list2.Count > 0)
					{
						Block item = BuildBlock(list2, flowDirection2);
						list.Add(item);
						list2.Clear();
					}
					list2.Add(charVisual);
					flowDirection3 = flowDirection;
				}
				else if (charVisual.Direction == CharFlowDirection.Neutral)
				{
					list3.Add(charVisual);
				}
				else
				{
					if (flowDirection3 == flowDirection && list2.Count > 0)
					{
						Block item2 = BuildBlock(list2, flowDirection);
						list.Add(item2);
						list2.Clear();
					}
					if (list3.Count > 0)
					{
						list2.AddRange(list3);
						list3.Clear();
					}
					list2.Add(charVisual);
					flowDirection3 = flowDirection2;
				}
			}
			if (list2.Count > 0)
			{
				Block item3 = BuildBlock(list2, flowDirection3);
				list.Add(item3);
			}
			if (list3.Count > 0)
			{
				Block block = list.LastOrDefault();
				if (block != null)
				{
					List<ICharVisual> list4 = block.CharVisuals.ToList();
					list4.AddRange(list3);
					list.Remove(block);
					list.Add(BuildBlock(list4, flowDirection3));
				}
			}
			return list;
		}

		protected abstract Block BuildBlock(List<ICharVisual> charVisuals, FlowDirection direction);

		internal virtual FlowDirection GetRunDirectionByCaretOffset(CaretOffset caretOffset)
		{
			int num = caretOffset.Offset - ParagraphNode.StartOffset;
			LineRenderData lineRenderDataByCaretOffset = GetLineRenderDataByCaretOffset(caretOffset);
			int lineIndex = GetLineIndex(lineRenderDataByCaretOffset);
			if (caretOffset.IsAtLineStart)
			{
				if (lineRenderDataByCaretOffset.CharVisuals.Count > 0)
				{
					return lineRenderDataByCaretOffset.CharVisuals[0].ActualDirection;
				}
				return FlowDirection.LeftToRight;
			}
			int index = num - Lines[lineIndex].LineProperty.ParagraphOffset;
			return lineRenderDataByCaretOffset.CharVisuals[index].ActualDirection;
		}

		protected double GetRenderLineSpace(RunProperty maxFontSizeRunProperty)
		{
			if (TargetParagraphProperty.FixedLineSpacing > 0.0)
			{
				return TargetParagraphProperty.FixedLineSpacing - maxFontSizeRunProperty.FontSize * maxFontSizeRunProperty.FontFamily.LineSpacing;
			}
			double lineSpacing = TargetParagraphProperty.LineSpacing;
			return LineSpaceHelper.CalcRenderLineSpace(maxFontSizeRunProperty, lineSpacing);
		}
	}
}
