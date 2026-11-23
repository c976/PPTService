using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.ComponentModel;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Windows.Threading;

namespace PPTServiceUltra.Ink.Core.Windows.Notifications
{
	public class ContentDialog : ContentControl, IPopupRootContent, IModelDialog
	{
		private static readonly DependencyPropertyKey DialogPropertyKey;

		public static readonly DependencyProperty DialogProperty;

		public static readonly DependencyProperty TitleProperty;

		public static readonly DependencyProperty IsModelProperty;

		private IInputElement _focusElement;

		public static readonly DependencyProperty DialogWidthProperty;

		public static readonly DependencyProperty DialogHeightProperty;

		private static readonly DependencyPropertyKey IsDisplayingPropertyKey;

		public static readonly DependencyProperty IsDisplayingProperty;

		public static readonly DependencyProperty HideDelayProperty;

		[CanBeNull]
		private static Window _defaultWindow;

		private DispatcherAsyncOperation<ContentDialogResult> _showingTask;

		private Action<ContentDialogResult, Exception> _reportResult;

		private bool _currentModelState;

		private CommandBindingCollection _commands = new CommandBindingCollection();

		private readonly Dictionary<UIElement, IEnumerable<KeyBinding>> _ancesterKeyBindings = new Dictionary<UIElement, IEnumerable<KeyBinding>>();

		public object Title
		{
			get
			{
				return GetValue(TitleProperty);
			}
			set
			{
				SetValue(TitleProperty, value);
			}
		}

		public DataTemplate TitleTemplate { get; set; }

		public bool IsModel
		{
			get
			{
				return (bool)GetValue(IsModelProperty);
			}
			set
			{
				SetValue(IsModelProperty, value);
			}
		}

		public bool IsWaitingForResult => _showingTask != null;

		public double DialogWidth
		{
			get
			{
				return (double)GetValue(DialogWidthProperty);
			}
			set
			{
				SetValue(DialogWidthProperty, value);
			}
		}

		public double DialogHeight
		{
			get
			{
				return (double)GetValue(DialogHeightProperty);
			}
			set
			{
				SetValue(DialogHeightProperty, value);
			}
		}

		public bool IsDisplaying
		{
			get
			{
				return (bool)GetValue(IsDisplayingProperty);
			}
			private set
			{
				SetValue(IsDisplayingPropertyKey, value);
			}
		}

		public TimeSpan HideDelay
		{
			get
			{
				return (TimeSpan)GetValue(HideDelayProperty);
			}
			set
			{
				SetValue(HideDelayProperty, value);
			}
		}

		public Window Owner { get; set; }

		[CanBeNull]
		public static Window DefaultWindow
		{
			get
			{
				return _defaultWindow;
			}
			set
			{
				if (DebuggingProperties.IsDebug)
				{
					if (value == null)
					{
						throw new ArgumentNullException("设置的值不能为空");
					}
					if (value.Content == null)
					{
						throw new ArgumentNullException("设置的 Windows 的 Content 必须有值");
					}
				}
				_defaultWindow = value;
			}
		}

		protected ContentDialogResult Result { get; set; }

		private ExecutedRoutedEventHandler _executeRoutedEventHandler => (object s, ExecutedRoutedEventArgs e) =>
		{
			e.Handled = true;
		};

		public event TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs> Closed;

		public event TypedEventHandler<ContentDialog, ContentDialogClosingEventArgs> Closing;

		public event TypedEventHandler<ContentDialog, ContentDialogOpenedEventArgs> Opened;

		public static ContentDialog GetDialog(DependencyObject dependencyObject)
		{
			return (ContentDialog)dependencyObject.GetValue(DialogProperty);
		}

