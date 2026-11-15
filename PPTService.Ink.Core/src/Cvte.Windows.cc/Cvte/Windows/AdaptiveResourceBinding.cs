using System;
using System.Windows;

namespace PPTService.Ink.Core.Windows
{
	public class AdaptiveResourceBinding : ResourceKeyBindingExtension
	{
		public IAdaptiveResourceInfo AdaptiveInfo { get; set; }

		public AdaptiveResourceBinding()
		{
		}

		public AdaptiveResourceBinding(PropertyPath path)
			: base(path)
		{
		}

		protected override object ProvideValue(FrameworkElement targetElement, DependencyProperty targetProperty, object resourceKey)
		{
			if (AdaptiveInfo == null)
			{
				throw new ArgumentNullException("AdaptiveInfo", "必须指定 AdaptiveInfo 的值才能进行自适应 UI 的绑定。");
			}
			return AdaptiveResourceExtension.ProvideValue(targetElement, targetProperty, resourceKey, AdaptiveInfo);
		}
	}
}
