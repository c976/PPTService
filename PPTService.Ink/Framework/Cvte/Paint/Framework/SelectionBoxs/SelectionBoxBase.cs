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
using Cvte.ComponentModel;
using Cvte.Linq;
using Cvte.Paint.Framework.Controls;
using Cvte.Windows.Controls;

namespace Cvte.Paint.Framework.SelectionBoxs
{
	[ContentProperty("Children")]
	public class SelectionBoxBase : FrameworkElement, ISelectionBox, ISelectionOperator, ISlideLayeredOperator
	{
		private enum TransformState
		{
			TransformCompleted,
			TransformStarting,
			Transforming
		}

		private UIElementCollection _children;

		private Grid _mainPanel;

		private readonly TransformGroup _transformGroup = new TransformGroup();

		private readonly TranslateTransform _translateTransform = new TranslateTransform();

		private readonly RotateTransform _rotateTransform = new RotateTransform();

		private IList<Element> _selectedElements;

		private readonly ValueChangedRoutedEventHandler<Rect> _boundsChangedHandler;

		private Thickness _padding;

		public static readonly DependencyProperty BoundsProperty = DependencyProperty.Register("Bounds", typeof(Rect), typeof(SelectionBoxBase), new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
		{
			SelectionBoxBase selectionBoxBase = (SelectionBoxBase)o;
			Rect rect = (Rect)args.NewValue;
			selectionBoxBase._translateTransform.X = rect.X;
			selectionBoxBase._translateTransform.Y = rect.Y;
		}));

		public static readonly DependencyProperty RotationProperty = DependencyProperty.Register("Rotation", typeof(double), typeof(SelectionBoxBase), new FrameworkPropertyMetadata(0.0, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
		{
			SelectionBoxBase selectionBoxBase = (SelectionBoxBase)o;
			double angle = (double)args.NewValue;
			selectionBoxBase._rotateTransform.Angle = angle;
		}));

		private TransformState _transformState;

		public static readonly DependencyProperty AttachmentProperty = DependencyProperty.Register("Attachment", typeof(UIElement), typeof(SelectionBoxBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnAttachmentChanged));

		public UIElementCollection Children => _mainPanel.Children;

		public UIElementCollection UIElementChildren => _children;

		public IList<Element> SelectedElements
		{
			get
			{
				return _selectedElements;
			}
			set
			{
				_selectedElements = value;
				OnSelectedElementChanged(this, EventArgs.Empty);
				SelectedElementChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		protected override int VisualChildrenCount => _children.Count;

		protected override IEnumerator LogicalChildren => _children.GetEnumerator();

		[Obsolete("请使用 GetElementsFrame 代替避免浪费性能。", true)]
		private Rect ElementsBounds
		{
			get
			{
				Rect empty = Rect.Empty;
				if (Box.SelectedElements == null)
				{
					return empty;
				}
				foreach (Element selectedElement in Box.SelectedElements)
				{
					empty.Union(selectedElement.Bounds);
				}
				return empty;
			}
		}

		[Obsolete("请使用 GetElementsFrame 代替避免浪费性能。", true)]
		private double ElementsRotation => SelectedElements.Average((Element element) => element.Rotation);

		public Rect Bounds
		{
			get
			{
				return (Rect)GetValue(BoundsProperty);
			}
			set
			{
				SetValue(BoundsProperty, value);
			}
		}

		public double Rotation
		{
			get
			{
				return (double)GetValue(RotationProperty);
			}
			set
			{
				SetValue(RotationProperty, value);
			}
		}

		public bool IsTransforming
		{
			get
			{
				if (_transformState == TransformState.Transforming)
				{
					return true;
				}
				return false;
			}
		}

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

		public event EventHandler<EventArgs> SelectionUpdated;

		public event EventHandler<EventArgs> SelectedElementChanged;

		public SelectionBoxBase()
		{
			Initialize();
			_boundsChangedHandler = OnBoundsChanged;
		}

		private void Initialize()
		{
			FocusVisualStyle = null;
			Focusable = true;
			_mainPanel = new Grid();
			BoardUnitBox boardUnitBox = new BoardUnitBox();
			boardUnitBox.Child = _mainPanel;
			_children = new UIElementCollection(this, this);
			_children.Add(boardUnitBox);
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;
			InitializeTransformGroup();
			Loaded += SelectionBoxBase_Loaded;
			Unloaded += SelectionBoxBase_Unloaded;
		}

		private void SelectionBoxBase_Unloaded(object sender, RoutedEventArgs e)
		{
			if (Box.SelectedElements.Count == 1)
			{
				Element element = Box.SelectedElements.First();
				element.PropertyChanged -= Element_PropertyChanged;
				element.RemoveHandler(Element.BoundsChanged2Event, _boundsChangedHandler);
			}
		}

		private void SelectionBoxBase_Loaded(object sender, RoutedEventArgs e)
		{
			if (Box.SelectedElements.Count == 1)
			{
				Element element = Box.SelectedElements.First();
				element.PropertyChanged -= Element_PropertyChanged;
				element.RemoveHandler(Element.BoundsChanged2Event, _boundsChangedHandler);
				element.PropertyChanged += Element_PropertyChanged;
				element.AddHandler(Element.BoundsChanged2Event, _boundsChangedHandler);
				UpdateBoundsAndRotation();
			}
		}

		private void OnBoundsChanged(object sender, ValueChangedRoutedEventArgs<Rect> e)
		{
			if (!IsTransforming && SelectedElements.Contains(sender))
			{
				UpdateBoundsAndRotation();
			}
		}

		private void UpdateBoundsAndRotation()
		{
			UpdateBoundsAndRotation(_transformState == TransformState.TransformCompleted);
		}

		private void UpdateBoundsAndRotation(bool containsPadding)
		{
			ValueTuple<Rect, double> elementsFrame = GetElementsFrame(containsPadding);
			Rect bounds = elementsFrame.Item1;
			double item = elementsFrame.Item2;
			if (!DataValidator.IsRectValid(bounds))
			{
				if (DebuggingProperties.IsDebug)
				{
					throw new InvalidOperationException("bounds的值不是有效的数值");
				}
				bounds = new Rect(0.0, 0.0, 0.0, 0.0);
			}
			Bounds = bounds;
			Rotation = item;
		}

		private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!IsTransforming && e.PropertyName == RotationProperty.Name)
			{
				UpdateBoundsAndRotation();
			}
		}

