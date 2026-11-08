using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace PPTService
{
	public partial class UpdateWindow : Window, IComponentConnector
	{
		public string NewVersion
		{
			set
			{
				VersionRun.Text = value;
			}
		}

		public UpdateWindow()
		{
			InitializeComponent();
			MouseLeftButtonDown += (object s, MouseButtonEventArgs e) =>
			{
				if (e.ButtonState == MouseButtonState.Pressed)
				{
					DragMove();
				}
			};
		}

		private void CloseButton_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
		{
			AutoUpdater.Update();
			Close();
		}

	}
}
