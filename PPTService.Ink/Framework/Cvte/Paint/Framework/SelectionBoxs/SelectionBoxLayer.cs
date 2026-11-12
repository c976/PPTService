using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Cvte.Paint.Framework.SelectionBoxs
{
	public class SelectionBoxLayer
	{
		private class SelectionAdorner : Adorner
		{
			private class ChildrenCollection : Collection<Visual>
			{
				private readonly SelectionAdorner _parent;

				public ChildrenCollection(SelectionAdorner parent)
				{
					_parent = parent;
				}

				protected override void InsertItem(int index, Visual item)
				{
					base.InsertItem(index, item);
					_parent.AddVisualChild(item);
					_parent.InvalidateMeasure();
				}

				protected override void RemoveItem(int index)
				{
					Visual child = base[index];
					base.RemoveItem(index);
					_parent.RemoveVisualChild(child);
				}

				protected override void SetItem(int index, Visual item)
				{
					Visual child = base[index];
					_parent.RemoveVisualChild(child);
					base.SetItem(index, item);
					_parent.AddVisualChild(item);
				}

				protected override void ClearItems()
				{
					foreach (Visual item in Items)
					{
						_parent.RemoveVisualChild(item);
					}
					base.ClearItems();
				}
			}

			private readonly ChildrenCollection _children;

			internal IList<Visual> Children => _children;

			protected override int VisualChildrenCount => _children.Count;

			public SelectionAdorner(Slide slide)
				: base(slide)
			{
				_children = new ChildrenCollection(this);
			}

			protected override Visual GetVisualChild(int index)
			{
				return _children[index];
			}

			protected override Size MeasureOverride(Size constraint)
			{
				Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
				double num = (double.IsInfinity(constraint.Width) ? 0.0 : constraint.Width);
				double num2 = (double.IsInfinity(constraint.Height) ? 0.0 : constraint.Height);
				foreach (UIElement item in _children.OfType<UIElement>())
				{
					item.Measure(availableSize);
					num = ((item.DesiredSize.Width > num) ? item.DesiredSize.Width : num);
					num2 = ((item.DesiredSize.Height > num2) ? item.DesiredSize.Height : num2);
				}
				return new Size(num, num2);
			}

			protected override Size ArrangeOverride(Size finalSize)
			{
				foreach (UIElement item in _children.OfType<UIElement>())
				{
					item.Arrange(new Rect(finalSize));
				}
				return finalSize;
			}
		}

		private AdornerLayer _innerLayer;

		private AdornerLayer _outerLayer;

		private SelectionAdorner _innerAdorner;

		private SelectionAdorner _outerAdorner;

		internal bool IsAttached { get; private set; }

		internal IEnumerable<Visual> Children
		{
			get
			{
				if (_innerAdorner != null && _outerAdorner != null)
				{
					return _innerAdorner.Children.Union(_outerAdorner.Children);
				}
				return Enumerable.Empty<Visual>();
			}
		}

		public IList<Visual> InnerVisuals => _innerAdorner?.Children;

		public IList<Visual> OuterVisuals => _outerAdorner?.Children;

		internal void Attach(Slide slide)
		{
			if (!TryAttach(slide))
			{
				throw new ArgumentException("Cannot attach to a slide which is not on a visual tree.", "slide");
			}
		}

		public void SetOutAdornerVisibility(Visibility visiblity)
		{
			if (_outerAdorner == null)
			{
				throw new ArgumentException("OutAdorner is not attach to a slide which is not on a visual tree.", "_outerLayer");
			}
			_outerAdorner.Visibility = visiblity;
		}

		public void SetOutAdornerOpacity(double opacity)
		{
			if (_outerAdorner == null)
			{
				throw new ArgumentException("OutAdorner is not attach to a slide which is not on a visual tree.", "_outerLayer");
			}
			_outerAdorner.Opacity = opacity;
		}

		internal bool TryAttach(Slide slide)
		{
			_innerLayer = AdornerLayer.GetAdornerLayer(slide.Canvas);
			_outerLayer = AdornerLayer.GetAdornerLayer(slide);
			if (_innerLayer == null || _outerLayer == null)
			{
				return false;
			}
			_innerAdorner = new SelectionAdorner(slide);
			_outerAdorner = new SelectionAdorner(slide);
			_innerLayer.Add(_innerAdorner);
			_outerLayer.Add(_outerAdorner);
			IsAttached = true;
			return true;
		}

		internal void Detach()
		{
			_innerLayer.Remove(_innerAdorner);
			_outerLayer.Remove(_outerAdorner);
			_innerAdorner.Children.Clear();
			_outerAdorner.Children.Clear();
			IsAttached = false;
		}

		public void UpdateLayout()
		{
			_innerAdorner?.UpdateLayout();
			_outerAdorner?.UpdateLayout();
		}
	}
}
