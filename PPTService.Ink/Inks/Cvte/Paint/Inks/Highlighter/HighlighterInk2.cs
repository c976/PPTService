using System;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;
using Cvte.Storage;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Inks.Highlighter
{
	[Obsolete("这个类还没完成，现在在克隆移动蒙层橡皮还有坑")]
	[Transform(PermittedTransforms.FreeTransform)]
	internal class HighlighterInk2 : Element, IInk, IEraseSensitive, ITransformedBounds, ISavable<HighlighterInkSaveInfo>, ISavable
	{
		private bool _init;

		private Rect _bounds;

		private double _rotation;

		private Geometry _stroke;

		private Brush _foreground;

		private Geometry _clipGeometry;

		private Geometry _erasedGeometry;

		private double _highlighterOpacity = 0.3;

		private Stroke _innerStroke;

		private MatrixTransform ClipGeometryMatrixTransform { get; set; }

		private Matrix ClipGeometryInvertMatrix { get; set; } = Matrix.Identity;

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
				return MatrixTransform.Matrix;
			}
			set
			{
				if (MatrixTransform == null)
				{
					MatrixTransform = new MatrixTransform(value);
				}
				else if (!object.Equals(MatrixTransform.Matrix, value))
				{
					if (ClipGeometryMatrixTransform != null)
					{
						ClipGeometryMatrixTransform.Matrix = value * ClipGeometryInvertMatrix;
					}
					MatrixTransform.Matrix = value;
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

		public Rect TransformedBounds => Stroke.Bounds;

		public override Geometry ClipGeometry
		{
			get
			{
				return _clipGeometry;
			}
			set
			{
				if (value == _clipGeometry)
				{
					return;
				}
				if (_clipGeometry == null)
				{
					Transform transform = value.Transform;
					if (transform != null)
					{
						ClipGeometryMatrixTransform = new MatrixTransform(transform.Value);
					}
				}
				else
				{
					ClipGeometryMatrixTransform.Matrix = default(Matrix);
					value.Transform = ClipGeometryMatrixTransform;
				}
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

		public double HighlighterOpacity
		{
			get
			{
				return _highlighterOpacity;
			}
			set
			{
				if (!value.Equals(_highlighterOpacity))
				{
					_highlighterOpacity = value;
					OnPropertyChanged("HighlighterOpacity");
				}
			}
		}

		private MatrixTransform MatrixTransform { get; set; }

		[UsedImplicitly]
		protected HighlighterInk2()
		{
			Name = "荧光笔";
			IsErasable = true;
		}

		public HighlighterInk2(Stroke stroke)
			: this()
		{
			_init = true;
			Geometry geometry = stroke.GetGeometry();
			SolidColorBrush solidColorBrush = new SolidColorBrush(stroke.DrawingAttributes.Color);
			_innerStroke = stroke;
			if (solidColorBrush.CanFreeze)
			{
				solidColorBrush.Freeze();
			}
			Foreground = solidColorBrush;
			_bounds = geometry.Bounds;
			MatrixTransform = new MatrixTransform
			{
				Matrix = geometry.Transform.Value
			};
			PathGeometry stroke2 = PathGeometry.CreateFromGeometry(geometry);
			Stroke = stroke2;
			Stroke.Transform = MatrixTransform;
			_clipGeometry = PathGeometry.CreateFromGeometry(new RectangleGeometry(_bounds));
			Matrix matrix = default(Matrix);
			ClipGeometryMatrixTransform = new MatrixTransform
			{
				Matrix = matrix
			};
			ClipGeometryInvertMatrix = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
			_clipGeometry.Transform = ClipGeometryMatrixTransform;
		}

		private void UpdateBounds(Rect newBounds, double? rotation = null)
		{
			Rect bounds = _bounds;
			if (MatrixTransform != null)
			{
				Matrix deltaTransform = MathMatrix.GetDeltaTransform(newBounds, bounds);
				if (rotation.HasValue)
				{
					Point point = newBounds.Center();
					deltaTransform.RotateAt((rotation - _rotation).Value, point.X, point.Y);
				}
				Transform *= deltaTransform;
			}
			else
			{
				Matrix matrix = default(Matrix);
				matrix.Translate(newBounds.X, newBounds.Y);
				if (rotation.HasValue)
				{
					Point point2 = newBounds.Center();
					matrix.RotateAt((rotation - _rotation).Value, point2.X, point2.Y);
				}
				MatrixTransform = new MatrixTransform(matrix);
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
			drawingContext.DrawRectangle(null, new Pen(Brushes.Black, 2.0), Bounds);
			drawingContext.DrawRectangle(null, new Pen(Brushes.Cornsilk, 6.0), Stroke.Bounds);
			drawingContext.DrawGeometry(null, new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AADc10BC")), 1.0), ClipGeometry);
			drawingContext.DrawGeometry(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AA55105C")), null, _erasedGeometry);
			DrawCore(drawingContext);
		}

		public override void SetErasedClip(Geometry erasedGeometry, bool sizeChanged, ScaleTransform scale = null)
		{
			_erasedGeometry = erasedGeometry.Clone();
			Matrix matrix = MatrixTransform.Matrix;
			InvalidateVisual();
		}

		protected virtual void DrawCore(DrawingContext drawingContext)
		{
			drawingContext.PushOpacity(HighlighterOpacity);
			drawingContext.DrawGeometry(Foreground, null, Stroke);
			drawingContext.Pop();
		}

		void IEraseSensitive.OnErasingCompleted()
		{
		}

		void IEraseSensitive.OnErasingStarted()
		{
		}

		public HighlighterInkSaveInfo GetSaveInfo()
		{
			Stroke innerStroke = _innerStroke;
			HighlighterInkSaveInfo highlighterInkSaveInfo = new HighlighterInkSaveInfo();
			highlighterInkSaveInfo.Points = innerStroke.StylusPoints;
			highlighterInkSaveInfo.Thickness = innerStroke.DrawingAttributes.Width;
			highlighterInkSaveInfo.Foreground = innerStroke.DrawingAttributes.Color;
			highlighterInkSaveInfo.HighlighterOpacity = HighlighterOpacity;
			highlighterInkSaveInfo.SetType("HighlighterInk");
			return highlighterInkSaveInfo;
		}

		public void SetSaveInfo(HighlighterInkSaveInfo saveInfo)
		{
			CombinedStroke combinedStroke = new CombinedStroke(saveInfo.Points, new DrawingAttributes
			{
				Width = saveInfo.Thickness,
				Height = saveInfo.Thickness,
				Color = saveInfo.Foreground
			});
			HighlighterOpacity = HighlighterOpacity;
			Geometry geometry = combinedStroke.GetGeometry();
			SolidColorBrush solidColorBrush = new SolidColorBrush(saveInfo.Foreground);
			if (solidColorBrush.CanFreeze)
			{
				solidColorBrush.Freeze();
			}
			Foreground = solidColorBrush;
			_bounds = geometry.Bounds;
			PathGeometry stroke = PathGeometry.CreateFromGeometry(geometry);
			Stroke = stroke;
			MatrixTransform = new MatrixTransform();
			Stroke.Transform = MatrixTransform;
			ClipGeometry.Transform = MatrixTransform;
		}
	}
}
