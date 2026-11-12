using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Linq;
using Cvte.Paint.Framework.Elements;
using Cvte.Paint.Framework.Exceptions;
using Cvte.Paint.Framework.Level;
using Cvte.Paint.Framework.SelectionBoxs;
using Cvte.Storage;
using Cvte.Storage.Standard;

namespace Cvte.Paint.Framework
{
	[DebuggerDisplay("Slide [{Id,nq}]")]
	[TemplatePart(Name = "PART_ContentHost", Type = typeof(ContentPresenter))]
	[TemplatePart(Name = "PART_UtilitiesHost", Type = typeof(ContentPresenter))]
	public class Slide : Control, IIdDefinition, IExtensible<Slide>, IUtilitiesContainer, ISavable<SlideSaveInfo>, ISavable, ILevelContainer, IElementContainer
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass109_0
		{
			public Slide _003C_003E4__this;

			public SaveInfoExceptionPackage exceptionPackage;

			public SlideSaveInfo slideSaveInfo;

			internal void _003CGetSaveInfo_003Eg__AppendElements_007C0()
			{
				List<Element> list = _003C_003E4__this.GetSortedElements().ToList();
				(from elementSaveInfo in list.Select((Element element) =>
					{
						try
						{
							return element.GetElementSaveInfo();
						}
						catch (SaveInfoException ex)
						{
							exceptionPackage?.Add(ex);
							return (ElementSaveInfo)null;
						}
						catch (Exception ex2)
						{
							exceptionPackage?.Add(new SaveInfoException(element.GetType().Name + ".GetElementSaveInfo() throws " + ex2.GetType().Name + ".", ex2, element.GetType()));
							return (ElementSaveInfo)null;
						}
					})
					where elementSaveInfo != null
					select elementSaveInfo).ForEach(slideSaveInfo.Elements.Add);
				if (!DebuggingProperties.IsDebug)
				{
					return;
				}
				foreach (Element tSortedElement in list)
				{
					if (list.Count((Element temp) => tSortedElement.Id == temp.Id) != 1)
					{
						throw new ArgumentException("存在两个id相同元素 " + tSortedElement.Id);
					}
				}
			}

			internal ElementSaveInfo _003CGetSaveInfo_003Eb__2(Element element)
			{
				try
				{
					return element.GetElementSaveInfo();
				}
				catch (SaveInfoException ex)
				{
					exceptionPackage?.Add(ex);
					return null;
				}
				catch (Exception ex2)
				{
					exceptionPackage?.Add(new SaveInfoException(element.GetType().Name + ".GetElementSaveInfo() throws " + ex2.GetType().Name + ".", ex2, element.GetType()));
					return null;
				}
			}

			internal void _003CGetSaveInfo_003Eg__AppendExtensions_007C1()
			{
				_003C_003E4__this.Extensions.OfType<IExtensionSavable<SlideSaveInfo>>().ForEach((IExtensionSavable<SlideSaveInfo> extension) =>
				{
					try
					{
						extension.GetSaveInfo(ref slideSaveInfo);
					}
					catch (SaveInfoException ex)
					{
						exceptionPackage?.Add(ex);
					}
					catch (Exception ex2)
					{
						exceptionPackage?.Add(new SaveInfoException(extension.GetType().Name + ".GetSaveInfo() throws " + ex2.GetType().Name + ".", ex2, extension.GetType()));
					}
				});
			}

			internal void _003CGetSaveInfo_003Eb__5(IExtensionSavable<SlideSaveInfo> extension)
			{
				try
				{
					extension.GetSaveInfo(ref slideSaveInfo);
				}
				catch (SaveInfoException ex)
				{
					exceptionPackage?.Add(ex);
				}
				catch (Exception ex2)
				{
					exceptionPackage?.Add(new SaveInfoException(extension.GetType().Name + ".GetSaveInfo() throws " + ex2.GetType().Name + ".", ex2, extension.GetType()));
				}
			}
		}

		private bool _keyBoardDisabled;

		public static readonly RoutedEvent SlideChangedEvent;

