using System;
using System.Collections.Generic;
using System.Linq;
using PPTService.Ink.Core.Documents;
using PPTService.Ink.Core.Windows.TextEditorPlus.Extensions;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class TextChangeOperation : IOperation
	{
		private readonly CharChangeEventArgs _charChangeEventArgs;

		private readonly DocumentManager _documentManager;

		private readonly RunPropertyInfo _runPropertyInfo;

		private readonly ParagraphPropertyInfo _paragraphPropertyInfo;

		public TextChangeOperation(CharChangeEventArgs charChangeEventArgs, RunPropertyInfo runPropertyInfo, ParagraphPropertyInfo paragraphPropertyInfo, DocumentManager documentManager)
		{
			_charChangeEventArgs = charChangeEventArgs;
			_runPropertyInfo = runPropertyInfo;
			_paragraphPropertyInfo = paragraphPropertyInfo;
			_documentManager = documentManager;
		}

		public void Undo()
		{
			DoOperation(new SimpleSegment(_charChangeEventArgs.Offset, _charChangeEventArgs.InsertLength), _charChangeEventArgs.RemovedChars.ToList(), _paragraphPropertyInfo.PreIndexes, _runPropertyInfo.PreIndexes, _paragraphPropertyInfo.PreCurrentParagraphProperty, _runPropertyInfo.PreCurrentRunProperty);
		}

		public void Redo()
		{
			DoOperation(new SimpleSegment(_charChangeEventArgs.Offset, _charChangeEventArgs.RemoveLength), _charChangeEventArgs.InsertedChars, _paragraphPropertyInfo.AfterIndexes, _runPropertyInfo.AfterIndexes, _paragraphPropertyInfo.AfterCurrentParagraphProperty, _runPropertyInfo.AfterCurrentRunProperty);
		}

		private void DoOperation(ISegment segment, IReadOnlyList<ICharObject> chars, IReadOnlyCollection<int> newParagraphPropIndexes, IReadOnlyCollection<int> newRunPropIndexes, ParagraphProperty currentParagraphProperty, RunProperty currentRunProperty)
		{
			CharChangeEventArgs textChangeArgs = new CharChangeEventArgs(segment.StartOffset, _documentManager.CharsManager.GetChars(segment.StartOffset, segment.Length), chars)
			{
				IsFromUndoRedo = true
			};
			_documentManager.OnDocumentChanging(textChangeArgs);
			_documentManager.CharsManager.Replace(segment, chars);
			if (_documentManager.CharsManager.CharCount != newRunPropIndexes.Count)
			{
				throw new InvalidOperationException("the count of newRunPropIndexes must equal to the length of new Text");
			}
			if (_documentManager.CharsManager.ParagraphTree.ParagraphCount != newParagraphPropIndexes.Count)
			{
				throw new InvalidOperationException("the count of newLinePropIndexes must equal to the count of new Text's lines");
			}
			_documentManager.PropertyManager.ParagraphProperties = new List<int>(newParagraphPropIndexes);
			_documentManager.PropertyManager.RunProperties = new List<int>(newRunPropIndexes);
			_documentManager.PropertyManager.SetCurrentProperty(currentRunProperty);
			_documentManager.PropertyManager.SetCurrentProperty(currentParagraphProperty);
			for (int i = 0; i < _documentManager.CharsManager.CharCount; i++)
			{
				ExtensionalChar extensionalChar = _documentManager.CharsManager.GetCharAt(i) as ExtensionalChar;
				if (extensionalChar != null)
				{
					TextEditorExtManager.StyleAdapter(extensionalChar, new PropertyChangedArgs(PropertyType.All, null, _documentManager.PropertyManager.GetRunProperty(i)), _documentManager.ArrangingType);
				}
			}
			_documentManager.OnDocumentChanged(textChangeArgs);
		}
	}
}
