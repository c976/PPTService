using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class CharsManager
	{
		internal ParagraphTree ParagraphTree { get; }

		internal List<ICharObject> Chars { get; }

		internal int CharCount => Chars.Count;

		internal GeometryPool GeometryPool { get; set; } = new GeometryPool();

		internal List<PhraseSegment> ContinuousPhraseGroups { get; } = new List<PhraseSegment>();

		internal List<PhraseSegment> IndependentPhraseGroups { get; } = new List<PhraseSegment>();

		internal List<int> InterPunctuationIndexes { get; } = new List<int>();

		internal string TextWrapContinuousPhraseRegexPattern { get; set; } = RegexPatterns.ContinuousPhrase;

		internal event Action<ParagraphNode> ParagraphInserted;

		internal event Action<ParagraphNode> ParagraphRemoving;

		internal event Action<ParagraphNode> ParagraphRemoved;

		internal event Action<List<ParagraphNode>> DocumentTreeBuilt;

		internal event Action<CharChangeEventArgs> CharsChanging;

		internal event Action<CharChangeEventArgs> CharsChanged;

		internal CharsManager()
		{
			Chars = new List<ICharObject>();
			ParagraphTree = new ParagraphTree();
			CharsChanged += OnCharsChanged;
			Rebuild();
		}

		internal bool TryGetOriginalContinuousPhraseGroup(int index, out PhraseSegment continuousPhraseSegment)
		{
			continuousPhraseSegment = null;
			if (ContinuousPhraseGroups.Any((PhraseSegment x) => x.Contains(index)))
			{
				continuousPhraseSegment = ContinuousPhraseGroups.First((PhraseSegment x) => x.Contains(index));
				return true;
			}
			return false;
		}

		internal bool TryGetContinuousPhraseGroup(int index, out PhraseSegment continuousPhraseSegment)
		{
			continuousPhraseSegment = null;
			PhraseSegment phraseSegment = ContinuousPhraseGroups.FirstOrDefault((PhraseSegment x) => x.Contains(index));
			if (phraseSegment == null)
			{
				return false;
			}
			continuousPhraseSegment = new PhraseSegment
			{
				Index = index,
				Length = phraseSegment.Index + phraseSegment.Length - index
			};
			return true;
		}

		internal bool TryGetIndependentPhraseGroup(int index, out PhraseSegment independentPhraseSegment)
		{
			independentPhraseSegment = null;
			PhraseSegment phraseSegment = IndependentPhraseGroups.FirstOrDefault((PhraseSegment x) => x.Contains(index));
			if (phraseSegment == null)
			{
				return false;
			}
			independentPhraseSegment = new PhraseSegment
			{
				Index = index,
				Length = phraseSegment.Index + phraseSegment.Length - index
			};
			return true;
		}

		private void OnCharsChanged(CharChangeEventArgs obj)
		{
			ContinuousPhraseGroups.Clear();
			IndependentPhraseGroups.Clear();
			InterPunctuationIndexes.Clear();
			if (CharCount == 0)
			{
				return;
			}
			string text = GetText(0, CharCount);
			text = Regex.Replace(text, "([\\ud800-\\udbff][\\udc00-\\udfff])", '\ufffd'.ToString());
			text = TextHelperInner.PreHandleNewLine(text);
			text = text.Replace("\r\n", $"{'\ufffe'}{'\ufffe'}");
			foreach (Match item3 in Regex.Matches(text, TextWrapContinuousPhraseRegexPattern))
			{
				PhraseSegment item = new PhraseSegment
				{
					Index = item3.Index,
					Length = item3.Length,
					Text = item3.Value
				};
				ContinuousPhraseGroups.Add(item);
			}
			foreach (Match item4 in Regex.Matches(text, RegexPatterns.IndependentPhrase))
			{
				PhraseSegment item2 = new PhraseSegment
				{
					Index = item4.Index,
					Length = item4.Length,
					Text = item4.Value
				};
				IndependentPhraseGroups.Add(item2);
			}
			foreach (Match item5 in Regex.Matches(text, "[\\p{P}]+"))
			{
				for (int i = item5.Index; i < item5.Index + item5.Length; i++)
				{
					InterPunctuationIndexes.Add(i);
				}
			}
		}

		internal List<ICharObject> GetChars(int offset, int length)
		{
			try
			{
				return Chars.GetRange(offset, length);
			}
			catch (ArgumentException innerException)
			{
				throw new ArgumentException(string.Format("获取字符的偏移量和长度超出数组的界限，字符数组 Chars Count={0} Offset={1} Length={2} 当前字符数组内容为{3}", Chars.Count, offset, length, string.Join(" ", Chars.Select((ICharObject temp) => "[" + temp.ToText() + "]"))), innerException)
				{
					Data = 
					{
						{
							(object)"Chars",
							(object)Chars
						},
						{
							(object)"Offset",
							(object)offset
						},
						{
							(object)"Length",
							(object)length
						}
					}
				};
			}
		}

		internal ICharObject GetCharAt(int offset)
		{
			return Chars[offset];
		}

		internal void Replace(ISegment segment, IReadOnlyList<ICharObject> charObjects)
		{
			Replace(segment.StartOffset, segment.Length, charObjects);
		}

		private void Replace(int offset, int length, IReadOnlyList<ICharObject> chars)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (length == 0 && chars.Count == 0)
			{
				return;
			}
			CharChangeEventArgs obj = new CharChangeEventArgs(offset, GetChars(offset, length), chars);
			CharsChanging?.Invoke(obj);
			if (offset == 0 && length == Chars.Count)
			{
				RemoveInner(offset, length);
				Chars.Clear();
				Chars.AddRange(chars);
				Rebuild();
			}
			else
			{
				Chars.RemoveRange(offset, length);
				RemoveInner(offset, length);
				if (chars.Any())
				{
					Chars.InsertRange(offset, chars);
					InsertInner(offset, chars);
				}
			}
			CharsChanged?.Invoke(obj);
		}

		internal CaretOffset GetNewCaretOffset(CharChangeEventArgs e)
		{
			CaretOffset result = default(CaretOffset);
			if (e.RemoveLength != 0 && e.InsertLength == 0)
			{
				result.Offset = e.Offset;
			}
			else if (e.RemoveLength == 0 && e.InsertLength != 0)
			{
				result.Offset = e.Offset + e.InsertLength;
			}
			else
			{
				result.Offset = e.Offset + e.InsertLength;
			}
			result.Offset--;
			foreach (ParagraphNode item in ParagraphTree)
			{
				if (!item.IsDeleted && result.Offset == item.EndOffset + 1)
				{
					result.IsAtParagraphStart = true;
				}
			}
			return result;
		}

		internal string GetText(int offset, int length)
		{
			List<ICharObject> chars = GetChars(offset, length);
			StringBuilder textStringBuilder = new StringBuilder();
			chars.ForEach((ICharObject x) =>
			{
				textStringBuilder.Append(x.ToText());
			});
			return textStringBuilder.ToString();
		}

		private void Rebuild()
		{
			ParagraphNode paragraphNode = ParagraphTree.GetParagraphNodeByNumber(1);
			for (ParagraphNode nextNode = paragraphNode.NextNode; nextNode != null; nextNode = nextNode.NextNode)
			{
				nextNode.ResetNode();
			}
			paragraphNode.ResetNode();
			SimpleSegment simpleSegment = TextUtilities.NextNewParagraph(Chars, 0);
			List<ParagraphNode> list = new List<ParagraphNode>();
			int num = 0;
			while (simpleSegment != SimpleSegment.Invalid)
			{
				paragraphNode.TotalLength = simpleSegment.StartOffset + simpleSegment.Length - num;
				paragraphNode.DelimiterLength = simpleSegment.Length;
				num = simpleSegment.StartOffset + simpleSegment.Length;
				list.Add(paragraphNode);
				paragraphNode = new ParagraphNode();
				simpleSegment = TextUtilities.NextNewParagraph(Chars, num);
			}
			paragraphNode.TotalLength = CharCount - num;
			list.Add(paragraphNode);
			ParagraphTree.RebuildTree(list);
			DocumentTreeBuilt?.Invoke(list);
		}

		private void RemoveInner(int offset, int length)
		{
			if (length == 0)
			{
				return;
			}
			ParagraphNode paragraphNodeByDocumentOffset = ParagraphTree.GetParagraphNodeByDocumentOffset(offset);
			int startOffset = paragraphNodeByDocumentOffset.StartOffset;
			if (offset + length < startOffset + paragraphNodeByDocumentOffset.TotalLength)
			{
				SetParagraphLength(paragraphNodeByDocumentOffset, paragraphNodeByDocumentOffset.TotalLength - length);
				return;
			}
			int num = startOffset + paragraphNodeByDocumentOffset.TotalLength - offset;
			ParagraphNode paragraphNodeByDocumentOffset2 = ParagraphTree.GetParagraphNodeByDocumentOffset(offset + length);
			if (paragraphNodeByDocumentOffset2 == paragraphNodeByDocumentOffset)
			{
				SetParagraphLength(paragraphNodeByDocumentOffset, paragraphNodeByDocumentOffset.TotalLength - length);
				return;
			}
			int num2 = paragraphNodeByDocumentOffset2.StartOffset + paragraphNodeByDocumentOffset2.TotalLength - (offset + length);
			ParagraphNode nextNode = paragraphNodeByDocumentOffset.NextNode;
			ParagraphNode paragraphNode;
			do
			{
				paragraphNode = nextNode;
				nextNode = nextNode.NextNode;
				ParagraphRemoving?.Invoke(paragraphNode);
				ParagraphTree.RemoveParagraphNode(paragraphNode);
				ParagraphRemoved?.Invoke(paragraphNode);
			}
			while (paragraphNode != paragraphNodeByDocumentOffset2);
			SetParagraphLength(paragraphNodeByDocumentOffset, paragraphNodeByDocumentOffset.TotalLength - num + num2);
		}

		private void InsertInner(int offset, IReadOnlyList<ICharObject> chars)
		{
			ParagraphNode paragraphNode = ParagraphTree.GetParagraphNodeByDocumentOffset(offset);
			SimpleSegment simpleSegment = TextUtilities.NextNewParagraph(chars, 0);
			int count = chars.Count;
			if (simpleSegment == SimpleSegment.Invalid)
			{
				SetParagraphLength(paragraphNode, paragraphNode.TotalLength + count);
				return;
			}
			int num = 0;
			while (simpleSegment != SimpleSegment.Invalid)
			{
				int num2 = offset + simpleSegment.StartOffset + simpleSegment.Length;
				int num3 = paragraphNode.StartOffset + paragraphNode.TotalLength - offset - num;
				paragraphNode = SetParagraphLength(paragraphNode, num2 - paragraphNode.StartOffset);
				ParagraphNode paragraph = InsertParagraphAfter(paragraphNode, num3);
				paragraph = SetParagraphLength(paragraph, num3);
				paragraphNode = paragraph;
				num = simpleSegment.StartOffset + simpleSegment.Length;
				simpleSegment = TextUtilities.NextNewParagraph(chars, num);
			}
			if (num != count)
			{
				SetParagraphLength(paragraphNode, paragraphNode.TotalLength + count - num);
			}
		}

		private ParagraphNode SetParagraphLength(ParagraphNode paragraph, int newTotalLength)
		{
			if (newTotalLength - paragraph.TotalLength != 0)
			{
				paragraph.TotalLength = newTotalLength;
				ParagraphTree.UpdateAfterChildrenChange(paragraph);
			}
			if (newTotalLength == 0)
			{
				paragraph.DelimiterLength = 0;
			}
			else
			{
				int startOffset = paragraph.StartOffset;
				ICharObject charAt = GetCharAt(startOffset + newTotalLength - 1);
				if (charAt.Equals("\r"))
				{
					paragraph.DelimiterLength = 1;
				}
				else if (charAt.Equals("\n"))
				{
					if (newTotalLength >= 2 && GetCharAt(startOffset + newTotalLength - 2).Equals("\r"))
					{
						paragraph.DelimiterLength = 2;
					}
					else
					{
						if (newTotalLength == 1 && startOffset > 0 && GetCharAt(startOffset - 1).Equals("\r"))
						{
							ParagraphNode previousNode = paragraph.PreviousNode;
							ParagraphTree.RemoveParagraphNode(paragraph);
							return SetParagraphLength(previousNode, previousNode.TotalLength + 1);
						}
						paragraph.DelimiterLength = 1;
					}
				}
				else
				{
					paragraph.DelimiterLength = 0;
				}
			}
			return paragraph;
		}

		private ParagraphNode InsertParagraphAfter(ParagraphNode paragraph, int length)
		{
			ParagraphNode paragraphNode = ParagraphTree.InsertParagraphNodeAfter(paragraph, length);
			ParagraphInserted?.Invoke(paragraphNode);
			return paragraphNode;
		}
	}
}
