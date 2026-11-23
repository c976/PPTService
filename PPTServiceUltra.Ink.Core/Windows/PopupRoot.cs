using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PPTServiceUltra.Ink.Core.Windows.Notifications;

namespace PPTServiceUltra.Ink.Core.Windows
{
	[Obsolete("所有逻辑已从 PopupRoot 移除。你无需做任何修改，曾经的功能也依旧可用；但仍建议删除此类型的引用，以保持代码的简洁。")]
	public sealed class PopupRoot : Decorator
	{
		private static readonly DependencyPropertyKey PopupRootPropertyKey = DependencyProperty.RegisterAttachedReadOnly("PopupRoot", typeof(PopupRoot), typeof(PopupRoot), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public static readonly DependencyProperty PopupRootProperty = PopupRootPropertyKey.DependencyProperty;

		public UIElementCollection LayeredChildren { get; private set; }

		protected sealed override int VisualChildrenCount
		{
			get
			{
				if (Child != null)
				{
					return LayeredChildren.Count + 1;
				}
				return LayeredChildren.Count;
			}
		}

		protected sealed override IEnumerator LogicalChildren
		{
			get
			{
				if (Child != null)
				{
					yield return Child;
				}
				foreach (UIElement layeredChild in LayeredChildren)
				{
					yield return layeredChild;
				}
			}
		}

		public static PopupRoot GetPopupRoot(DependencyObject dependencyObject)
		{
			return (PopupRoot)dependencyObject.GetValue(PopupRootProperty);
		}

		public PopupRoot()
		{
			SetValue(PopupRootPropertyKey, this);
			LayeredChildren = new UIElementCollection(this, this);
			Loaded += (object sender, RoutedEventArgs args) =>
			{
				Window window = Window.GetWindow(this);
				if (window != null)
				{
					window.PreviewKeyDown -= Window_PreviewKeyDown;
					window.PreviewKeyUp -= Window_PreviewKeyUp;
					window.PreviewKeyDown += Window_PreviewKeyDown;
					window.PreviewKeyUp += Window_PreviewKeyUp;
				}
			};
			Unloaded += (object sender, RoutedEventArgs args) =>
			{
				Window window = Window.GetWindow(this);
				if (window != null)
				{
					window.PreviewKeyDown -= Window_PreviewKeyDown;
					window.PreviewKeyUp -= Window_PreviewKeyUp;
				}
			};
		}

		protected sealed override Visual GetVisualChild(int index)
		{
			if (Child == null)
			{
				return LayeredChildren[index];
			}
			if (index == 0)
			{
				return Child;
			}
			return LayeredChildren[index - 1];
		}

		protected sealed override Size MeasureOverride(Size constraint)
		{
			if (Child != null)
			{
				Child.Measure(constraint);
			}
			foreach (UIElement layeredChild in LayeredChildren)
			{
				layeredChild.Measure(constraint);
			}
			if (Child != null)
			{
				return Child.DesiredSize;
			}
			return constraint;
		}

		protected sealed override Size ArrangeOverride(Size arrangeSize)
		{
			if (Child != null)
			{
				Child.Arrange(new Rect(arrangeSize));
			}
			foreach (UIElement layeredChild in LayeredChildren)
			{
				layeredChild.Arrange(new Rect(arrangeSize));
			}
			return arrangeSize;
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			if (visualAdded != null && visualAdded != Child)
			{
				if (LayeredChildren.Count == 1)
				{
					OnFirstAdded();
				}
				OnAdded((UIElement)visualAdded);
			}
			if (visualRemoved != null && visualRemoved != Child)
			{
				OnRemoved((UIElement)visualRemoved);
				if (LayeredChildren.Count <= 1)
				{
					OnLastRemoved();
				}
			}
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}

		private void OnFirstAdded()
		{
		}

		private void OnAdded(UIElement visual)
		{
			Mouse.Capture(null);
			Stylus.Capture(null);
			if (visual == null)
			{
				return;
			}
			FocusManager.SetIsFocusScope(visual, true);
			KeyboardNavigation.SetTabNavigation(visual, KeyboardNavigationMode.Cycle);
			KeyboardNavigation.SetControlTabNavigation(visual, KeyboardNavigationMode.Cycle);
			KeyboardNavigation.SetDirectionalNavigation(visual, KeyboardNavigationMode.Contained);
			Dispatcher.BeginInvoke((Action)(() =>
			{
				FrameworkElement obj = visual as FrameworkElement;
				if (obj != null && obj.IsLoaded && (Keyboard.FocusedElement == null || (Keyboard.FocusedElement as Visual)?.VisualAncestor((DependencyObject x) => x.GetType() == visual.GetType()) == null))
				{
					visual.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
				}
			}), DispatcherPriority.ContextIdle);
		}

		private void OnRemoved(UIElement visual)
		{
			visual.ClearValue(KeyboardNavigation.TabNavigationProperty);
			visual.ClearValue(KeyboardNavigation.ControlTabNavigationProperty);
			visual.ClearValue(KeyboardNavigation.DirectionalNavigationProperty);
			visual.ClearValue(FocusManager.IsFocusScopeProperty);
		}

		private void OnLastRemoved()
		{
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (LayeredChildren.Count > 0 && e.SystemKey == Key.F4 && Keyboard.Modifiers == ModifierKeys.Alt)
			{
				if (!e.IsRepeat)
				{
					(LayeredChildren[LayeredChildren.Count - 1] as ContentDialog)?.Hide();
				}
				e.Handled = true;
			}
		}

		private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (LayeredChildren.Count > 0 && e.SystemKey == Key.F4 && Keyboard.Modifiers == ModifierKeys.Alt)
			{
				e.Handled = true;
			}
		}
	}
}
