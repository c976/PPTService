using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Windows
{
	public class ResourceKeyBindingExtension : MarkupExtension
	{
		[DefaultValue(false)]
		public bool BindsDirectlyToSource { get; set; }

		[DefaultValue(null)]
		public IValueConverter Converter { get; set; }

		[DefaultValue(null)]
		public object ConverterParameter { get; set; }

		[DefaultValue(null)]
		public string ElementName { get; set; }

		[ConstructorArgument("path")]
		public PropertyPath Path { get; set; }

		[DefaultValue(null)]
		public RelativeSource RelativeSource { get; set; }

		public object Source { get; set; }

		[DefaultValue(null)]
		public string StringFormat { get; set; }

		[DefaultValue(null)]
		public string XPath { get; set; }

		public sealed override object ProvideValue(IServiceProvider serviceProvider)
		{
			Binding binding = new Binding();
			binding.BindsDirectlyToSource = BindsDirectlyToSource;
			binding.Mode = BindingMode.OneWay;
			binding.Path = Path;
			binding.XPath = XPath;
			Binding binding2 = binding;
			if (ElementName != null)
			{
				binding2.ElementName = ElementName;
			}
			else if (RelativeSource != null)
			{
				binding2.RelativeSource = RelativeSource;
			}
			else if (Source != null)
			{
				binding2.Source = Source;
			}
			Binding binding3 = new Binding();
			binding3.RelativeSource = new RelativeSource
			{
				Mode = RelativeSourceMode.Self
			};
			Binding item = binding3;
			DependencyProperty targetProperty = serviceProvider.GetTargetProperty<DependencyProperty>();
			ResourceKeyToResourceConverter resourceKeyToResourceConverter = new ResourceKeyToResourceConverter();
			resourceKeyToResourceConverter.ResourceKeyConverter = Converter;
			resourceKeyToResourceConverter.ConverterParameter = ConverterParameter;
			resourceKeyToResourceConverter.StringFormat = StringFormat;
			resourceKeyToResourceConverter.FindResource = (FrameworkElement element, object o) => (element != null && o != null) ? ProvideValue(element, targetProperty, o) : null;
			ResourceKeyToResourceConverter converter = resourceKeyToResourceConverter;
			MultiBinding multiBinding = new MultiBinding();
			multiBinding.Converter = converter;
			MultiBinding multiBinding2 = multiBinding;
			multiBinding2.Bindings.Add(item);
			multiBinding2.Bindings.Add(binding2);
			return multiBinding2.ProvideValue(serviceProvider);
		}

		protected virtual object ProvideValue([NotNull] FrameworkElement targetElement, [CanBeNull] DependencyProperty targetProperty, [NotNull] object resourceKey)
		{
			return targetElement.TryFindResource(resourceKey);
		}

		public ResourceKeyBindingExtension()
		{
		}

		public ResourceKeyBindingExtension(PropertyPath path)
		{
			Path = path;
		}
	}
}
