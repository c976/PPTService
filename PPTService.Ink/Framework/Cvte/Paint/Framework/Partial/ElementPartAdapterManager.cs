using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.Partial
{
	public class ElementPartAdapterManager
	{
		public static readonly DependencyProperty ElementPartAdapterManagerProperty = DependencyProperty.RegisterAttached("ElementPartAdapterManager", typeof(ElementPartAdapterManager), typeof(ElementPartAdapterManager), new PropertyMetadata((object)null));

		private readonly Dictionary<Type, IElementPartialAdapter> _elementPartAdapters = new Dictionary<Type, IElementPartialAdapter>();

		public static void SetElementPartAdapterManager(DependencyObject element, ElementPartAdapterManager value)
		{
			element.SetValue(ElementPartAdapterManagerProperty, value);
		}

		public static ElementPartAdapterManager GetElementPartAdapterManager(DependencyObject element)
		{
			return (ElementPartAdapterManager)element.GetValue(ElementPartAdapterManagerProperty);
		}

		public void Add([NotNull] IElementPartialAdapter elementPartialAdapter)
		{
			if (elementPartialAdapter == null)
			{
				throw new ArgumentNullException("elementPartialAdapter");
			}
			Type type = elementPartialAdapter.GetType();
			if (_elementPartAdapters.ContainsKey(type))
			{
				throw new InvalidOperationException("相同的适配性只能添加一个");
			}
			_elementPartAdapters.Add(type, elementPartialAdapter);
		}

		[CanBeNull]
		public T GetAdapterByElementType<T>([NotNull] Element element) where T : IElementPartialAdapter
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			foreach (T item in _elementPartAdapters.Values.OfType<T>())
			{
				if (item.ElementType == element.GetType())
				{
					return item;
				}
			}
			return default(T);
		}
	}
}