		public static readonly RoutedEvent SlideLoadingEvent;

		public static readonly RoutedEvent SlideLoadedEvent;

		public static readonly RoutedEvent ElementsAddedEvent;

		public static readonly RoutedEvent ElementsRemovedEvent;

		public static readonly RoutedEvent ElementsChangedEvent;

		public static readonly RoutedEvent SelectionChangedEvent;

		public static readonly RoutedEvent InnerSelectionChangedEvent;

		private ContentPresenter PART_ContentHost;

		private ContentPresenter PART_UtilitiesHost;

		private string _id;

		public static readonly DependencyProperty SlideProperty;

		private bool _isDataLoaded = true;

		[NotNull]
		public string Id
		{
			get
			{
				return _id;
			}
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentException("Empty or whitespace is not valid Id value.", "value");
				}
				_id = value;
			}
		}

		[NotNull]
		public ReadOnlyCollection<Element> Elements { get; }

		private ElementCollection InternalElements => Canvas.ElementCollection;

		[NotNull]
		[Obsolete("当状态不变时，属性不应该每次都返回不同的对象。请使用 GetAllUtilities 方法替代。")]
		public IEnumerable<UIElement> Utilities => GetAllUtilities();

		internal ElementCanvas Canvas { get; }

		private Panel UtilitiesGrid { get; }

		public ILevelManager LevelStorage => InternalLevelStorage;

		private ILevelStorage InternalLevelStorage { get; }

		protected override IEnumerator LogicalChildren
		{
			get
			{
				yield return Canvas;
				yield return UtilitiesGrid;
				foreach (ISelectionOperator selectionOperator in this.GetSelectionOperators())
				{
					yield return selectionOperator;
				}
			}
		}

		private IList<Action> WaitingForLoadedActions { get; } = new List<Action>();

		internal bool IsDataLoaded
		{
			get
			{
				return _isDataLoaded;
			}
			set
			{
				if (_isDataLoaded == value)
				{
					return;
				}
				_isDataLoaded = value;
				if (value)
				{
					foreach (Action waitingForLoadedAction in WaitingForLoadedActions)
					{
						waitingForLoadedAction();
					}
					DataLoaded?.Invoke(this, EventArgs.Empty);
				}
				else
				{
					WaitingForLoadedActions.Clear();
				}
			}
		}

		public ExtensionCollection<Slide> Extensions { get; }

		public event EventHandler<SlideChangeRoutedEventArgs> SlideChanged
		{
			add
			{
				AddHandler(SlideChangedEvent, value);
			}
			remove
			{
				RemoveHandler(SlideChangedEvent, value);
			}
		}

		public event EventHandler<SlideRoutedEventArgs> SlideLoading
		{
			add
			{
				AddHandler(SlideLoadingEvent, value);
			}
			remove
			{
				RemoveHandler(SlideLoadingEvent, value);
			}
		}

		public event EventHandler<SlideRoutedEventArgs> SlideLoaded
		{
			add
			{
				AddHandler(SlideLoadedEvent, value);
			}
			remove
			{
				RemoveHandler(SlideLoadedEvent, value);
			}
		}

		public event RoutedCollectionChangedEventHandler<Element> ElementsAdded
		{
			add
			{
				AddHandler(ElementsAddedEvent, value);
			}
			remove
			{
				RemoveHandler(ElementsAddedEvent, value);
			}
		}

		public event RoutedCollectionChangedEventHandler<Element> ElementsRemoved
		{
			add
			{
				AddHandler(ElementsRemovedEvent, value);
			}
			remove
			{
				RemoveHandler(ElementsRemovedEvent, value);
			}
		}

		public event RoutedElementsChangedEventHandler ElementsChanged
		{
			add
			{
				AddHandler(ElementsChangedEvent, value);
			}
			remove
			{
				RemoveHandler(ElementsChangedEvent, value);
			}
		}

		public event RoutedCollectionChangedEventHandler<Element> SelectionChanged
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

		public event RoutedInnerCollectionChangedEventHandler<object> InnerSelectionChanged
		{
			add
			{
				AddHandler(InnerSelectionChangedEvent, value);
			}
			remove
			{
				RemoveHandler(InnerSelectionChangedEvent, value);
			}
		}

		internal event EventHandler DataLoaded;

		static Slide()
		{
			SlideChangedEvent = EventManager.RegisterRoutedEvent("SlideChanged", RoutingStrategy.Bubble, typeof(EventHandler<SlideChangeRoutedEventArgs>), typeof(Slide));
			SlideLoadingEvent = EventManager.RegisterRoutedEvent("SlideLoadingEvent", RoutingStrategy.Bubble, typeof(EventHandler<SlideRoutedEventArgs>), typeof(Slide));
			SlideLoadedEvent = EventManager.RegisterRoutedEvent("SlideLoadedEvent", RoutingStrategy.Bubble, typeof(EventHandler<SlideRoutedEventArgs>), typeof(Slide));
			ElementsAddedEvent = EventManager.RegisterRoutedEvent("ElementsAdded", RoutingStrategy.Bubble, typeof(RoutedCollectionChangedEventHandler<Element>), typeof(Slide));
			ElementsRemovedEvent = EventManager.RegisterRoutedEvent("ElementsRemoved", RoutingStrategy.Bubble, typeof(RoutedCollectionChangedEventHandler<Element>), typeof(Slide));
			ElementsChangedEvent = EventManager.RegisterRoutedEvent("ElementsChanged", RoutingStrategy.Bubble, typeof(RoutedElementsChangedEventHandler), typeof(Slide));
			SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedCollectionChangedEventHandler<Element>), typeof(Slide));
			InnerSelectionChangedEvent = EventManager.RegisterRoutedEvent("InnerSelectionChanged", RoutingStrategy.Bubble, typeof(RoutedInnerCollectionChangedEventHandler<object>), typeof(Slide));
			SlideProperty = DependencyProperty.RegisterAttached("Slide", typeof(Slide), typeof(Slide), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Slide), new FrameworkPropertyMetadata(typeof(Slide)));
			Control.BackgroundProperty.OverrideMetadata(typeof(Slide), new FrameworkPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.None, BackgroundPropertyChangedCallback));
			EventManager.RegisterClassHandler(typeof(Slide), ElementsChangedEvent, new RoutedElementsChangedEventHandler(Slide_ElementsChanged));
		}

		protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
		{
			base.OnTemplateChanged(oldTemplate, newTemplate);
		}

		public void SuspendKeyEvents()
		{
			_keyBoardDisabled = true;
		}

		public void ResumeKeyEvents()
		{
			_keyBoardDisabled = false;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (_keyBoardDisabled && e.Key != Key.Escape)
			{
				e.Handled = true;
			}
			else
			{
				base.OnKeyDown(e);
			}
		}

		private static void BackgroundPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			((Slide)dependencyObject)?.RaiseSlideChangedEvent(Control.BackgroundProperty);
		}

		public Slide()
			: this(Guid.NewGuid().ToString("N"))
		{
		}

		public Slide([NotNull] string id)
		{
			Id = id;
			Canvas = new ElementCanvas();
			Elements = new ReadOnlyCollection<Element>(Canvas.ElementCollection);
			UtilitiesGrid = new Canvas();
			Extensions = new ExtensionCollection<Slide>(this);
			InternalLevelStorage = new LevelStorage(this);
			SetValue(SlideProperty, this);
			AddLogicalChild(Canvas);
			AddLogicalChild(UtilitiesGrid);
		}

		[CanBeNull]
		public static Slide GetSlide([NotNull] DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return (Slide)element.GetValue(SlideProperty);
		}

		internal void Connect(DependencyObject logicalChild)
		{
			AddLogicalChild(logicalChild);
		}

		internal void Disconnect(DependencyObject logicalChild)
		{
			RemoveLogicalChild(logicalChild);
		}

		void IIdDefinition.SetId(string id)
		{
			Id = id;
		}

		[NotNull]
		public IEnumerable<Element> GetSortedElements()
		{
			return this.GetLevelSortedElements();
		}

		public void RemoveAll<T>() where T : Element
		{
			Remove(Elements.OfType<T>());
		}

		public void Add([NotNull] Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			AddInternal(element);
			RaiseEvent(new RoutedCollectionChangedEventArgs<Element>(ElementsAddedEvent, new Element[1] { element }));
			RaiseSlideChangedEvent(null);
		}

		public void Add([NotNull] IEnumerable<Element> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			Element[] array = AddInternal(elements);
			if (array.Any())
			{
				RaiseEvent(new RoutedCollectionChangedEventArgs<Element>(ElementsAddedEvent, array));
				RaiseSlideChangedEvent(null);
			}
		}

		public bool Contains([NotNull] Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (!Canvas.ElementCollection.Contains(element))
			{
				return Canvas.ElementCollection.ContainsById(element);
			}
			return true;
		}

		public void Remove([NotNull] Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			RemoveInternal(element);
			RaiseEvent(new RoutedCollectionChangedEventArgs<Element>(ElementsRemovedEvent, new Element[1] { element }));
			RaiseSlideChangedEvent(null);
		}

		public void Remove([NotNull] IEnumerable<Element> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			Element[] array = (elements as Element[]) ?? elements.ToArray();
			if (array.Any())
			{
				Element[] array2 = array;
				foreach (Element element in array2)
				{
					RemoveInternal(element);
				}
				RaiseEvent(new RoutedCollectionChangedEventArgs<Element>(ElementsRemovedEvent, array));
				RaiseSlideChangedEvent(null);
			}
		}

		private void RaiseSlideChangedEvent(DependencyProperty property)
		{
			RaiseEvent(new SlideChangeRoutedEventArgs(SlideChangedEvent, this, property));
		}

		public void AddUtilities([NotNull] UIElement utilitiesElement)
		{
			if (utilitiesElement == null)
			{
				throw new ArgumentNullException("utilitiesElement");
			}
			UtilitiesGrid.Children.Add(utilitiesElement);
			InitializeElement(utilitiesElement);
		}

		public void AddUtilitiesAsElement([NotNull] UIElement uiElement)
		{
			if (uiElement == null)
			{
				throw new ArgumentNullException("uiElement");
			}
			UtilitiesPanelElement utilitiesPanelElement = Elements.LastOrDefault() as UtilitiesPanelElement;
			if (utilitiesPanelElement != null)
			{
				utilitiesPanelElement.Add(uiElement);
				return;
			}
			utilitiesPanelElement = new UtilitiesPanelElement
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};
			utilitiesPanelElement.Add(uiElement);
			Add(utilitiesPanelElement);
		}

		public void RemoveUtilities([NotNull] UIElement utilitiesElement)
		{
			if (utilitiesElement == null)
			{
				throw new ArgumentNullException("utilitiesElement");
			}
			using (IEnumerator<UtilitiesPanelElement> enumerator = (from panelElement in Elements.OfType<UtilitiesPanelElement>()
				where panelElement.Children.Contains(utilitiesElement)
				select panelElement).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					UtilitiesPanelElement current = enumerator.Current;
					current.Remove(utilitiesElement);
					if (current.Count == 0)
					{
						Remove(current);
					}
					return;
				}
			}
			UtilitiesGrid.Children.Remove(utilitiesElement);
		}

		public IEnumerable<UIElement> GetAllUtilities()
		{
			IEnumerable<UIElement> enumerable = UtilitiesGrid.Children.Cast<UIElement>();
			foreach (UtilitiesPanelElement item in Elements.OfType<UtilitiesPanelElement>())
			{
				enumerable = enumerable.Union(item.Children.Cast<UIElement>());
			}
			return enumerable;
		}

		public bool ContainsUtilities([NotNull] UIElement utilitiesElement)
		{
			if (utilitiesElement == null)
			{
				throw new ArgumentNullException("utilitiesElement");
			}
			if (!UtilitiesGrid.Children.Contains(utilitiesElement))
			{
				return Elements.Contains(utilitiesElement);
			}
			return true;
		}

		public override void OnApplyTemplate()
		{
			if (PART_ContentHost != null || GetSlide(Canvas) == this)
			{
				RemoveLogicalChild(Canvas);
			}
			PART_ContentHost = (ContentPresenter)Template.FindName("PART_ContentHost", this);
			if (PART_ContentHost != null)
			{
				PART_ContentHost.Content = Canvas;
				AddLogicalChild(Canvas);
			}
			if (PART_UtilitiesHost != null || GetSlide(UtilitiesGrid) == this)
			{
				RemoveLogicalChild(UtilitiesGrid);
			}
			PART_UtilitiesHost = (ContentPresenter)Template.FindName("PART_UtilitiesHost", this);
			if (PART_UtilitiesHost != null)
			{
				PART_UtilitiesHost.Content = UtilitiesGrid;
				AddLogicalChild(UtilitiesGrid);
			}
		}

		private void AddInternal(Element element)
		{
			Ensure.ArgumentIsNotNull(() => element);
			Ensure.That(!Canvas.ElementCollection.ContainsById(element)).OtherwiseThrow(new InvalidOperationException("不可向 Slide 中添加重复元素。")
			{
				Data = { 
				{
					(object)"元素id",
					(object)element.Id
				} }
			});
			Canvas.ElementCollection.Add(element);
			InitializeElement(element);
			InternalLevelStorage.AddElement(element);
		}

		private Element[] AddInternal(IEnumerable<Element> elements)
		{
			Element[] array = (elements as Element[]) ?? elements.ToArray();
			List<InvalidOperationException> list = null;
			Element[] array2 = array;
			foreach (Element element in array2)
			{
				try
				{
					AddInternal(element);
				}
				catch (InvalidOperationException item)
				{
					if (list == null)
					{
						list = new List<InvalidOperationException>();
					}
					list.Add(item);
				}
			}
			if (list != null && list.Count > 0)
			{
				throw new AggregateException("不可向 Slide 中添加重复元素。", list);
			}
			return array;
		}

		private void RemoveInternal(Element element)
		{
			this.AppendUnselect(element);
			Ensure.ArgumentIsNotNull(() => element);
			if (!Canvas.ElementCollection.ContainsById(element))
			{
				string text = "不能删除 Slide 中不包含的元素。当前元素是 " + element.GetType().FullName + " 元素";
				Slide slide = GetSlide(element);
				if (slide != null && slide.Id != Id)
				{
					text = text + " 元素所在页面为" + slide.Id + " 当前页面为" + Id;
				}
				throw new ElementNotFoundException(text, this);
			}
			Canvas.ElementCollection.Remove(element);
		}

		private static void Slide_ElementsChanged(object sender, RoutedElementsChangedEventArgs e)
		{
			((Slide)sender).RaiseSlideChangedEvent(null);
		}

		private void InitializeElement(UIElement element)
		{
			IElementInitialize elementInitialize = element as IElementInitialize;
			if (elementInitialize != null)
			{
				ElementInitializeProvider.WaitingForLoaded(this, elementInitialize.OnInitialized);
			}
			IGroupElement groupElement = element as IGroupElement;
			if (groupElement == null)
			{
				return;
			}
			foreach (Element element2 in groupElement.Elements)
			{
				InitializeElement(element2);
			}
		}

		private void InnerLoad(SlideSaveInfo saveInfo, SaveInfoIntegrity integrity)
		{
			if (saveInfo == null)
			{
				return;
			}
			IsDataLoaded = false;
			try
			{
				SaveInfoExceptionPackage exceptionCollection = SaveInfoExceptionPackage.GetExceptionCollection(this);
				try
				{
					RaiseEvent(new SlideRoutedEventArgs(SlideLoadingEvent, this));
				}
				catch (Exception ex)
				{
					exceptionCollection.Add(ex);
				}
				Width = saveInfo.Width;
				Height = saveInfo.Height;
				if (saveInfo.KeepIndependentSize)
				{
					SetValue(SpecialSlide.KeepIndependentSizeProperty, true);
				}
				else
				{
					ClearValue(SpecialSlide.KeepIndependentSizeProperty);
				}
				Background = saveInfo.Background ?? ((Cvte.Storage.Standard.Brush)Brushes.White);
				Id = saveInfo.Id ?? Guid.NewGuid().ToString();
				if (integrity == SaveInfoIntegrity.Full)
				{
					LoadElements(saveInfo, exceptionCollection);
				}
				LoadExtensions(saveInfo, exceptionCollection);
				if (exceptionCollection.HasException())
				{
					throw new AggregateException(exceptionCollection.Exceptions);
				}
			}
			finally
			{
				IsDataLoaded = true;
				RaiseEvent(new SlideRoutedEventArgs(SlideLoadedEvent, this));
			}
		}

		private void LoadExtensions(SlideSaveInfo saveInfo, SaveInfoExceptionPackage exceptionPackage)
		{
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
			catch (AggregateException ex)
			{
				exceptionPackage.AddRange(ex.InnerExceptions);
			}
			catch (Exception ex2)
			{
				exceptionPackage.Add(ex2);
			}
		}

		private void LoadElements(SlideSaveInfo saveInfo, SaveInfoExceptionPackage exceptionPackage)
		{
			Remove(Elements);
			List<Element> elements = new List<Element>();
			saveInfo.Elements?.ForEach((ElementSaveInfo e, int i) =>
			{
				Element element;
				try
				{
					element = e.ToElement();
				}
				catch (FatalSaveInfoException ex2)
				{
					exceptionPackage.Add(ex2.InnerException);
					return;
				}
				if (element != null && !Canvas.ElementCollection.ContainsById(element))
				{
					elements.Add(element);
				}
			});
			try
			{
				AddInternal(elements);
			}
			catch (Exception ex)
			{
				exceptionPackage.Add(ex);
			}
		}

		[CanBeNull]
		public Element GetElementById(string id)
		{
			return Canvas.ElementCollection.FirstOrDefault((Element e) => e.Id == id);
		}

		public SlideSaveInfo GetSaveInfo()
		{
			return GetSaveInfo(SaveInfoIntegrity.Full);
		}

		public SlideSaveInfo GetSaveInfo(SaveInfoIntegrity integrity)
		{
			_003C_003Ec__DisplayClass109_0 obj = new _003C_003Ec__DisplayClass109_0();
			obj._003C_003E4__this = this;
			if (DebuggingProperties.IsDebug && (double.IsNaN(Width) || double.IsNaN(Height)) && Debugger.IsAttached)
			{
				Debugger.Break();
			}
			obj.slideSaveInfo = new SlideSaveInfo
			{
				Id = Id,
				Width = Width,
				Height = Height,
				KeepIndependentSize = SpecialSlide.GetKeepIndependentSize(this),
				Background = Background,
				Elements = new FallbackArray<ElementSaveInfo>()
			};
			obj.exceptionPackage = SaveInfoExceptionPackage.GetExceptionCollection(this);
			if (integrity == SaveInfoIntegrity.Full)
			{
				obj._003CGetSaveInfo_003Eg__AppendElements_007C0();
			}
			obj._003CGetSaveInfo_003Eg__AppendExtensions_007C1();
			return obj.slideSaveInfo;
		}

		public void SetSaveInfo(SlideSaveInfo info)
		{
			InnerLoad(info, SaveInfoIntegrity.Full);
		}

		public void SetSaveInfo(SlideSaveInfo info, SaveInfoIntegrity integrity)
		{
			InnerLoad(info, integrity);
		}

		public void Load(SlideSaveInfo saveInfo)
		{
			InnerLoad(saveInfo, SaveInfoIntegrity.Full);
		}

		internal void WaitingForLoaded(Action action)
		{
			if (IsDataLoaded)
			{
				action();
			}
			else
			{
				WaitingForLoadedActions.Add(action);
			}
		}

		internal StylusPlugInCollection GetStylusPlugins()
		{
			return StylusPlugIns;
		}

		IList<ILevelElement> IElementContainer.GetLevelElementList()
		{
			return Elements.Cast<ILevelElement>().ToList();
		}
	}
}
