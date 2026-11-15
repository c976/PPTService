using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PPTService.Ink.Core.ComponentModel;
using PPTService.Ink.Core.Contracts;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Documents;
using PPTService.Ink.Core.Linq;
using PPTService.Ink.Core.Windows.Controls;
using PPTService.Ink.Core.Windows.TextEditorPlus.Core;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader;
using PPTService.Ink.Core.Windows.TextEditorPlus.Editing;
using PPTService.Ink.Core.Windows.TextEditorPlus.Extensions;
using PPTService.Ink.Core.Windows.TextEditorPlus.Layout;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;
using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils.Caret;
using PPTService.Ink.Core.Windows.Threading;

namespace PPTService.Ink.Core.Windows.TextEditorPlus
{
	public class TextEditor : FrameworkElement
	{
		private readonly TextView _textView;

		private readonly StorageManager _storageManager;

		private readonly DispatcherRequiring _propertyFeedbackRequiring;

		private readonly LazyInitialzer _lazyInitialzer = new LazyInitialzer();

		private readonly ParagraphProperty _defaultParagraphProp = new ParagraphProperty
		{
			DefaultRunProperty = 
			{
				FontSize = 40.0,
				Foreground = InnerTextContext.DefaultForeground
			}
		};

		private readonly TextTracerConditional _enableShowTextRenderSizeDebugConditional;

		private bool _changeInner;

		private InputGestureInfo _previewInputGestureInfo;

		private InputGestureInfo _inputGestureInfo;

		public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(ValueChangedRoutedEventHandler<ISelection>), typeof(TextEditor));

		public static readonly RoutedEvent DocumentChangingEvent = EventManager.RegisterRoutedEvent("DocumentChanging", RoutingStrategy.Bubble, typeof(EventHandler<CharChangeRoutedEventArgs>), typeof(TextEditor));

		public static readonly RoutedEvent DocumentChangedEvent = EventManager.RegisterRoutedEvent("DocumentChanged", RoutingStrategy.Bubble, typeof(EventHandler<CharChangeRoutedEventArgs>), typeof(TextEditor));

		public static readonly RoutedEvent DocumentSizeChangingEvent = EventManager.RegisterRoutedEvent("DocumentSizeChanging", RoutingStrategy.Bubble, typeof(EventHandler), typeof(TextEditor));

		public static readonly RoutedEvent DocumentSizeChangedEvent = EventManager.RegisterRoutedEvent("DocumentSizeChanged", RoutingStrategy.Bubble, typeof(ValueChangedRoutedEventHandler<Size>), typeof(TextEditor));

