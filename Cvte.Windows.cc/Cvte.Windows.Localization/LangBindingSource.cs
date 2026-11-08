using System.Diagnostics;
using System.Windows;

namespace Cvte.Windows.Localization
{
	[DebuggerDisplay("{Key}, {Value}")]
	public sealed class LangBindingSource : DependencyObject
	{
		public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(LangBindingSource), new PropertyMetadata(null, PropertyChangedCallback));

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(LangBindingSource), new PropertyMetadata((object)null));

		public string Key
		{
			get
			{
				return (string)GetValue(KeyProperty);
			}
			set
			{
				SetValue(KeyProperty, value);
			}
		}

		public string Value
		{
			get
			{
				return (string)GetValue(ValueProperty);
			}
			set
			{
				SetValue(ValueProperty, value);
			}
		}

		private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((LangBindingSource)sender).ReferenceLang();
		}

		private void ReferenceLang()
		{
			Lang.Reference(Key, this, ValueProperty);
		}
	}
}
