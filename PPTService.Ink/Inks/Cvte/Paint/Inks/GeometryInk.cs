using System.Windows;
using System.Windows.Media;
using Cvte.Paint.Framework;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Inks
{
	[Transform(PermittedTransforms.FreeTransform)]
	public class GeometryInk : Element, ITransformSensitive, IEraseSensitive, IInk, IPermittedTransform
	{
		private MatrixTransform _matrixTransform;

		private Rect _bounds;

		private double _rotation;

		private Geometry _stroke;

		private Brush _foreground;

		private Geometry _clipGeometry;

		public override Rect Bounds
		{
			get
			{
				return _bounds;
			}
			set
			{
				UpdateBounds(value);
			}
		}

		public override Point Location
		{
			get
			{
				return new Point(0.0, 0.0);
			}
			set
			{
				UpdateBounds(new Rect(value, Size));
			}
		}

		public override double Rotation
		{
			get
			{
				return _rotation;
			}
			set
			{
				UpdateBounds(Bounds, value);
				_rotation = value;
			}
		}

		public override Matrix Transform
		{
			get
			{
				return _matrixTransform.Matrix;
			}
			set
			{
				if (_matrixTransform == null)
				{
					_matrixTransform = new MatrixTransform(value);
				}
				else if (!object.Equals(_matrixTransform.Matrix, value))
				{
					_matrixTransform.Matrix = value;
				}
			}
		}

		protected override Size Size
		{
			get
			{
				return _bounds.Size;
			}
			set
			{
				UpdateBounds(new Rect(Location, value));
			}
		}

		public Geometry Stroke
		{
			get
			{
				return _stroke;
			}
			private set
			{
				_stroke = value;
				InvalidateVisual();
			}
		}

		public override Geometry ClipGeometry
		{
			get
			{
				return _clipGeometry;
			}
			set
			{
				_clipGeometry = value;
				InvalidateVisual();
			}
		}

		public Brush Foreground
		{
			get
			{
				return _foreground;
			}
			set
			{
				_foreground = value;
				InvalidateVisual();
			}
		}

		PermittedTransforms IPermittedTransform.PermittedTransform => PermittedTransforms.FreeTransform;

		protected GeometryInk()
		{
			IsErasable = true;
		}

		public GeometryInk(Geometry stroke, Brush brush, Matrix? matrix = null)
		{
			IsErasable = true;
			ClipGeometry = stroke;
			InitStroke(stroke, brush, matrix);
		}

		protected void InitStroke(Geometry stroke, Brush brush, Matrix? matrix = null)
		{
			if (brush.CanFreeze)
			{
				brush.Freeze();
			}
			Foreground = brush;
			_bounds = stroke.Bounds;
			if (_matrixTransform == null)
			{
				_matrixTransform = new MatrixTransform
				{
					Matrix = (matrix ?? stroke.Transform.Value)
				};
			}
			PathGeometry stroke2 = PathGeometry.CreateFromGeometry(stroke);
			Stroke = stroke2;
			Stroke.Transform = _matrixTransform;
			ClipGeometry.Transform = _matrixTransform;
		}

		private void UpdateBounds(Rect newBounds, double? rotation = null)
		{
			Rect bounds = _bounds;
			if (_matrixTransform != null)
			{
				Matrix deltaTransform = MathMatrix.GetDeltaTransform(newBounds, bounds);
				if (rotation.HasValue)
				{
					Point point = newBounds.Center();
					deltaTransform.RotateAt((rotation - _rotation).Value, point.X, point.Y);
				}
				Transform *= deltaTransform;
			}
			_bounds = newBounds;
			OnBoundsChanged(bounds, newBounds);
			InvalidateVisual();
		}

		public override Geometry DefineOutline(IOutlineDefinetionContext context)
		{
			return Stroke;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.PushClip(ClipGeometry);
			DrawCore(drawingContext);
			drawingContext.Pop();
		}

		protected virtual void DrawCore(DrawingContext drawingContext)
		{
			drawingContext.DrawGeometry(Foreground, null, Stroke);
		}

		public void TransformStarted(TransformRoutedEventArgs args)
		{
		}

		public void TransformCompleted(TransformRoutedEventArgs args)
		{
		}

		public void OnErasingStarted()
		{
		}

		public void OnErasingCompleted()
		{
		}
	}
}
