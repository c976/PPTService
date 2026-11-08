using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Office.Interop.PowerPoint;
using PPTService.Collection;

namespace PPTService
{
	public class Gesture
	{
		private Point _startPoint;

		private Rect _gestureScope;

		private double _maxLengthX;

		private double _maxLengthY;

		private readonly Stopwatch _stopwatch;

		private static readonly double MinGestureTime = 100.0;

		private static readonly double MaxGestureTime = 600.0;

		private static readonly double MinGestureLengthXorY = 30.0;

		private static readonly double MaxGestureLengthXorY = 70.0;

		public Gesture()
		{
			_stopwatch = new Stopwatch();
			ViewHelper.MainWindow.LocationChanged += (object s, EventArgs e) =>
			{
				ChangeGestureScope();
			};
			ViewHelper.MainWindow.SizeChanged += (object s, SizeChangedEventArgs e) =>
			{
				ChangeGestureScope();
			};
			MouseHelper.Instance.Down += Instance_Down;
			MouseHelper.Instance.Move += Instance_Move;
			MouseHelper.Instance.Up += Instance_Up;
		}

		public void ChangeGestureScope()
		{
			_gestureScope = new Rect(ViewHelper.MainWindow.Left, ViewHelper.MainWindow.Top, ViewHelper.MainWindow.Width, ViewHelper.MainWindow.Height);
		}

		private void Instance_Down(object sender, MouseHelperEventArgs e)
		{
			if (_gestureScope.Contains(e.MousePoint))
			{
				_maxLengthX = 0.0;
				_maxLengthY = 0.0;
				_startPoint = e.MousePoint;
				_stopwatch.Restart();
			}
		}

		private void Instance_Move(object sender, MouseHelperEventArgs e)
		{
			if (_gestureScope.Contains(e.MousePoint))
			{
				double num = Math.Abs(e.MousePoint.X - _startPoint.X);
				double num2 = Math.Abs(e.MousePoint.Y - _startPoint.Y);
				if (num > _maxLengthX)
				{
					_maxLengthX = num;
				}
				if (num2 > _maxLengthY)
				{
					_maxLengthY = num2;
				}
			}
		}

		private void Instance_Up(object sender, MouseHelperEventArgs e)
		{
			if (_gestureScope.Contains(e.MousePoint))
			{
				_stopwatch.Stop();
				if (!((double)_stopwatch.ElapsedMilliseconds > MaxGestureTime) && !((double)_stopwatch.ElapsedMilliseconds < MinGestureTime) && !HorizontalGesture(e.MousePoint))
				{
					VerticalGesture(e.MousePoint);
				}
			}
		}

		private bool HorizontalGesture(Point position)
		{
			if (_maxLengthY > MaxGestureLengthXorY)
			{
				return false;
			}
			double num = _startPoint.X - position.X;
			if (num > MinGestureLengthXorY && isPptArrow())
			{
				NextPage();
				return true;
			}
			if (num < 0.0 - MinGestureLengthXorY && isPptArrow())
			{
				PreviousPage();
				return true;
			}
			return false;
		}

		private void VerticalGesture(Point position)
		{
			if (!(_maxLengthX > MaxGestureLengthXorY))
			{
				double num = _startPoint.Y - position.Y;
				if (num > MinGestureLengthXorY && isPptArrow())
				{
					NextPage();
				}
				if (num < 0.0 - MinGestureLengthXorY && isPptArrow())
				{
					PreviousPage();
				}
			}
		}

		private bool isPptArrow()
		{
			if (PptOperator.Instance.PointerType != PpSlideShowPointerType.ppSlideShowPointerNone && PptOperator.Instance.PointerType != PpSlideShowPointerType.ppSlideShowPointerAutoArrow)
			{
				return PptOperator.Instance.PointerType == PpSlideShowPointerType.ppSlideShowPointerArrow;
			}
			return true;
		}

		private void PreviousPage()
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				if (!ViewHelper.BottomArea.IsNeedStopMouseLineTrack && !ViewHelper.MainWindow.IsLeftSlideManagerViewShowed && !ViewHelper.MainWindow.IsRightSlideManagerViewShowed && !(Math.Abs(ViewHelper.MainWindow.Opacity) < 0.1))
				{
					PptOperator.Instance.PreviousPage();
					FridayReporter.ReportEventImmediately(EventID.GesturePreviousPage);
				}
			});
		}

		private void NextPage()
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				if (!ViewHelper.BottomArea.IsNeedStopMouseLineTrack && !ViewHelper.MainWindow.IsLeftSlideManagerViewShowed && !ViewHelper.MainWindow.IsRightSlideManagerViewShowed && !(Math.Abs(ViewHelper.MainWindow.Opacity) < 0.1))
				{
					PptOperator.Instance.NextPage();
					FridayReporter.ReportEventImmediately(EventID.GestureNextPage);
				}
			});
		}
	}
}