		internal void InitializeTransformGroup()
		{
			RenderTransformOrigin = new Point(0.5, 0.5);
			_transformGroup.Children.Add(_rotateTransform);
			_transformGroup.Children.Add(_translateTransform);
			RenderTransform = _transformGroup;
		}

		protected virtual void OnSelectedElementChanged(object sender, EventArgs e)
		{
			if (_selectedElements != null && _selectedElements.Any())
			{
				UpdateBoundsAndRotation();
			}
		}

		protected override Visual GetVisualChild(int index)
		{
			return _children[index];
		}

		protected override Size MeasureOverride(Size constraint)
		{
			UpdateBoundsAndRotation();
			if (Bounds.IsEmpty)
			{
				return new Size(0.0, 0.0);
			}
			Size size = Bounds.Size;
			_children.OfType<UIElement>().ForEach((UIElement x) =>
			{
				x.Measure(size);
			});
			return size;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (Bounds.IsEmpty)
			{
				return new Size(0.0, 0.0);
			}
			Size size = Bounds.Size;
			_children.OfType<UIElement>().ForEach((UIElement x) =>
			{
				x.Arrange(new Rect(size));
			});
			return size;
		}

		private ValueTuple<Rect, double> GetElementsFrame(bool containsPadding)
		{
			if (SelectedElements == null)
			{
				throw new InvalidOperationException("SelectedElements为空，无法执行元素Bounds的计算操作");
			}
			if (SelectedElements.Count == 1)
			{
				return GetElementFrame(SelectedElements.First(), containsPadding);
			}
			if (SelectedElements.Count > 1)
			{
				return new ValueTuple<Rect, double>(SelectedElements.GetOutlineExternalBounds(), 0.0);
			}
			if (DebuggingProperties.IsDebug)
			{
				throw new InvalidOperationException("No element is selected so that bounds and angle cannot be calculated.");
			}
			return new ValueTuple<Rect, double>(Rect.Empty, 0.0);
		}

		private ValueTuple<Rect, double> GetElementFrame(Element element, bool containsPadding)
		{
			ValueTuple<Rect, double> valueTuple = element.Outline.ToBoundsRotation();
			Rect rect = valueTuple.Item1;
			double item = valueTuple.Item2;
			if (containsPadding && !rect.IsEmpty)
			{
				ISelectionPadding selectionPadding = element as ISelectionPadding;
				if (selectionPadding != null)
				{
					Thickness selectionPadding2 = selectionPadding.GetSelectionPadding();
					rect = GetPaddingBounds(rect, selectionPadding2);
				}
				return new ValueTuple<Rect, double>(rect, item);
			}
			return new ValueTuple<Rect, double>(rect, item);
		}

		private Rect GetPaddingBounds(Rect bounds, Thickness padding)
		{
			return new Rect(bounds.Left - padding.Left, bounds.Top - padding.Top, bounds.Width + padding.Left + padding.Right, bounds.Height + padding.Top + padding.Bottom);
		}

		public void OnSelectionUpdated()
		{
			UpdateBoundsAndRotation();
			SelectionUpdated?.Invoke(this, EventArgs.Empty);
		}

		public void NotifyTransformStarting(TransformRoutedEventArgs args)
		{
			_transformState = TransformState.TransformStarting;
			UpdateBoundsAndRotation();
		}

		public void NotifyTransformStarted(TransformRoutedEventArgs args)
		{
			_transformState = TransformState.Transforming;
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

		public void NotifyTransformCompleted(TransformRoutedEventArgs args)
		{
			_transformState = TransformState.TransformCompleted;
			ISelectionTransformStyle selectionTransformStyle = Box as ISelectionTransformStyle;
			if (selectionTransformStyle == null)
			{
				((UIElement)Box).Opacity = 1.0;
			}
			else
			{
				selectionTransformStyle.TransformCompleted(args);
			}
			OnSelectionUpdated();
		}

		private static void OnAttachmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SelectionBoxBase selectionBoxBase = (SelectionBoxBase)d;
			UIElement uIElement = (UIElement)e.OldValue;
			UIElement uIElement2 = (UIElement)e.NewValue;
			if (uIElement != null)
			{
				selectionBoxBase.Children.Remove(uIElement);
			}
			if (uIElement2 != null)
			{
				selectionBoxBase.Children.Add(uIElement2);
			}
		}

		protected override Geometry GetLayoutClip(Size layoutSlotSize)
		{
			return null;
		}
	}
}
