using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace PPTService.Ink.Core.Windows
{
	public class ContentDialogDragging
	{
		public static readonly DependencyProperty DraggingProperty = DependencyProperty.RegisterAttached("Dragging", typeof(ContentDialogDragging), typeof(ContentDialogDragging), new PropertyMetadata(null, OnInnerDraggingChanged));

		private FrameworkElement _draggingTarget;

		private FrameworkElement _container;

		private FrameworkElement _target;

		private FrameworkElement _offsetTarget;

		private bool _isDragging;

		private Point _downPosition;

		private Thickness _originMargin;

		private Thickness _offsetRange;

		public string ContainerName { get; set; }

		public string TargetName { get; set; }

		public string OffsetTargetName { get; set; }

		public static ContentDialogDragging GetDragging(DependencyObject obj)
		{
			return (ContentDialogDragging)obj.GetValue(DraggingProperty);
		}

		public static void SetDragging(DependencyObject obj, ContentDialogDragging value)
		{
			obj.SetValue(DraggingProperty, value);
		}

		private static void OnInnerDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement element = (FrameworkElement)d;
			if (e.OldValue != null)
			{
				((ContentDialogDragging)e.OldValue).Detach();
			}
			if (e.NewValue != null)
			{
				((ContentDialogDragging)e.NewValue).Attach(element);
			}
		}

		private void Attach(FrameworkElement element)
		{
			_draggingTarget = element;
			element.Loaded += Element_Loaded;
			element.Unloaded += Element_Unloaded;
			element.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
		}

		private void Detach()
		{
			FrameworkElement draggingTarget = _draggingTarget;
			_draggingTarget = null;
			draggingTarget.Loaded -= Element_Loaded;
			draggingTarget.Unloaded -= Element_Unloaded;
			draggingTarget.ClearValue(WindowChrome.IsHitTestVisibleInChromeProperty);
		}

		private void Element_Loaded(object sender, RoutedEventArgs e)
		{
			FrameworkElement draggingTarget = _draggingTarget;
			draggingTarget.MouseLeftButtonDown += OnMouseLeftButtonDown;
			draggingTarget.MouseLeftButtonUp += OnMouseLeftButtonUp;
			draggingTarget.MouseMove += OnMouseMove;
			draggingTarget.LostMouseCapture += OnLostMouseCapture;
			_container = (FrameworkElement)_draggingTarget.FindName(ContainerName);
			_target = (FrameworkElement)_draggingTarget.FindName(TargetName);
			if (string.IsNullOrEmpty(OffsetTargetName))
			{
				_offsetTarget = _target;
			}
			else
			{
				_offsetTarget = (FrameworkElement)_draggingTarget.FindName(OffsetTargetName);
			}
			_container.SizeChanged += Container_SizeChanged;
		}

		private void Element_Unloaded(object sender, RoutedEventArgs e)
		{
			FrameworkElement draggingTarget = _draggingTarget;
			draggingTarget.MouseLeftButtonDown -= OnMouseLeftButtonDown;
			draggingTarget.MouseLeftButtonUp -= OnMouseLeftButtonUp;
			draggingTarget.MouseMove -= OnMouseMove;
			draggingTarget.LostMouseCapture -= OnLostMouseCapture;
			_container.SizeChanged -= Container_SizeChanged;
			_container = null;
			_target = null;
			_offsetTarget = null;
		}

		private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			_target.InvalidateProperty(FrameworkElement.MarginProperty);
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_originMargin = _target.Margin;
			Rect rect = new Rect(_offsetTarget.TranslatePoint(default(Point), _container), _offsetTarget.TranslatePoint(new Point(_offsetTarget.ActualWidth, _offsetTarget.ActualHeight), _container));
			_offsetRange = new Thickness(rect.Left, rect.Top, _container.ActualWidth - rect.Right, _container.ActualHeight - rect.Bottom);
			_downPosition = e.GetPosition(_container);
			_draggingTarget.CaptureMouse();
			_isDragging = true;
		}

		private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_draggingTarget.ReleaseMouseCapture();
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (_isDragging)
			{
				Vector vector = e.GetPosition(_container) - _downPosition;
				vector.X = Math.Max(vector.X, 0.0 - _offsetRange.Left);
				vector.X = Math.Min(vector.X, _offsetRange.Right);
				vector.Y = Math.Max(vector.Y, 0.0 - _offsetRange.Top);
				vector.Y = Math.Min(vector.Y, _offsetRange.Bottom);
				Thickness originMargin = _originMargin;
				originMargin = new Thickness(originMargin.Left + vector.X, originMargin.Top + vector.Y, originMargin.Right - vector.X, originMargin.Bottom - vector.Y);
				_target.SetCurrentValue(FrameworkElement.MarginProperty, originMargin);
			}
		}

		private void OnLostMouseCapture(object sender, MouseEventArgs e)
		{
			_isDragging = false;
			_originMargin = default(Thickness);
			_downPosition = default(Point);
			_offsetRange = default(Thickness);
		}
	}
}
