using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Cvte.ComponentModel;

namespace Cvte.Paint.Framework.Controls
{
	public abstract class BoardUnitExtension : MarkupExtension
	{
		protected abstract object Value { get; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValueTarget == null)
			{
				throw new InvalidOperationException("只能在 XAML 中使用 BoardUnitExtension。");
			}
			if (provideValueTarget.TargetObject.ToString().EndsWith("SharedDp"))
			{
				return this;
			}
			if (provideValueTarget.TargetObject is Setter)
			{
				throw new InvalidOperationException("请不要在 Setter 中使用 BoardUnitExtension 类型。如需使用动态值，请使用：{Binding Value, Source={StaticResource XXValueSource}}。");
			}
			return BindingAndProvideValue(serviceProvider, (DependencyObject)provideValueTarget.TargetObject, (DependencyProperty)provideValueTarget.TargetProperty);
		}

		protected object BindingAndProvideValue(IServiceProvider serviceProvider, DependencyObject d, DependencyProperty p)
		{
			if (DebuggingProperties.IsDebug && DesignerProperties.GetIsInDesignMode(d))
			{
				return Value;
			}
			Binding binding = new Binding(Board.ContentScaleProperty.Name)
			{
				RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
				{
					AncestorType = typeof(Board)
				},
				Mode = BindingMode.OneWay,
				Converter = BoardContentScaleConverter.Instance,
				ConverterParameter = Value
			};
			BindingOperations.SetBinding(d, p, binding);
			return binding.ProvideValue(serviceProvider);
		}
	}
}
