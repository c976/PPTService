using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Cvte.Diagnostics.Annotations;
using Cvte.Windows.Localization;

namespace Cvte.Paint.Framework.Elements
{
	[Transform(PermittedTransforms.None)]
	internal class UtilitiesPanelElement : Element, ITransformedBounds, IPermittedTransform
	{
		private Canvas _elementsHost;

		public int Count => Children.Count;

		public new UIElementCollection Children => _elementsHost.Children;

		Rect ITransformedBounds.TransformedBounds { get; } = new Rect(new Size(1.0, 1.0));

		PermittedTransforms IPermittedTransform.PermittedTransform => PermittedTransforms.None;

		static UtilitiesPanelElement()
		{
			ElementSelectionExtensions.CanSelectProperty.OverrideMetadata(typeof(UtilitiesPanelElement), new PropertyMetadata(false));
			Lang.Sources.Add(new DictionaryLanguageSource { [Lang.Current] = new Dictionary<string, string> { { "Lang.Element.Type.UtilitiesPanelElement", "UtilitiesPanelElement" } } });
		}

		internal UtilitiesPanelElement()
		{
			_elementsHost = new Canvas();
			base.Children.Add(_elementsHost);
			IsHitTestVisible = false;
			this.SetIsLocked(true);
			Loaded += PanelElement_Loaded;
			Unloaded += PanelElement_Unloaded;
		}

		private void PanelElement_Unloaded(object sender, RoutedEventArgs e)
		{
			BindingOperations.ClearBinding(this, FrameworkElement.WidthProperty);
			BindingOperations.ClearBinding(this, FrameworkElement.HeightProperty);
		}

		private void PanelElement_Loaded(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = Parent as FrameworkElement;
			if (frameworkElement != null)
			{
				BindingOperations.SetBinding(this, FrameworkElement.WidthProperty, new Binding(FrameworkElement.ActualWidthProperty.Name)
				{
					Source = frameworkElement,
					Mode = BindingMode.OneWay
				});
				BindingOperations.SetBinding(this, FrameworkElement.HeightProperty, new Binding(FrameworkElement.ActualHeightProperty.Name)
				{
					Source = frameworkElement,
					Mode = BindingMode.OneWay
				});
			}
		}

		public void Add([NotNull] UIElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			Children.Add(element);
		}

		public void Remove(UIElement element)
		{
			Children.Remove(element);
		}

		public void Clear()
		{
			Children.Clear();
		}

		public void AddRange([NotNull] IEnumerable<UIElement> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			foreach (UIElement element in elements)
			{
				Add(element);
			}
		}
	}
}
