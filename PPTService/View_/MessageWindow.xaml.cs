using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace PPTService
{
	public partial class MessageWindow : Window, IComponentConnector
	{
		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageWindow), new PropertyMetadata((object)null));

		public string Message
		{
			get
			{
				return (string)GetValue(MessageProperty);
			}
			set
			{
				SetValue(MessageProperty, value);
			}
		}

		public MessageWindow(string message)
		{
			InitializeComponent();
			Message = message;
			DataContext = this;
			Loaded += MessageWindow_Loaded;
			Top = ViewHelper.MainWindow.Top;
			Left = ViewHelper.MainWindow.Left;
			Width = ViewHelper.MainWindow.Width;
			Height = ViewHelper.MainWindow.Height;
		}

		private void MessageWindow_Loaded(object sender, RoutedEventArgs e)
		{
			AdjustControls();
			(Resources["Style.ShowMessage"] as Storyboard)?.Begin();
		}

		private void Timeline_OnCompleted(object sender, EventArgs e)
		{
			Close();
		}

		private void AdjustControls()
		{
			if (Width > 3800.0)
			{
				PerchGrid.Height = 140.0;
				MessageTextBlock.FontSize = 32.0;
			}
			if (Width > 1440.0 && Width <= 3800.0)
			{
				PerchGrid.Height = 80.0;
				MessageTextBlock.FontSize = 18.0;
			}
			if (Width > 0.0 && Width <= 1400.0)
			{
				PerchGrid.Height = 50.0;
				MessageTextBlock.FontSize = 12.0;
			}
		}
	}
}
