using System;
using System.Windows;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows.Localization
{
	public class XamlLangResource : DependencyObject
	{
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(XamlLangResource), new PropertyMetadata(null, OnSourceChanged));

		public Uri Source
		{
			get
			{
				return (Uri)GetValue(SourceProperty);
			}
			set
			{
				SetValue(SourceProperty, value);
			}
		}

		private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			XamlLangResource xamlLangResource = (XamlLangResource)d;
			if (e.OldValue != null)
			{
				throw new InvalidOperationException("Lang.Sources 一旦加入则不可撤回。");
			}
			if (e.NewValue != null)
			{
				xamlLangResource.OnSourceChanged((Uri)e.NewValue);
			}
		}

		private void OnSourceChanged([NotNull] Uri source)
		{
			Lang.Sources.Add(new XamlLanguageSource(source));
		}
	}
}
