using System;
using System.ComponentModel;
using System.Windows;

namespace PPTService.Ink.Core.Windows
{
	[Obsolete("请使用SafeWindow")]
	public class WindowBugExtensions
	{
		public static readonly DependencyProperty ShouldFixProperty = DependencyProperty.RegisterAttached("ShouldFix", typeof(bool), typeof(WindowBugExtensions), new PropertyMetadata(false, PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
		{
			if (s.IsDesignTimeWindow())
			{
				return;
			}
			object newValue = e.NewValue;
			if (newValue is bool)
			{
				if (!(bool)newValue)
				{
					throw new ArgumentException("你在逗我，不需要修复还设置");
				}
				Window window = s as Window;
				if (window == null)
				{
					throw new ArgumentException("你在逗我，这个值只可以设置在Window，不能设置在其他的属性", e.Property?.Name);
				}
				FixShowAnotherAppWhenCloseDialogWindow(window);
			}
		}

		private static void FixShowAnotherAppWhenCloseDialogWindow(Window window)
		{
			window.Closing += (object s, CancelEventArgs e) =>
			{
				window.Owner?.Activate();
			};
		}

		public static void SetShouldFix(DependencyObject element, bool value)
		{
			element.SetValue(ShouldFixProperty, value);
		}

		public static bool GetShouldFix(DependencyObject element)
		{
			object value = element.GetValue(ShouldFixProperty);
			if (value is bool)
			{
				return (bool)value;
			}
			return false;
		}
	}
}
