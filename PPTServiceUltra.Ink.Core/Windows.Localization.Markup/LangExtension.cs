using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PPTServiceUltra.Ink.Core.Windows.Localization.Markup
{
	public sealed class LangExtension : MarkupExtension
	{
		[ConstructorArgument("key")]
		public ReferencedLang Reference { get; set; }

		public LangConverterFunc Converter { get; set; }

		public LangExtension()
		{
		}

		public LangExtension(ReferencedLang key)
		{
			Reference = key;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Reference == null)
			{
				throw new ArgumentNullException("必须为 " + GetType().Name + " 指定一个语言的 Key。");
			}
			bool isInDesignMode = DesignerProperties.GetIsInDesignMode(Reference);
			string key = Reference.Key;
			IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValueTarget == null)
			{
				return Convert(Reference.UpdateValue());
			}
			if (provideValueTarget.TargetObject != null && provideValueTarget.TargetObject.ToString().EndsWith("SharedDp"))
			{
				return this;
			}
			if (provideValueTarget.TargetObject is SetterBase)
			{
				throw new NotSupportedException("属性 Setter.Value 不支持 LangExtension，请修改实现方式为：创建 LangBindingSource 对象作为资源，然后使用 Binding 标记绑定到此资源。");
			}
			if (isInDesignMode)
			{
				if (Lang.Languages.Current != null)
				{
					string value;
					if (Lang.TryGet(key, out value))
					{
						return Convert(value);
					}
					throw new KeyNotFoundException("区域 " + Lang.Current.Name + " 中找不到语言 " + key + "。");
				}
				return "$" + key + "$";
			}
			return Convert(ProvideBindingValue(provideValueTarget.TargetObject, provideValueTarget.TargetProperty));
		}

		private string ProvideBindingValue(object target, object property)
		{
			DependencyObject dependencyObject = target as DependencyObject;
			DependencyProperty dependencyProperty = property as DependencyProperty;
			string result = Reference.UpdateValue();
			if (dependencyObject == null || dependencyProperty == null)
			{
				return result;
			}
			Binding binding = new Binding(ReferencedLang.ValueProperty.Name)
			{
				Source = Reference,
				Mode = BindingMode.OneWay
			};
			BindingOperations.SetBinding(dependencyObject, dependencyProperty, binding);
			return result;
		}

		private object Convert(string source)
		{
			if (Converter != null)
			{
				return Converter(Reference.Key, source, Lang.Current);
			}
			return source;
		}
	}
}
