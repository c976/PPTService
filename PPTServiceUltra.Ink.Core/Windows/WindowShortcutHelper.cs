using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public class WindowShortcutHelper : DependencyObject
	{
		public static readonly DependencyProperty AltF4EnabledProperty = DependencyProperty.RegisterAttached("AltF4Enabled", typeof(bool), typeof(WindowShortcutHelper), new PropertyMetadata(true, OnPropertyChangedCallback));

		private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Window window = d as Window;
			if (window != null)
			{
				bool value = (bool)e.NewValue;
				SetAltF4Enabled(window, value);
			}
		}

		public static void SetAltF4Enabled([NotNull] Window window, bool value)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window");
			}
			window.SetValue(AltF4EnabledProperty, value);
			if (!value)
			{
				window.PreviewKeyDown += Window_PreviewKeyDown;
			}
			else
			{
				window.PreviewKeyDown -= Window_PreviewKeyDown;
			}
		}

		public static bool GetAltF4Enabled([NotNull] Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window");
			}
			object value = window.GetValue(AltF4EnabledProperty);
			if (value is bool)
			{
				return (bool)value;
			}
			return false;
		}

		private static void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt && e.SystemKey == Key.F4)
			{
				e.Handled = true;
			}
		}
	}
}
