using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input.StylusPlugIns;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;
using Cvte.Linq;
using Cvte.Paint.Framework.Input;
using Cvte.Paint.Framework.Operations;
using Cvte.Paint.Framework.Settings;
using Cvte.Storage;
using Cvte.Windows.Media;

namespace Cvte.Paint.Framework
{
	[DebuggerDisplay("Board [{Name,nq}] [{GetHashCode(),nq}]")]
	public sealed class Board : Control, ISavable<BoardSaveInfo>, ISavable, INotifyPropertyChanged, IExtensible<Board>
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass52_0
		{
			public Board _003C_003E4__this;

			public Slide newSlide;

			public Slide oldSlide;

			public Slide value;

			internal void _003Cset_CurrentSlide_003Eg__ReloadSlide_007C0()
			{
				_003C_003E4__this.ReloadSlide(newSlide);
			}

			internal void _003Cset_CurrentSlide_003Eg__RaiseCurrentSlideChangingEvent_007C1()
			{
				_003C_003E4__this.RaiseEvent(new RoutedPropertyChangedEventArgs<Slide>(oldSlide, newSlide, CurrentSlideChangingEvent));
			}

			internal void _003Cset_CurrentSlide_003Eg__CloseOldMode_007C2()
			{
				if (oldSlide != null)
				{
					try
					{
						_003C_003E4__this._modeController.SwitchOff();
						_003C_003E4__this._modeController.SwitchOffFilters();
					}
					finally
					{
						oldSlide.Unselect();
					}
				}
			}

			internal void _003Cset_CurrentSlide_003Eg__UpdateCurrentSlide_007C3()
			{
				_003C_003E4__this._currentSlide = value;
			}

			internal void _003Cset_CurrentSlide_003Eg__OpenMode_007C4()
			{
				if (newSlide != null)
				{
					_003C_003E4__this._modeController.Container = newSlide;
					_003C_003E4__this._modeController.SwitchOnFilters(newSlide);
					_003C_003E4__this._modeController.SwitchOn();
				}
			}

			internal void _003Cset_CurrentSlide_003Eg__RaiseCurrentSlideChangedEvent_007C5()
			{
				_003C_003E4__this.OnPropertyChanged("CurrentSlide");
				_003C_003E4__this._slideList.OnPropertyChanged("CurrentIndex");
				_003C_003E4__this.RaiseEvent(new RoutedPropertyChangedEventArgs<Slide>(oldSlide, newSlide, CurrentSlideChangedEvent));
			}
		}

		public static readonly DependencyProperty UndoRedoProviderProperty;

		public static readonly DependencyProperty ContentScaleProperty;

		public static readonly DependencyProperty ContentScaleXProperty;

		public static readonly DependencyProperty ContentScaleYProperty;

		public static readonly DependencyProperty ContentOffsetProperty;

		public static readonly DependencyProperty ContentFittingProperty;

		private static readonly DependencyPropertyKey RenderSourcePropertyKey;

		public static readonly DependencyProperty RenderSourceProperty;

		public static readonly DependencyProperty SlideSizeProperty;

		private Mode _currentMode;

		private BoardEventSource _eventSource;

		private Slide _currentSlide;

		private readonly SlideCollection _slideList;

		private readonly ModeController _modeController;

		private SlideViewbox _slideViewbox;

		private ContentPresenter _contentHost;

		private Grid _utilitiesGrid;

		private readonly List<Action> _lazyBoardLoadedActionList = new List<Action>();

		private Action _switchModeAction;

		private const string InvalidPageExceptionString = "不包含指定页面。";

		public static readonly RoutedEvent SlidesSortingEvent;

		public static readonly RoutedEvent SlidesSortedEvent;

		public static readonly RoutedEvent CurrentSlideChangingEvent;

		public static readonly RoutedEvent CurrentSlideChangedEvent;

		public static readonly RoutedEvent SlidesInsertingEvent;

		public static readonly RoutedEvent SlidesInsertedEvent;

		public static readonly RoutedEvent SlidesDeletingEvent;

		public static readonly RoutedEvent SlidesDeletedEvent;

		public static readonly RoutedEvent BoardClearingEvent;

		public static readonly RoutedEvent BoardClearedEvent;

		public static readonly RoutedEvent BoardResetingEvent;

		public static readonly RoutedEvent BoardResetedEvent;

