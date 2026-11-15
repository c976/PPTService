using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows
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
			Binding item = new Binding
			{
				RelativeSource = new RelativeSource
				{
					Mode = RelativeSourceMode.Self
				}
			};
			Binding binding = new Binding
			{
				BindsDirectlyToSource = BindsDirectlyToSource,
				Mode = BindingMode.OneWay,
				Path = Path,
				XPath = XPath
			};
			if (ElementName != null)
			{
				binding.ElementName = ElementName;
			}
			else if (RelativeSource != null)
			{
				binding.RelativeSource = RelativeSource;
			}
			else if (Source != null)
			{
				binding.Source = Source;
			}
			DependencyProperty targetProperty = serviceProvider.GetTargetProperty<DependencyProperty>();
			ResourceKeyToResourceConverter converter = new ResourceKeyToResourceConverter
			{
				ResourceKeyConverter = Converter,
				ConverterParameter = ConverterParameter,
				StringFormat = StringFormat,
				FindResource = (FrameworkElement element, object o) => (element == null || o == null) ? null : ProvideValue(element, targetProperty, o)
			};
			return new MultiBinding
			{
				Converter = converter,
				Bindings = 
				{
					(BindingBase)item,
					(BindingBase)binding
				}
			}.ProvideValue(serviceProvider);
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
