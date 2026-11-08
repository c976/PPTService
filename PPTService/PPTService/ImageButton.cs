using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PPTService
{
	public class ImageButton : Button
	{
		private Storyboard _storyboard;

		public static readonly DependencyProperty PressedSourceProperty;

		public static readonly DependencyProperty NormalSourceProperty;

		public ImageSource PressedSource
		{
			get
			{
				return (ImageSource)GetValue(PressedSourceProperty);
			}
			set
			{
				SetValue(PressedSourceProperty, value);
			}
		}

		public ImageSource NormalSource
		{
			get
			{
				return (ImageSource)GetValue(NormalSourceProperty);
			}
			set
			{
				SetValue(NormalSourceProperty, value);
			}
		}

		static ImageButton()
		{
			PressedSourceProperty = DependencyProperty.Register("PressedSource", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata((object)null));
			NormalSourceProperty = DependencyProperty.Register("NormalSource", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata((object)null));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			Grid grid = GetTemplateChild("BackgroundImageGrid") as Grid;
			if (grid != null)
			{
				_storyboard = grid.Resources["ButtonPressedStoryBoard"] as Storyboard;
				if (_storyboard != null)
				{
					UpdateTemplateImageSource();
				}
			}
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == NormalSourceProperty || e.Property == PressedSourceProperty || e.Property == UIElement.IsEnabledProperty || e.Property == UIElement.IsMouseOverProperty || e.Property == UIElement.IsStylusOverProperty || e.Property == ButtonBase.IsPressedProperty)
			{
				UpdateTemplateImageSource();
			}
		}

		private void UpdateTemplateImageSource()
		{
			if (_storyboard != null && IsPressed)
			{
				_storyboard.Begin();
			}
		}
	}
}
