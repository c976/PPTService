using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class FlowDocumentConverter
	{
		public static RichTextSaveInfo ToRichTextInfo(this FlowDocument flowDocument)
		{
			RichTextSaveInfo richTextSaveInfo = new RichTextSaveInfo();
			try
			{
				if (XamlWriter.Save(flowDocument).Contains("Cambria Math"))
				{
					return richTextSaveInfo;
				}
				richTextSaveInfo.Paragraphs = flowDocument.GetParagraphInfo();
				if (richTextSaveInfo.Paragraphs.Count == 0)
				{
					return richTextSaveInfo;
				}
				string text = richTextSaveInfo.Paragraphs.Last().TextRuns.Last().Text;
				if (text.EndsWith(Environment.NewLine))
				{
					richTextSaveInfo.Paragraphs.Last().TextRuns.Last().Text = text.Replace(Environment.NewLine, string.Empty);
				}
			}
			catch (Exception exception)
			{
				TextTracer.Error(exception, "将流文档转RichTextInfo");
			}
			return richTextSaveInfo;
		}

		public static FlowDocument ToFlowDocument(this RichTextSaveInfo saveInfo)
		{
			FlowDocument flowDocument = new FlowDocument();
			List list = null;
			foreach (ParagraphSaveInfo paragraph in saveInfo.Paragraphs)
			{
				if (paragraph.TextMarkerSaveInfo.IsEmpty())
				{
					Paragraph item = ConvertTextLineToParagrah(paragraph);
					flowDocument.Blocks.Add(item);
					if (list != null)
					{
						flowDocument.Blocks.Add(list);
						list = null;
					}
				}
				else
				{
					if (list == null)
					{
						list = CreateListFromTextLine(paragraph);
					}
					list.ListItems.Add(new ListItem(ConvertTextLineToParagrah(paragraph)));
				}
			}
			if (list != null)
			{
				flowDocument.Blocks.Add(list);
			}
			return flowDocument;
		}

		public static FlowDocument ToFlowDocument(this string rtfStr)
		{
			FlowDocument flowDocument = new FlowDocument();
			try
			{
				TextRange textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (StreamWriter streamWriter = new StreamWriter(memoryStream))
					{
						streamWriter.Write(rtfStr);
						streamWriter.Flush();
						memoryStream.Seek(0L, SeekOrigin.Begin);
						textRange.Load(memoryStream, DataFormats.Rtf);
					}
				}
			}
			catch (Exception exception)
			{
				TextTracer.Error(exception, "从字符串转FlowDocument");
			}
			return flowDocument;
		}

		public static string ToRtf(this FlowDocument document)
		{
			string input = string.Empty;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					new TextRange(document.ContentStart, document.ContentEnd).Save(memoryStream, DataFormats.Rtf);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					using (StreamReader streamReader = new StreamReader(memoryStream))
					{
						input = streamReader.ReadToEnd();
					}
				}
			}
			catch (Exception exception)
			{
				TextTracer.Error(exception, "从FlowDocument转字符串");
			}
			return RectifyRtfString(input);
		}

		private static List CreateListFromTextLine(ParagraphSaveInfo paragraphSaveInfo)
		{
			return new List
			{
				Margin = new Thickness((double)paragraphSaveInfo.IndentLevel * 24.0, 0.0, 0.0, 0.0)
			};
		}

		private static Paragraph ConvertTextLineToParagrah(ParagraphSaveInfo paragraphSaveInfo)
		{
			Paragraph paragraph = new Paragraph();
			paragraph.TextAlignment = paragraphSaveInfo.TextAlignment;
			paragraph.TextIndent = (double)paragraphSaveInfo.IndentLevel * 24.0;
			foreach (RunSaveInfo textRun in paragraphSaveInfo.TextRuns)
			{
				Run run = new Run();
				run.Text = textRun.Text;
				run.FontSize = textRun.FontSize;
				run.FontFamily = textRun.FontFamily;
				run.FontStyle = textRun.FontStyle;
				run.FontWeight = textRun.FontWeight;
				run.Background = textRun.Background;
				run.Foreground = textRun.Foreground;
				run.TextDecorations = textRun.Decoration;
				paragraph.Inlines.Add(run);
			}
			return paragraph;
		}

		private static string RectifyRtfString(string input)
		{
			List<string> list = new List<string>();
			MatchCollection matchCollection = new Regex("\\\\red\\d{1,3}\\\\green\\d{1,3}\\\\blue\\d{1,3}", RegexOptions.IgnoreCase).Matches(input);
			if (matchCollection.Count > 0)
			{
				list.AddRange(from Match match in matchCollection
					select match.Value);
			}
			int num = list.IndexOf("\\red255\\green255\\blue255");
			if (num == -1)
			{
				return input;
			}
			return input.Replace("\\highlight" + num, "");
		}

		private static List<ParagraphSaveInfo> GetParagraphInfo(this FlowDocument flowDocument)
		{
			List<ParagraphSaveInfo> list = new List<ParagraphSaveInfo>();
			int num = 1;
			List<Block> list2 = flowDocument.Blocks.Where((Block x) => x is Paragraph || x is List).ToList();
			int count = list2.Count;
			if (count == 0)
			{
				return list;
			}
			foreach (Block item in list2)
			{
				List<ParagraphSaveInfo> paragraphsFromBlock = GetParagraphsFromBlock(item, TextMarkerStyle.None, num == count);
				if (flowDocument.FlowDirection == FlowDirection.RightToLeft)
				{
					paragraphsFromBlock.ForEach((ParagraphSaveInfo info) =>
					{
						info.Direction = FlowDirection.RightToLeft;
					});
				}
				list.AddRange(paragraphsFromBlock);
				num++;
			}
			return list;
		}

		private static List<ParagraphSaveInfo> GetParagraphsFromBlock(Block block, TextMarkerStyle markerStyle = TextMarkerStyle.None, bool isLastLine = false)
		{
			List<ParagraphSaveInfo> list = new List<ParagraphSaveInfo>();
			Paragraph paragraph = block as Paragraph;
			if (paragraph != null)
			{
				ParagraphSaveInfo paragraphSaveInfo = new ParagraphSaveInfo();
				if (IsParagraphEmpty(paragraph))
				{
					paragraphSaveInfo = SetBlankLineFormat(paragraph);
				}
				else
				{
					paragraphSaveInfo.TextRuns = GetTextRuns(paragraph, isLastLine);
					RunSaveInfo runSaveInfo = paragraphSaveInfo.TextRuns.First();
					paragraphSaveInfo.DefaultRunProperty = new RunSaveInfo
					{
						Background = runSaveInfo.Background,
						Decoration = runSaveInfo.Decoration,
						FontFamily = runSaveInfo.FontFamily,
						FontSize = runSaveInfo.FontSize,
						FontStyle = runSaveInfo.FontStyle,
						FontWeight = runSaveInfo.FontWeight,
						Foreground = runSaveInfo.Foreground,
						FontVariants = runSaveInfo.FontVariants
					};
					paragraphSaveInfo.TextAlignment = paragraph.TextAlignment;
					if (paragraphSaveInfo.TextAlignment.Equals(TextAlignment.Justify))
					{
						paragraphSaveInfo.TextAlignment = TextAlignment.Left;
					}
					if (markerStyle != TextMarkerStyle.None)
					{
						RunProperty runProperty = paragraphSaveInfo.DefaultRunProperty.ToRunProperty();
						MarkerProperty markerProperty = MarkerFactory.GetMarkerProperty(runProperty, MarkerStyle.Circle);
						paragraphSaveInfo.TextMarkerSaveInfo = markerProperty.ToMarkerSaveInfo(runProperty);
					}
					paragraphSaveInfo.LineSpacing = 1.0;
				}
				list.Add(paragraphSaveInfo);
			}
			List list2 = block as List;
			if (list2 != null)
			{
				foreach (ListItem listItem in list2.ListItems)
				{
					List<ParagraphSaveInfo> list3 = new List<ParagraphSaveInfo>();
					foreach (Block block2 in listItem.Blocks)
					{
						TextMarkerStyle markerStyle2 = listItem.List?.MarkerStyle ?? TextMarkerStyle.None;
						list3.AddRange(GetParagraphsFromBlock(block2, markerStyle2));
					}
					list.AddRange(list3);
				}
			}
			return list;
		}

		private static ParagraphSaveInfo SetBlankLineFormat(Paragraph paragraph)
		{
			ParagraphSaveInfo paragraphSaveInfo = new ParagraphSaveInfo();
			RunSaveInfo item = new RunSaveInfo
			{
				Text = Environment.NewLine
			};
			paragraphSaveInfo.DefaultRunProperty = new RunSaveInfo
			{
				Background = (paragraph.Background ?? TextContext.DefaultBackground),
				Decoration = paragraph.TextDecorations,
				FontFamily = paragraph.FontFamily,
				FontSize = paragraph.FontSize,
				FontStyle = paragraph.FontStyle,
				FontWeight = paragraph.FontWeight,
				Foreground = (paragraph.Foreground ?? TextContext.DefaultForeground),
				Text = Environment.NewLine
			};
			paragraphSaveInfo.TextRuns = new List<RunSaveInfo> { item };
			return paragraphSaveInfo;
		}

		private static List<RunSaveInfo> GetTextRuns(Paragraph paragraph, bool isLastLine = false)
		{
			List<RunSaveInfo> list = new List<RunSaveInfo>();
			int num = 0;
			List<Inline> list2 = paragraph.Inlines.Where((Inline x) => (x is Span || x is Run) && !(x is Hyperlink)).ToList();
			foreach (Inline item in list2)
			{
				num++;
				RunSaveInfo runSaveInfo = new RunSaveInfo
				{
					FontSize = item.FontSize,
					FontFamily = item.FontFamily,
					FontVariants = item.Typography.Variants
				};
				Span span = item as Span;
				if (span != null)
				{
					Run run = span.Inlines.FirstInline as Run;
					if (run != null)
					{
						if (num == list2.Count && !isLastLine && !run.Text.EndsWith(Environment.NewLine))
						{
							runSaveInfo.Text = run.Text + Environment.NewLine;
						}
						else
						{
							runSaveInfo.Text = run.Text;
						}
					}
				}
				else
				{
					Run run2 = item as Run;
					if (run2 != null)
					{
						if (num == list2.Count && !isLastLine)
						{
							runSaveInfo.Text = run2.Text + Environment.NewLine;
						}
						else
						{
							runSaveInfo.Text = run2.Text;
						}
					}
				}
				if (!string.IsNullOrEmpty(runSaveInfo.Text))
				{
					runSaveInfo.FontStyle = item.FontStyle;
					runSaveInfo.FontWeight = item.FontWeight;
					if (item.TextDecorations != null && item.TextDecorations.Count > 0 && item.TextDecorations[0].Location == TextDecorationLocation.Underline)
					{
						runSaveInfo.Decoration = TextDecorations.Underline;
					}
					runSaveInfo.Foreground = item.Foreground ?? Brushes.Transparent;
					list.Add(runSaveInfo);
				}
			}
			return list;
		}

		private static bool IsParagraphEmpty(Paragraph p)
		{
			if (p.Inlines.Count == 0)
			{
				return true;
			}
			foreach (Inline item in p.Inlines.Where((Inline x) => x is Span || x is Run).ToList())
			{
				if (!string.IsNullOrEmpty(((item as Span)?.Inlines.FirstInline as Run)?.Text))
				{
					return false;
				}
				if (!string.IsNullOrEmpty((item as Run)?.Text))
				{
					return false;
				}
			}
			return true;
		}
	}
}
