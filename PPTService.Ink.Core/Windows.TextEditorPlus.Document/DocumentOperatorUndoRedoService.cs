using System.Collections.Generic;
using System.Linq;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class DocumentOperatorUndoRedoService
	{
		private readonly DocumentManager _documentManager;

		private RunPropertyInfo _runPropertyInfo;

		private ParagraphPropertyInfo _paragraphPropertyInfo;

		private DocumentPropertyInfo _documentPropertyInfo;

		public DocumentOperatorUndoRedoService(DocumentManager documentManager)
		{
			_documentManager = documentManager;
			_documentManager.StyleChanging += DocumentManger_OnStyleChanging;
			_documentManager.StyleChanged += DocumentManger_OnStyleChanged;
			_documentManager.DocumentChanging += DocumentManger_OnDocumentChanging;
			_documentManager.DocumentChanged += DocumentManager_OnDocumentChanged;
		}

		private void DocumentManger_OnDocumentChanging(object sender, CharChangeEventArgs obj)
		{
			if (!obj.IsFromUndoRedo)
			{
				_runPropertyInfo = new RunPropertyInfo();
				_paragraphPropertyInfo = new ParagraphPropertyInfo();
				_runPropertyInfo.PreCurrentRunProperty = _documentManager.PropertyManager.CurrentRunProperty.Clone();
				_runPropertyInfo.PreIndexes = new List<int>(_documentManager.PropertyManager.RunProperties);
				_paragraphPropertyInfo.PreIndexes = new List<int>(_documentManager.PropertyManager.ParagraphProperties);
				_paragraphPropertyInfo.PreCurrentParagraphProperty = _documentManager.PropertyManager.CurrentParagraphProperty.Clone();
			}
		}

		private void DocumentManager_OnDocumentChanged(object sender, CharChangeEventArgs obj)
		{
			if (!obj.IsFromUndoRedo)
			{
				_runPropertyInfo.AfterCurrentRunProperty = _documentManager.PropertyManager.CurrentRunProperty.Clone();
				_runPropertyInfo.AfterIndexes = new List<int>(_documentManager.PropertyManager.RunProperties);
				_paragraphPropertyInfo.AfterIndexes = new List<int>(_documentManager.PropertyManager.ParagraphProperties);
				_paragraphPropertyInfo.AfterCurrentParagraphProperty = _documentManager.PropertyManager.CurrentParagraphProperty.Clone();
				TextChangeOperation operation = new TextChangeOperation(new CharChangeEventArgs(obj.Offset, obj.RemovedChars.Select((ICharObject temp) => (ICharObject)temp.Clone()).ToList(), obj.InsertedChars.Select((ICharObject temp) => (ICharObject)temp.Clone()).ToList()), _runPropertyInfo, _paragraphPropertyInfo, _documentManager);
				_documentManager.InsertOperation(operation);
			}
		}

		private void DocumentManger_OnStyleChanging(object sender, StyleChangeEventArgs obj)
		{
			if (!obj.IsFromUndoRedo)
			{
				_runPropertyInfo = new RunPropertyInfo();
				_paragraphPropertyInfo = new ParagraphPropertyInfo();
				_documentPropertyInfo = new DocumentPropertyInfo();
				_runPropertyInfo.PreCurrentRunProperty = _documentManager.PropertyManager.CurrentRunProperty.Clone();
				_runPropertyInfo.PreIndexes = new List<int>(_documentManager.PropertyManager.RunProperties);
				_paragraphPropertyInfo.PreIndexes = new List<int>(_documentManager.PropertyManager.ParagraphProperties);
				_paragraphPropertyInfo.PreCurrentParagraphProperty = _documentManager.PropertyManager.CurrentParagraphProperty.Clone();
				_documentPropertyInfo.PreArrangingType = _documentManager.ArrangingType;
				_documentPropertyInfo.PreTextVerticalAlignment = _documentManager.TextVerticalAlignment;
			}
		}

		private void DocumentManger_OnStyleChanged(object sender, StyleChangeEventArgs obj)
		{
			if (!obj.IsFromUndoRedo)
			{
				_runPropertyInfo.AfterCurrentRunProperty = _documentManager.PropertyManager.CurrentRunProperty.Clone();
				_runPropertyInfo.AfterIndexes = new List<int>(_documentManager.PropertyManager.RunProperties);
				_paragraphPropertyInfo.AfterIndexes = new List<int>(_documentManager.PropertyManager.ParagraphProperties);
				_paragraphPropertyInfo.AfterCurrentParagraphProperty = _documentManager.PropertyManager.CurrentParagraphProperty.Clone();
				_documentPropertyInfo.AfterArrangingType = _documentManager.ArrangingType;
				_documentPropertyInfo.AfterTextVerticalAlignment = _documentManager.TextVerticalAlignment;
				StyleChangeOperation operation = new StyleChangeOperation(_runPropertyInfo, _paragraphPropertyInfo, _documentPropertyInfo, obj, _documentManager, obj.ChangedProperty);
				_documentManager.InsertOperation(operation);
			}
		}
	}
}