		public static readonly RoutedEvent ModeChangingEvent;

		public static readonly RoutedEvent ModeChangedEvent;

		public static readonly RoutedEvent ElementClipGeometryChangedEvent;

		public static readonly RoutedEvent UndoRedoEnableChangedEvent;

		public static readonly RoutedEvent EraserSlippingClearEvent;

		public IUndoRedoProvider UndoRedoProvider
		{
			get
			{
				return (IUndoRedoProvider)GetValue(UndoRedoProviderProperty);
			}
			set
			{
				SetValue(UndoRedoProviderProperty, value);
			}
		}

		public double ContentScale
		{
			get
			{
				return (double)GetValue(ContentScaleProperty);
			}
			set
			{
				SetValue(ContentScaleProperty, value);
			}
		}

		public double ContentScaleX
		{
			get
			{
				return (double)GetValue(ContentScaleXProperty);
			}
			set
			{
				SetValue(ContentScaleXProperty, value);
			}
		}

		public double ContentScaleY
		{
			get
			{
				return (double)GetValue(ContentScaleYProperty);
			}
			set
			{
				SetValue(ContentScaleYProperty, value);
			}
		}

		public Vector ContentOffset
		{
			get
			{
				return (Vector)GetValue(ContentOffsetProperty);
			}
			set
			{
				SetValue(ContentOffsetProperty, value);
			}
		}

		public FittingMode ContentFitting
		{
			get
			{
				return (FittingMode)GetValue(ContentFittingProperty);
			}
			set
			{
				SetValue(ContentFittingProperty, value);
			}
		}

		[Obsolete("命名错了，请使用 DefaultSlideSize 属性")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Size DefalutSlideSize => DefaultSlideSize;

		public static Size DefaultSlideSize => new Size(1280.0, 720.0);

		public Size SlideSize
		{
			get
			{
				return (Size)GetValue(SlideSizeProperty);
			}
			set
			{
				SetValue(SlideSizeProperty, value);
			}
		}

		public BoardSettingsProvider BoardSettings => this.Settings();

		public SlideCollection Slides => _slideList;

		public Slide CurrentSlide
		{
			get
			{
				return _currentSlide;
			}
			private set
			{
				_003C_003Ec__DisplayClass52_0 obj = new _003C_003Ec__DisplayClass52_0();
				obj._003C_003E4__this = this;
				obj.value = value;
				obj.oldSlide = _currentSlide;
				obj.newSlide = obj.value;
				if (!object.Equals(obj.oldSlide, obj.newSlide))
				{
					PaintAggregateExceptionBuilder.DoActionsWithException("切换页面时发生了异常，异常的详细信息如下：", obj._003Cset_CurrentSlide_003Eg__ReloadSlide_007C0, obj._003Cset_CurrentSlide_003Eg__RaiseCurrentSlideChangingEvent_007C1, obj._003Cset_CurrentSlide_003Eg__CloseOldMode_007C2, obj._003Cset_CurrentSlide_003Eg__UpdateCurrentSlide_007C3, UpdateCurrentSlideValue, obj._003Cset_CurrentSlide_003Eg__OpenMode_007C4, obj._003Cset_CurrentSlide_003Eg__RaiseCurrentSlideChangedEvent_007C5);
				}
			}
		}

		public Mode Mode
		{
			get
			{
				return _currentMode ?? _modeController.CurrentMode;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Mode 不能被设置为 null，请使用 NoneMode 替代。");
				}
				if (object.Equals(_currentMode, value))
				{
					return;
				}
				if (_switchModeAction != null)
				{
					_lazyBoardLoadedActionList.Remove(_switchModeAction);
					_switchModeAction = null;
				}
				if (IsLoaded)
				{
					_modeController.CurrentMode = value;
					_currentMode = null;
					return;
				}
				_currentMode = value;
				_switchModeAction = () =>
				{
					_modeController.CurrentMode = value;
					_currentMode = null;
				};
				_lazyBoardLoadedActionList.Add(_switchModeAction);
			}
		}

		public ModeController Controller => _modeController;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public SlideViewbox Viewbox => _slideViewbox;

		private static PaintInitializationManager PaintInitializationManager => PaintInitializationManager.Instance;

		protected override IEnumerator LogicalChildren => _slideList.GetEnumerator();

		public ExtensionCollection<Board> Extensions { get; }

