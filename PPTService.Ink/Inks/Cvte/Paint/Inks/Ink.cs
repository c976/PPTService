using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Cache;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Level;
using Cvte.Paint.Inks.Storage;
using Cvte.Storage;
using Cvte.Windows.Mathematics;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Inks
{
	[DebuggerDisplay("points = {InnerStroke.StylusPoints.Count}, id = {GetHashCode()}")]
	[Transform(PermittedTransforms.FreeTransform)]
	public class Ink : Element, ISavable<InkSaveInfo>, ISavable, ITransformSensitive, ITransformedBounds, IErasableInk, IInk, IEraseSensitive, ISplitErasableInk, IPermittedTransform
	{
		private Rect _bounds;

		private Stroke _innerStroke;

		private Stroke _originalStroke;

		private Rect _originalBounds;

		private bool _isTransforming;

		private static readonly Dictionary<string, string> StrokeTypes;

		private double _rotation;

		private double _transformingRotation;

		private Rect _transformingBounds;

		private readonly CachePool<string, Func<StrokeBase>> _cache = new CachePool<string, Func<StrokeBase>>((string name) =>
		{
			Type type = Type.GetType(name);
			return (type == null) ? null : System.Linq.Expressions.Expression.Lambda<Func<StrokeBase>>(System.Linq.Expressions.Expression.New(type), new ParameterExpression[0]).Compile();
		});

		private StrokeCollection _tempInkContainer;

		public Stroke InnerStroke
		{
			get
			{
				return _innerStroke;
			}
			protected set
			{
				if (_innerStroke != null)
				{
					InnerStroke.Invalidated -= InnerStroke_Invalidated;
				}
				_innerStroke = value;
				InnerStroke.Invalidated += InnerStroke_Invalidated;
				_originalStroke = _innerStroke.Clone();
				UpdateBoundsAndRotation();
				OnInnerStrokeChanged();
			}
		}

		[Obsolete("此属性已无效，设置了没有啥用")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsConstrain { get; set; }

		public bool IsBezierInterpolationEnabled { get; set; }

		public IEnumerable<Point> Points => InnerStroke.StylusPoints.Select((StylusPoint x) => x.ToPoint());

		public override Rect Bounds
		{
			get
			{
				if (!_isTransforming)
				{
					return _bounds;
				}
				return _transformingBounds;
			}
			set
			{
				if (InnerStroke != null)
				{
					Rect bounds = _bounds;
					if (_isTransforming)
					{
						LazylyTransform(value, Rotation);
					}
					else
					{
						InstantlyTransform(value, Rotation);
					}
					OnBoundsChanged(bounds, value);
				}
			}
		}

		public override double Rotation
		{
			get
			{
				if (!_isTransforming)
				{
					return _rotation;
				}
				return _transformingRotation;
			}
			set
			{
				if (InnerStroke != null)
				{
					if (_isTransforming)
					{
						LazylyTransform(Bounds, value);
					}
					else
					{
						InstantlyTransform(Bounds, value);
					}
				}
			}
		}

		public override Point Location
		{
			get
			{
				return Bounds.Location;
			}
			set
			{
				if (InnerStroke != null)
				{
					Rect bounds = Bounds;
					bounds.X = value.X;
					bounds.Y = value.Y;
					Bounds = bounds;
				}
			}
		}

		protected override Size Size
		{
			get
			{
				return Bounds.Size;
			}
			set
			{
				if (InnerStroke != null)
				{
					Rect bounds = Bounds;
					bounds.Width = value.Width;
					bounds.Height = value.Height;
					Bounds = bounds;
				}
			}
		}

		public Rect TransformedBounds => Geometry2D.UnionRect(InnerStroke.StylusPoints.Select((StylusPoint x) => x.ToPoint()));

		public Color ForegroundColor
		{
			get
			{
				return InnerStroke.DrawingAttributes.Color;
			}
			set
			{
				IForegroundColor foregroundColor = InnerStroke as IForegroundColor;
				if (foregroundColor != null)
				{
					foregroundColor.ForegroundColor = value;
				}
				else
				{
					InnerStroke.DrawingAttributes.Color = value;
				}
				InvalidateVisual();
			}
		}

		public double Thickness
		{
			get
			{
				return InnerStroke.DrawingAttributes.Width;
			}
			set
			{
				InnerStroke.DrawingAttributes.Width = value;
				InnerStroke.DrawingAttributes.Height = value;
				InvalidateVisual();
			}
		}

		public PermittedTransforms PermittedTransform => PermittedTransforms.FreeTransform;

		StrokeCollection IErasableInk.TempInkContainer => _tempInkContainer;

		bool IErasableInk.IsErasing { get; set; }

		bool IErasableInk.IsErasable => IsInkingErasable;

		public bool IsInkingErasable { get; set; } = true;

		static Ink()
		{
			StrokeTypes = new Dictionary<string, string>();
			StrokeTypes.Add("HardPenStroke", typeof(HardPenStroke).AssemblyQualifiedName);
			StrokeTypes.Add("CombinedStroke", typeof(CombinedStroke).AssemblyQualifiedName);
		}

		[UsedImplicitly]
		protected Ink()
		{
		}

		public Ink(StrokeBase stroke)
			: this((Stroke)stroke)
		{
		}

		public static Ink Make(Color color, double thickness, Point[] points)
		{
			return new Ink(new Stroke(new StylusPointCollection(points.Select((Point p) => new StylusPoint(p.X, p.Y)))))
			{
				ForegroundColor = color,
				Thickness = thickness
			};
		}

		internal Ink(Stroke stroke)
		{
			Name = "笔迹";
			InnerStroke = stroke;
		}

		protected virtual void OnInnerStrokeChanged()
		{
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (((IErasableInk)this).IsErasing)
			{
				((IErasableInk)this).TempInkContainer.Draw(drawingContext);
			}
			else if (IsBezierInterpolationEnabled)
			{
				DrawBezierStroke(drawingContext);
			}
			else
			{
				DrawStroke(drawingContext);
			}
		}

		private void DrawStroke(DrawingContext drawingContext)
		{
			InnerStroke.Draw(drawingContext);
		}

		private void DrawBezierStroke(DrawingContext drawingContext)
		{
			if (!InnerStroke.StylusPoints.Any())
			{
				return;
			}
			Point[] array = Points.ToArray();
			int num = array.Length;
			PathFigure pathFigure = new PathFigure();
			Point point = (pathFigure.StartPoint = array.First());
			Point point3 = point;
			Point point4 = point;
			Point point5 = default(Point);
			for (int i = 1; i < num; i++)
			{
				if (Nearly.Equals(point3, array[i], 1E-06))
				{
					if (i == num - 1)
					{
						pathFigure.Segments.Add(new System.Windows.Media.LineSegment(array[i], true));
						break;
					}
				}
				else
				{
					point5.X = (point3.X + array[i].X) / 2.0;
					point5.Y = (point3.Y + array[i].Y) / 2.0;
					BezierSegment bezierSegment = new BezierSegment(point4, point3, point5, true);
					bezierSegment.IsSmoothJoin = true;
					pathFigure.Segments.Add(bezierSegment);
					point3 = array[i];
					point4 = point5;
				}
			}
			PathGeometry geometry = new PathGeometry(new PathFigureCollection { pathFigure });
			Pen pen = new Pen
			{
				Brush = new SolidColorBrush(ForegroundColor),
				Thickness = Thickness,
				StartLineCap = PenLineCap.Round,
				EndLineCap = PenLineCap.Round
			};
			pen.Freeze();
			drawingContext.DrawGeometry(null, pen, geometry);
		}

		void ITransformSensitive.TransformStarted(TransformRoutedEventArgs args)
		{
			_transformingBounds = _bounds;
			_transformingRotation = _rotation;
			_isTransforming = true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("此方法没有任何作用，里面没有代码")]
		public void TransformDelta(TransformRoutedEventArgs args)
		{
		}

		void ITransformSensitive.TransformCompleted(TransformRoutedEventArgs args)
		{
			_isTransforming = false;
			InstantlyTransform(_transformingBounds, _transformingRotation);
			Transform = Matrix.Identity;
			_transformingBounds = _bounds;
			_transformingRotation = _rotation;
		}

		private Matrix GetDeltaTransform(Rect newBounds, Rect oldBounds)
		{
			Matrix result = default(Matrix);
			double num = newBounds.Width / oldBounds.Width;
			if (double.IsNaN(num))
			{
				num = 1.0;
			}
			double num2 = newBounds.Height / oldBounds.Height;
			if (double.IsNaN(num2))
			{
				num2 = 1.0;
			}
			result.Scale(num, num2);
			result.Translate(newBounds.X - oldBounds.X * num, newBounds.Y - oldBounds.Y * num2);
			return result;
		}

		private void InstantlyTransform(Rect bounds, double rotation)
		{
			Matrix deltaTransform = GetDeltaTransform(bounds, _originalBounds);
			Point point = bounds.Center();
			deltaTransform.RotateAt(rotation, point.X, point.Y);
			InnerStroke.StylusPoints = _originalStroke.StylusPoints.Clone();
			InnerStroke.Transform(deltaTransform, false);
			_bounds = bounds;
			_rotation = rotation;
		}

		private void LazylyTransform(Rect bounds, double rotation)
		{
			Matrix deltaTransform = GetDeltaTransform(bounds, _bounds);
			Point point = bounds.Center();
			double angle = rotation - _rotation;
			deltaTransform.RotateAt(angle, point.X, point.Y);
			Transform = deltaTransform;
			_transformingBounds = bounds;
			_transformingRotation = rotation;
		}

		private void InnerStroke_Invalidated(object sender, EventArgs e)
		{
			InvalidateVisual();
		}

		private void UpdateBoundsAndRotation()
		{
			_originalBounds = Geometry2D.UnionRect(_originalStroke.StylusPoints.Select((StylusPoint x) => x.ToPoint()));
			_originalBounds.Inflate(Thickness / 2.0, Thickness / 2.0);
			_bounds = _originalBounds;
			_transformingBounds = _bounds;
			_transformingRotation = _rotation;
		}

		public InkSaveInfo GetSaveInfo()
		{
			return (InnerStroke as StrokeBase)?.GetInkSaveInfo();
		}

		public void SetSaveInfo(InkSaveInfo saveInfo)
		{
			string value;
			if (StrokeTypes.TryGetValue(saveInfo.Type, out value))
			{
				StrokeBase strokeBase = _cache[value]();
				if (strokeBase != null)
				{
					strokeBase.SetInkSaveInfo(saveInfo);
					InnerStroke = strokeBase;
					InvalidateVisual();
				}
			}
		}

		void IEraseSensitive.OnErasingStarted()
		{
			_tempInkContainer = new StrokeCollection();
			((IErasableInk)this).IsErasing = true;
			((IErasableInk)this).TempInkContainer.Add(InnerStroke);
		}

		void IEraseSensitive.OnErasingCompleted()
		{
			((IErasableInk)this).IsErasing = false;
			((IErasableInk)this).TempInkContainer.Clear();
		}

		public virtual IEnumerable<Element> CreateNewInkOnErased()
		{
			return ((IErasableInk)this).TempInkContainer.Distinct().Select((Stroke temp) =>
			{
				Element element = CreateNewInkOnErased(temp);
				element.SetInitializedLevelProperties(new InitializedLevelProperties(this, InsertAction.Before));
				return element;
			}).ToList();
		}

		protected virtual Element CreateNewInkOnErased(Stroke stroke)
		{
			return new Ink(stroke);
		}
	}
}
