using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PPTService
{
	public class ControlHelper : DependencyObject
	{
		public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.RegisterAttached("ContentMargin", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(null));

		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlHelper), new PropertyMetadata(default(CornerRadius)));

		public static readonly DependencyProperty NormalImageProperty = DependencyProperty.RegisterAttached("NormalImage", typeof(ImageSource), typeof(ControlHelper), new PropertyMetadata(null));

		public static readonly DependencyProperty PressedImageProperty = DependencyProperty.RegisterAttached("PressedImage", typeof(ImageSource), typeof(ControlHelper), new PropertyMetadata(null));

		public static readonly DependencyProperty CheckedImageProperty = DependencyProperty.RegisterAttached("CheckedImage", typeof(ImageSource), typeof(ControlHelper), new PropertyMetadata(null));

		public static readonly DependencyProperty AltF4EnabledProperty = DependencyProperty.RegisterAttached("AltF4Enabled", typeof(bool), typeof(ControlHelper), new PropertyMetadata(true));

		private static readonly DependencyProperty IsAltDownProperty = DependencyProperty.RegisterAttached("IsAltDown", typeof(bool), typeof(ControlHelper), new PropertyMetadata(false));

		public static Thickness GetContentMargin(DependencyObject obj)
		{
			return (Thickness)obj.GetValue(ContentMarginProperty);
		}

		public static void SetContentMargin(DependencyObject obj, Thickness value)
		{
			obj.SetValue(ContentMarginProperty, value);
		}

		public static CornerRadius GetCornerRadius(DependencyObject obj)
		{
			return (CornerRadius)obj.GetValue(CornerRadiusProperty);
		}

		public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
		{
			obj.SetValue(CornerRadiusProperty, value);
		}

		public static ImageSource GetNormalImage(DependencyObject obj)
		{
			return (ImageSource)obj.GetValue(PressedImageProperty);
		}

		public static void SetNormalImage(DependencyObject obj, ImageSource value)
		{
			obj.SetValue(NormalImageProperty, value);
		}

		public static ImageSource GetPressedImage(DependencyObject obj)
		{
			return (ImageSource)obj.GetValue(PressedImageProperty);
		}

		public static void SetPressedImage(DependencyObject obj, ImageSource value)
		{
			obj.SetValue(PressedImageProperty, value);
		}

		public static ImageSource GetCheckedImage(DependencyObject obj)
		{
			return (ImageSource)obj.GetValue(CheckedImageProperty);
		}

		public static void SetCheckedImage(DependencyObject obj, ImageSource value)
		{
			obj.SetValue(CheckedImageProperty, value);
		}

		public static void SetAltF4Enabled(Window window, bool value)
		{
			window.SetValue(AltF4EnabledProperty, value);
			if (!value)
			{
				window.PreviewKeyDown += Window_PreviewKeyDown;
				window.PreviewKeyUp += Window_PreviewKeyUp;
			}
			else
			{
				window.PreviewKeyDown -= Window_PreviewKeyDown;
				window.PreviewKeyUp -= Window_PreviewKeyUp;
			}
		}

		public static bool GetAltF4Enabled(Window window)
		{
			return (bool)window.GetValue(AltF4EnabledProperty);
		}

		private static void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			Window window = sender as Window;
			if (window != null)
			{
				if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
				{
					SetIsAltDown(window, true);
				}
				else if (e.SystemKey == Key.F4 && GetIsAltDown(window))
				{
					e.Handled = true;
				}
			}
		}

		private static void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			Window window = sender as Window;
			if (window != null && (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt))
			{
				SetIsAltDown(window, false);
			}
		}

		private static void SetIsAltDown(DependencyObject element, bool value)
		{
			element.SetValue(IsAltDownProperty, value);
		}

		private static bool GetIsAltDown(DependencyObject element)
		{
			return (bool)element.GetValue(IsAltDownProperty);
		}
	}
}
