using System.Windows;
using System.Windows.Controls;

namespace PPTServiceUltra.Ink.Core.Windows.Controls
{
	public class LoadingView : Control
	{
		public static readonly DependencyPropertyKey IsLoadingPropertyKey;

		public static readonly DependencyProperty IsLoadingProperty;

		public bool IsLoading
		{
			get
			{
				return (bool)GetValue(IsLoadingProperty);
			}
			internal set
			{
				SetValue(IsLoadingPropertyKey, value);
			}
		}

		static LoadingView()
		{
			IsLoadingPropertyKey = DependencyProperty.RegisterReadOnly("IsLoading", typeof(bool), typeof(LoadingView), new PropertyMetadata(false));
			IsLoadingProperty = IsLoadingPropertyKey.DependencyProperty;
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingView), new FrameworkPropertyMetadata(typeof(LoadingView)));
		}
	}
}
