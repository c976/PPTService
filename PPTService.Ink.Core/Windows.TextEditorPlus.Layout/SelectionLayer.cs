using System.Collections.Generic;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Layout
{
	internal class SelectionLayer : Layer
	{
		private readonly DocumentManager _documentManager;

		private readonly RenderManager _renderManager;

		private readonly List<DrawingVisual> _children = new List<DrawingVisual>();

		private bool _isAddedCleanSelectionOnRenderCompleted;

		protected override int VisualChildrenCount => _children.Count;

		internal SelectionLayer(DocumentManager documentManager, RenderManager renderManager)
			: base(KnownLayer.Selection)
		{
			_documentManager = documentManager;
			_renderManager = renderManager;
			_documentManager.SelectionChanged += DocumentManager_OnSelectionChanged;
			_renderManager.RenderCompleted += DocumentManager_OnRenderCompleted;
		}

		private void DocumentManager_OnRenderCompleted(object sender, List<ParagraphLayoutData> obj)
		{
			if (!_documentManager.Selection.IsEmpty)
			{
				RefreshSelection();
			}
		}

		private void DocumentManager_OnSelectionChanged(object sender, ValueChangeEventArgs<ISelection> e)
		{
			if (_documentManager.IsDocumentBuilding)
			{
				if (_documentManager.Selection.IsEmpty)
				{
					CleanSelectionOnceAfterRenderCompleted();
				}
			}
			else
			{
				RefreshSelection();
			}
		}

		private void CleanSelectionOnceAfterRenderCompleted()
		{
			if (!_isAddedCleanSelectionOnRenderCompleted)
			{
				_isAddedCleanSelectionOnRenderCompleted = true;
				_renderManager.RenderCompleted += CleanSelection_OnRenderCompleted;
			}
		}

		private void CleanSelection_OnRenderCompleted(object sender, List<ParagraphLayoutData> e)
		{
			_renderManager.RenderCompleted -= CleanSelection_OnRenderCompleted;
			_isAddedCleanSelectionOnRenderCompleted = false;
			CleanSelection();
		}

		private void CleanSelection()
		{
			_children.ForEach(RemoveVisualChild);
			_children.Clear();
		}

		private void RefreshSelection()
		{
			CleanSelection();
			AddSelection();
		}

		private void AddSelection()
		{
			if (!_documentManager.Selection.IsEmpty)
			{
				_children.Add(BuildSelectionVisual());
				_children.ForEach(AddVisualChild);
			}
		}

		private DrawingVisual BuildSelectionVisual()
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			DrawingContext drawingContext = drawingVisual.RenderOpen();
			drawingContext.DrawGeometry(geometry: TextHelperInner.BuildSelectionGeometry(_documentManager, _renderManager), brush: TextContext.DefaultSelectionBrush, pen: null);
			drawingContext.Close();
			return drawingVisual;
		}

		protected override Visual GetVisualChild(int index)
		{
			return _children[index];
		}
	}
}