		public event PropertyChangedEventHandler PropertyChanged;

		public event RoutedEventHandler SlidesSorting
		{
			add
			{
				AddHandler(SlidesSortingEvent, value);
			}
			remove
			{
				RemoveHandler(SlidesSortingEvent, value);
			}
		}

		public event RoutedEventHandler SlidesSorted
		{
			add
			{
				AddHandler(SlidesSortedEvent, value);
			}
			remove
			{
				RemoveHandler(SlidesSortedEvent, value);
			}
		}

		public event RoutedPropertyChangedEventHandler<Slide> CurrentSlideChanging
		{
			add
			{
				AddHandler(CurrentSlideChangingEvent, value);
			}
			remove
			{
				RemoveHandler(CurrentSlideChangingEvent, value);
			}
		}

		public event RoutedPropertyChangedEventHandler<Slide> CurrentSlideChanged
		{
			add
			{
				AddHandler(CurrentSlideChangedEvent, value);
			}
			remove
			{
				RemoveHandler(CurrentSlideChangedEvent, value);
			}
		}

		public event EventHandler<SlidesRoutedEventArgs> SlidesInserting
		{
			add
			{
				AddHandler(SlidesInsertingEvent, value);
			}
			remove
			{
				RemoveHandler(SlidesInsertingEvent, value);
			}
		}

		public event EventHandler<SlidesRoutedEventArgs> SlidesInserted
		{
			add
			{
				AddHandler(SlidesInsertedEvent, value);
			}
			remove
			{
				RemoveHandler(SlidesInsertedEvent, value);
			}
		}

		public event EventHandler<SlidesRoutedEventArgs> SlidesDeleting
		{
			add
			{
				AddHandler(SlidesDeletingEvent, value);
			}
			remove
			{
				RemoveHandler(SlidesDeletingEvent, value);
			}
		}

		public event EventHandler<SlidesRoutedEventArgs> SlidesDeleted
		{
			add
			{
				AddHandler(SlidesDeletedEvent, value);
			}
			remove
			{
				RemoveHandler(SlidesDeletedEvent, value);
			}
		}

		public event RoutedEventHandler BoardClearing
		{
			add
			{
				AddHandler(BoardClearingEvent, value);
			}
			remove
			{
				RemoveHandler(BoardClearingEvent, value);
			}
		}

		public event RoutedEventHandler BoardCleared
		{
			add
			{
				AddHandler(BoardClearedEvent, value);
			}
			remove
			{
				RemoveHandler(BoardClearedEvent, value);
			}
		}

		public event RoutedEventHandler BoardReseting
		{
			add
			{
				AddHandler(BoardResetingEvent, value);
			}
			remove
			{
				RemoveHandler(BoardResetingEvent, value);
			}
		}

		public event RoutedEventHandler BoardReseted
		{
			add
			{
				AddHandler(BoardResetedEvent, value);
			}
			remove
			{
				RemoveHandler(BoardResetedEvent, value);
			}
		}

		public event RoutedPropertyChangedEventHandler<Mode> ModeChanging
		{
			add
			{
				AddHandler(ModeChangingEvent, value);
			}
			remove
			{
				RemoveHandler(ModeChangingEvent, value);
			}
		}

		public event RoutedPropertyChangedEventHandler<Mode> ModeChanged
		{
			add
			{
				AddHandler(ModeChangedEvent, value);
			}
			remove
			{
				RemoveHandler(ModeChangedEvent, value);
			}
		}

		public event EventHandler<ElementClipGeometryChangedEventArgs> ElementClipGeometryChanged
		{
			add
			{
				AddHandler(ElementClipGeometryChangedEvent, value);
			}
			remove
			{
				RemoveHandler(ElementClipGeometryChangedEvent, value);
			}
		}

		public event RoutedEventHandler UndoRedoEnableChanged
		{
			add
			{
				AddHandler(UndoRedoEnableChangedEvent, value);
			}
			remove
			{
				RemoveHandler(UndoRedoEnableChangedEvent, value);
			}
		}

		public event EventHandler<RoutedEventArgs> EraserSlippingClear
		{
			add
			{
				AddHandler(EraserSlippingClearEvent, value);
			}
			remove
			{
				RemoveHandler(EraserSlippingClearEvent, value);
			}
		}

