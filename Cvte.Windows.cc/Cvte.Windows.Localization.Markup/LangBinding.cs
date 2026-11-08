using System;
using System.Diagnostics;
using System.Windows;

namespace Cvte.Windows.Localization.Markup
{
	public class LangBinding : ResourceKeyBindingExtension
	{
		public LangBinding()
		{
		}

		public LangBinding(PropertyPath path)
			: base(path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
		}

		[DebuggerStepThrough]
		protected override object ProvideValue(FrameworkElement targetElement, DependencyProperty targetProperty, object resourceKey)
		{
			string key = resourceKey.ToString();
			if (targetProperty == null)
			{
				string value;
				Lang.TryGet(key, out value);
				return value;
			}
			return Lang.Reference(key, targetElement, targetProperty);
		}
	}
}