		static ContentDialog()
		{
			DialogPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Dialog", typeof(ContentDialog), typeof(ContentDialog), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			DialogProperty = DialogPropertyKey.DependencyProperty;
			TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(ContentDialog), new PropertyMetadata(null));
			IsModelProperty = DependencyProperty.Register("IsModel", typeof(bool), typeof(ContentDialog), new PropertyMetadata(true, OnIsModelChanged));
			DialogWidthProperty = DependencyProperty.Register("DialogWidth", typeof(double), typeof(ContentDialog), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));
			DialogHeightProperty = DependencyProperty.Register("DialogHeight", typeof(double), typeof(ContentDialog), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));
			IsDisplayingPropertyKey = DependencyProperty.RegisterReadOnly("IsDisplaying", typeof(bool), typeof(ContentDialog), new PropertyMetadata(false));
			IsDisplayingProperty = IsDisplayingPropertyKey.DependencyProperty;
			HideDelayProperty = DependencyProperty.Register("HideDelay", typeof(TimeSpan), typeof(ContentDialog), new PropertyMetadata(TimeSpan.FromMilliseconds(0.0)));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentDialog), new FrameworkPropertyMetadata(typeof(ContentDialog)));
			FrameworkElement.WidthProperty.OverrideMetadata(typeof(ContentDialog), new FrameworkPropertyMetadata(double.NaN, OnWidthChanged));
			FrameworkElement.HeightProperty.OverrideMetadata(typeof(ContentDialog), new FrameworkPropertyMetadata(double.NaN, OnHeightChanged));
		}

		public ContentDialog()
		{
			SetValue(DialogPropertyKey, this);
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		private static void OnIsModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ContentDialog contentDialog = (ContentDialog)d;
			if (contentDialog.IsLoaded)
			{
				contentDialog.UpdateModel((bool)e.NewValue);
			}
		}

		public async void Hide()
		{
			await HideAsync();
		}

		public async Task HideAsync()
		{
			if (_showingTask == null)
			{
				throw new InvalidOperationException("The ContentDialog cannot be Hide because it is not visible.");
			}
			if (_focusElement != null)
			{
				_focusElement.Focusable = true;
				_focusElement.Focus();
			}
			await HidePrivate();
		}

		public async Task<ContentDialogResult> ShowAsync()
		{
			if (IsWaitingForResult)
			{
				throw new InvalidOperationException("The ContentDialog cannot be shown because it is waiting for result. Check IsWaitingForResult value is possible.");
			}
			_showingTask = DispatcherAsyncOperation<ContentDialogResult>.Create(out _reportResult);
			_focusElement = Keyboard.FocusedElement;
			if (_focusElement != null)
			{
				_focusElement.Focusable = false;
			}
			ShowPrivate();
			await _showingTask;
			_showingTask = null;
			_reportResult = null;
			return Result;
		}

		public async Task<ContentDialogResult?> WaitForResultAsync()
		{
			if (IsWaitingForResult)
			{
				await _showingTask;
				return Result;
			}
			return null;
		}

		private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(FrameworkElement.WidthProperty.DefaultMetadata?.DefaultValue, e.NewValue))
			{
				d.SetValue(DialogWidthProperty, e.NewValue);
				d.ClearValue(FrameworkElement.WidthProperty);
			}
		}

		private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(FrameworkElement.HeightProperty.DefaultMetadata.DefaultValue, e.NewValue))
			{
				d.SetValue(DialogHeightProperty, e.NewValue);
				d.ClearValue(FrameworkElement.HeightProperty);
			}
		}

		private void ShowPrivate()
		{
			Window window = Owner;
			if (window == null && Dispatcher == DefaultWindow?.Dispatcher)
			{
				window = DefaultWindow;
			}
			DependencyObject dependencyObject = (DependencyObject)(window?.Content);
			if (dependencyObject == null && DebuggingProperties.IsDebug)
			{
				throw new ArgumentNullException(("在显示的时候 Window 的 Content 不能为空" + Owner == null) ? " 当前因为 Owner 为空，拿到的窗口是 DefaultWindow 这个窗口是静态变量" : "传入的 Owner 里面没有内容");
			}
			DependencyObject dependencyObject2 = ((window != null && dependencyObject != null) ? VisualTreeHelper.GetParent(dependencyObject) : null);
			((dependencyObject2 == null) ? PopupRootProxy.GetOrCreate() : PopupRootProxy.GetOrCreate(dependencyObject2)).LayeredChildren.Add(this);
			IsDisplaying = true;
			IsEnabled = true;
		}

		private async Task HidePrivate()
		{
			Window window = Window.GetWindow(this);
			DependencyObject source = window?.Content as DependencyObject;
			if (source == null)
			{
				return;
			}
			ContentDialogClosingEventArgs e = new ContentDialogClosingEventArgs(Result);
			Closing?.Invoke(this, e);
			if (!e.Cancel)
			{
				IsEnabled = false;
				IsDisplaying = false;
				UpdateModel(false);
				await Task.Delay(HideDelay);
				DependencyObject parent = VisualTreeHelper.GetParent(source);
				PopupRootProxy popupRootProxy = ((parent == null) ? null : PopupRootProxy.GetPopupRoot(parent));
				if (popupRootProxy != null)
				{
					popupRootProxy.LayeredChildren.Remove(this);
				}
				else
				{
					window.Close();
				}
				if (IsWaitingForResult)
				{
					await _showingTask;
				}
				Closed?.Invoke(this, new ContentDialogClosedEventArgs(Result));
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				UpdateModel(IsModel);
				Opened?.Invoke(this, new ContentDialogOpenedEventArgs());
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return;
			}
			Dispatcher.BeginInvoke((Action)(() =>
			{
				if (_reportResult == null)
				{
					throw new InvalidOperationException("The ContentDialog is not waiting for any result, so it cannot be unloaded.");
				}
				_reportResult(Result, null);
			}));
		}

		private void UpdateModel(bool model)
		{
			if (_currentModelState == model)
			{
				return;
			}
			_currentModelState = model;
			PopupRootProxy popupRoot = PopupRootProxy.GetPopupRoot(this);
			if (popupRoot != null)
			{
				if (model)
				{
					EnterModel(popupRoot);
				}
				else
				{
					ExitModel(popupRoot);
				}
			}
		}

		private void SetRoutingStarategy(RoutedEvent routedEvent, RoutingStrategy routingStrategy)
		{
			GetRoutingStarategyField().SetValue(routedEvent, routingStrategy);
		}

		private FieldInfo GetRoutingStarategyField()
		{
			return typeof(RoutedEvent).GetField("_routingStrategy", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
		}

		private void EnterModel(PopupRootProxy owner)
		{
			Window window = Owner ?? DefaultWindow;
			if (window != null)
			{
				_commands.AddRange(window.CommandBindings);
				window.CommandBindings.Clear();
				window.GotFocus += Owner_GotFocus;
			}
			if (owner.LayeredChildren.Count == 1)
			{
				Focusable = true;
				AddHandler(CommandManager.ExecutedEvent, _executeRoutedEventHandler);
				BlockAncesterKeyBindings();
			}
		}

		private void Owner_GotFocus(object sender, RoutedEventArgs e)
		{
			Visual visual = e.OriginalSource as Visual;
			if (visual != null && visual.VisualAncestor<ContentDialog>() == null && visual.VisualAncestor((DependencyObject v) => v.GetType().FullName == "System.Windows.Controls.Primitives.PopupRoot") == null)
			{
				Focus();
			}
		}

		private void ExitModel(PopupRootProxy owner)
		{
			Window window = Owner ?? DefaultWindow;
			if (window != null)
			{
				window.GotFocus -= Owner_GotFocus;
				window.CommandBindings.AddRange(_commands);
				RemoveHandler(CommandManager.ExecutedEvent, _executeRoutedEventHandler);
			}
			if (owner.LayeredChildren.Count == 1)
			{
				RestoreAncesterKeyBindings();
			}
		}

		private void BlockAncesterKeyBindings()
		{
			for (DependencyObject parent = VisualTreeHelper.GetParent(this); parent != null; parent = VisualTreeHelper.GetParent(parent))
			{
				UIElement uIElement = parent as UIElement;
				if (uIElement != null)
				{
					KeyBinding[] array = uIElement.InputBindings.OfType<KeyBinding>().ToArray();
					if (array.Any())
					{
						_ancesterKeyBindings[uIElement] = array;
						uIElement.InputBindings.Clear();
					}
				}
			}
		}

		private void RestoreAncesterKeyBindings()
		{
			for (DependencyObject parent = VisualTreeHelper.GetParent(this); parent != null; parent = VisualTreeHelper.GetParent(parent))
			{
				UIElement uIElement = parent as UIElement;
				IEnumerable<KeyBinding> value;
				if (uIElement != null && _ancesterKeyBindings.TryGetValue(uIElement, out value))
				{
					_ancesterKeyBindings.Remove(uIElement);
					foreach (KeyBinding item in value)
					{
						uIElement.InputBindings.Add(item);
					}
				}
			}
		}
	}
}
