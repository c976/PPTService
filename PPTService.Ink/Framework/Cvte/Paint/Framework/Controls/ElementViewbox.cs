using System;
using System.Collections;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Cvte.Documents._Operations;
using Cvte.Windows;
using Cvte.Windows.Controls;
using Cvte.Windows.Mathematics;

namespace Cvte.Paint.Framework.Controls
{
	[ContentProperty("Child")]
	public sealed class ElementViewbox : Decorator
	{
		private Element _element;

		private UIElement _child;

		private readonly ContainerVisual _container = new ContainerVisual
		{
			Transform = new ScaleTransform()
		};

		private bool _isEvaluating;

		private bool _isReverting = true;

		private static readonly DependencyProperty IsUndoRedoingProperty = DependencyProperty.RegisterAttached("IsUndoRedoing", typeof(bool), typeof(ElementViewbox), new PropertyMetadata(false));

		public double Scale
		{
			get
			{
				return ((ScaleTransform)_container.Transform).ScaleX;
			}
			private set
			{
				((ScaleTransform)_container.Transform).ScaleX = value;
				((ScaleTransform)_container.Transform).ScaleY = value;
			}
		}

		public override UIElement Child
		{
			get
			{
				return _child;
			}
			set
			{
				if (!object.Equals(_child, value))
				{
					_container.Children.Remove(_child);
					RemoveLogicalChild(_child);
					_child = value;
					AddLogicalChild(value);
					_container.Children.Add(value);
				}
			}
		}

		protected override int VisualChildrenCount => 1;

		protected override IEnumerator LogicalChildren
		{
			get
			{
				if (Child != null)
				{
					yield return Child;
				}
			}
		}

		public ElementViewbox()
		{
			AddVisualChild(_container);
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_element = this.VisualAncestor<Element>();
			if (_element != null)
			{
				_element.BoundsChanged -= Element_BoundsChanged;
				_element.BoundsChanged += Element_BoundsChanged;
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			if (_element != null)
			{
				_element.BoundsChanged -= Element_BoundsChanged;
			}
			_element = null;
		}

		protected override Visual GetVisualChild(int index)
		{
			return _container;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			if (Child == null)
			{
				return default(Size);
			}
			_isReverting = false;
			Child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			double width = Math.Min(Child.DesiredSize.Width * Scale, constraint.Width);
			double height = Math.Min(Child.DesiredSize.Height * Scale, constraint.Height);
			return new Size(width, height);
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			if (Child == null)
			{
				return arrangeSize;
			}
			Child.Arrange(new Rect(Child.DesiredSize));
			Size desiredSize = Child.DesiredSize;
			Size size = (_isEvaluating ? new Size(desiredSize.Width * Scale, desiredSize.Height * Scale) : arrangeSize);
			_isEvaluating = false;
			if (!object.Equals(desiredSize, default(Size)) && !object.Equals(arrangeSize, default(Size)))
			{
				double val = size.Width / desiredSize.Width;
				double val2 = size.Height / desiredSize.Height;
				Scale = Math.Min(val, val2);
			}
			return arrangeSize;
		}

		public void ReevaluateSize(Action actionThatMayChangeTheSize = null)
		{
			if (_isReverting)
			{
				_isReverting = false;
				actionThatMayChangeTheSize?.Invoke();
				return;
			}
			Element element = this.VisualAncestor<Element>();
			if (element == null)
			{
				actionThatMayChangeTheSize?.Invoke();
				return;
			}
			Exception ex = null;
			try
			{
				actionThatMayChangeTheSize?.Invoke();
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			element.Width = double.NaN;
			element.Height = double.NaN;
			_isEvaluating = true;
			if (ex != null)
			{
				ExceptionDispatchInfo.Capture(ex).Throw();
			}
		}

		private void Element_BoundsChanged(object sender, ValueChangedRoutedEventArgs<Rect> e)
		{
			if (Nearly.Equals(e.OldValue, e.NewValue))
			{
				_isReverting = true;
			}
		}

		public static PropertyChangedCallback ReevaluationCallBack<T>(Func<T, ElementViewbox> getElementViewbox, PropertyChangedCallback callback = null) where T : DependencyObject
		{
			return (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
			{
				T arg = (T)d;
				(getElementViewbox?.Invoke(arg))?.ReevaluateSize(() =>
				{
					callback?.Invoke(d, e);
				});
				if (false.Equals(d.GetValue(IsUndoRedoingProperty)))
				{
					Board.GetUndoRedoProvider(d).Insert(new ActionOperation(() =>
					{
						SetValueInOperation(d, e.Property, e.OldValue);
					}, () =>
					{
						SetValueInOperation(d, e.Property, e.NewValue);
					}));
				}
			};
		}

		public static void SetValueInOperation(DependencyObject d, DependencyProperty property, object value)
		{
			d.SetValue(IsUndoRedoingProperty, true);
			try
			{
				d.SetValue(property, value);
			}
			finally
			{
				d.ClearValue(IsUndoRedoingProperty);
			}
		}
	}
}
