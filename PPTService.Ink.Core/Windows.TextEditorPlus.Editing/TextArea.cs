using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Core;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Extensions;
using PPTService.Ink.Core.Windows.TextEditorPlus.Layout;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;
using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Editing
{
	public class TextArea : FrameworkElement, ILazyInitialize
	{
		private readonly LazyInitialzer _lazyInitialzer = new LazyInitialzer();

		internal DocumentManager DocumentManager { get; }

		internal RenderManager RenderManager { get; }

		internal CaretManager Caret { get; }

		internal CaretLayer CaretLayer { get; }

		internal List<MenuItem> ExtentionContextMenuItems { get; }

		private TextInputManager TextInputManager { get; }

		private List<Rect> ExtensionCharVisualBounds { get; } = new List<Rect>();

		internal event EventHandler AddHyperlink;

		internal event InternalHyperlinkHandler EditHyperlink;

		internal event InternalHyperlinkHandler RaiseHyperlink;

		internal event EventHandler<RichTextSaveInfo> BeforCopy;

		internal event EventHandler DragTextStarting;

		internal event EventHandler DragTextCompleted;

		internal event EventHandler DropTextCompleted;

		internal event EventHandler PasteNotSupported;

		internal event EventHandler<ExtensionalChar> DoubleClickExtensionalChar;

		static TextArea()
		{
			KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(TextArea), new FrameworkPropertyMetadata(true));
			UIElement.FocusableProperty.OverrideMetadata(typeof(TextArea), new FrameworkPropertyMetadata(true));
			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TextArea), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
		}

		internal TextArea(DocumentManager documentManager, RenderManager renderManager)
		{
			FocusVisualStyle = null;
			DocumentManager = documentManager;
			RenderManager = renderManager;
			ExtentionContextMenuItems = new List<MenuItem>();
			CommandHandler part = new CommandHandler(this);
			_lazyInitialzer.Add(part);
			new ImeSupport(this);
			CaretLayer = new CaretLayer();
			Caret = new CaretManager(documentManager, renderManager, CaretLayer);
			Caret.Show();
			IsKeyboardFocusWithinChanged += (object _, DependencyPropertyChangedEventArgs args) =>
			{
				Caret.IsCaretKeyBoardFocusWithin = (bool)args.NewValue;
			};
			_lazyInitialzer.Add(CaretLayer);
			TextInputManager = new TextInputManager(documentManager);
			renderManager.RenderCompleted += RenderManager_RenderCompleted;
		}

		private void RenderManager_RenderCompleted(object sender, List<ParagraphLayoutData> e)
		{
			ExtensionCharVisualBounds.Clear();
			foreach (ParagraphLayoutData item2 in e)
			{
				foreach (LineVisualBase line in item2.Lines)
				{
					(from visual in line.LineRenderData.CharVisuals.OfType<ExtensionCharVisual>()
						where visual.IsHitTestVisible
						select visual).ToList().ForEach((ExtensionCharVisual visual) =>
					{
						Rect item = line.TranslateRect(visual.Bounds, this);
						ExtensionCharVisualBounds.Add(item);
					});
				}
			}
		}

		public void InitializeUntilEditable()
		{
			new MouseHandler(this);
			new KeyboardHandler(this);
			_lazyInitialzer.NowItIsEditable();
		}

		public void InitializeUntilNotEmpty()
		{
		}

		protected override void OnTextInput(TextCompositionEventArgs e)
		{
			base.OnTextInput(e);
			if (!e.Handled)
			{
				TextInputManager.PerformTextInput(e);
				e.Handled = true;
			}
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			if (ExtensionCharVisualBounds.Any((Rect temp) => temp.Contains(hitTestParameters.HitPoint)))
			{
				return null;
			}
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}

		public void OnHyperlinkAdded()
		{
			AddHyperlink?.Invoke(this, EventArgs.Empty);
		}

		public void OnHyperlinkEdited()
		{
			int num = HyperlinkHelper.GetSelectionHyperlinkIndexes(this).First().First();
			InternalHyperlinkEventArgs eventArgs = new InternalHyperlinkEventArgs(DocumentManager.PropertyManager.GetRunProperty(num).Hyperlink);
			EditHyperlink?.Invoke(this, eventArgs);
		}

		internal void OnRaiseHyperlinkByDocumentOffsets(List<int> segment)
		{
			if (segment != null && segment.Any())
			{
				int num = segment.First();
				string text = ((num < 0) ? DocumentManager.PropertyManager.GetParagraphProperty(0).DefaultRunProperty.Hyperlink : DocumentManager.PropertyManager.GetRunProperty(num).Hyperlink);
				if (text != null)
				{
					InternalHyperlinkEventArgs eventArgs = new InternalHyperlinkEventArgs(text);
					RaiseHyperlink?.Invoke(this, eventArgs);
				}
				SimpleSegment segment2 = new SimpleSegment(segment.First(), segment.Count);
				DocumentManager.SetRunProperty((RunProperty p) =>
				{
					p.IsHyperlinkHit = true;
				}, segment2, PropertyType.Hyperlink);
			}
		}

		internal void OnBeforCopy(RichTextSaveInfo saveInfo)
		{
			BeforCopy?.Invoke(this, saveInfo);
		}

		internal void OnDragTextStarting()
		{
			DragTextStarting?.Invoke(this, EventArgs.Empty);
		}

		internal void OnDragTextCompleted()
		{
			DragTextCompleted?.Invoke(this, EventArgs.Empty);
		}

		internal void OnDropTextCompleted()
		{
			DropTextCompleted?.Invoke(this, EventArgs.Empty);
		}

		internal void OnPasteNotSupported()
		{
			PasteNotSupported?.Invoke(this, EventArgs.Empty);
		}

		internal void AddContextMenuItem(MenuItem item)
		{
			if (!ExtentionContextMenuItems.Any((MenuItem x) => x.GetType() == item.GetType() && x.Header == item.Header))
			{
				ExtentionContextMenuItems.Add(item);
			}
		}

		internal void OnDoubleClickExtensionalChar(ExtensionalChar textChar)
		{
			DoubleClickExtensionalChar?.Invoke(this, textChar);
		}
	}
}
