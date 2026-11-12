using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Cvte.ComponentModel;
using Cvte.Windows;

namespace Cvte.Paint.Framework.Controls
{
	[ContentProperty("Child")]
	public class BoardUnitBox : Decorator
	{
		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass25_0
		{
			public Board board;
		}

		private Board _board;

		private bool _hasBinded;

		private ContainerVisual _internalVisual;

		private static readonly DependencyProperty ContentScaleProperty = DependencyProperty.Register("ContentScale", typeof(double), typeof(BoardUnitBox), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsArrange));

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
				if (children.Count != 0)
				{
					return children[0] as UIElement;
				}
				return null;
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

		private ScaleTransform InternalTransform
		{
			get
			{
				return (ScaleTransform)InternalVisual.Transform;
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

		protected override int VisualChildrenCount => 1;

		protected override IEnumerator LogicalChildren
		{
			get
			{
				if (Child != null)
				{
					yield return InternalChild;
				}
			}
		}

		public BoardUnitBox()
		{
			InternalTransform = new ScaleTransform();
			Loaded += (object sender, RoutedEventArgs args) =>
			{
				_board = this.VisualAncestor<Board>();
				if (_board != null)
				{
					BindForScale(_board);
				}
				InvalidateArrange();
			};
			Unloaded += (object sender, RoutedEventArgs args) =>
			{
				_board = null;
			};
		}

		protected override Visual GetVisualChild(int index)
		{
			return InternalVisual;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			double scale;
			if (!TryGetBoardScale(out scale))
			{
				Loaded -= OnLoadedForLayout;
				if (!IsLoaded)
				{
					Loaded += OnLoadedForLayout;
					return default(Size);
				}
			}
			InternalChild.Measure(new Size(availableSize.Width * scale, availableSize.Height * scale));
			Size desiredSize = InternalChild.DesiredSize;
			return new Size(desiredSize.Width / scale, desiredSize.Height / scale);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			double scale;
			if (!TryGetBoardScale(out scale))
			{
				Loaded -= OnLoadedForLayout;
				if (!IsLoaded)
				{
					Loaded += OnLoadedForLayout;
					return default(Size);
				}
			}
			InternalChild.Arrange(new Rect(0.0, 0.0, finalSize.Width * scale, finalSize.Height * scale));
			return finalSize;
		}

		private void OnLoadedForLayout(object sender, RoutedEventArgs e)
		{
			Loaded -= OnLoadedForLayout;
			InvalidateMeasure();
		}

		private bool TryGetBoardScale(out double scale)
		{
			scale = 1.0;
			if (_board == null)
			{
				if (_hasBinded)
				{
					return false;
				}
				_board = this.VisualAncestor<Board>();
				if (_board != null && _board.IsLoaded)
				{
					_hasBinded = true;
					BindForScale(_board);
					scale = _board.ContentScale;
					return !object.Equals(scale, 0.0);
				}
				return false;
			}
			scale = _board.ContentScale;
			return !object.Equals(scale, 0.0);
		}

		private void BindForScale(Board board)
		{
			_003C_003Ec__DisplayClass25_0 obj = default(_003C_003Ec__DisplayClass25_0);
			obj.board = board;
			if (!DebuggingProperties.IsDebug || !DesignerProperties.GetIsInDesignMode(this))
			{
				_003CBindForScale_003Eg__BindScale_007C25_0(InternalTransform, ScaleTransform.ScaleXProperty, ref obj);
				_003CBindForScale_003Eg__BindScale_007C25_0(InternalTransform, ScaleTransform.ScaleYProperty, ref obj);
				BindingOperations.SetBinding(this, ContentScaleProperty, new Binding(Board.ContentScaleProperty.Name)
				{
					Source = obj.board,
					Mode = BindingMode.OneWay
				});
			}
		}

		[CompilerGenerated]
		private static void _003CBindForScale_003Eg__BindScale_007C25_0(DependencyObject source, DependencyProperty property, ref _003C_003Ec__DisplayClass25_0 P_2)
		{
			BindingOperations.SetBinding(source, property, new Binding(Board.ContentScaleProperty.Name)
			{
				Source = P_2.board,
				Mode = BindingMode.OneWay,
				Converter = BoardContentScaleConverter.Instance,
				ConverterParameter = 1.0
			});
		}
	}
}
