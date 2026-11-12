using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CircleErasing
{
	internal class CircleErasingTester : ErasingTesterBase, IErasingTester
	{
		private Point _currentPoint;

		private Point _lastPoint;

		private Geometry _resultGeometry;

		private 圆 当前落点的圆 { get; set; }

		private 两圆轨迹 当前轨迹线 { get; set; }

		private double 半径 { get; set; } = 75.0;

		private 圆 前一个落点圆 { get; set; }

		private EraserTouchEventArgs? LastEraserTouchEventArgs { get; set; }

		public void Move(Point point)
		{
			Move(point, 半径);
		}

		public void Move(Point point, double 半径 = 75.0)
		{
			_currentPoint = point;
			this.半径 = 半径;
			当前落点的圆 = new 圆(_currentPoint, 半径);
			两圆轨迹 两圆轨迹2 = 前一个落点圆.求两圆轨迹线(当前落点的圆);
			当前轨迹线 = 两圆轨迹2;
			_resultGeometry = null;
		}

		public void Move(double 半径)
		{
			Point currentPoint = _currentPoint;
			Move(currentPoint, 半径);
		}

		public bool HitTest(Point point)
		{
			if (当前轨迹线 == null)
			{
				if (前一个落点圆 != null)
				{
					return 前一个落点圆.Contains(point);
				}
				return false;
			}
			if (当前轨迹线.Contains(point))
			{
				return true;
			}
			return false;
		}

		public Geometry GetCurrentGeometry()
		{
			两圆轨迹 两圆轨迹2 = 当前轨迹线;
			return new PathGeometry(new PathFigure[1]
			{
				new PathFigure(两圆轨迹2.四边形的点集[0], new PathSegment[1]
				{
					new PolyLineSegment(两圆轨迹2.四边形的点集, true)
				}, true)
			});
		}

		public void SetLastPoint(Point point, double 半径 = 75.0)
		{
			_lastPoint = point;
			前一个落点圆 = new 圆(_lastPoint, 半径);
		}

		public override void Move([In]ref EraserTouchEventArgs eraserTouchEventArgs)
		{
			if (LastEraserTouchEventArgs.HasValue)
			{
				if (!LastEraserTouchEventArgs.Value.Equals(eraserTouchEventArgs))
				{
					SetLastPoint(LastEraserTouchEventArgs.Value.Position, LastEraserTouchEventArgs.Value.Width / 2.0);
					LastEraserTouchEventArgs = eraserTouchEventArgs;
					Move(eraserTouchEventArgs.Position, eraserTouchEventArgs.Width / 2.0);
				}
			}
			else
			{
				SetLastPoint(eraserTouchEventArgs.Position, eraserTouchEventArgs.Width / 2.0);
				LastEraserTouchEventArgs = eraserTouchEventArgs;
			}
		}

		protected override bool CheckErasedPoint(Point point)
		{
			return HitTest(point);
		}

		protected override Rect GetCurrentBounds()
		{
			if (当前轨迹线 == null)
			{
				if (前一个落点圆 != null)
				{
					return 前一个落点圆.Bounds;
				}
				return Rect.Empty;
			}
			return 当前轨迹线.GetBounds();
		}

		protected override bool CheckCanEraseTest()
		{
			if (当前轨迹线 == null && 前一个落点圆 == null)
			{
				return false;
			}
			return true;
		}

		protected override Geometry GetEraserGeometry()
		{
			if (当前轨迹线 == null)
			{
				return Geometry.Empty;
			}
			if (_resultGeometry == null)
			{
				StreamGeometry streamGeometry = new StreamGeometry();
				using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
				{
					streamGeometryContext.BeginFigure(当前轨迹线.四边形的点集[0], true, false);
					streamGeometryContext.LineTo(当前轨迹线.四边形的点集[1], false, false);
					streamGeometryContext.LineTo(当前轨迹线.四边形的点集[2], false, false);
					streamGeometryContext.LineTo(当前轨迹线.四边形的点集[3], false, false);
				}
				PathGeometry flattenedPathGeometry = new EllipseGeometry(当前轨迹线.圆1.Bounds).GetFlattenedPathGeometry(0.05, ToleranceType.Relative);
				PathGeometry flattenedPathGeometry2 = new EllipseGeometry(当前轨迹线.圆2.Bounds).GetFlattenedPathGeometry(0.05, ToleranceType.Relative);
				_resultGeometry = Geometry.Combine(flattenedPathGeometry, Geometry.Combine(streamGeometry.GetFlattenedPathGeometry(0.05, ToleranceType.Relative), flattenedPathGeometry2, GeometryCombineMode.Union, Transform.Identity), GeometryCombineMode.Union, Transform.Identity);
			}
			return _resultGeometry;
		}
	}
}