		public static readonly RoutedEvent DocumentOutlineBoundsChangedEvent = EventManager.RegisterRoutedEvent("DocumentOutlineBoundsChanged", RoutingStrategy.Bubble, typeof(DocumentOutlineBoundsChangedRoutedEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent PropertyFeedbackChangedEvent = EventManager.RegisterRoutedEvent("PropertyFeedbackChangedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent DragTextStartingEvent = EventManager.RegisterRoutedEvent("DragTextStarting", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent DragTextCompletedEvent = EventManager.RegisterRoutedEvent("DragTextCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent DropTextCompletedEvent = EventManager.RegisterRoutedEvent("DropTextCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextEditor));

		[Obsolete("命名不规范，请使用 PasteNotSupportedEvent 代替")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly RoutedEvent PasteNotSupportEvent = EventManager.RegisterRoutedEvent("PasteNotSupport", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent PasteNotSupportedEvent = EventManager.RegisterRoutedEvent("PasteNotSupported", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent SizeToContentChangedEvent = EventManager.RegisterRoutedEvent("SizeToContentChanged", RoutingStrategy.Bubble, typeof(ValueChangedRoutedEventHandler<SizeToContent>), typeof(TextEditor));

		public static readonly RoutedEvent ArrangingTypeChangedEvent = EventManager.RegisterRoutedEvent("ArrangingTypeChangedEvent", RoutingStrategy.Bubble, typeof(ValueChangedRoutedEventHandler<ArrangingType>), typeof(TextEditor));

		public static readonly RoutedEvent TextVerticalAlignmentChangedEvent = EventManager.RegisterRoutedEvent("TextVerticalAlignmentChangedEvent", RoutingStrategy.Bubble, typeof(ValueChangedRoutedEventHandler<VerticalAlignment>), typeof(TextEditor));

		public static readonly RoutedEvent RenderCompletedEvent = EventManager.RegisterRoutedEvent("RenderCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent CaretOffsetChangedEvent = EventManager.RegisterRoutedEvent("CaretOffsetChangedEvent", RoutingStrategy.Bubble, typeof(ValueChangedRoutedEventHandler<CaretOffset>), typeof(TextEditor));

		public static readonly RoutedEvent CaretBoundsChangedEvent = EventManager.RegisterRoutedEvent("CaretBoundsChangedEvent", RoutingStrategy.Bubble, typeof(ValueChangedRoutedEventHandler<Rect>), typeof(TextEditor));

		public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent("MouseDoubleClickEvent", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent PreviewMouseDoubleClickEvent = EventManager.RegisterRoutedEvent("PreviewMouseDoubleClickEvent", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent TextEditorPreviewMouseLeftButtonUpEvent = EventManager.RegisterRoutedEvent("TextEditorPreviewMouseLeftButtonUpEvent", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent TextEditorPreviewMouseLeftButtonDownEvent = EventManager.RegisterRoutedEvent("TextEditorPreviewMouseLeftButtonDownEvent", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent TextEditorPreviewMouseRightButtonUpEvent = EventManager.RegisterRoutedEvent("TextEditorPreviewMouseRightButtonUp", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent TextEditorPreviewMouseRightButtonDownEvent = EventManager.RegisterRoutedEvent("TextEditorPreviewMouseRightButtonDown", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent DoubleClickExtensionalCharEvent = EventManager.RegisterRoutedEvent("DoubleClickExtensionalChar", RoutingStrategy.Bubble, typeof(ExtensionalCharRoutedEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent AddHyperlinkEvent = EventManager.RegisterRoutedEvent("AddHyperlink", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent EditHyperlinkEvent = EventManager.RegisterRoutedEvent("EditHyperlink", RoutingStrategy.Bubble, typeof(HyperlinkRoutedEventHandler), typeof(TextEditor));

		public static readonly RoutedEvent RaiseHyperlinkEvent = EventManager.RegisterRoutedEvent("RaiseHyperlink", RoutingStrategy.Bubble, typeof(HyperlinkRoutedEventHandler), typeof(TextEditor));

		private Size _documentSize;

		private Rect _lastDocumentOutlineBounds;

		public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(TextEditor), new PropertyMetadata(true, OnIsEditableChanged));

		public static readonly DependencyProperty SizeToContentProperty = DependencyProperty.Register("SizeToContent", typeof(SizeToContent), typeof(TextEditor), new PropertyMetadata(SizeToContent.Manual, OnSizeToContentChanged));

		public static readonly DependencyProperty LineHeightProperty = DependencyProperty.Register("LineHeight", typeof(double), typeof(TextEditor), new PropertyMetadata(1.0, OnLineHeightChanged));

		public static readonly DependencyProperty LineSpacingStrategyProperty = DependencyProperty.Register("LineSpacingStrategy", typeof(LineSpacingStrategy), typeof(TextEditor), new PropertyMetadata(LineSpacingStrategy.FullExpand, OnLineSpacingStrategyChanged));

		public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(TextEditor), new PropertyMetadata(int.MaxValue, OnMaxLengthChanged));

		public static readonly DependencyProperty ArrangingTypeProperty = DependencyProperty.Register("ArrangingType", typeof(ArrangingType), typeof(TextEditor), new PropertyMetadata(ArrangingType.Horizontal, TextEditor_OnArrangingTypeChanged));

		public static readonly DependencyProperty AcceptingInputTypeProperty = DependencyProperty.Register("AcceptingInputType", typeof(AcceptingInputTypes), typeof(TextEditor), new PropertyMetadata(AcceptingInputTypes.All, OnAcceptingInputTypeChanged));

		public static readonly DependencyProperty AcceptingTypeRegexProperty = DependencyProperty.Register("AcceptingTypeRegex", typeof(string), typeof(TextEditor), new PropertyMetadata(string.Empty, OnAcceptingTypeRegexChanged));

		public static readonly DependencyProperty IsUndoRedoEnabledProperty = DependencyProperty.Register("IsUndoRedoEnabled", typeof(bool), typeof(TextEditor), new PropertyMetadata(true, OnIsUndoRedoEnabledChanged));

		public static readonly DependencyProperty IsAutoCaretBrushProperty = DependencyProperty.Register("IsAutoCaretBrush", typeof(bool), typeof(TextEditor), new PropertyMetadata(true, OnAutoCaretBrushChanged));

		public static readonly DependencyProperty IsExtCharEditSupportedProperty = DependencyProperty.Register("IsExtCharEditSupported", typeof(bool), typeof(TextEditor), new PropertyMetadata(false, OnIsExtCharEditSupportedChanged));

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextEditor), new PropertyMetadata(string.Empty, OnTextPropertyChanged));

		public static readonly DependencyProperty TextHorizontalAlignmentProperty = DependencyProperty.Register("TextHorizontalAlignment", typeof(TextAlignment), typeof(TextEditor), new PropertyMetadata(TextAlignment.Left, OnTextHorizontalAlignmentPropertyChanged));

		public static readonly DependencyProperty TextVerticalAlignmentProperty = DependencyProperty.Register("TextVerticalAlignment", typeof(VerticalAlignment), typeof(TextEditor), new PropertyMetadata(VerticalAlignment.Top, OnTextVerticalAlignmentChanged));

		public new static readonly DependencyProperty OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(TextEditor), new PropertyMetadata(UIElement.OpacityProperty.DefaultMetadata.DefaultValue, OnRenderPropertyChanged));

		public static readonly DependencyProperty FontStretchProperty = DependencyProperty.Register("FontStretch", typeof(FontStretch), typeof(TextEditor), new FrameworkPropertyMetadata(TextElement.FontStretchProperty.DefaultMetadata.DefaultValue, OnRenderPropertyChanged));

		public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(TextEditor), new PropertyMetadata(TextElement.FontFamilyProperty.DefaultMetadata.DefaultValue, OnRenderPropertyChanged));

		public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(TextEditor), new PropertyMetadata(40.0, OnRenderPropertyChanged));

		public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(TextEditor), new PropertyMetadata(TextElement.FontStyleProperty.DefaultMetadata.DefaultValue, OnRenderPropertyChanged));

		public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(TextEditor), new PropertyMetadata(TextElement.FontWeightProperty.DefaultMetadata.DefaultValue, OnRenderPropertyChanged));

		public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(TextEditor), new PropertyMetadata(InnerTextContext.DefaultForeground, OnRenderPropertyChanged));

		public static readonly DependencyProperty BaselineAlignmentProperty = DependencyProperty.Register("BaselineAlignment", typeof(BaselineAlignment), typeof(TextEditor), new PropertyMetadata(BaselineAlignment.Baseline, OnRenderPropertyChanged));

		public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(TextEditor), new PropertyMetadata(TextWrapping.Wrap, OnTextWrappingChanged));

		public TextArea TextArea { get; }

		public DocumentManager DocumentManager { get; }

		internal RenderManager RenderManager { get; }

		public RenderInfo RenderInfo { get; }

		protected override int VisualChildrenCount => 2;

		public Size DocumentSize
		{
			get
			{
				return _documentSize;
			}
			private set
			{
				Size documentSize = _documentSize;
				Rect documentOutlineBounds = DocumentOutlineBounds;
				if (documentSize != value)
				{
					_documentSize = value;
					RaiseDocumentSizeChangedEvent(value, documentSize);
				}
				if (documentOutlineBounds != _lastDocumentOutlineBounds)
				{
					RaiseDocumentOutlineBoundsChangedEvent(documentOutlineBounds, _lastDocumentOutlineBounds);
					_lastDocumentOutlineBounds = documentOutlineBounds;
				}
			}
		}

		public Rect DocumentOutlineBounds
		{
			get
			{
				Size documentOutlineSize = RenderManager.DocumentOutlineSize;
				if (ArrangingType == ArrangingType.Horizontal)
				{
					return new Rect(new Point(0.0, 0.0 - RenderManager.DeltaOutlineTop), documentOutlineSize);
				}
				return new Rect(new Point(0.0 - RenderManager.DeltaOutlineBottom, 0.0), new Size(documentOutlineSize.Height, documentOutlineSize.Width));
			}
		}

		public List<ICharObject> Chars => DocumentManager.CharsManager.Chars.Select((ICharObject x) => (ICharObject)x.Clone()).ToList();

		public bool IsCharGeometryCacheable
		{
			get
			{
				return RenderManager.IsCharGeometryCacheable;
			}
			set
			{
				RenderManager.IsCharGeometryCacheable = value;
			}
		}

		public bool IsEditable
		{
			get
			{
				return (bool)GetValue(IsEditableProperty);
			}
			set
			{
				SetValue(IsEditableProperty, value);
			}
		}

		public SizeToContent SizeToContent
		{
			get
			{
				return (SizeToContent)GetValue(SizeToContentProperty);
			}
			set
			{
				SetValue(SizeToContentProperty, value);
			}
		}

		public double LineHeight
		{
			get
			{
				return (double)GetValue(LineHeightProperty);
			}
			set
			{
				SetValue(LineHeightProperty, value);
			}
		}

		public LineSpacingStrategy LineSpacingStrategy
		{
			get
			{
				return (LineSpacingStrategy)GetValue(LineHeightProperty);
			}
			set
			{
				SetValue(LineHeightProperty, LineSpacingStrategy);
			}
		}

		public int MaxLength
		{
			get
			{
				return (int)GetValue(MaxLengthProperty);
			}
			set
			{
				SetValue(MaxLengthProperty, value);
			}
		}

		public ArrangingType ArrangingType
		{
			get
			{
				return (ArrangingType)GetValue(ArrangingTypeProperty);
			}
			set
			{
				SetValue(ArrangingTypeProperty, value);
			}
		}

		public AcceptingInputTypes AcceptingInputType
		{
			get
			{
				return (AcceptingInputTypes)GetValue(AcceptingInputTypeProperty);
			}
			set
			{
				SetValue(AcceptingInputTypeProperty, value);
			}
		}

		public string AcceptingTypeRegex
		{
			get
			{
				return (string)GetValue(AcceptingTypeRegexProperty);
			}
			set
			{
				SetValue(AcceptingTypeRegexProperty, value);
			}
		}

		public bool IsUndoRedoEnabled
		{
			get
			{
				return (bool)GetValue(IsUndoRedoEnabledProperty);
			}
			set
			{
				SetValue(IsUndoRedoEnabledProperty, value);
			}
		}

		public bool IsAutoCaretBrush
		{
			get
			{
				return (bool)GetValue(IsAutoCaretBrushProperty);
			}
			set
			{
				SetValue(IsAutoCaretBrushProperty, value);
			}
		}

		public bool IsExtCharEditSupported
		{
			get
			{
				return (bool)GetValue(IsExtCharEditSupportedProperty);
			}
			set
			{
				SetValue(IsExtCharEditSupportedProperty, value);
			}
		}

		public string Text
		{
			get
			{
				return DocumentManager.CharsManager.GetText(0, CharCount);
			}
			set
			{
				SetValue(TextProperty, value);
				string text = DocumentManager.CharsManager.GetText(0, CharCount);
				if (text != value)
				{
					OnTextPropertyChanged(this, new DependencyPropertyChangedEventArgs(TextProperty, text, value));
				}
			}
		}

		public TextAlignment TextHorizontalAlignment
		{
			get
			{
				return (TextAlignment)GetValue(TextHorizontalAlignmentProperty);
			}
			set
			{
				SetValue(TextHorizontalAlignmentProperty, value);
			}
		}

		public VerticalAlignment TextVerticalAlignment
		{
			get
			{
				return (VerticalAlignment)GetValue(TextVerticalAlignmentProperty);
			}
			set
			{
				SetValue(TextVerticalAlignmentProperty, value);
			}
		}

		public new double Opacity
		{
			get
			{
				return (double)GetValue(OpacityProperty);
			}
			set
			{
				SetValue(OpacityProperty, value);
			}
		}

		public FontStretch FontStretch
		{
			get
			{
				return (FontStretch)GetValue(FontStretchProperty);
			}
			set
			{
				SetValue(FontStretchProperty, value);
			}
		}

		public FontFamily FontFamily
		{
			get
			{
				return (FontFamily)GetValue(FontFamilyProperty);
			}
			set
			{
				SetValue(FontFamilyProperty, value);
			}
		}

		public double FontSize
		{
			get
			{
				return (double)GetValue(FontSizeProperty);
			}
			set
			{
				SetValue(FontSizeProperty, value);
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return (FontStyle)GetValue(FontStyleProperty);
			}
			set
			{
				SetValue(FontStyleProperty, value);
			}
		}

		public FontWeight FontWeight
		{
			get
			{
				return (FontWeight)GetValue(FontWeightProperty);
			}
			set
			{
				SetValue(FontWeightProperty, value);
			}
		}

		public Brush Foreground
		{
			get
			{
				return (Brush)GetValue(ForegroundProperty);
			}
			set
			{
				SetValue(ForegroundProperty, value);
			}
		}

		public BaselineAlignment BaselineAlignment
		{
			set
			{
				SetValue(BaselineAlignmentProperty, value);
			}
		}

		public TextWrapping TextWrapping
		{
			set
			{
				SetValue(TextWrappingProperty, value);
			}
		}

		public RunProperty CurrentRunProperty => DocumentManager.PropertyManager.CurrentRunProperty;

		public ParagraphProperty CurrentParagraphProperty => DocumentManager.PropertyManager.CurrentParagraphProperty;

		public Feature CurrentFeature => DocumentManager.CurrentFeature;

		public CaretManager CaretOperator => TextArea.Caret;

		public HyperlinkOperator HyperlinkOperator { get; }

		public LegacyApi LegacyApi { get; }

		public CaretInfo CaretInfo => DocumentManager.CaretInfo;

		public ISelection Selection => DocumentManager.Selection;

		public int CharCount => DocumentManager.CharsManager.CharCount;

		public int ParagraphCount => DocumentManager.CharsManager.ParagraphTree.ParagraphCount;

		public IUndoRedoProvider UndoRedo
		{
			get
			{
				return DocumentManager.UndoRedo;
			}
			set
			{
				DocumentManager.UndoRedo = value;
			}
		}

		public RunProperty DefaultRunProperty
		{
			get
			{
				return DocumentManager.PropertyManager.DefaultRunProperty;
			}
			set
			{
				DocumentManager.PropertyManager.DefaultRunProperty = value;
			}
		}

		public EffectArgsCollection TextEffect
		{
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				DocumentManager.ChangeProperty((RunProperty p) =>
				{
					p.EffectsArgs = value;
				});
			}
		}

		private static Dictionary<Type, IPropertyFeedbackReader> PropertyFeedbackReaderCache { get; } = new Dictionary<Type, IPropertyFeedbackReader>();

		public event ValueChangedRoutedEventHandler<ISelection> SelectionChanged
		{
			add
			{
				AddHandler(SelectionChangedEvent, value);
			}
			remove
			{
				RemoveHandler(SelectionChangedEvent, value);
			}
		}

		public event EventHandler<CharChangeRoutedEventArgs> DocumentChanging
		{
			add
			{
				AddHandler(DocumentChangingEvent, value);
			}
			remove
			{
				RemoveHandler(DocumentChangingEvent, value);
			}
		}

		public event EventHandler<CharChangeRoutedEventArgs> DocumentChanged
		{
			add
			{
				AddHandler(DocumentChangedEvent, value);
			}
			remove
			{
				RemoveHandler(DocumentChangedEvent, value);
			}
		}

		public event EventHandler DocumentSizeChanging
		{
			add
			{
				AddHandler(DocumentSizeChangingEvent, value);
			}
			remove
			{
				RemoveHandler(DocumentSizeChangingEvent, value);
			}
		}

		public event ValueChangedRoutedEventHandler<Size> DocumentSizeChanged
		{
			add
			{
				AddHandler(DocumentSizeChangedEvent, value);
			}
			remove
			{
				RemoveHandler(DocumentSizeChangedEvent, value);
			}
		}

		public event DocumentOutlineBoundsChangedRoutedEventHandler DocumentOutlineBoundsChanged
		{
			add
			{
				AddHandler(DocumentOutlineBoundsChangedEvent, value);
			}
			remove
			{
				RemoveHandler(DocumentOutlineBoundsChangedEvent, value);
			}
		}

		public event RoutedEventHandler PropertyFeedbackChanged
		{
			add
			{
				AddHandler(PropertyFeedbackChangedEvent, value);
			}
			remove
			{
				RemoveHandler(PropertyFeedbackChangedEvent, value);
			}
		}

		public event RoutedEventHandler DragTextStarting
		{
			add
			{
				AddHandler(DragTextStartingEvent, value);
			}
			remove
			{
				RemoveHandler(DragTextStartingEvent, value);
			}
		}

		public event RoutedEventHandler DragTextCompleted
		{
			add
			{
				AddHandler(DragTextCompletedEvent, value);
			}
			remove
			{
				RemoveHandler(DragTextCompletedEvent, value);
			}
		}

		public event RoutedEventHandler DropTextCompleted
		{
			add
			{
				AddHandler(DropTextCompletedEvent, value);
			}
			remove
			{
				RemoveHandler(DropTextCompletedEvent, value);
			}
		}

		[Obsolete("命名不规范，请使用 PasteNotSupported 代替")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event RoutedEventHandler PasteNotSupport
		{
			add
			{
				AddHandler(PasteNotSupportEvent, value);
			}
			remove
			{
				RemoveHandler(PasteNotSupportEvent, value);
			}
		}

		public event RoutedEventHandler PasteNotSupported
		{
			add
			{
				AddHandler(PasteNotSupportedEvent, value);
			}
			remove
			{
				RemoveHandler(PasteNotSupportedEvent, value);
			}
		}

		public event ValueChangedRoutedEventHandler<SizeToContent> SizeToContentChanged
		{
			add
			{
				AddHandler(SizeToContentChangedEvent, value);
			}
			remove
			{
				RemoveHandler(SizeToContentChangedEvent, value);
			}
		}

		public event ValueChangedRoutedEventHandler<ArrangingType> ArrangingTypeChanged
		{
			add
			{
				AddHandler(ArrangingTypeChangedEvent, value);
			}
			remove
			{
				RemoveHandler(ArrangingTypeChangedEvent, value);
			}
		}

		public event ValueChangedRoutedEventHandler<VerticalAlignment> TextVerticalAlignmentChanged
		{
			add
			{
				AddHandler(TextVerticalAlignmentChangedEvent, value);
			}
			remove
			{
				RemoveHandler(TextVerticalAlignmentChangedEvent, value);
			}
		}

		public event RoutedEventHandler RenderCompleted
		{
			add
			{
				AddHandler(RenderCompletedEvent, value);
			}
			remove
			{
				RemoveHandler(RenderCompletedEvent, value);
			}
		}

		public event ValueChangedRoutedEventHandler<CaretOffset> CaretOffsetChanged
		{
			add
			{
				AddHandler(CaretOffsetChangedEvent, value);
			}
			remove
			{
				RemoveHandler(CaretOffsetChangedEvent, value);
			}
		}

		public event ValueChangedRoutedEventHandler<Rect> CaretBoundsChanged
		{
			add
			{
				AddHandler(CaretBoundsChangedEvent, value);
			}
			remove
			{
				RemoveHandler(CaretBoundsChangedEvent, value);
			}
		}

		public event MouseButtonEventHandler MouseDoubleClick
		{
			add
			{
				AddHandler(MouseDoubleClickEvent, value);
			}
			remove
			{
				RemoveHandler(MouseDoubleClickEvent, value);
			}
		}

		public event MouseButtonEventHandler PreviewMouseDoubleClick
		{
			add
			{
				AddHandler(PreviewMouseDoubleClickEvent, value);
			}
			remove
			{
				RemoveHandler(PreviewMouseDoubleClickEvent, value);
			}
		}

		public event MouseButtonEventHandler TextEditorPreviewMouseLeftButtonUp
		{
			add
			{
				AddHandler(TextEditorPreviewMouseLeftButtonUpEvent, value);
			}
			remove
			{
				RemoveHandler(TextEditorPreviewMouseLeftButtonUpEvent, value);
			}
		}

		public event MouseButtonEventHandler TextEditorPreviewMouseLeftButtonDown
		{
			add
			{
				AddHandler(TextEditorPreviewMouseLeftButtonDownEvent, value);
			}
			remove
			{
				RemoveHandler(TextEditorPreviewMouseLeftButtonDownEvent, value);
			}
		}

		public event MouseButtonEventHandler TextEditorPreviewMouseRightButtonUp
		{
			add
			{
				AddHandler(TextEditorPreviewMouseRightButtonUpEvent, value);
			}
			remove
			{
				RemoveHandler(TextEditorPreviewMouseRightButtonUpEvent, value);
			}
		}

		public event MouseButtonEventHandler TextEditorPreviewMouseRightButtonDown
		{
			add
			{
				AddHandler(TextEditorPreviewMouseRightButtonDownEvent, value);
			}
			remove
			{
				RemoveHandler(TextEditorPreviewMouseRightButtonDownEvent, value);
			}
		}

		public event ExtensionalCharRoutedEventHandler DoubleClickExtensionalChar
		{
			add
			{
				AddHandler(DoubleClickExtensionalCharEvent, value);
			}
			remove
			{
				RemoveHandler(DoubleClickExtensionalCharEvent, value);
			}
		}

		public event RoutedEventHandler AddHyperlink
		{
			add
			{
				AddHandler(AddHyperlinkEvent, value);
			}
			remove
			{
				RemoveHandler(AddHyperlinkEvent, value);
			}
		}

		public event HyperlinkRoutedEventHandler EditHyperlink
		{
			add
			{
				AddHandler(EditHyperlinkEvent, value);
			}
			remove
			{
				RemoveHandler(EditHyperlinkEvent, value);
			}
		}

		public event HyperlinkRoutedEventHandler RaiseHyperlink
		{
			add
			{
				AddHandler(RaiseHyperlinkEvent, value);
			}
			remove
			{
				RemoveHandler(RaiseHyperlinkEvent, value);
			}
		}

		public TextEditor()
			: this(true)
		{
		}

		public TextEditor(bool isEditable)
		{
			FocusVisualStyle = null;
			_propertyFeedbackRequiring = new DispatcherRequiring(OnPropertyFeedbackChangedInner, DispatcherPriority.Background);
			CharsManager charsManager = new CharsManager();
			PropertyManager propertyManager = new PropertyManager(_defaultParagraphProp, charsManager);
			DocumentManager = new DocumentManager(charsManager, propertyManager);
			_storageManager = new StorageManager(this);
			RenderManager = new RenderManager(DocumentManager);
			RenderManager.RenderCompleted += RenderManager_OnRenderCompleted;
			_textView = new TextView(RenderManager);
			TextArea = new TextArea(DocumentManager, RenderManager);
			_lazyInitialzer.Add(_textView);
			_lazyInitialzer.UntilNotEmpty(() =>
			{
				_textView.SwitchTextLayer(DocumentManager.ArrangingType);
				_textView.InsertLayer(new SelectionLayer(DocumentManager, RenderManager));
				_textView.InsertLayer(TextArea.CaretLayer);
			});
			_lazyInitialzer.UntilEditable(RenderManager.ForceRedraw);
			AddVisualChild(_textView);
			AddVisualChild(TextArea);
			InitHyperlinkEvents();
			InitGlobalEvents();
			RenderManager.Redraw();
			Loaded += TextEditor_OnLoaded;
			IsEditable = isEditable;
			AttachDocumentManagerEvents();
			AttachControlEvents();
			AttachTextAreaEvents();
			AttachPropertyFeedbackChangedEvent();
			SizeChanged += (object sender, SizeChangedEventArgs args) =>
			{
				if (IsEditable)
				{
					CaretOperator.Show();
				}
			};
			HyperlinkOperator = new HyperlinkOperator(TextArea);
			LegacyApi = new LegacyApi(this);
			_enableShowTextRenderSizeDebugConditional = TextTracer.CreateConditional("文本渲染大小");
			_lazyInitialzer.Add(TextArea);
			DocumentManager.DocumentChanging += LazyInitialzerDocumentManager_DocumentChanging;
			RenderInfo = new RenderInfo(this);
		}

		private void LazyInitialzerDocumentManager_DocumentChanging(object sender, CharChangeEventArgs e)
		{
			DocumentManager.DocumentChanging -= LazyInitialzerDocumentManager_DocumentChanging;
			_lazyInitialzer.NowItIsNotEmpty();
		}

		private void InitGlobalEvents()
		{
			MouseMove += CoverRectangleOnMouseMove;
			StylusUp += CoverRectangleOnStylusUp;
			MouseLeftButtonUp += CoverRectangleOnMouseLeftButtonUp;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AttachPropertyFeedbackChangedEvent()
		{
			DocumentManager.PropertyManager.CurrentRunPropertyChanged +=  (object sender,  ValueChangeEventArgs<RunProperty> args) =>
			{
				RaisePropertyFeedbackChanged();
			};
			DocumentManager.PropertyManager.CurrentParagraphPropertyChanged +=  (object sender,  ValueChangeEventArgs<ParagraphProperty> args) =>
			{
				RaisePropertyFeedbackChanged();
			};
			DocumentManager.SelectionChanged +=  (object sender,  ValueChangeEventArgs<ISelection> args) =>
			{
				RaisePropertyFeedbackChanged();
			};
			TextArea.GotFocus += (object sender, RoutedEventArgs args) =>
			{
				RaisePropertyFeedbackChanged();
			};
		}

		private void TextEditor_OnLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= TextEditor_OnLoaded;
			if (IsEditable)
			{
				EnterEdit();
			}
			else
			{
				QuitEdit();
			}
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			bool flag = false;
			if (e.Property == FrameworkElement.WidthProperty)
			{
				double value = (double)e.NewValue;
				DocumentManager.EditorWidth = value.CoerceValue(MinWidth, MaxWidth);
				flag = true;
			}
			else if (e.Property == FrameworkElement.HeightProperty)
			{
				double value2 = (double)e.NewValue;
				DocumentManager.EditorHeight = value2.CoerceValue(MinHeight, MaxHeight);
				flag = true;
			}
			else if (e.Property == FrameworkElement.MaxHeightProperty)
			{
				double maxHeight = (double)e.NewValue;
				DocumentManager.MaxHeight = maxHeight;
				flag = true;
			}
			else if (e.Property == FrameworkElement.MinHeightProperty)
			{
				double minHeight = (double)e.NewValue;
				DocumentManager.MinHeight = minHeight;
				flag = true;
			}
			else if (e.Property == FrameworkElement.MaxWidthProperty)
			{
				double maxWidth = (double)e.NewValue;
				DocumentManager.MaxWidth = maxWidth;
				flag = true;
			}
			else if (e.Property == FrameworkElement.MinWidthProperty)
			{
				double minWidth = (double)e.NewValue;
				DocumentManager.MinWidth = minWidth;
				flag = true;
			}
			if (flag && !_changeInner)
			{
				RenderManager.Redraw();
			}
		}

		private void RenderManager_OnRenderCompleted(object sender, List<ParagraphLayoutData> obj)
		{
			DocumentSize = ((ArrangingType == ArrangingType.Horizontal) ? new Size(RenderManager.DocumentWidth, RenderManager.DocumentHeight) : new Size(RenderManager.DocumentHeight, RenderManager.DocumentWidth));
			ModifyEditorSize();
			RaiseEvent(new RoutedEventArgs(RenderCompletedEvent, this));
		}

		private void ModifyEditorSize()
		{
			_changeInner = true;
			if (DocumentManager.ArrangingType == ArrangingType.Horizontal)
			{
				switch (DocumentManager.SizeToContent)
				{
				case SizeToContent.Width:
					Width = DocumentSize.Width;
					DocumentManager.EditorWidth = Width;
					break;
				case SizeToContent.Height:
					Height = DocumentSize.Height;
					DocumentManager.EditorHeight = Height;
					break;
				case SizeToContent.WidthAndHeight:
					Width = DocumentSize.Width;
					Height = DocumentSize.Height;
					DocumentManager.EditorWidth = Width;
					DocumentManager.EditorHeight = Height;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				case SizeToContent.Manual:
					break;
				}
			}
			else
			{
				switch (DocumentManager.SizeToContent)
				{
				case SizeToContent.Width:
					Height = DocumentSize.Height;
					DocumentManager.EditorHeight = Height;
					break;
				case SizeToContent.Height:
					Width = DocumentSize.Width;
					DocumentManager.EditorWidth = Width;
					break;
				case SizeToContent.WidthAndHeight:
					Height = DocumentSize.Height;
					Width = DocumentSize.Width;
					DocumentManager.EditorHeight = Height;
					DocumentManager.EditorWidth = Width;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				case SizeToContent.Manual:
					break;
				}
			}
			_changeInner = false;
		}

		private void RaisePropertyFeedbackChanged()
		{
			_propertyFeedbackRequiring.Require();
		}

		private void OnPropertyFeedbackChangedInner()
		{
			RoutedEventArgs e = new RoutedEventArgs(PropertyFeedbackChangedEvent, this);
			RaiseEvent(e);
		}

		private void InitHyperlinkEvents()
		{
			TextArea.EditHyperlink += TextArea_OnEditHyperlink;
			TextArea.AddHyperlink += TextArea_OnAddHyperlink;
			TextArea.RaiseHyperlink += TextArea_OnRaiseHyperlink;
		}

		private void CoverRectangleOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!IsEditable && DocumentManager.CurrentFeature.HasFeature(Features.HyperlinkMouseHotInner) && !IsEditable)
			{
				OnRaiseHyperlink(e);
			}
		}

		private void CoverRectangleOnStylusUp(object sender, StylusEventArgs e)
		{
			if (!IsEditable && DocumentManager.CurrentFeature.HasFeature(Features.HyperlinkMouseHotInner) && !IsEditable)
			{
				OnRaiseHyperlink(e);
			}
		}

		private void CoverRectangleOnMouseMove(object sender, MouseEventArgs e)
		{
			Point position = TextHelperInner.CorrectPosition(e, TextArea);
			DocumentOffset documentOffsetByPositionFromRunBounds = TextArea.RenderManager.GetDocumentOffsetByPositionFromRunBounds(position);
			RunProperty runProperty = DocumentManager.PropertyManager.GetRunProperty(documentOffsetByPositionFromRunBounds);
			TextEditorDecorationCollection textEditorDecorationCollection = TextEditorRuntimeDecoration.GetTextEditorDecorationCollection(runProperty);
			if (documentOffsetByPositionFromRunBounds != DocumentOffset.DefaultDocumentOffset.Offset && textEditorDecorationCollection != null)
			{
				foreach (TextEditorDecoration item in textEditorDecorationCollection)
				{
					item.HandleMouseMove(position);
				}
			}
			if (!IsEditable && DocumentManager.CurrentFeature.HasFeature(Features.HyperlinkMouseHotInner))
			{
				if (runProperty.Hyperlink != null)
				{
					Cursor = Cursors.Hand;
				}
				else
				{
					ClearValue(FrameworkElement.CursorProperty);
				}
			}
		}

		private void OnRaiseHyperlink(RoutedEventArgs e)
		{
			Point controlPosition = TextHelperInner.CorrectPosition(e, TextArea);
			CaretOffset caretOffsetByPosition = TextArea.RenderManager.GetCaretOffsetByPosition(controlPosition);
			List<int> hyperlinkIndexesByCaretOffset = HyperlinkHelper.GetHyperlinkIndexesByCaretOffset(TextArea.DocumentManager, caretOffsetByPosition);
			TextArea.OnRaiseHyperlinkByDocumentOffsets(hyperlinkIndexesByCaretOffset);
		}

		public void FullInitialize()
		{
			_lazyInitialzer.NowItIsNotEmpty();
			_lazyInitialzer.NowItIsEditable();
		}

		private void EnterEdit()
		{
			FullInitialize();
			TextArea.IsHitTestVisible = true;
			UpdateMarkerByCaret();
			TextArea.Focusable = true;
			TextArea.Focus();
			TextArea.Caret.Show();
			TextTracer.LogCallMemberName("EnterEdit");
		}

		private void QuitEdit()
		{
			TextArea.IsHitTestVisible = false;
			TextArea.Caret.Hide();
			UpdateMarkerByCaret();
			DocumentManager.ClearSelection();
			TextArea.Focusable = false;
			TextTracer.LogCallMemberName("QuitEdit");
		}

		private void UpdateMarkerByCaret()
		{
			if (CaretInfo.CurrentCaretOffset.IsAtParagraphStart)
			{
				ParagraphNode paragraphNodeByCaretOffset = DocumentManager.CharsManager.ParagraphTree.GetParagraphNodeByCaretOffset(CaretInfo.CurrentCaretOffset);
				int paragraphNumber = paragraphNodeByCaretOffset.ParagraphNumber;
				ParagraphProperty paragraphProperty = DocumentManager.PropertyManager.GetParagraphProperty(paragraphNumber - 1);
				if (paragraphProperty.MarkerProperty != null && !paragraphProperty.MarkerProperty.IsEmpty() && paragraphNodeByCaretOffset.Length == 0)
				{
					RenderManager.UpdateMarkerState();
				}
			}
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(default(Point), RenderSize));
			if (_enableShowTextRenderSizeDebugConditional)
			{
				drawingContext.DrawRectangle(Brushes.FloralWhite, null, new Rect(default(Point), new Size(DocumentManager.EditorWidth, DocumentManager.EditorHeight)));
			}
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (DocumentManager.SizeToContent == SizeToContent.Manual)
			{
				double value = (double.IsNaN(Width) ? availableSize.Width : Width);
				value = value.CoerceValue(MinWidth, MaxWidth);
				double value2 = (double.IsNaN(Height) ? availableSize.Height : Height);
				value2 = value2.CoerceValue(MinHeight, MaxHeight);
				DocumentManager.EditorWidth = value;
				DocumentManager.EditorHeight = value2;
			}
			else if (DocumentManager.ArrangingType == ArrangingType.Horizontal)
			{
				if (DocumentManager.SizeToContent == SizeToContent.Height)
				{
					double value3 = (double.IsNaN(Width) ? availableSize.Width : Width);
					DocumentManager.EditorWidth = value3.CoerceValue(MinWidth, MaxWidth);
				}
				if (DocumentManager.SizeToContent == SizeToContent.Width)
				{
					double value4 = (double.IsNaN(Height) ? availableSize.Height : Height);
					DocumentManager.EditorHeight = value4.CoerceValue(MinHeight, MaxHeight);
				}
			}
			else
			{
				if (DocumentManager.SizeToContent == SizeToContent.Height)
				{
					double value5 = (double.IsNaN(Height) ? availableSize.Height : Height);
					DocumentManager.EditorHeight = value5.CoerceValue(MinHeight, MaxHeight);
				}
				if (DocumentManager.SizeToContent == SizeToContent.Width)
				{
					double value6 = (double.IsNaN(Width) ? availableSize.Width : Width);
					DocumentManager.EditorWidth = value6.CoerceValue(MinWidth, MaxWidth);
				}
			}
			double num = DocumentManager.EditorWidth;
			double num2 = DocumentManager.EditorHeight;
			if (double.IsInfinity(num) || double.IsNaN(num))
			{
				num = 0.0;
			}
			if (double.IsInfinity(num2) || double.IsNaN(num2))
			{
				num2 = 0.0;
			}
			return new Size(num, num2);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			TextArea.Arrange(new Rect(default(Point), arrangeBounds));
			_textView.Arrange(new Rect(default(Point), arrangeBounds));
			return base.ArrangeOverride(arrangeBounds);
		}

		protected override Visual GetVisualChild(int index)
		{
			if (index != 0)
			{
				return TextArea;
			}
			return _textView;
		}

		protected override Geometry GetLayoutClip(Size layoutSlotSize)
		{
			return null;
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			if (!IsEditable)
			{
				return new PointHitTestResult(this, hitTestParameters.HitPoint);
			}
			return base.HitTestCore(hitTestParameters);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AttachControlEvents()
		{
			_previewInputGestureInfo = new InputGestureInfo();
			_inputGestureInfo = new InputGestureInfo();
			PreviewMouseDown += TextEditor_OnPreviewMouseDown;
			MouseDown += TextEditor_OnMouseDown;
			PreviewMouseLeftButtonUp += TextEditor_OnPreviewMouseLeftButtonUp;
			PreviewMouseLeftButtonDown += TextEditor_OnPreviewMouseLeftButtonDown;
			PreviewMouseRightButtonUp += TextEditor_OnPreviewMouseRightButtonUp;
			PreviewMouseRightButtonDown += TextEditor_OnPreviewMouseRightButtonDown;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AttachDocumentManagerEvents()
		{
			DocumentManager.ArrangingTypeChanged += DocumentManager_OnArrangingTypeChanged;
			DocumentManager.TextVerticalAlignmentChanged += DocumentManager_OnTextVerticalAlignmentChanged;
			DocumentManager.DocumentChanged += DocumentManager_OnDocumentChanged;
			DocumentManager.SizeToContentChanged += DocumentManager_OnSizeToContentChanged;
			DocumentManager.SelectionChanged +=  (object sender,  ValueChangeEventArgs<ISelection> args) =>
			{
				RaiseEvent(new ValueChangedRoutedEventArgs<ISelection>(SelectionChangedEvent, args.OldValue, args.NewValue));
			};
			DocumentManager.DocumentChanging += (object sender, CharChangeEventArgs args) =>
			{
				RaiseEvent(new CharChangeRoutedEventArgs(DocumentChangingEvent, args));
			};
			DocumentManager.CaretInfo.OffsetChanged +=  (object sender, ValueChangeEventArgs<CaretOffset> args) =>
			{
				RaiseEvent(new ValueChangedRoutedEventArgs<CaretOffset>(CaretOffsetChangedEvent, args.OldValue, args.NewValue));
			};
			DocumentManager.CaretInfo.BoundsChanged +=  (object sender, ValueChangeEventArgs<Rect> args) =>
			{
				RaiseEvent(new ValueChangedRoutedEventArgs<Rect>(CaretBoundsChangedEvent, args.OldValue, args.NewValue));
			};
			DocumentManager.DocumentSizeChanging += (object sender, EventArgs args) =>
			{
				RaiseEvent(new RoutedEventArgs(DocumentSizeChangingEvent));
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AttachTextAreaEvents()
		{
			TextArea.DragTextStarting += (object sender, EventArgs args) =>
			{
				RaiseEvent(new RoutedEventArgs(DragTextStartingEvent));
			};
			TextArea.DragTextCompleted += (object sender, EventArgs args) =>
			{
				RaiseEvent(new RoutedEventArgs(DragTextCompletedEvent));
			};
			TextArea.DropTextCompleted += (object sender, EventArgs args) =>
			{
				RaiseEvent(new RoutedEventArgs(DropTextCompletedEvent));
			};
			TextArea.PasteNotSupported += (object sender, EventArgs args) =>
			{
				RaiseEvent(new RoutedEventArgs(PasteNotSupportEvent));
				RaiseEvent(new RoutedEventArgs(PasteNotSupportedEvent));
			};
			TextArea.DoubleClickExtensionalChar += (object sender, ExtensionalChar textChar) =>
			{
				RaiseEvent(new ExtensionalCharRoutedEventArgs(DoubleClickExtensionalCharEvent, textChar));
			};
		}

		private void TextEditor_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
			{
				RoutedEvent = TextEditorPreviewMouseLeftButtonDownEvent
			});
		}

		private void TextEditor_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
			{
				RoutedEvent = TextEditorPreviewMouseLeftButtonUpEvent
			});
		}

		private void TextEditor_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
			{
				RoutedEvent = TextEditorPreviewMouseRightButtonDownEvent
			});
		}

		private void TextEditor_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
			{
				RoutedEvent = TextEditorPreviewMouseRightButtonUpEvent
			});
		}

		private void TextEditor_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			_inputGestureInfo.RecordDown(this, e);
			if (_inputGestureInfo.ClickCount == 2)
			{
				RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
				{
					RoutedEvent = MouseDoubleClickEvent
				});
			}
		}

		private void TextEditor_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			_previewInputGestureInfo.RecordDown(this, e);
			if (_previewInputGestureInfo.ClickCount == 2)
			{
				RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
				{
					RoutedEvent = PreviewMouseDoubleClickEvent
				});
			}
		}

		private void DocumentManager_OnTextVerticalAlignmentChanged(object sender, ValueChangeEventArgs<VerticalAlignment> e)
		{
			TextVerticalAlignment = e.NewValue;
			ValueChangedRoutedEventArgs<VerticalAlignment> e2 = new ValueChangedRoutedEventArgs<VerticalAlignment>(TextVerticalAlignmentChangedEvent, e.OldValue, e.NewValue, this);
			RaiseEvent(e2);
		}

		private void DocumentManager_OnDocumentChanged(object sender, CharChangeEventArgs e)
		{
			if (!IsExtCharEditSupported)
			{
				Text = DocumentManager.CharsManager.GetText(0, CharCount);
			}
			RaiseEvent(new CharChangeRoutedEventArgs(DocumentChangedEvent, e, this));
		}

		private void DocumentManager_OnSizeToContentChanged(object sender, ValueChangeEventArgs<SizeToContent> e)
		{
			SizeToContent = e.NewValue;
			RaiseEvent(new ValueChangedRoutedEventArgs<SizeToContent>(SizeToContentChangedEvent, e.OldValue, e.NewValue, this));
		}

		private void DocumentManager_OnArrangingTypeChanged(object sender, ValueChangeEventArgs<ArrangingType> e)
		{
			ArrangingType = e.NewValue;
			HandlerControlSize(e.NewValue, e.OldValue);
			_textView.SwitchTextLayer(e.NewValue);
			ValueChangedRoutedEventArgs<ArrangingType> e2 = new ValueChangedRoutedEventArgs<ArrangingType>(ArrangingTypeChangedEvent, e.OldValue, e.NewValue, this);
			RaiseEvent(e2);
		}

		private void HandlerControlSize(ArrangingType newValue, ArrangingType oldValue)
		{
			if (oldValue == ArrangingType.Horizontal)
			{
				if (SizeToContent == SizeToContent.Manual)
				{
					double width = Width;
					Width = Height;
					Height = width;
				}
				else if (SizeToContent == SizeToContent.Width)
				{
					Width = Height;
				}
				else if (SizeToContent == SizeToContent.Height)
				{
					Height = Width;
				}
			}
			else if (newValue == ArrangingType.Horizontal)
			{
				if (SizeToContent == SizeToContent.Manual)
				{
					double width2 = Width;
					Width = Height;
					Height = width2;
				}
				else if (SizeToContent == SizeToContent.Width)
				{
					Height = Width;
				}
				else if (SizeToContent == SizeToContent.Height)
				{
					Width = Height;
				}
			}
		}

		[NotNull]
		public virtual RichTextSaveInfo GetRichText()
		{
			return _storageManager.GetRichText();
		}

		public virtual void SetRichText([NotNull] RichTextSaveInfo richTextSaveInfo, bool withForceRedraw = true)
		{
			bool isUndoRedoEnabled = IsUndoRedoEnabled;
			try
			{
				IsUndoRedoEnabled = false;
				_storageManager.SetRichText(richTextSaveInfo, withForceRedraw);
			}
			finally
			{
				IsUndoRedoEnabled = isUndoRedoEnabled;
			}
		}

		[NotNull]
		public TextEditorProperty GetFormatPaintProperty()
		{
			TextEditorProperty textEditorProperty = new TextEditorProperty();
			if (!IsEditable)
			{
				ParagraphProperty paragraphProperty = DocumentManager.PropertyManager.GetParagraphProperty(0);
				RunProperty runProperty = paragraphProperty.DefaultRunProperty;
				if (DocumentManager.CharsManager.CharCount > 0)
				{
					runProperty = DocumentManager.PropertyManager.GetRunProperty(0);
				}
				textEditorProperty.RunProperty = runProperty.Clone();
				textEditorProperty.ParagraphProperty = paragraphProperty.Clone();
				textEditorProperty.TextVerticalAlignment = DocumentManager.TextVerticalAlignment;
			}
			else if (DocumentManager.Selection.IsEmpty)
			{
				textEditorProperty.RunProperty = DocumentManager.PropertyManager.CurrentRunProperty.Clone();
				textEditorProperty.ParagraphProperty = DocumentManager.PropertyManager.CurrentParagraphProperty.Clone();
				textEditorProperty.TextVerticalAlignment = TextVerticalAlignment;
			}
			else
			{
				textEditorProperty.RunProperty = DocumentManager.PropertyManager.GetRunProperty(DocumentManager.Selection.FrontOffset.Offset + 1).Clone();
				textEditorProperty.ParagraphProperty = DocumentManager.PropertyManager.GetParagraphProperty(TextHelperInner.GetParagraphIndexBySegment(DocumentManager.CharsManager, DocumentManager.Selection.ToSegment()).First()).Clone();
				textEditorProperty.TextVerticalAlignment = TextVerticalAlignment;
			}
			return textEditorProperty;
		}

		public double CalcDocumentWidthWithoutWrapping()
		{
			return RenderManager.CalcDocumentWidthRenderInOneLine();
		}

		public double CalcDocumentMinWidth()
		{
			return RenderManager.CalculateDocumentMinWidth();
		}

		public double CalcDocumentMinHeight()
		{
			return RenderManager.CalculateDocumentMinHeight();
		}

		public double CalcDocumentHeightWithExactWidth(double exactWidth)
		{
			if (exactWidth < 0.0 || exactWidth > 32000.0)
			{
				throw new ArgumentOutOfRangeException("exactWidth", $"exactWidth must between 0 to {32000.0}");
			}
			RenderManager.UpdateGeometries();
			double width = exactWidth + 0.001;
			ParagraphNode paragraphNode = DocumentManager.CharsManager.ParagraphTree.First();
			double num = 0.0;
			Dictionary<int, ParagraphTextBeforeData> dictionary = RenderManager.PreParagraphDatas();
			while (paragraphNode != null)
			{
				ParagraphBase paragraphBase = RenderManager.CreateParagraph(paragraphNode);
				paragraphBase.ParagraphTextBeforeData = dictionary[paragraphNode.ParagraphNumber];
				paragraphBase.BuildRenderData(width);
				num += paragraphBase.VisualHeight;
				paragraphNode = paragraphNode.NextNode;
			}
			return num;
		}

		public List<Rect> GetRunBoundsBySegment(ISegment segment)
		{
			return RenderManager.GetBoundsBySegment(segment);
		}

		public void ForceRedraw()
		{
			RenderManager.ForceRedraw();
		}

		public CaretOffset GetCaretOffsetByPosition(Point controlPosition)
		{
			return RenderManager.GetCaretOffsetByPosition(controlPosition);
		}

		public Rect GetRunBoundsByCaretOffset(CaretOffset caretOffset)
		{
			return RenderManager.GetRunBoundsByCaretOffset(caretOffset);
		}

		public Rect GetRunBoundsByDocumentOffset(int documentOffset)
		{
			return RenderManager.GetRunBoundsByCaretOffset(new CaretOffset
			{
				Offset = documentOffset
			});
		}

		[Obsolete("请使用 GetLineVisualBounds 代替，此方法直接调用 GetLineVisualBounds 方法")]
		public Rect GetLineVisualBoundsByDocumentOffset(int documentOffset)
		{
			return GetLineVisualBounds(new DocumentOffset(documentOffset));
		}

		public Rect GetLineVisualBounds(DocumentOffset documentOffset)
		{
			if (DebuggingProperties.IsDebug && ParagraphCount != RenderManager.Paragraphs.Count)
			{
				throw new InstanceNotReadyException("不能在文本完成渲染之前调用获取文本行信息，请尝试通过 Dispatcher 延迟到 Background 再调用此方法或通过调用ForceRedraw直接刷新");
			}
			ParagraphNode paragraphNodeByDocumentOffset = GetParagraphNodeByDocumentOffset(documentOffset);
			ParagraphBase paragraphBase = RenderManager.Paragraphs[paragraphNodeByDocumentOffset.ParagraphNumber - 1];
			LineRenderData lineRenderDataByDocumentOffset = paragraphBase.GetLineRenderDataByDocumentOffset(documentOffset);
			double num = paragraphBase.VisualTop;
			double x = (lineRenderDataByDocumentOffset.Equals(paragraphBase.LineRenderData.First()) ? paragraphBase.ParagraphTextBeforeData.FirstLineTextIndent : paragraphBase.ParagraphTextBeforeData.NotFirstLineTextIndent);
			foreach (LineRenderData lineRenderDatum in paragraphBase.LineRenderData)
			{
				if (!lineRenderDatum.Equals(lineRenderDataByDocumentOffset))
				{
					num += lineRenderDatum.Height;
					continue;
				}
				break;
			}
			return new Rect(x, num, lineRenderDataByDocumentOffset.Width, lineRenderDataByDocumentOffset.Height);
		}

		public void SetCaretOffsetToDocumentStart()
		{
			DocumentManager.CaretInfo.CurrentCaretOffset = CaretOffset.Default;
		}

		public void SetCaretOffsetToDocumentEnd()
		{
			CaretOffset caretOffset = new CaretOffset
			{
				Offset = CharCount - 1
			};
			caretOffset = TextHelperInner.UpdateCaretOffsetIsAtParagraphStart(DocumentManager, caretOffset);
			DocumentManager.CaretInfo.CurrentCaretOffset = caretOffset;
		}

		public void SetCaretOffsetToPosition(Point position)
		{
			CaretOffset caretOffsetByPosition = RenderManager.GetCaretOffsetByPosition(position);
			DocumentManager.CaretInfo.CurrentCaretOffset = caretOffsetByPosition;
		}

		[Obsolete("请使用重载 GetRunProperty(DocumentOffset documentOffset) 代替")]
		public RunProperty GetRunProperty(int documentOffset)
		{
			return GetRunProperty(new DocumentOffset(documentOffset));
		}

		public RunProperty GetRunProperty(DocumentOffset documentOffset)
		{
			return DocumentManager.PropertyManager.GetRunProperty(documentOffset).Clone();
		}

		public bool ParagraphPropertiesAll(Func<ParagraphProperty, bool> func)
		{
			return DocumentManager.PropertyManager.ParagraphProperties.All((int x) => func(DocumentManager.PropertyManager.ParagraphPropDic[x]));
		}

		public bool ParagraphPropertiesAny(Func<ParagraphProperty, bool> func)
		{
			return DocumentManager.PropertyManager.ParagraphProperties.Any((int x) => func(DocumentManager.PropertyManager.ParagraphPropDic[x]));
		}

		public void SetTextEditorProperty([NotNull] TextEditorProperty property)
		{
			ApplyTextEditorPropertyHelper.SetTextEditorProperty(property, this);
		}

		public TextEditorProperty GetTextEditorProperty()
		{
			TextEditorProperty textEditorProperty = new TextEditorProperty();
			if (!IsEditable)
			{
				textEditorProperty.RunProperty = DocumentManager.PropertyManager.GetRunProperty(0).Clone();
				textEditorProperty.ParagraphProperty = DocumentManager.PropertyManager.GetParagraphProperty(0).Clone();
				textEditorProperty.TextVerticalAlignment = DocumentManager.TextVerticalAlignment;
				textEditorProperty.ArrangingType = DocumentManager.ArrangingType;
			}
			else if (DocumentManager.Selection.IsEmpty)
			{
				textEditorProperty.RunProperty = DocumentManager.PropertyManager.CurrentRunProperty.Clone();
				textEditorProperty.ParagraphProperty = DocumentManager.PropertyManager.CurrentParagraphProperty.Clone();
				textEditorProperty.TextVerticalAlignment = TextVerticalAlignment;
				textEditorProperty.ArrangingType = DocumentManager.ArrangingType;
			}
			else
			{
				textEditorProperty.RunProperty = DocumentManager.PropertyManager.GetRunProperty(DocumentManager.Selection.FrontOffset.Offset + 1).Clone();
				textEditorProperty.ParagraphProperty = DocumentManager.PropertyManager.GetParagraphProperty(RenderManager.GetParagraphByCaretOffset(DocumentManager.Selection.FrontOffset).ParagraphNode.ParagraphNumber - 1).Clone();
				textEditorProperty.TextVerticalAlignment = TextVerticalAlignment;
				textEditorProperty.ArrangingType = DocumentManager.ArrangingType;
			}
			return textEditorProperty;
		}

		public void ScaleFontSize(double ratio)
		{
			DocumentManager.PropertyManager.ParagraphPropDic.Values.ForEach((ParagraphProperty x) =>
			{
				x.DefaultRunProperty.FontSize *= ratio;
			});
			DocumentManager.PropertyManager.RunPropDic.Values.ForEach((RunProperty x) =>
			{
				x.FontSize *= ratio;
			});
			CurrentRunProperty.FontSize *= ratio;
			CurrentParagraphProperty.DefaultRunProperty.FontSize *= ratio;
			RenderManager.Redraw();
		}

		public new void Focus()
		{
			TextArea.Focus();
		}

		public void SelectAll()
		{
			DocumentManager.SelectAll();
		}

		public void ClearSelection()
		{
			DocumentManager.ClearSelection();
		}

		[NotNull]
		public string GetText(int offset, int length)
		{
			if (IsExtCharEditSupported)
			{
				throw new InvalidOperationException("Call 'GetChars' method please");
			}
			return DocumentManager.CharsManager.GetText(offset, length);
		}

		public IEnumerable<ICharObject> GetChars(ISegment segment)
		{
			return GetChars(segment.FrontOffset, segment.Length);
		}

		public IEnumerable<ICharObject> GetChars(int offset, int length)
		{
			return DocumentManager.CharsManager.GetChars(offset, length);
		}

		public string GetText(ISegment segment)
		{
			return GetText(segment.FrontOffset, segment.Length);
		}

		public void SetDefaultPasteType(PasteType pasteType)
		{
			DocumentManager.DefaultPasteType = pasteType;
		}

		public void RemoveTextWithUndoRedo([NotNull] ISegment segment)
		{
			EditTextWithUndoRedo(segment, string.Empty);
		}

		public void EditTextWithUndoRedo([NotNull] ISegment segment, [NotNull] string text)
		{
			if (segment == null || text == null)
			{
				throw new ArgumentNullException();
			}
			DocumentManager.EditChars(segment, TextUtilities.GetCharsFromText(text));
		}

		public void EditTextWithUndoRedo([NotNull] string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			DocumentManager.EditChars(TextUtilities.GetCharsFromText(text));
		}

		public void EditCharWithUndoRedo([NotNull] ISegment segment, [NotNull] ICharObject charObject)
		{
			if (segment == null || charObject == null)
			{
				throw new ArgumentNullException();
			}
			DocumentManager.EditChar(segment, charObject);
		}

		public void EditCharWithUndoRedo([NotNull] ICharObject charObject)
		{
			if (charObject == null)
			{
				throw new ArgumentNullException("charObject");
			}
			DocumentManager.EditChar(charObject);
		}

		public void EditRichTextPropertyWithUndoRedo([NotNull] RichTextProperty richTextProperty)
		{
			if (richTextProperty == null)
			{
				throw new ArgumentNullException("richTextProperty");
			}
			if (richTextProperty.RunProperties.Any())
			{
				DocumentManager.ReplaceSelectionWithRichTextProperty(Selection.ToSegment(), richTextProperty);
			}
			else
			{
				DocumentManager.EditChars(Selection.ToSegment(), richTextProperty.CharObjects);
			}
		}

		public void SetChars(List<ICharObject> chars)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars", "chars can not be null");
			}
			List<ICharObject> charObjects = chars.Select((ICharObject x) => (ICharObject)x.Clone()).ToList();
			bool isUndoRedoEnabled = IsUndoRedoEnabled;
			try
			{
				IsUndoRedoEnabled = false;
				DocumentManager.EditChars(new SimpleSegment(0, DocumentManager.CharsManager.CharCount), charObjects);
				DocumentManager.CaretInfo.CurrentCaretOffset = CaretNavigationHelper.GetDocumentEndCaretOffset(TextArea);
			}
			finally
			{
				IsUndoRedoEnabled = isUndoRedoEnabled;
			}
		}

		[Obsolete("请使用重载的GetParagraphNodeByDocumentOffset(DocumentOffset offset)代替")]
		public ParagraphNode GetParagraphNodeByDocumentOffset(int offset)
		{
			return DocumentManager.CharsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(offset);
		}

		public ParagraphNode GetParagraphNodeByDocumentOffset(DocumentOffset offset)
		{
			return DocumentManager.CharsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(offset);
		}

		public ParagraphNode GetParagraphNodeByNumber(int paragraphNumber)
		{
			if (DebuggingProperties.IsDebug && paragraphNumber < 1)
			{
				throw new ArgumentException("段落编号从 1 开始", "paragraphNumber");
			}
			return DocumentManager.CharsManager.ParagraphTree.GetParagraphNodeByNumber(paragraphNumber);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("请使用 GetLineProperty 代替，此方法直接调用 GetLineProperty 方法")]
		public LineProperty GetLinePropertyByDocumentOffset(int documentOffset)
		{
			return GetLineProperty(new DocumentOffset(documentOffset));
		}

		public LineProperty GetLineProperty(DocumentOffset documentOffset)
		{
			ParagraphNode paragraphNodeByDocumentOffset = GetParagraphNodeByDocumentOffset(documentOffset);
			ParagraphBase paragraphBase = RenderManager.Paragraphs[paragraphNodeByDocumentOffset.ParagraphNumber - 1];
			if (!paragraphBase.ParagraphLineInfoRenderedData.Any())
			{
				throw new InvalidOperationException("you have to render Text before get lineproperty");
			}
			return paragraphBase.GetLinePropertyByDocumentOffset(documentOffset);
		}

		[Obsolete("请使用 GetDocumentOffsetFromRunBoundsPosition 代替，此方法内部直接调用 GetDocumentOffsetFromRunBoundsPosition 方法")]
		public int GetDocumentOffsetByPositionFromRunBounds(Point position)
		{
			return GetDocumentOffsetFromRunBoundsPosition(position);
		}

		public DocumentOffset GetDocumentOffsetFromRunBoundsPosition(Point position)
		{
			return RenderManager.GetDocumentOffsetByPositionFromRunBounds(position);
		}

		public List<Rect> GetExtCharBounds<T>()
		{
			List<Rect> list = new List<Rect>();
			for (int i = 0; i < CharCount; i++)
			{
				if (DocumentManager.CharsManager.GetCharAt(i) is T)
				{
					Rect runBoundsByDocumentOffset = GetRunBoundsByDocumentOffset(i);
					list.Add(runBoundsByDocumentOffset);
				}
			}
			return list;
		}

		public List<int> GetExtCharIndexes<T>()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < CharCount; i++)
			{
				if (DocumentManager.CharsManager.GetCharAt(i) is T)
				{
					list.Add(i);
				}
			}
			return list;
		}

		public void SetDefaultIndentType(IndentType indentType)
		{
			DocumentManager.DefaultIndentType = indentType;
		}

		public void SetTextWrapContinuousPhraseRegexPattern([NotNull] string regexPattern)
		{
			if (string.IsNullOrEmpty(regexPattern))
			{
				throw new ArgumentNullException("regexPattern");
			}
			DocumentManager.CharsManager.TextWrapContinuousPhraseRegexPattern = regexPattern;
		}

		public void AddContextMenuItem(MenuItem item)
		{
			TextArea.AddContextMenuItem(item);
		}

		public void UpdateCacheMode(CacheMode cacheMode)
		{
			CacheMode = cacheMode;
			_textView.UpdateCacheMode(cacheMode);
		}

		public CaretOffsetTransaction GetCaretOffsetTransaction()
		{
			return new CaretOffsetTransaction(this, CaretInfo.CurrentCaretOffset);
		}

		public CaretOffsetOperation GetCaretOffsetOperation(Action action)
		{
			CaretOffset currentCaretOffset = CaretInfo.CurrentCaretOffset;
			action();
			CaretOffset currentCaretOffset2 = CaretInfo.CurrentCaretOffset;
			return new CaretOffsetOperation(this, currentCaretOffset, currentCaretOffset2);
		}

		private void RaiseDocumentSizeChangedEvent(Size newValue, Size oldValue)
		{
			RaiseEvent(new ValueChangedRoutedEventArgs<Size>(DocumentSizeChangedEvent, oldValue, newValue, this));
		}

		private void RaiseDocumentOutlineBoundsChangedEvent(Rect newValue, Rect oldValue)
		{
			RaiseEvent(new DocumentOutlineBoundsChangedRoutedEventArgs(DocumentOutlineBoundsChangedEvent, oldValue, newValue, this));
		}

		private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TextEditor)d).ChangeTextInner(e.NewValue?.ToString());
		}

		private static void OnTextWrappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextWrapping textWrapping = (TextWrapping)e.NewValue;
			TextEditor textEditor = (TextEditor)d;
			textEditor.DocumentManager.PropertyManager.TextWrapping = textWrapping;
			textEditor.RenderManager.Redraw();
		}

		private static void OnTextVerticalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TextEditor)d).ChangeTextVerticalAlignmentInner((VerticalAlignment)e.NewValue);
		}

		private static void OnTextHorizontalAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextAlignment newValue = (TextAlignment)e.NewValue;
			((TextEditor)d).DocumentManager.ChangeProperty((ParagraphProperty p) =>
			{
				p.TextAlignment = newValue;
			});
		}

		private static void OnLineHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextEditor textEditor = (TextEditor)d;
			double lineSpace;
			if (double.TryParse(e.NewValue.ToString(), out lineSpace) && textEditor.DocumentManager.CurrentFeature.HasFeature(Features.SetLineSpaceInner))
			{
				textEditor.DocumentManager.ChangeProperty((ParagraphProperty p) =>
				{
					p.LineSpacing = lineSpace;
					p.FixedLineSpacing = double.NaN;
				});
			}
		}

		private static void OnLineSpacingStrategyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextEditor textEditor = (TextEditor)d;
			LineSpacingStrategy result;
			if (Enum.TryParse<LineSpacingStrategy>(e.NewValue.ToString(), out result) && textEditor.DocumentManager.CurrentFeature.HasFeature(Features.SetLineSpacingStrategyInner))
			{
				textEditor.DocumentManager.PropertyManager.LineSpacingStrategy = result;
			}
		}

		private static void OnAutoCaretBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TextEditor)d).DocumentManager.CaretInfo.IsAutoBrush = (bool)e.NewValue;
		}

		private static void OnIsUndoRedoEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TextEditor)d).DocumentManager.IsUndoRedoEnabled = (bool)e.NewValue;
		}

		private static void OnRenderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextEditor textEditor = d as TextEditor;
			if (textEditor == null)
			{
				return;
			}
			if (e.Property == FontFamilyProperty)
			{
				FontFamily value = (FontFamily)textEditor.GetValue(FontFamilyProperty);
				textEditor.DocumentManager.ChangeProperty((RunProperty p) =>
				{
					p.FontFamily = value;
				});
			}
			else if (e.Property == FontSizeProperty)
			{
				double value2 = (double)textEditor.GetValue(FontSizeProperty);
				textEditor.DocumentManager.ChangeProperty((RunProperty p) =>
				{
					p.FontSize = value2;
				});
			}
			else if (e.Property == OpacityProperty)
			{
				double value3 = (double)textEditor.GetValue(OpacityProperty);
				textEditor.DocumentManager.ChangeProperty((RunProperty p) =>
				{
					p.Opacity = value3;
				});
			}
			else if (e.Property == FontStyleProperty)
			{
				FontStyle value4 = (FontStyle)textEditor.GetValue(FontStyleProperty);
				textEditor.DocumentManager.ChangeProperty((RunProperty p) =>
				{
					p.FontStyle = value4;
				});
			}
			else if (e.Property == FontWeightProperty)
			{
				FontWeight value5 = (FontWeight)textEditor.GetValue(FontWeightProperty);
				textEditor.DocumentManager.ChangeProperty((RunProperty p) =>
				{
					p.FontWeight = value5;
				});
			}
			else if (e.Property == ForegroundProperty)
			{
				Brush value6 = (Brush)textEditor.GetValue(ForegroundProperty);
				textEditor.DocumentManager.ChangeProperty((RunProperty p) =>
				{
					p.Foreground = value6;
				});
			}
			else if (e.Property != FontStretchProperty && e.Property == BaselineAlignmentProperty)
			{
				BaselineAlignment value7 = (BaselineAlignment)textEditor.GetValue(BaselineAlignmentProperty);
				textEditor.DocumentManager.ChangeProperty((RunProperty p) =>
				{
					p.BaselineAlignment = value7;
				});
			}
			textEditor.RenderManager.Redraw();
		}

		private static void OnSizeToContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TextEditor)d).DocumentManager.SizeToContent = (SizeToContent)e.NewValue;
		}

		private static void OnAcceptingTypeRegexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DocumentManager documentManager = ((TextEditor)d).DocumentManager;
			string text = e.NewValue?.ToString();
			if (text == null)
			{
				throw new ArgumentNullException("NewValue", "AcceptingTypeRegex can not be null");
			}
			documentManager.AcceptingTypeRegexString = text;
		}

		private static void OnAcceptingInputTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TextEditor)d).DocumentManager.AcceptingInputType = (AcceptingInputTypes)e.NewValue;
		}

		private static void OnIsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TextEditor)d).ChangeIsEditableInner((bool)e.NewValue);
		}

		private static void TextEditor_OnArrangingTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TextEditor)d).DocumentManager.ArrangingType = (ArrangingType)e.NewValue;
		}

		private static void OnMaxLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TextEditor)d).DocumentManager.MaxLength = (int)e.NewValue;
		}

		private static void OnIsExtCharEditSupportedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TextEditor)d).DocumentManager.IsExtCharEditSupported = (bool)e.NewValue;
		}

		private void ChangeTextVerticalAlignmentInner(VerticalAlignment value)
		{
			VerticalAlignment textVerticalAlignment = DocumentManager.TextVerticalAlignment;
			DocumentManager.TextVerticalAlignment = value;
			RenderManager.Redraw();
			if (value != textVerticalAlignment)
			{
				RaiseEvent(new ValueChangedRoutedEventArgs<VerticalAlignment>(TextVerticalAlignmentChangedEvent, textVerticalAlignment, value));
			}
		}

		private void ChangeTextInner(string text)
		{
			if (text == null)
			{
				if (DebuggingProperties.IsDebug)
				{
					throw new ArgumentNullException("text", "text can not be null 仅调试下抛出，请找德熙");
				}
				text = string.Empty;
			}
			if (DocumentManager.CharsManager.Chars.All((ICharObject x) => x is TextChar) && text == DocumentManager.CharsManager.GetText(0, CharCount))
			{
				return;
			}
			bool isUndoRedoEnabled = IsUndoRedoEnabled;
			try
			{
				IsUndoRedoEnabled = false;
				EditTextWithUndoRedo(new SimpleSegment(0, DocumentManager.CharsManager.CharCount), text);
				DocumentManager.CaretInfo.CurrentCaretOffset = CaretNavigationHelper.GetDocumentEndCaretOffset(TextArea);
			}
			finally
			{
				IsUndoRedoEnabled = isUndoRedoEnabled;
			}
		}

		private void ChangeIsEditableInner(bool value)
		{
			DocumentManager.IsEditable = value;
			if (value)
			{
				EnterEdit();
			}
			else
			{
				QuitEdit();
			}
		}

		private void TextArea_OnAddHyperlink(object sender, EventArgs e)
		{
			RaiseEvent(new RoutedEventArgs(AddHyperlinkEvent));
		}

		private void TextArea_OnEditHyperlink(object sender, InternalHyperlinkEventArgs eventArgs)
		{
			RaiseEvent(new HyperlinkRoutedEventArgs(eventArgs.Hyperlink, EditHyperlinkEvent));
		}

		private void TextArea_OnRaiseHyperlink(object sender, InternalHyperlinkEventArgs eventArgs)
		{
			RaiseEvent(new HyperlinkRoutedEventArgs(eventArgs.Hyperlink, RaiseHyperlinkEvent));
		}

		public FeedbackProperty GetFeedbackProperty()
		{
			FeedbackProperty feedbackProperty = new FeedbackProperty();
			if (RenderManager.Paragraphs.Count == 0)
			{
				return feedbackProperty;
			}
			if (DocumentManager.IsEditable && DocumentManager.Selection.IsEmpty)
			{
				feedbackProperty.SetParagraphProperty(DocumentManager.PropertyManager.CurrentParagraphProperty);
				feedbackProperty.SetRunProperty(DocumentManager.PropertyManager.CurrentRunProperty);
			}
			else
			{
				GetRunPropertyFeedback(feedbackProperty);
				GetParagraphPropertyFeedback(feedbackProperty);
			}
			feedbackProperty.TextVerticalAlignment = DocumentManager.TextVerticalAlignment.ToTextVerticalAlignment();
			feedbackProperty.ArrangingType = DocumentManager.ArrangingType;
			return feedbackProperty;
		}

		
		private ValueTuple<int, int> GetParagraphFrontOffset()
		{
			if (DocumentManager.IsEditable)
			{
				List<int> paragraphIndexBySegment = TextHelperInner.GetParagraphIndexBySegment(DocumentManager.CharsManager, DocumentManager.Selection.ToSegment());
				int item = paragraphIndexBySegment.First();
				int item2 = paragraphIndexBySegment.Last();
				return new ValueTuple<int, int>(item, item2);
			}
			int item3 = DocumentManager.CharsManager.ParagraphTree.ParagraphCount - 1;
			return new ValueTuple<int, int>(0, item3);
		}

		
		private ValueTuple<int, int> GetRunOffset()
		{
			if (DocumentManager.IsEditable)
			{
				ISelection selection = DocumentManager.Selection;
				return new ValueTuple<int, int>(selection.FrontOffset.Offset, selection.BehindOffset.Offset);
			}
			return new ValueTuple<int, int>(-1, DocumentManager.CharsManager.CharCount - 1);
		}

		private void GetParagraphPropertyFeedback(FeedbackProperty feedbackProperty)
		{
			ValueTuple<int, int> paragraphFrontOffset = GetParagraphFrontOffset();
			int item = paragraphFrontOffset.Item1;
			int item2 = paragraphFrontOffset.Item2;
			IPropertyFeedbackReader[] array = new IPropertyFeedbackReader[7]
			{
				GetPropertyFeedbackReader<LineSpacingPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<TextAlignmentPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<TextMarkerStylePropertyFeedbackReader>(),
				GetPropertyFeedbackReader<ParagraphBeforeStylePropertyFeedbackReader>(),
				GetPropertyFeedbackReader<ParagraphAfterStylePropertyFeedbackReader>(),
				GetPropertyFeedbackReader<ParagraphIndentLevelPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<ParagraphDirectionPropertyFeedbackReader>()
			};
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetSelectionPropertyToFeedbackProperty(this, item, item2, feedbackProperty);
			}
		}

		private void GetRunPropertyFeedback(FeedbackProperty feedbackProperty)
		{
			ValueTuple<int, int> runOffset = GetRunOffset();
			int item = runOffset.Item1;
			int item2 = runOffset.Item2;
			IPropertyFeedbackReader[] array = new IPropertyFeedbackReader[15]
			{
				GetPropertyFeedbackReader<FontWeightPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<FontFamilyPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<FontSizePropertyFeedbackReader>(),
				GetPropertyFeedbackReader<FontVariantsPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<UnderlinePropertyFeedbackReader>(),
				GetPropertyFeedbackReader<FontStylePropertyFeedbackReader>(),
				GetPropertyFeedbackReader<ForegroundPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<TextEffectsPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<ShadowAnglePropertyFeedbackReader>(),
				GetPropertyFeedbackReader<ShadowBlurPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<ShadowOffsetPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<ShadowOpacityPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<ReflectionOpacityPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<ShadowBrushPropertyFeedbackReader>(),
				GetPropertyFeedbackReader<OpacityPropertyFeedbackReader>()
			};
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetSelectionPropertyToFeedbackProperty(this, item, item2, feedbackProperty);
			}
		}

		private static T GetPropertyFeedbackReader<T>() where T : IPropertyFeedbackReader, new()
		{
			Type typeFromHandle = typeof(T);
			IPropertyFeedbackReader value;
			if (PropertyFeedbackReaderCache.TryGetValue(typeFromHandle, out value))
			{
				return (T)value;
			}
			value = new T();
			PropertyFeedbackReaderCache[typeFromHandle] = value;
			return (T)value;
		}
	}
}
