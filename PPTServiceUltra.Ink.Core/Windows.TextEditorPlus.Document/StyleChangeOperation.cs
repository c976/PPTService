using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Ink.Core.Documents;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class StyleChangeOperation : IOperation
	{
		private RunPropertyInfo RunPropertyInfo { get; }

		private ParagraphPropertyInfo ParagraphPropertyInfo { get; }

		private DocumentPropertyInfo DocumentPropertyInfo { get; }

		private StyleChangeEventArgs StyleChangeEventArgs { get; }

		private DocumentManager DocumentManager { get; }

		private PropertyType ChangedProperty { get; }

		public StyleChangeOperation(RunPropertyInfo runPropertyInfo, ParagraphPropertyInfo paragraphPropertyInfo, DocumentPropertyInfo documentPropertyInfo, StyleChangeEventArgs styleChangeEventArgs, DocumentManager documentManager, PropertyType propertyType)
		{
			RunPropertyInfo = runPropertyInfo;
			ParagraphPropertyInfo = paragraphPropertyInfo;
			DocumentPropertyInfo = documentPropertyInfo;
			StyleChangeEventArgs = styleChangeEventArgs;
			DocumentManager = documentManager;
			ChangedProperty = propertyType;
		}

		public void Undo()
		{
			DoOperation(RunPropertyInfo.PreIndexes, ParagraphPropertyInfo.PreIndexes, RunPropertyInfo.PreCurrentRunProperty, ParagraphPropertyInfo.PreCurrentParagraphProperty, DocumentPropertyInfo.PreTextVerticalAlignment, DocumentPropertyInfo.PreArrangingType);
		}

		public void Redo()
		{
			DoOperation(RunPropertyInfo.AfterIndexes, ParagraphPropertyInfo.AfterIndexes, RunPropertyInfo.AfterCurrentRunProperty, ParagraphPropertyInfo.AfterCurrentParagraphProperty, DocumentPropertyInfo.AfterTextVerticalAlignment, DocumentPropertyInfo.AfterArrangingType);
		}

		private void DoOperation(List<int> runProperties, List<int> paragraphProperties, RunProperty currentRunProperty, ParagraphProperty currentParagraphProperty, VerticalAlignment textVerticalAlignment, ArrangingType arrangingType)
		{
			StyleChangeEventArgs styleChangeEventArgs = new StyleChangeEventArgs(StyleChangeEventArgs.ChangedSegment, StyleChangeEventArgs.ChangedProperty)
			{
				IsFromUndoRedo = true
			};
			DocumentManager.OnStyleChanging(styleChangeEventArgs);
			DocumentManager.PropertyManager.RunProperties = runProperties;
			DocumentManager.PropertyManager.ParagraphProperties = paragraphProperties;
			DocumentManager.PropertyManager.SetCurrentProperty(currentRunProperty);
			DocumentManager.PropertyManager.SetCurrentProperty(currentParagraphProperty);
			DocumentManager.ArrangingType = arrangingType;
			DocumentManager.TextVerticalAlignment = textVerticalAlignment;
			for (int i = 0; i < DocumentManager.CharsManager.CharCount; i++)
			{
				ExtensionalChar extensionalChar = DocumentManager.CharsManager.GetCharAt(i) as ExtensionalChar;
				if (extensionalChar != null)
				{
					TextEditorExtManager.StyleAdapter(extensionalChar, new PropertyChangedArgs(ChangedProperty, null, DocumentManager.PropertyManager.GetRunProperty(i)), DocumentManager.ArrangingType);
				}
			}
			DocumentManager.OnStyleChanged(styleChangeEventArgs);
		}
	}
}
