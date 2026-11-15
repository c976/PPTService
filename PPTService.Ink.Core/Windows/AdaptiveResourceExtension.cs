using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows
{
	public class AdaptiveResourceExtension : MarkupExtension
	{
		[ConstructorArgument("resourceKey")]
		public string ResourceKey { get; set; }

		public IAdaptiveResourceInfo AdaptiveInfo { get; set; }

		public AdaptiveResourceExtension()
		{
		}

		public AdaptiveResourceExtension([NotNull] string resourceKey)
		{
			if (resourceKey == null)
			{
				throw new ArgumentNullException("resourceKey");
			}
			ResourceKey = resourceKey;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValueTarget == null)
			{
				return null;
			}
			if (provideValueTarget.TargetObject.ToString().EndsWith("SharedDp"))
			{
				return this;
			}
			if (provideValueTarget.TargetObject is SetterBase)
			{
				throw new NotSupportedException("属性 Setter.Value 不支持 AdaptiveResourceExtension，请尽量避免。");
			}
			if (AdaptiveInfo == null)
			{
				throw new ArgumentNullException("AdaptiveInfo", "必须指定 AdaptiveInfo 的值才能进行自适应 UI 的绑定。");
			}
			FrameworkElement frameworkElement = provideValueTarget.TargetObject as FrameworkElement;
			DependencyProperty dependencyProperty = provideValueTarget.TargetProperty as DependencyProperty;
			if (frameworkElement == null || dependencyProperty == null)
			{
				throw new NotSupportedException("AdaptiveResourceExtension 仅支持设置在 FrameworkElement 的 DependencyProperty 上。");
			}
			return ProvideValue(frameworkElement, dependencyProperty, ResourceKey, AdaptiveInfo);
		}

		internal static object ProvideValue(FrameworkElement targetElement, DependencyProperty targetProperty, object resourceKey, IAdaptiveResourceInfo adaptiveInfo)
		{
			IAdaptiveResourceInfo2 adaptiveInfo2 = adaptiveInfo as IAdaptiveResourceInfo2;
			AdaptiveUI adaptiveUI = ((adaptiveInfo2 != null) ? new AdaptiveUI(new AdaptiveTriggerCollection(adaptiveInfo2.AdaptiveResourceFormats2.Select((KeyValuePair<Size, string> pair) => CreateAdaptiveTrigger(new AdaptiveCondition(new SetterBaseCollection
			{
				new AdaptiveSetter
				{
					Target = targetElement,
					Property = targetProperty,
					InternalValue = () => targetElement.TryFindResource(string.Format(pair.Value, resourceKey)) ?? targetElement.FindResource(resourceKey)
				}
			}), adaptiveInfo2, pair.Key)).ToList())) : new AdaptiveUI(new AdaptiveTriggerCollection(adaptiveInfo.AdaptiveResourceFormats.Select((KeyValuePair<double, string> pair) => CreateAdaptiveTrigger(new AdaptiveCondition(new SetterBaseCollection
			{
				new AdaptiveSetter
				{
					Target = targetElement,
					Property = targetProperty,
					InternalValue = () => targetElement.TryFindResource(string.Format(pair.Value, resourceKey)) ?? targetElement.FindResource(resourceKey)
				}
			}), adaptiveInfo, pair.Key)).ToList())));
			AdaptiveUI.SetAdaptive(targetElement, adaptiveUI);
			FrameworkElement window = adaptiveUI.FindHwndRootElement(targetElement);
			if (window != null && !object.Equals(window.ActualWidth, 0.0) && !object.Equals(window.ActualHeight, 0.0))
			{
				return (((adaptiveUI.Triggers.WidthSortedList.LastOrDefault((KeyValuePair<double, AdaptiveCondition> pair) => pair.Key <= window.ActualWidth).Value ?? adaptiveUI.Triggers.HeightSortedList.LastOrDefault((KeyValuePair<double, AdaptiveCondition> pair) => pair.Key <= window.ActualHeight).Value) ?? adaptiveUI.Triggers.SizeSortedList.LastOrDefault(( KeyValuePair<double, ValueTuple<Size, AdaptiveCondition>> pair) => pair.Value.Item1.Width <= window.ActualWidth && pair.Value.Item1.Height <= window.ActualHeight).Value.Item2)?.Setters.First() as AdaptiveSetter)?.GetValue();
			}
			return targetElement.TryFindResource(resourceKey);
		}

		private static AdaptiveCondition CreateAdaptiveTrigger(AdaptiveCondition condition, IAdaptiveResourceInfo info, double value)
		{
			info.AssignAdaptiveValue(condition, value);
			return condition;
		}

		private static AdaptiveCondition CreateAdaptiveTrigger(AdaptiveCondition condition, IAdaptiveResourceInfo2 info, Size value)
		{
			info.AssignAdaptiveValue(condition, value);
			return condition;
		}
	}
}
