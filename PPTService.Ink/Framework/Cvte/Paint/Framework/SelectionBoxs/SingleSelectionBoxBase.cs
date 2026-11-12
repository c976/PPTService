using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Cvte.Linq;
using Cvte.Windows.Mathematics;

namespace Cvte.Paint.Framework.SelectionBoxs
{
	[ContentProperty("Children")]
	public class SingleSelectionBoxBase : FrameworkElement, ISelectionOperator, ISlideLayeredOperator, IDisposable, ISelectionBox
	{
		private readonly TranslateTransform _translate;

		private Thickness _padding;

		private Rect _bounds;

		private IList<Element> _selectedElement;

		public static readonly DependencyProperty AttachmentProperty = SelectionBoxBase.AttachmentProperty.AddOwner(typeof(SingleSelectionBoxBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnAttachmentChanged));

		public IList<Element> SelectedElements
		{
			get
			{
				return _selectedElement;
			}
			set
			{
				_selectedElement = value;
				UpdateBounds();
				SelectedElementChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public bool IsTransforming { get; private set; }

		public ISelectionBox Box => this;

		public UIElement Attachment
		{
			get
			{
				return (UIElement)GetValue(AttachmentProperty);
			}
			set
			{
				SetValue(AttachmentProperty, value);
			}
		}

		public UIElementCollection Children { get; private set; }

		public Rect Bounds
		{
			get
			{
				return _bounds;
			}
			set
			{
				if (!Nearly.Equals(_bounds, value, 0.01))
				{
					if (!Nearly.Equals(_bounds.Size, value.Size, 0.01))
					{
						_translate.X = value.X;
						_translate.Y = value.Y;
						InvalidateMeasure();
					}
					else if (!Nearly.Equals(_bounds.Location, value.Location, 0.01))
					{
						_translate.X = value.X;
						_translate.Y = value.Y;
					}
					_bounds = value;
				}
			}
		}

		public double Rotation { get; set; }

		public Rect ElementBounds => SelectedElements.GetOutlineExternalBounds();

		protected override int VisualChildrenCount => Children.Count;

		protected override IEnumerator LogicalChildren => Children.GetEnumerator();

		public event EventHandler<EventArgs> SelectionUpdated;

		public event EventHandler<EventArgs> SelectedElementChanged;

		public SingleSelectionBoxBase()
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				HorizontalAlignment = HorizontalAlignment.Left;
				VerticalAlignment = VerticalAlignment.Top;
				_translate = new TranslateTransform();
				RenderTransform = _translate;
				Children = new UIElementCollection(this, this);
			}
		}

		private static void OnAttachmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SingleSelectionBoxBase singleSelectionBoxBase = (SingleSelectionBoxBase)d;
			UIElement uIElement = (UIElement)e.OldValue;
			UIElement uIElement2 = (UIElement)e.NewValue;
			if (uIElement != null)
			{
				singleSelectionBoxBase.Children.Remove(uIElement);
			}
			if (uIElement2 != null)
			{
				singleSelectionBoxBase.Children.Add(uIElement2);
			}
		}

		public void NotifyTransformCompleted(TransformRoutedEventArgs args)
		{
			IsTransforming = false;
			(Box as ISelectionTransformStyle)?.TransformCompleted(args);
			ISelectionTransformStyle selectionTransformStyle = Box as ISelectionTransformStyle;
			if (selectionTransformStyle == null)
			{
				((UIElement)Box).Opacity = 1.0;
			}
			else
			{
				selectionTransformStyle.TransformCompleted(args);
			}
			Rotation = 0.0;
			OnSelectionUpdated();
		}

		public void NotifyTransformStarting(TransformRoutedEventArgs args)
		{
		}

		public void NotifyTransformStarted(TransformRoutedEventArgs args)
		{
			IsTransforming = true;
			(Box as ISelectionTransformStyle)?.TransformStarted(args);
			ISelectionTransformStyle selectionTransformStyle = Box as ISelectionTransformStyle;
			if (selectionTransformStyle == null)
			{
				((UIElement)Box).Opacity = 0.0;
			}
			else
			{
				selectionTransformStyle.TransformStarted(args);
			}
		}

		public void OnSelectionUpdated()
		{
			SelectionUpdated?.Invoke(this, EventArgs.Empty);
			UpdateBounds();
			(this as IRefreshSelectionBox)?.OnRefresh();
		}

		private void UpdateBounds()
		{
			Rect elementBounds = ElementBounds;
			Bounds = new Rect(_003CUpdateBounds_003Eg__GetValideValue_007C42_0(elementBounds.Left), _003CUpdateBounds_003Eg__GetValideValue_007C42_0(elementBounds.Top), _003CUpdateBounds_003Eg__GetValideValue_007C42_0(elementBounds.Width), _003CUpdateBounds_003Eg__GetValideValue_007C42_0(elementBounds.Height));
		}

		protected override Visual GetVisualChild(int index)
		{
			return Children[index];
		}

		protected override Size MeasureOverride(Size constraint)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return new Size(0.0, 0.0);
			}
			UpdateBounds();
			Size size = Bounds.Size;
			Children.OfType<UIElement>().ForEach((UIElement x) =>
			{
				x.Measure(size);
			});
			return size;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return new Size(0.0, 0.0);
			}
			if (Bounds.IsEmpty)
			{
				return new Size(0.0, 0.0);
			}
			Size size = Bounds.Size;
			Children.OfType<UIElement>().ForEach((UIElement x) =>
			{
				x.Arrange(new Rect(size));
			});
			return size;
		}

		public void Dispose()
		{
			Children.Clear();
		}

		protected override Geometry GetLayoutClip(Size layoutSlotSize)
		{
			return null;
		}

		[CompilerGenerated]
		internal static double _003CUpdateBounds_003Eg__GetValideValue_007C42_0(double value)
		{
			if (!DataValidator.IsDoubleValid(value))
			{
				return 0.0;
			}
			return value;
		}
	}
}
