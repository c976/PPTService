using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	internal class HighPrecisionEraserViewBase : UIElement, IEraserView
	{
		public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(HighPrecisionEraserViewBase), AffectsRenderDoublePropertyMetadata);

		public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(HighPrecisionEraserViewBase), AffectsRenderDoublePropertyMetadata);

		public static readonly DependencyProperty RotationProperty = DependencyProperty.Register("Rotation", typeof(double), typeof(HighPrecisionEraserViewBase), AffectsRenderDoublePropertyMetadata);

		public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(HighPrecisionEraserViewBase), AffectsRenderDoublePropertyMetadata);

		public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(HighPrecisionEraserViewBase), AffectsRenderDoublePropertyMetadata);

		protected DrawingVisual DrawingVisual { get; }

		private static PropertyMetadata AffectsRenderDoublePropertyMetadata => new PropertyMetadata(0.0, OnPropertyChanged);

		public double X
		{
			get
			{
				return (double)GetValue(XProperty);
			}
			set
			{
				SetValue(XProperty, value);
			}
		}

		public double Y
		{
			get
			{
				return (double)GetValue(YProperty);
			}
			set
			{
				SetValue(YProperty, value);
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

		public double Width
		{
			get
			{
				return (double)GetValue(WidthProperty);
			}
			set
			{
				SetValue(WidthProperty, value);
			}
		}

		public double Height
		{
			get
			{
				return (double)GetValue(HeightProperty);
			}
			set
			{
				SetValue(HeightProperty, value);
			}
		}

		private TranslateTransform TranslateTransform { get; }

		private RotateTransform RotateTransform { get; }

		private ScaleTransform ScaleTransform { get; }

		private TransformGroup TransformGroup { get; }

		public HighPrecisionEraserViewBase(DrawingVisual drawingVisual)
		{
			DrawingVisual = drawingVisual;
			TranslateTransform = new TranslateTransform();
			RotateTransform = new RotateTransform();
			ScaleTransform = new ScaleTransform();
			TransformGroup = new TransformGroup
			{
				Children = new TransformCollection { ScaleTransform, RotateTransform, TranslateTransform }
			};
			IsHitTestVisible = false;
		}

		private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((UIElement)d).InvalidateVisual();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			Rect descendantBounds = DrawingVisual.DescendantBounds;
			double width = Width;
			double height = Height;
			ScaleTransform scaleTransform = ScaleTransform;
			if (double.IsNaN(width) || double.IsNaN(height))
			{
				scaleTransform.ScaleX = 1.0;
				scaleTransform.ScaleY = 1.0;
			}
			else
			{
				scaleTransform.ScaleX = width / descendantBounds.Width;
				scaleTransform.ScaleY = height / descendantBounds.Height;
			}
			TranslateTransform.X = X;
			TranslateTransform.Y = Y;
			RotateTransform.Angle = Rotation;
			RotateTransform.CenterX = width / 2.0;
			RotateTransform.CenterY = height / 2.0;
			drawingContext.PushTransform(TransformGroup);
			drawingContext.DrawDrawing(DrawingVisual.Drawing);
			drawingContext.Pop();
		}
	}
}
