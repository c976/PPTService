using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing
{
	internal class RectangleErasingTester : ErasingTesterBase, IErasingTester
	{
		private Rect? _currentBounds;

		private 圆角矩形 _last圆角矩形;

		private 两个圆角矩形相连后所得的包含途径范围的图形信息 _result;

		private StreamGeometry _resultGeometry;

		protected override bool CheckCanEraseTest()
		{
			if (_result == null)
			{
				if (_last圆角矩形 == null)
				{
					return false;
				}
				return true;
			}
			return true;
		}

		protected override bool CheckErasedPoint(Point point)
		{
			if (_result == null)
			{
				if (_last圆角矩形 == null)
				{
					return false;
				}
				return _last圆角矩形.Contains(point);
			}
			return _result.Contains(point);
		}

		public override void Move([In]ref EraserTouchEventArgs eraserTouchEventArgs)
		{
			_currentBounds = null;
			if (_last圆角矩形 == null)
			{
				_last圆角矩形 = Create圆角矩形(eraserTouchEventArgs);
			}
			else if (!(eraserTouchEventArgs.Position == _last圆角矩形.Position))
			{
				圆角矩形 last圆角矩形 = _last圆角矩形;
				圆角矩形 圆角矩形2 = Create圆角矩形(eraserTouchEventArgs);
				DebuggablePathInfo debuggablePathInfo = last圆角矩形.与另一圆角矩形相连(圆角矩形2);
				DrawCore(debuggablePathInfo);
				_result = debuggablePathInfo;
				_resultGeometry = null;
				_last圆角矩形 = 圆角矩形2;
			}
		}

		private void DrawCore(DebuggablePathInfo path)
		{
		}

		protected override Rect GetCurrentBounds()
		{
			if (!_currentBounds.HasValue)
			{
				if (_result != null)
				{
					_currentBounds = _result.GetBounds();
				}
				else
				{
					if (_last圆角矩形 == null)
					{
						return Rect.Empty;
					}
					_currentBounds = _last圆角矩形.GetBounds();
				}
			}
			return _currentBounds.Value;
		}

		private static 圆角矩形 Create圆角矩形(EraserTouchEventArgs eraserTouchEventArgs)
		{
			return new 圆角矩形(eraserTouchEventArgs.Width, eraserTouchEventArgs.Height, 5.0 / eraserTouchEventArgs.SlideScaleX, new Point(eraserTouchEventArgs.X, eraserTouchEventArgs.Y), eraserTouchEventArgs.Rotation);
		}

		protected override Geometry GetEraserGeometry()
		{
			if (_result == null)
			{
				return Geometry.Empty;
			}
			if (_resultGeometry == null)
			{
				_resultGeometry = new StreamGeometry();
				using (StreamGeometryContext streamGeometryContext = _resultGeometry.Open())
				{
					streamGeometryContext.BeginFigure(_result.十二边形顶点[0], true, false);
					streamGeometryContext.LineTo(_result.十二边形顶点[1], false, false);
					streamGeometryContext.LineTo(_result.十二边形顶点[2], false, false);
					streamGeometryContext.LineTo(_result.十二边形顶点[3], false, false);
					streamGeometryContext.LineTo(_result.十二边形顶点[4], false, false);
					streamGeometryContext.LineTo(_result.十二边形顶点[5], false, false);
					streamGeometryContext.LineTo(_result.十二边形顶点[6], false, false);
					streamGeometryContext.LineTo(_result.十二边形顶点[7], false, false);
					streamGeometryContext.LineTo(_result.十二边形顶点[8], false, false);
					streamGeometryContext.LineTo(_result.十二边形顶点[9], false, false);
					streamGeometryContext.LineTo(_result.十二边形顶点[10], false, false);
					streamGeometryContext.LineTo(_result.十二边形顶点[11], false, false);
				}
			}
			return _resultGeometry;
		}
	}
}