		[CanBeNull]
		public static IUndoRedoProvider GetUndoRedoProvider([NotNull] DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return (IUndoRedoProvider)element.GetValue(UndoRedoProviderProperty);
		}

		public static void SetUndoRedoProvider([NotNull] DependencyObject element, IUndoRedoProvider value)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			element.SetValue(UndoRedoProviderProperty, value);
		}

		public static double GetContentScale([NotNull] DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return (double)element.GetValue(ContentScaleProperty);
		}

		public static void SetContentScale([NotNull] DependencyObject element, double value)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			element.SetValue(ContentScaleProperty, value);
		}

		public static double GetContentScaleX(DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return (double)element.GetValue(ContentScaleXProperty);
		}

		public static double GetContentScaleY(DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return (double)element.GetValue(ContentScaleYProperty);
		}

		public static Vector GetContentOffset([NotNull] DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return (Vector)element.GetValue(ContentOffsetProperty);
		}

		public static void SetContentOffset([NotNull] DependencyObject element, Vector value)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			element.SetValue(ContentOffsetProperty, value);
		}

		public static FittingMode GetContentFitting([NotNull] DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return (FittingMode)element.GetValue(ContentFittingProperty);
		}

		public static void SetContentFitting([NotNull] DependencyObject element, FittingMode value)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			element.SetValue(ContentFittingProperty, value);
		}

		public static UIElement GetRenderSource([NotNull] UIElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return ((UIElement)element.GetValue(RenderSourceProperty)) ?? element;
		}

		private static void SlideSizeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs o)
		{
			Size size = (Size)o.NewValue;
			Board board = (Board)d;
			board.Slides.Where((Slide x) => !SpecialSlide.GetKeepIndependentSize(x)).ForEach((Slide s) =>
			{
				s.Width = size.Width;
				s.Height = size.Height;
			});
			board._contentHost?.InvalidateMeasure();
			board._slideViewbox?.InvalidateMeasure();
		}

		static Board()
		{
			UndoRedoProviderProperty = DependencyProperty.RegisterAttached("UndoRedoProvider", typeof(IUndoRedoProvider), typeof(Board), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			ContentScaleProperty = DependencyProperty.RegisterAttached("ContentScale", typeof(double), typeof(Board), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits));
			ContentScaleXProperty = DependencyProperty.RegisterAttached("ContentScaleX", typeof(double), typeof(Board), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits));
			ContentScaleYProperty = DependencyProperty.RegisterAttached("ContentScaleY", typeof(double), typeof(Board), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits));
			ContentOffsetProperty = DependencyProperty.RegisterAttached("ContentOffset", typeof(Vector), typeof(Board), new FrameworkPropertyMetadata(new Vector(0.0, 0.0), FrameworkPropertyMetadataOptions.Inherits));
			ContentFittingProperty = DependencyProperty.RegisterAttached("ContentFitting", typeof(FittingMode), typeof(Board), new FrameworkPropertyMetadata(FittingMode.AutoFit, FrameworkPropertyMetadataOptions.Inherits));
			RenderSourcePropertyKey = DependencyProperty.RegisterAttachedReadOnly("RenderSource", typeof(UIElement), typeof(Board), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			RenderSourceProperty = RenderSourcePropertyKey.DependencyProperty;
			SlideSizeProperty = DependencyProperty.Register("SlideSize", typeof(Size), typeof(Board), new PropertyMetadata(DefaultSlideSize, SlideSizeChangedCallback));
			SlidesSortingEvent = EventManager.RegisterRoutedEvent("SlidesSorting", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Board));
			SlidesSortedEvent = EventManager.RegisterRoutedEvent("SlidesSorted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Board));
			CurrentSlideChangingEvent = EventManager.RegisterRoutedEvent("CurrentSlideChanging", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Slide>), typeof(Board));
			CurrentSlideChangedEvent = EventManager.RegisterRoutedEvent("CurrentSlideChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Slide>), typeof(Board));
			SlidesInsertingEvent = EventManager.RegisterRoutedEvent("SlidesInserting", RoutingStrategy.Bubble, typeof(EventHandler<SlidesRoutedEventArgs>), typeof(Board));
			SlidesInsertedEvent = EventManager.RegisterRoutedEvent("SlidesInserted", RoutingStrategy.Bubble, typeof(EventHandler<SlidesRoutedEventArgs>), typeof(Board));
			SlidesDeletingEvent = EventManager.RegisterRoutedEvent("SlidesDeleting", RoutingStrategy.Bubble, typeof(EventHandler<SlidesRoutedEventArgs>), typeof(Board));
			SlidesDeletedEvent = EventManager.RegisterRoutedEvent("SlidesDeleted", RoutingStrategy.Bubble, typeof(EventHandler<SlidesRoutedEventArgs>), typeof(Board));
			BoardClearingEvent = EventManager.RegisterRoutedEvent("BoardClearing", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Board));
			BoardClearedEvent = EventManager.RegisterRoutedEvent("BoardCleared", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Board));
			BoardResetingEvent = EventManager.RegisterRoutedEvent("BoardReseting", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Board));
			BoardResetedEvent = EventManager.RegisterRoutedEvent("BoardReseted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Board));
			ModeChangingEvent = EventManager.RegisterRoutedEvent("ModeChanging", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Mode>), typeof(Board));
			ModeChangedEvent = EventManager.RegisterRoutedEvent("ModeChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Mode>), typeof(Board));
			ElementClipGeometryChangedEvent = EventManager.RegisterRoutedEvent("ElementClipGeometryChanged", RoutingStrategy.Bubble, typeof(EventHandler<ElementClipGeometryChangedEventArgs>), typeof(Board));
			UndoRedoEnableChangedEvent = EventManager.RegisterRoutedEvent("UndoRedoEnableChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Board));
			EraserSlippingClearEvent = EventManager.RegisterRoutedEvent("EraserSlippingClear", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(Board));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Board), new FrameworkPropertyMetadata(typeof(Board)));
			PaintInitializationManager.Initialzie();
		}

		public Board()
			: this(Enumerable.Empty<Slide>())
		{
		}

		internal Board(IEnumerable<Slide> slideList)
		{
			Extensions = new ExtensionCollection<Board>(this);
			List<Slide> slides = CheckSlideListArgument(slideList);
			_slideList = new SlideCollection(this, AddLogicalChild, RemoveLogicalChild);
			_slideList.AddRange(slides);
			InitUndoRedo();
			InitSetting();
			_modeController = new ModeController(() => CurrentSlide);
			ModeController.SetController(this, _modeController);
			BoardFocusManager.Manage(this);
			RegisterSlidesInserted();
			StylusPlugIns.Add(new DynamicRenderer());
			Mode = _modeController.CurrentMode;
			GoTo(0);
			PaintInitializationManager.Initialzie(this);
			Loaded += (object s, RoutedEventArgs e) =>
			{
				Action[] array = _lazyBoardLoadedActionList.ToArray();
				_lazyBoardLoadedActionList.Clear();
				List<Exception> list = new List<Exception>();
				Action[] array2 = array;
				foreach (Action action in array2)
				{
					try
					{
						action?.Invoke();
					}
					catch (Exception item)
					{
						list.Add(item);
					}
				}
				if (list.Any())
				{
					this.GetSettings<TraceSettings>().Tracer?.Trace(new AggregateException("Board 初始化出现异常", list), null);
				}
			};
		}

		private static List<Slide> CheckSlideListArgument(IEnumerable<Slide> slideList)
		{
			List<Slide> list = (slideList as List<Slide>) ?? slideList.ToList();
			if (!list.Any())
			{
				list.Add(new Slide());
			}
			if (list.Any((Slide slide) => slide == null))
			{
				throw new ArgumentException("指定 Slide 集合中存在 null 值。", "slideList");
			}
			if (list.Distinct().Count() != list.Count)
			{
				throw new ArgumentException("指定 Slide 集合中存重复的对象实例。", "slideList");
			}
			return list;
		}

		private void InitUndoRedo()
		{
			UndoRedoProvider = new BoardUndoRedoProvider(this);
			UndoRedoProvider.UndoRedoEnableChanged += (object sender, EventArgs args) =>
			{
				RaiseEvent(new RoutedEventArgs(UndoRedoEnableChangedEvent, this));
			};
		}

		private void InitSetting()
		{
			BoardSettingsProvider settings = new BoardSettingsProvider
			{
				Board = this
			};
			this.SetSettings(settings);
		}

		private void RegisterSlidesInserted()
		{
			SlidesInserted += (object s, SlidesRoutedEventArgs e) =>
			{
				foreach (Slide slide in e.Slides)
				{
					ElementInitializeProvider.OnInsertSlide(this, slide);
				}
			};
		}

		public override void OnApplyTemplate()
		{
			_slideViewbox = (SlideViewbox)GetTemplateChild("PART_SlideViewBox");
			_eventSource = GetTemplateChild("PART_DecoratorSource") as BoardEventSource;
			if (_eventSource == null)
			{
				throw new ArgumentException("Board的样式需要包含 PART_DecoratorSource 同时要求这个类是 BoardEventSource 用于接受输入");
			}
			_contentHost = (ContentPresenter)GetTemplateChild("PART_ContentHost");
			_modeController.EventSource = _eventSource;
			SetValue(RenderSourcePropertyKey, _eventSource);
			UpdateCurrentSlideValue();
			UpdateUtilitiesGrid();
		}

		private void UpdateCurrentSlideValue()
		{
			if (_contentHost != null && _contentHost.GetBindingExpression(ContentPresenter.ContentProperty) == null)
			{
				Slide currentSlide = CurrentSlide;
				_contentHost.Content = currentSlide;
			}
		}

		private void ReloadSlide(Slide slide)
		{
			if (object.Equals(slide.RenderSize, default(Size)) && slide.Width > 0.0 && slide.Height > 0.0)
			{
				slide.Measure(new Size(slide.Width, slide.Height));
				slide.Arrange(new Rect(0.0, 0.0, slide.Width, slide.Height));
			}
			slide.ReloadElements(ReloadMode.Display);
		}

		private void UpdateUtilitiesGrid()
		{
			_utilitiesGrid?.Children.Clear();
			_utilitiesGrid = (Grid)GetTemplateChild("PART_UtilitiesGrid");
		}

		public void AddBoardUtilitiesElement(UIElement element)
		{
			_utilitiesGrid.Children.Add(element);
		}

		public void RemoveBoardUtilitiesElement(UIElement element)
		{
			_utilitiesGrid.Children.Remove(element);
		}

		public bool ContainsBoardUtilitiesElement(UIElement element)
		{
			return _utilitiesGrid.Children.Contains(element);
		}

		public void SortSlides(IList<int> sortList)
		{
			RaiseEvent(new RoutedEventArgs(SlidesSortingEvent, _slideList.ToList()));
			_slideList.Sort(sortList);
			RaiseEvent(new RoutedEventArgs(SlidesSortedEvent, _slideList.ToList()));
		}

		public int GetSlideIndex(Slide slide)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide cannot be null");
			}
			return _slideList.IndexOf(slide);
		}

		public Slide GetSlideById(string id)
		{
			return _slideList.FirstOrDefault((Slide x) => x.Id == id);
		}

		public void Insert(Slide slide)
		{
			Insert(new Slide[1] { slide });
		}

		public void Insert(List<Slide> slides)
		{
			Insert((IList<Slide>)slides);
		}

		private void Insert(IList<Slide> slides)
		{
			RaiseEvent(new SlidesRoutedEventArgs(SlidesInsertingEvent, slides));
			_slideList.Insert(slides);
			RaiseEvent(new SlidesRoutedEventArgs(SlidesInsertedEvent, slides));
		}

		public void InsertAt(Slide slide, int index)
		{
			InsertAt(new Slide[1] { slide }, new int[1] { index });
		}

		public void InsertAt(IList<Slide> slides, IList<int> indexes)
		{
			RaiseEvent(new SlidesRoutedEventArgs(SlidesInsertingEvent, slides));
			_slideList.InsertAt(slides, indexes);
			RaiseEvent(new SlidesRoutedEventArgs(SlidesInsertedEvent, slides));
		}

		public void Delete(Slide slide)
		{
			Ensure.ArgumentIsNotNull(() => slide);
			Delete(new Slide[1] { slide });
		}

		public void DeleteAt(int index)
		{
			DeleteAt(new int[1] { index });
		}

		public void Delete(IList<Slide> deleteSlides, string newSlideId = null)
		{
			IList<Slide> slides = GetSlidesBySlideId(deleteSlides.Select((Slide tSlide) => tSlide.Id));
			bool flag = slides.Contains(CurrentSlide);
			Slide slide = CurrentSlide;
			if (flag)
			{
				if (slides.Count == _slideList.Count)
				{
					slide = (string.IsNullOrEmpty(newSlideId) ? new Slide() : new Slide(newSlideId));
					_slideList.Insert(slide);
				}
				else
				{
					int indexCurrentSlide = _slideList.IndexOf(CurrentSlide);
					slide = _slideList.Where((Slide s, int i) => i > indexCurrentSlide && !slides.Contains(s)).FirstOrDefault() ?? _slideList.Reverse().Where((Slide s, int i) => i > _slideList.Count - indexCurrentSlide - 1 && !slides.Contains(s)).FirstOrDefault();
				}
			}
			RaiseEvent(new SlidesRoutedEventArgs(SlidesDeletingEvent, slides));
			_slideList.Delete(slides);
			CurrentSlide = slide;
			RaiseEvent(new SlidesRoutedEventArgs(SlidesDeletedEvent, slides));
		}

		public void DeleteAt(IEnumerable<int> indexes, string newSlideId = null)
		{
			List<Slide> deleteSlides = indexes.Select((int index) => _slideList[index]).ToList();
			Delete(deleteSlides, newSlideId);
		}

		public void Clear(Slide newSlide = null)
		{
			if (newSlide == null)
			{
				newSlide = new Slide();
			}
			RaiseEvent(new RoutedEventArgs(BoardClearingEvent, this));
			CurrentSlide?.Unselect();
			if (double.IsInfinity(newSlide.Width) || double.IsNaN(newSlide.Width) || double.IsInfinity(newSlide.Height) || double.IsNaN(newSlide.Height))
			{
				ClearValue(SlideSizeProperty);
			}
			else
			{
				SlideSize = new Size(newSlide.Width, newSlide.Height);
			}
			Slide[] deleteSlides = _slideList.ToArray();
			Insert(newSlide);
			CurrentSlide = newSlide;
			Delete(deleteSlides);
			UndoRedoProvider.ClearAll();
			_slideList.OnPropertyChanged("CurrentIndex");
			RaiseEvent(new RoutedEventArgs(BoardClearedEvent, this));
		}

		public void Reset(IEnumerable<Slide> slides)
		{
			RaiseEvent(new RoutedEventArgs(BoardResetingEvent, this));
			_slideList.Reset(slides);
			RaiseEvent(new RoutedEventArgs(BoardResetedEvent, this));
		}

		[Obsolete("请使用 Clear")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearWithNewSlide(Slide newSlide)
		{
			RaiseEvent(new RoutedEventArgs(BoardClearingEvent, this));
			CurrentSlide?.Unselect();
			if (double.IsInfinity(newSlide.Width) || double.IsNaN(newSlide.Width) || double.IsInfinity(newSlide.Height) || double.IsNaN(newSlide.Height))
			{
				ClearValue(SlideSizeProperty);
			}
			else
			{
				SlideSize = new Size(newSlide.Width, newSlide.Height);
			}
			Slide[] deleteSlides = _slideList.ToArray();
			Insert(newSlide);
			CurrentSlide = newSlide;
			Delete(deleteSlides);
			UndoRedoProvider.ClearAll();
			_slideList.OnPropertyChanged("CurrentIndex");
			RaiseEvent(new RoutedEventArgs(BoardClearedEvent, this));
		}

		[Obsolete("请使用 GetSlideIndex")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int IndexOf(Slide slide)
		{
			return GetSlideIndex(slide);
		}

		public bool GoToNextSlide()
		{
			if (_slideList.CurrentIndex == _slideList.Count - 1)
			{
				return false;
			}
			GoTo(_slideList.CurrentIndex + 1);
			return true;
		}

		public bool GoToPreviousSlide()
		{
			if (_slideList.CurrentIndex == 0)
			{
				return false;
			}
			GoTo(_slideList.CurrentIndex - 1);
			return true;
		}

		public void GoTo(int index)
		{
			if (!IsSlideIndexValid(index))
			{
				throw new ArgumentOutOfRangeException("index", "不包含指定页面。");
			}
			Slide slide = _slideList[index];
			slide.WaitingForLoaded(() =>
			{
				CurrentSlide = slide;
			});
		}

		public void GoTo(Slide slide)
		{
			int num = _slideList.IndexOf(slide);
			if (!IsSlideIndexValid(num))
			{
				throw new ArgumentException("不包含指定页面。", "slide");
			}
			GoTo(num);
		}

		public Slide GetSlide(int index)
		{
			if (!IsSlideIndexValid(index))
			{
				throw new ArgumentOutOfRangeException("index", "不包含指定页面。");
			}
			return _slideList[index];
		}

		private bool IsSlideIndexValid(int slideIndex)
		{
			if (slideIndex >= 0)
			{
				return slideIndex < _slideList.Count;
			}
			return false;
		}

		private IList<Slide> GetSlidesBySlideId(IEnumerable<string> idList)
		{
			return (from slide in idList.Select((string id) =>
				{
					Slide slide = Slides.FirstOrDefault((Slide tSlide) => tSlide.Id.Equals(id));
					if (slide == null && DebuggingProperties.IsDebug)
					{
						throw new ArgumentException("找不到当前页面ID:(" + id + ")对应的页面，仅在调试下抛出，请联系林德熙！")
						{
							Data = 
							{
								{
									(object)"id",
									(object)id
								},
								{
									(object)"idList",
									(object)idList
								},
								{
									(object)"Slides",
									(object)Slides
								}
							}
						};
					}
					return slide;
				})
				where slide != null
				select slide).ToList();
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public StylusPlugInCollection GetStylusPlugins()
		{
			return StylusPlugIns;
		}

		public BoardSaveInfo GetSaveInfo()
		{
			BoardSaveInfo boardSaveInfo = new BoardSaveInfo
			{
				SlideWidth = SlideSize.Width,
				SlideHeight = SlideSize.Height,
				SlideIds = _slideList.Select((Slide s) => s.Id).ToArray()
			};
			Extensions.OfType<IExtensionSavable<BoardSaveInfo>>().ForEach((IExtensionSavable<BoardSaveInfo> extension) =>
			{
				try
				{
					extension.GetSaveInfo(ref boardSaveInfo);
				}
				catch (Exception exception)
				{
					this.GetSettings<TraceSettings>().Tracer?.Trace(exception, null);
				}
			});
			return boardSaveInfo;
		}

		public void SetSaveInfo(BoardSaveInfo saveInfo)
		{
			if (saveInfo == null)
			{
				throw new ArgumentNullException("saveInfo");
			}
			if (!double.IsNaN(saveInfo.SlideWidth) && !double.IsNaN(saveInfo.SlideHeight) && !double.IsInfinity(saveInfo.SlideWidth) && !double.IsInfinity(saveInfo.SlideHeight) && saveInfo.SlideWidth >= 1.0 && saveInfo.SlideHeight >= 1.0)
			{
				SlideSize = new Size(saveInfo.SlideWidth, saveInfo.SlideHeight);
			}
			else
			{
				ClearValue(SlideSizeProperty);
			}
			_003CSetSaveInfo_003Eg__OffController_007C121_1();
			CurrentSlide?.Unselect();
			_slideList.Reset(saveInfo.SlideIds.Select((string id) => new Slide(id)
			{
				IsDataLoaded = false
			}));
			try
			{
				ISaveInfoProvider interfaceInstance = InterfaceCreator.GetInterfaceInstance<ISaveInfoProvider>(new object[0]);
				ICreateExtensionsSaveInfoProvider createExtensionsSaveInfoProvider = interfaceInstance as ICreateExtensionsSaveInfoProvider;
				if (createExtensionsSaveInfoProvider != null)
				{
					createExtensionsSaveInfoProvider.CreateExtensionsAndSetSaveInfoAndAddToExtensionCollection(saveInfo, Extensions);
				}
				else
				{
					interfaceInstance.CreateExtensions(saveInfo, Extensions);
				}
			}
			catch (Exception exception)
			{
				this.GetSettings<TraceSettings>().Tracer?.Trace(exception, null);
			}
			if (_slideList.Contains(CurrentSlide))
			{
				_003CSetSaveInfo_003Eg__OnController_007C121_2();
			}
		}

		[CompilerGenerated]
		private void _003CSetSaveInfo_003Eg__OffController_007C121_1()
		{
			Controller.SwitchOff();
			Controller.SwitchOffFilters();
		}

		[CompilerGenerated]
		private void _003CSetSaveInfo_003Eg__OnController_007C121_2()
		{
			Controller.SwitchOnFilters(CurrentSlide);
			Controller.SwitchOn();
		}
	}
}
