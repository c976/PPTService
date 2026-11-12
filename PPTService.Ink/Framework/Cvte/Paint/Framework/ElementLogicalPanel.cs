using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	internal class ElementLogicalPanel : FrameworkElement
	{
		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(ElementLogicalPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		private readonly UIElement _parent;

		private readonly Action<Visual> _addVisualChild;

		private readonly Action<Visual> _removeVisualChild;

		private bool _isEmpty = true;

		public Brush Background
		{
			get
			{
				return (Brush)GetValue(BackgroundProperty);
			}
			set
			{
				SetValue(BackgroundProperty, value);
			}
		}

		public UIElementCollection Children { get; }

		internal bool ContainsValidContent => Children.OfType<UIElement>().Any((UIElement x) =>
		{
			ElementLogicalPanel elementLogicalPanel = x as ElementLogicalPanel;
			return (elementLogicalPanel != null && elementLogicalPanel.ContainsValidContent) || !(x is ElementLogicalPanel);
		});

		protected override int VisualChildrenCount => Children.Count;

		protected override IEnumerator LogicalChildren => Children.GetEnumerator();

		public ElementLogicalPanel(FrameworkElement logicalParent, Action<Visual> addVisualChild, Action<Visual> removeVisualChild)
		{
			Children = new NotificationUIElementCollection(this, logicalParent, OnChildrenChanged);
			if (logicalParent == null)
			{
				throw new ArgumentNullException("logicalParent");
			}
			_parent = logicalParent;
			if (addVisualChild == null)
			{
				throw new ArgumentNullException("addVisualChild");
			}
			_addVisualChild = addVisualChild;
			if (removeVisualChild == null)
			{
				throw new ArgumentNullException("removeVisualChild");
			}
			_removeVisualChild = removeVisualChild;
		}

		protected override Visual GetVisualChild(int index)
		{
			return Children[index];
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			double num = 0.0;
			double num2 = 0.0;
			foreach (UIElement item in Children.OfType<UIElement>())
			{
				item.Measure(availableSize);
				Size desiredSize = item.DesiredSize;
				num = Math.Max(desiredSize.Width, num);
				num2 = Math.Max(desiredSize.Height, num2);
			}
			return new Size(num, num2);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (UIElement item in Children.OfType<UIElement>())
			{
				item.Arrange(new Rect(finalSize));
			}
			return finalSize;
		}

		protected override void OnRender(DrawingContext dc)
		{
			dc.DrawRectangle(Background, null, new Rect(0.0, 0.0, ActualWidth, ActualHeight));
			UpdateVisualTree();
		}

		private void OnChildrenChanged(object sender, EventArgs e)
		{
			UpdateVisualTree();
		}

		private void UpdateVisualTree()
		{
			if (!ContainsValidContent && Background == null)
			{
				if (!_isEmpty)
				{
					try
					{
						OnChildrenDisappeared();
					}
					finally
					{
						_isEmpty = true;
					}
				}
			}
			else if (_isEmpty)
			{
				try
				{
					OnChildrenAppeared();
				}
				finally
				{
					_isEmpty = false;
				}
			}
		}

		private void OnChildrenAppeared()
		{
			_addVisualChild(this);
		}

		private void OnChildrenDisappeared()
		{
			_removeVisualChild(this);
		}
	}
}
