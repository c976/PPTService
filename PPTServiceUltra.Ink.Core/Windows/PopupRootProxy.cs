using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public class PopupRootProxy
	{
		public static readonly DependencyProperty PopupRootProperty = DependencyProperty.RegisterAttached("PopupRoot", typeof(PopupRootProxy), typeof(PopupRootProxy), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		private static readonly DependencyProperty IsPopupRootHostProperty = DependencyProperty.RegisterAttached("IsPopupRootHost", typeof(bool), typeof(PopupRootProxy), new PropertyMetadata(false));

		private readonly Dictionary<IPopupRootContent, PopupContentAdorner> _layeredChildrenDictionary = new Dictionary<IPopupRootContent, PopupContentAdorner>();

		public static readonly RoutedEvent PopupContentFirstAddedEvent = EventManager.RegisterRoutedEvent("PopupContentFirstAdded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PopupRootProxy));

		public static readonly RoutedEvent PopupContentLastRemovedEvent = EventManager.RegisterRoutedEvent("PopupContentLastRemoved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PopupRootProxy));

		public PopupRootContentCollection LayeredChildren { get; }

		private AdornerDecorator Decorator { get; }

		public event RoutedEventHandler PopupContentFirstAdded
		{
			add
			{
				Decorator.AddHandler(PopupContentFirstAddedEvent, value);
			}
			remove
			{
				Decorator.RemoveHandler(PopupContentFirstAddedEvent, value);
			}
		}

		public event RoutedEventHandler PopupContentLastRemoved
		{
			add
			{
				Decorator.AddHandler(PopupContentLastRemovedEvent, value);
			}
			remove
			{
				Decorator.RemoveHandler(PopupContentLastRemovedEvent, value);
			}
		}

		[Pure]
		internal static PopupRootProxy GetPopupRoot([NotNull] DependencyObject dependencyObject)
		{
			if (dependencyObject == null)
			{
				throw new ArgumentNullException("dependencyObject");
			}
			return (PopupRootProxy)dependencyObject.GetValue(PopupRootProperty);
		}

		[Pure]
		[NotNull]
		internal static PopupRootProxy GetOrCreate([NotNull] DependencyObject dependencyObject)
		{
			if (dependencyObject == null)
			{
				throw new ArgumentNullException("dependencyObject");
			}
			PopupRootProxy popupRootProxy = (PopupRootProxy)dependencyObject.GetValue(PopupRootProperty);
			if (popupRootProxy != null)
			{
				return popupRootProxy;
			}
			UIElement uIElement = Window.GetWindow(dependencyObject)?.Content as UIElement;
			if (uIElement != null)
			{
				AdornerDecorator adornerDecorator = FindAdornerDecorator(uIElement);
				return (adornerDecorator != null) ? new PopupRootProxy(adornerDecorator) : GetOrCreate();
			}
			return GetOrCreate();
		}

		internal static PopupRootProxy GetOrCreate()
		{
			AdornerDecorator adornerDecorator = new AdornerDecorator
			{
				Child = new Border()
			};
			Window window = new Window
			{
				WindowStyle = WindowStyle.None,
				WindowState = WindowState.Maximized,
				AllowsTransparency = true,
				Background = Brushes.Transparent,
				ResizeMode = ResizeMode.NoResize,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				Content = adornerDecorator
			};
			SetIsPopupRootHost(window, true);
			window.Show();
			return new PopupRootProxy(adornerDecorator);
		}

		private static bool GetIsPopupRootHost(Window window)
		{
			return (bool)window.GetValue(IsPopupRootHostProperty);
		}

		private static void SetIsPopupRootHost(Window window, bool value)
		{
			window.SetValue(IsPopupRootHostProperty, value);
		}

		public PopupRootProxy([NotNull] AdornerDecorator adornerDecorator)
		{
			if (adornerDecorator == null)
			{
				throw new ArgumentNullException("adornerDecorator");
			}
			Decorator = adornerDecorator;
			adornerDecorator.SetValue(PopupRootProperty, this);
			LayeredChildren = new PopupRootContentCollection(OnAdded, OnRemoved, OnFirstAdded, OnLastRemoving, OnLastRemoved);
		}

		private void OnFirstAdded()
		{
			Decorator.RaiseEvent(new RoutedEventArgs(PopupContentFirstAddedEvent, this));
		}

		private void OnAdded(IPopupRootContent content)
		{
			AddContent(content);
			UIElement element = (UIElement)content;
			FocusContent(element);
		}

		private void OnRemoved(IPopupRootContent content)
		{
			RemoveContent(content);
			UIElement element = (UIElement)content;
			UnfocusContent(element);
		}

		private void OnLastRemoving()
		{
			Window window = Window.GetWindow(Decorator);
			if (window != null && GetIsPopupRootHost(window))
			{
				window.Close();
			}
		}

		private void OnLastRemoved()
		{
			Decorator.RaiseEvent(new RoutedEventArgs(PopupContentLastRemovedEvent, this));
		}

		private void AddContent(IPopupRootContent content)
		{
			PopupContentAdorner popupContentAdorner = new PopupContentAdorner(Decorator)
			{
				Child = content
			};
			_layeredChildrenDictionary.Add(content, popupContentAdorner);
			Decorator.AdornerLayer.Add(popupContentAdorner);
		}

		private void RemoveContent(IPopupRootContent content)
		{
			try
			{
				PopupContentAdorner popupContentAdorner = _layeredChildrenDictionary[content];
				popupContentAdorner.Child = null;
				Decorator.AdornerLayer.Remove(popupContentAdorner);
			}
			finally
			{
				_layeredChildrenDictionary.Remove(content);
			}
		}

		private void FocusContent(UIElement element)
		{
			Mouse.Capture(null);
			Stylus.Capture(null);
			FocusManager.SetIsFocusScope(element, true);
			KeyboardNavigation.SetTabNavigation(element, KeyboardNavigationMode.Cycle);
			KeyboardNavigation.SetControlTabNavigation(element, KeyboardNavigationMode.Cycle);
			KeyboardNavigation.SetDirectionalNavigation(element, KeyboardNavigationMode.Contained);
			element.Dispatcher.BeginInvoke((Action)(() =>
			{
				FrameworkElement obj = element as FrameworkElement;
				if (obj != null && obj.IsLoaded && (Keyboard.FocusedElement == null || (Keyboard.FocusedElement as Visual)?.VisualAncestor((DependencyObject x) => x.GetType() == element.GetType()) == null))
				{
					IInputElement inputElement = Keyboard.Focus(element);
					if (inputElement != null && inputElement.GetType().Name != element.GetType().Name)
					{
						element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
					}
				}
			}), DispatcherPriority.ContextIdle);
		}

		private void UnfocusContent(UIElement element)
		{
			element.ClearValue(KeyboardNavigation.TabNavigationProperty);
			element.ClearValue(KeyboardNavigation.ControlTabNavigationProperty);
			element.ClearValue(KeyboardNavigation.DirectionalNavigationProperty);
			element.ClearValue(FocusManager.IsFocusScopeProperty);
		}

		[CanBeNull]
		private static AdornerDecorator FindAdornerDecorator([NotNull] Visual visual)
		{
			if (visual == null)
			{
				throw new ArgumentNullException("visual");
			}
			for (Visual visual2 = VisualTreeHelper.GetParent(visual) as Visual; visual2 != null; visual2 = VisualTreeHelper.GetParent(visual2) as Visual)
			{
				AdornerDecorator adornerDecorator = visual2 as AdornerDecorator;
				if (adornerDecorator != null)
				{
					return adornerDecorator;
				}
			}
			return null;
		}
	}
}
