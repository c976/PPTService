using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Cvte.Diagnostics.Trace;
using Cvte.Windows;
using Cvte.Windows.Mathematics;

namespace Cvte.Paint.Framework
{
	public sealed class SlideViewbox : Decorator
	{
		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass27_0
		{
			public double desiredScale;

			public Size finalSize;

			public Size childSize;

			public double? desiredScaleX;

			public double? desiredScaleY;
		}

		private readonly Lazy<Board> _boardLazy;

		private bool _isLayoutUpdating;

		private ContainerVisual _internalVisual;

		private Size _lastArrangeSize;

		private FittingMode _actualFitting = (FittingMode)Board.ContentFittingProperty.DefaultMetadata.DefaultValue;

		public static readonly DependencyProperty ScaleProperty = Board.ContentScaleProperty.AddOwner(typeof(SlideViewbox), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ScaleOffsetPropertyChangedCallback));

		public static readonly DependencyProperty ScaleXProperty = Board.ContentScaleXProperty.AddOwner(typeof(SlideViewbox), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ScaleOffsetPropertyChangedCallback));

		public static readonly DependencyProperty ScaleYProperty = Board.ContentScaleYProperty.AddOwner(typeof(SlideViewbox), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ScaleOffsetPropertyChangedCallback));

		public static readonly DependencyProperty OffsetProperty = Board.ContentOffsetProperty.AddOwner(typeof(SlideViewbox), new FrameworkPropertyMetadata(default(Vector), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ScaleOffsetPropertyChangedCallback));

		public static readonly DependencyProperty FittingProperty = Board.ContentFittingProperty.AddOwner(typeof(SlideViewbox), new FrameworkPropertyMetadata(FittingMode.AutoFit, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty FrontProperty = DependencyProperty.Register("Front", typeof(UIElement), typeof(SlideViewbox), new PropertyMetadata((object)null));

		public static readonly DependencyProperty BackProperty = DependencyProperty.Register("Back", typeof(UIElement), typeof(SlideViewbox), new PropertyMetadata((object)null));

		public Vector BoardOffset
		{
			get
			{
				Point point = TranslatePoint(default(Point), _boardLazy.Value);
				return new Vector(point.X, point.Y);
			}
		}

		private ContainerVisual InternalVisual
		{
			get
			{
				if (_internalVisual == null)
				{
					_internalVisual = new ContainerVisual();
					AddVisualChild(_internalVisual);
				}
				return _internalVisual;
			}
		}

		private UIElement InternalChild
		{
			get
			{
				VisualCollection children = InternalVisual.Children;
				if (children.Count == 0)
				{
					return null;
				}
				return children[0] as UIElement;
			}
			set
			{
				VisualCollection children = InternalVisual.Children;
				if (children.Count != 0)
				{
					children.Clear();
				}
				children.Add(value);
			}
		}

		private Transform InternalTransform
		{
			get
			{
				return InternalVisual.Transform;
			}
			set
			{
				InternalVisual.Transform = value;
			}
		}

		public override UIElement Child
		{
			get
			{
				return InternalChild;
			}
			set
			{
				UIElement internalChild = InternalChild;
				if (!object.Equals(internalChild, value))
				{
					RemoveLogicalChild(internalChild);
					if (value != null)
					{
						AddLogicalChild(value);
					}
					InternalChild = value;
					InvalidateMeasure();
				}
			}
		}

		protected override int VisualChildrenCount => VisualChildren.Count();

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

		private IEnumerable<Visual> VisualChildren
		{
			get
			{
				if (Back != null)
				{
					yield return Back;
				}
				if (Child != null)
				{
					yield return InternalVisual;
				}
				if (Front != null)
				{
					yield return Front;
				}
			}
		}

		public double AutoFitScaleFactor => ComputeScaleFactor(DesiredSize, Child.DesiredSize);

		public Vector AutoCenterOffset => ComputeCenterOffset(DesiredSize, Child.DesiredSize, Scale);

		public Vector AutoFitCenterOffset => ComputeCenterOffset(DesiredSize, Child.DesiredSize, AutoFitScaleFactor);

		private double Scale
		{
			get
			{
				return (double)GetValue(ScaleProperty);
			}
			set
			{
				SetValue(ScaleProperty, value);
			}
		}

		private double ScaleX
		{
			get
			{
				return (double)GetValue(ScaleXProperty);
			}
			set
			{
				SetValue(ScaleXProperty, value);
			}
		}

		private double ScaleY
		{
			get
			{
				return (double)GetValue(ScaleYProperty);
			}
			set
			{
				SetValue(ScaleYProperty, value);
			}
		}

		public Vector Offset
		{
			get
			{
				return (Vector)GetValue(OffsetProperty);
			}
			set
			{
				SetValue(OffsetProperty, value);
			}
		}

		private FittingMode Fitting
		{
			get
			{
				return (FittingMode)GetValue(FittingProperty);
			}
			set
			{
				SetValue(FittingProperty, value);
			}
		}

		public UIElement Front
		{
			get
			{
				return (UIElement)GetValue(FrontProperty);
			}
			set
			{
				SetValue(FrontProperty, value);
			}
		}

		public UIElement Back
		{
			get
			{
				return (UIElement)GetValue(BackProperty);
			}
			set
			{
				SetValue(BackProperty, value);
			}
		}

		public SlideViewbox()
		{
			UseLayoutRounding = true;
			_boardLazy = new Lazy<Board>(() =>
			{
				Board board = this.VisualAncestor<Board>();
				if (board == null)
				{
					throw new InvalidOperationException(typeof(SlideViewbox).FullName + " 只能放在 " + typeof(Board).FullName + " 控件中。");
				}
				BindingOperations.SetBinding(board, Board.ContentScaleProperty, new Binding(ScaleProperty.Name)
				{
					Source = this,
					Mode = BindingMode.TwoWay
				});
				BindingOperations.SetBinding(board, Board.ContentScaleXProperty, new Binding(ScaleXProperty.Name)
				{
					Source = this,
					Mode = BindingMode.TwoWay
				});
				BindingOperations.SetBinding(board, Board.ContentScaleYProperty, new Binding(ScaleYProperty.Name)
				{
					Source = this,
					Mode = BindingMode.TwoWay
				});
				BindingOperations.SetBinding(board, Board.ContentOffsetProperty, new Binding(OffsetProperty.Name)
				{
					Source = this,
					Mode = BindingMode.TwoWay
				});
				BindingOperations.SetBinding(board, Board.ContentFittingProperty, new Binding(FittingProperty.Name)
				{
					Source = this,
					Mode = BindingMode.TwoWay
				});
				return board;
			});
		}

		protected override Visual GetVisualChild(int index)
		{
			return VisualChildren.ElementAt(index);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (Child != null)
			{
				Size availableSize2 = availableSize;
				Child.InvalidateMeasure();
				Child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				if (Fitting != FittingMode.AutoFit)
				{
					double scale = Scale;
					availableSize2 = new Size(Child.DesiredSize.Width * scale, Child.DesiredSize.Height * scale);
				}
				Back?.Measure(availableSize2);
				Front?.Measure(availableSize2);
			}
			if (!double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
			{
				return availableSize;
			}
			return default(Size);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			_003C_003Ec__DisplayClass27_0 obj = default(_003C_003Ec__DisplayClass27_0);
			obj.finalSize = finalSize;
			if (Child == null)
			{
				return obj.finalSize;
			}
			obj.childSize = Child.DesiredSize;
			if (obj.childSize == default(Size))
			{
				return obj.finalSize;
			}
			obj.desiredScaleX = null;
			obj.desiredScaleY = null;
			Vector offset;
			switch (Fitting)
			{
			case FittingMode.AutoFit:
				obj.desiredScale = ComputeScaleFactor(obj.finalSize, obj.childSize);
				offset = ComputeCenterOffset(obj.finalSize, obj.childSize, obj.desiredScale);
				ChangeSelfState(obj.desiredScale, offset);
				break;
			case FittingMode.ResizeFit:
				if (_lastArrangeSize != default(Size) && obj.finalSize != _lastArrangeSize)
				{
					double d = ComputeScaleFactor(_lastArrangeSize, obj.childSize);
					obj.desiredScale = (Nearly.Equals(d, Scale, 0.001) ? ComputeScaleFactor(obj.finalSize, obj.childSize) : Scale);
					double num = ComputeScaleFactor(_lastArrangeSize, obj.childSize);
					double num2 = ComputeScaleFactor(obj.finalSize, obj.childSize);
					if (obj.desiredScale < num2 || obj.desiredScale < num || Nearly.Equals(d, Scale, 0.001))
					{
						offset = ComputeCenterOffset(obj.finalSize, obj.childSize, obj.desiredScale);
						ChangeSelfState(obj.desiredScale, offset);
					}
					else
					{
						offset = Offset;
					}
				}
				else
				{
					obj.desiredScale = Scale;
					ScaleX = Scale;
					ScaleY = Scale;
					offset = Offset;
				}
				InternalTransform = new ScaleTransform(Scale, Scale);
				_lastArrangeSize = obj.finalSize;
				break;
			case FittingMode.Stretch:
				if (obj.finalSize != _lastArrangeSize)
				{
					ValueTuple<double, double, double> valueTuple = ComputeScaleFactorXY(obj.finalSize, obj.childSize);
					double? desiredScaleX = valueTuple.Item2;
					double? desiredScaleY = valueTuple.Item3;
					obj.desiredScale = valueTuple.Item1;
					obj.desiredScaleX = desiredScaleX;
					obj.desiredScaleY = desiredScaleY;
					offset = new Vector(0.0, 0.0);
					ChangeSelfState(obj.desiredScale, obj.desiredScaleX.Value, obj.desiredScaleY.Value, offset);
				}
				else
				{
					Scale = Math.Min(ScaleX, ScaleY);
					obj.desiredScale = Scale;
					obj.desiredScaleX = ScaleX;
					obj.desiredScaleY = ScaleY;
					offset = Offset;
				}
				_lastArrangeSize = obj.finalSize;
				break;
			default:
				obj.desiredScale = Scale;
				offset = Offset;
				break;
			}
			offset -= BoardOffset;
			double valueOrDefault = obj.desiredScaleX.GetValueOrDefault();
			if (!obj.desiredScaleX.HasValue)
			{
				valueOrDefault = obj.desiredScale;
				obj.desiredScaleX = valueOrDefault;
			}
			valueOrDefault = obj.desiredScaleY.GetValueOrDefault();
			if (!obj.desiredScaleY.HasValue)
			{
				valueOrDefault = obj.desiredScale;
				obj.desiredScaleY = valueOrDefault;
			}
			Rect bounds = new Rect(offset.X, offset.Y, obj.childSize.Width * obj.desiredScaleX.Value, obj.childSize.Height * obj.desiredScaleY.Value);
			bounds = bounds.MoveToRound();
			Rect rect = bounds;
			rect.Scale(1.0 / obj.desiredScaleX.Value, 1.0 / obj.desiredScaleY.Value);
			InternalTransform = new ScaleTransform(obj.desiredScaleX.Value, obj.desiredScaleY.Value);
			Rect rect2 = new Rect(default(Point), new Size(1280.0, 720.0));
			if (!CheckArrangeRectCorrect(bounds))
			{
				_003CArrangeOverride_003Eg__TraceArrangeIncorrect_007C27_0(string.Format("{0}={1}", "desiredBounds", bounds), ref obj);
				bounds = rect2;
			}
			if (!CheckArrangeRectCorrect(rect))
			{
				_003CArrangeOverride_003Eg__TraceArrangeIncorrect_007C27_0(string.Format("{0}={1}", "childBounds", rect), ref obj);
				bounds = rect2;
				rect = rect2;
			}
			Back?.Arrange(bounds);
			Child.Arrange(rect);
			Front?.Arrange(bounds);
			return obj.finalSize;
		}

		private static bool CheckArrangeRectCorrect(Rect rect)
		{
			if (!double.IsNaN(rect.Width) && !double.IsInfinity(rect.Width) && !double.IsNaN(rect.Height))
			{
				return !double.IsInfinity(rect.Height);
			}
			return false;
		}

		private void ChangeSelfState(double ratio, Vector offset)
		{
			_isLayoutUpdating = true;
			if (!Nearly.Equals(ratio, Scale, 0.0001))
			{
				Scale = ratio;
				ScaleX = ratio;
				ScaleY = ratio;
			}
			if (!Nearly.Equals(offset, Offset, 0.1))
			{
				Offset = offset;
			}
			_isLayoutUpdating = false;
		}

		private void ChangeSelfState(double ratio, double ratioX, double ratioY, Vector offset)
		{
			_isLayoutUpdating = true;
			if (!Nearly.Equals(ratio, Scale, 0.0001))
			{
				Scale = ratio;
			}
			if (!Nearly.Equals(ratioX, ScaleX, 0.0001))
			{
				ScaleX = ratioX;
			}
			if (!Nearly.Equals(ratioY, ScaleY, 0.0001))
			{
				ScaleY = ratioY;
			}
			if (!Nearly.Equals(offset, Offset, 0.1))
			{
				Offset = offset;
			}
			_isLayoutUpdating = false;
		}

		public Vector GetAutoCenterOffset(double scale)
		{
			return ComputeCenterOffset(DesiredSize, Child.DesiredSize, scale);
		}

		private static double ComputeScaleFactor(Size availableSize, Size contentSize)
		{
			double val = availableSize.Width / contentSize.Width;
			double val2 = availableSize.Height / contentSize.Height;
			double num = Math.Min(val, val2);
			if (!(num <= 0.0))
			{
				return num;
			}
			return 1.0;
		}

		private static ValueTuple<double, double, double> ComputeScaleFactorXY(Size availableSize, Size contentSize)
		{
			double num = availableSize.Width / contentSize.Width;
			double num2 = availableSize.Height / contentSize.Height;
			double num3 = Math.Min(num, num2);
			return new ValueTuple<double, double, double>((num3 <= 0.0) ? 1.0 : num3, num, num2);
		}

		private Vector ComputeCenterOffset(Size availableSize, Size contentSize, double scaleFactor)
		{
			double x = (availableSize.Width - contentSize.Width * scaleFactor) / 2.0;
			double y = (availableSize.Height - contentSize.Height * scaleFactor) / 2.0;
			return new Vector(x, y) + BoardOffset;
		}

		private static void ScaleOffsetPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SlideViewbox slideViewbox = (SlideViewbox)d;
			if (!slideViewbox._isLayoutUpdating && slideViewbox.Fitting != FittingMode.Stretch)
			{
				slideViewbox.Fitting = FittingMode.ResizeFit;
			}
		}

		[CompilerGenerated]
		private static void _003CArrangeOverride_003Eg__TraceArrangeIncorrect_007C27_0(string argumentMessage, ref _003C_003Ec__DisplayClass27_0 P_1)
		{
			Tracer.Error(string.Format("在{0}的{1}中，发现{2}存在不合法的值\r\n当前局部变量\r\n {3}={4} ;", "SlideViewbox", "ArrangeOverride", argumentMessage, "desiredScale", P_1.desiredScale) + string.Format(" {0}={1} ; {2}={3} ; {4}={5} ; {6}={7}", "finalSize", P_1.finalSize, "childSize", P_1.childSize, "desiredScaleX", P_1.desiredScaleX, "desiredScaleY", P_1.desiredScaleY));
		}
	}
}
