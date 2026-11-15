using System.Windows;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DownMoveUpToDragTapEventConverter
	{
		public delegate void TapHandler(Point originPoint, Point currentPoint);

		public delegate void DragHandler(Point originPoint, Point currentPoint, Vector offsetToOrigin, Vector offsetToLast);

		private readonly TapHandler _tap;

		private readonly DragHandler _dragStarted;

		private readonly DragHandler _dragDelta;

		private readonly DragHandler _dragCompleted;

		private double _tolerance;

		private Point _originPoint;

		private Point _currentPoint;

		private Vector _offsetToOrigin;

		private Vector _offsetToLast;

		private bool _isDragging;

		public DownMoveUpToDragTapEventConverter(TapHandler tap, DragHandler dragStarted, DragHandler dragDelta, DragHandler dragCompleted)
		{
			_tap = tap;
			_dragStarted = dragStarted;
			_dragDelta = dragDelta;
			_dragCompleted = dragCompleted;
		}

		public void Down(Point point, double tolerance)
		{
			_tolerance = tolerance;
			_originPoint = point;
			_currentPoint = point;
			_offsetToOrigin = default(Vector);
			_offsetToLast = default(Vector);
			_isDragging = false;
		}

		public void Move(Point point)
		{
			_offsetToOrigin = point - _originPoint;
			_offsetToLast = point - _currentPoint;
			_currentPoint = point;
			if (!_isDragging)
			{
				if (_offsetToOrigin.Length >= _tolerance)
				{
					_isDragging = true;
					_dragStarted(_originPoint, _originPoint, default(Vector), default(Vector));
					_dragDelta(_originPoint, _currentPoint, _offsetToOrigin, _offsetToLast);
				}
			}
			else
			{
				_dragDelta(_originPoint, _currentPoint, _offsetToOrigin, _offsetToLast);
			}
		}

		public void Up(Point point)
		{
			_offsetToOrigin = point - _originPoint;
			_offsetToLast = point - _currentPoint;
			_currentPoint = point;
			if (!_isDragging)
			{
				_tap(_originPoint, _currentPoint);
			}
			else
			{
				_dragCompleted(_originPoint, _currentPoint, _offsetToOrigin, _offsetToLast);
			}
		}
	}
}
