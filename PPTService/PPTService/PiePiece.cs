using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PPTService
{
	public class PiePiece : Canvas
	{
		public delegate void PiePieceClicked(PiePiece sender);

		public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(PiePiece), new PropertyMetadata(OnDependencyPropertyChanged));

		public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(PiePiece), new PropertyMetadata(OnDependencyPropertyChanged));

		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(PiePiece), new PropertyMetadata(OnDependencyPropertyChanged));

		public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register("InnerRadius", typeof(double), typeof(PiePiece), new PropertyMetadata(OnDependencyPropertyChanged));

		public static readonly DependencyProperty WedgeAngleProperty = DependencyProperty.Register("WedgeAngle", typeof(double), typeof(PiePiece), new PropertyMetadata(OnDependencyPropertyChanged));

		public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register("RotationAngle", typeof(double), typeof(PiePiece), new PropertyMetadata(OnDependencyPropertyChanged));

		public static readonly DependencyProperty CentreYProperty = DependencyProperty.Register("CentreY", typeof(double), typeof(PiePiece), new PropertyMetadata(OnDependencyPropertyChanged));

		public static readonly DependencyProperty CentreXProperty = DependencyProperty.Register("CentreX", typeof(double), typeof(PiePiece), new PropertyMetadata(OnDependencyPropertyChanged));

		public double Radius
		{
			get
			{
				return (double)GetValue(RadiusProperty);
			}
			set
			{
				SetValue(RadiusProperty, value);
			}
		}

		public Brush Fill
		{
			get
			{
				return (Brush)GetValue(FillProperty);
			}
			set
			{
				SetValue(FillProperty, value);
			}
		}

		public Brush Stroke
		{
			get
			{
				return (Brush)GetValue(StrokeProperty);
			}
			set
			{
				SetValue(StrokeProperty, value);
			}
		}

		public double InnerRadius
		{
			get
			{
				return (double)GetValue(InnerRadiusProperty);
			}
			set
			{
				SetValue(InnerRadiusProperty, value);
			}
		}

		public double WedgeAngle
		{
			get
			{
				return (double)GetValue(WedgeAngleProperty);
			}
			set
			{
				SetValue(WedgeAngleProperty, value);
			}
		}

		public double RotationAngle
		{
			get
			{
				return (double)GetValue(RotationAngleProperty);
			}
			set
			{
				SetValue(RotationAngleProperty, value);
			}
		}

		public double CentreY
		{
			get
			{
				return (double)GetValue(CentreYProperty);
			}
			set
			{
				SetValue(CentreYProperty, value);
			}
		}

		public double CentreX
		{
			get
			{
				return (double)GetValue(CentreXProperty);
			}
			set
			{
				SetValue(CentreXProperty, value);
			}
		}

		public double PieceValue { get; set; }

		public event PiePieceClicked PiePieceClickedEvent;

		private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as PiePiece).AddPathToCanvas();
		}

		public PiePiece()
		{
			AddPathToCanvas();
			MouseLeftButtonUp += PiePiece_MouseLeftButtonUp;
		}

		private void PiePiece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			HitTestResult hitTestResult = VisualTreeHelper.HitTest(this, e.GetPosition(null));
			if (hitTestResult != null && hitTestResult.VisualHit is Path && hitTestResult.VisualHit is Path && PiePieceClickedEvent != null)
			{
				PiePieceClickedEvent(this);
			}
		}

		private void AddPathToCanvas()
		{
			Children.Clear();
			Path element = ConstructPath();
			Children.Add(element);
		}

		private Path ConstructPath()
		{
			if (WedgeAngle >= 360.0)
			{
				return new Path
				{
					Fill = Fill,
					Stroke = Stroke,
					StrokeThickness = 1.0,
					Data = new GeometryGroup
					{
						FillRule = FillRule.EvenOdd,
						Children = new GeometryCollection
						{
							new EllipseGeometry
							{
								Center = new Point(CentreX, CentreY),
								RadiusX = Radius,
								RadiusY = Radius
							},
							new EllipseGeometry
							{
								Center = new Point(CentreX, CentreY),
								RadiusX = InnerRadius,
								RadiusY = InnerRadius
							}
						}
					}
				};
			}
			new Point(CentreX, CentreY);
			Point point = PointUtils.ComputeCartesianCoordinate(RotationAngle, InnerRadius).OffsetEx(CentreX, CentreY);
			Point point2 = PointUtils.ComputeCartesianCoordinate(RotationAngle + WedgeAngle, InnerRadius).OffsetEx(CentreX, CentreY);
			Point point3 = PointUtils.ComputeCartesianCoordinate(RotationAngle, Radius).OffsetEx(CentreX, CentreY);
			Point point4 = PointUtils.ComputeCartesianCoordinate(RotationAngle + WedgeAngle, Radius).OffsetEx(CentreX, CentreY);
			bool isLargeArc = WedgeAngle > 180.0;
			Size size = new Size(Radius, Radius);
			Size size2 = new Size(InnerRadius, InnerRadius);
			PathFigure value = new PathFigure
			{
				StartPoint = point,
				Segments = new PathSegmentCollection
				{
					new ArcSegment
					{
						Point = point3,
						Size = new Size(2.0, 2.0),
						IsLargeArc = isLargeArc,
						SweepDirection = SweepDirection.Clockwise,
						RotationAngle = 0.0
					},
					new ArcSegment
					{
						Point = point4,
						Size = size,
						IsLargeArc = isLargeArc,
						SweepDirection = SweepDirection.Clockwise,
						RotationAngle = 0.0
					},
					new ArcSegment
					{
						Point = point2,
						Size = new Size(2.0, 2.0),
						IsLargeArc = isLargeArc,
						SweepDirection = SweepDirection.Clockwise,
						RotationAngle = 0.0
					},
					new ArcSegment
					{
						Point = point,
						Size = size2,
						IsLargeArc = isLargeArc,
						SweepDirection = SweepDirection.Counterclockwise,
						RotationAngle = 0.0
					}
				}
			};
			return new Path
			{
				Fill = Fill,
				Stroke = Stroke,
				StrokeThickness = 1.0,
				Data = new PathGeometry
				{
					Figures = new PathFigureCollection { value }
				}
			};
		}
	}
}
