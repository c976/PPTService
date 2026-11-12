using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;
using Cvte.Windows.Threading;

namespace Cvte.Paint.Inks
{
	public class LassoEraserMode : Mode, IEraserMode
	{
		private Ink[] _inkArray;

		private List<Ink> _deleInk;

		private List<Point> _points;

		private EraserMask _eraserMask;

		private static readonly int MaxPoints = 3;

		private StrokeCollection _strokeCollection;

		private readonly DispatcherTask _autoHideEraserTask;

		protected PointEraserBoardSettings PointEraserBoardSettings => CurrentSlide.GetSettings<PointEraserBoardSettings>();

		public FrameworkElement EraserCursor { get; set; }

		public LassoEraserMode()
		{
			EraserCursor = MakeDefaultCursorElement(default(Size));
			_autoHideEraserTask = new DispatcherTask(500.0, HideEraserCursor);
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			_deleInk = new List<Ink>();
			_points = new List<Point>();
			_strokeCollection = new StrokeCollection();
			IEnumerable<Ink> enumerable = CurrentSlide.Elements.OfType<Ink>();
			_inkArray = (enumerable as Ink[]) ?? enumerable.ToArray();
			Ink[] inkArray = _inkArray;
			for (int i = 0; i < inkArray.Length; i++)
			{
				Stroke innerStroke = inkArray[i].InnerStroke;
				_strokeCollection.Add(innerStroke);
			}
			if (GetDeviceCount() == 1)
			{
				_eraserMask = new EraserMask(CurrentSlide);
			}
			base.OnDeviceDown(e);
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			if (GetMainDeviceId() != e.Id)
			{
				return;
			}
			Point position = e.Position;
			MoveEraserCursorTo(position);
			_points.Add(position);
			foreach (Stroke item in _strokeCollection.HitTest(_points, new EllipseStylusShape(PointEraserBoardSettings.PointEraserSize.Width, PointEraserBoardSettings.PointEraserSize.Height, 0.0)))
			{
				Ink[] inkArray = _inkArray;
				foreach (Ink ink in inkArray)
				{
					if (item == ink.InnerStroke && CurrentSlide.Contains(ink))
					{
						ink.Visibility = Visibility.Hidden;
						_deleInk.Add(ink);
						_strokeCollection.Remove(item);
					}
				}
			}
			if (_points.Count > MaxPoints)
			{
				_points.RemoveRange(0, _points.Count - MaxPoints);
			}
			_eraserMask?.MoveTexture(position, PointEraserBoardSettings.PointEraserSize);
			base.OnDeviceMove(e);
		}

		protected override void OnDeviceHover(ModeInputEventArgs e)
		{
			MoveEraserCursorTo(e.Position);
		}

		protected override void OnDeviceUp(ModeInputEventArgs e)
		{
			if (_deleInk.Any())
			{
				List<Ink> list = _deleInk.Where((Ink ink) => CurrentSlide.Contains(ink)).ToList();
				CurrentSlide.Remove(list);
				foreach (Ink item in list)
				{
					item.Visibility = Visibility.Visible;
				}
				RemoveElementsOperation operation = new RemoveElementsOperation(CurrentSlide, list);
				Board.GetUndoRedoProvider(CurrentSlide).Insert(operation);
			}
			if (GetDeviceCount() == 0)
			{
				ElementOperation elementOperation = _eraserMask?.CreateUndoRedo();
				if (elementOperation != null)
				{
					Board.GetUndoRedoProvider(CurrentSlide).Insert(elementOperation);
				}
			}
			base.OnDeviceUp(e);
		}

		protected override void OnSwitchOn()
		{
			base.OnSwitchOn();
			CurrentSlide.Cursor = Cursors.None;
			CurrentSlide.MouseLeave += CurrentSlide_MouseLeave;
		}

		protected override void OnSwitchOff()
		{
			base.OnSwitchOff();
			CurrentSlide.RemoveUtilities(EraserCursor);
			CurrentSlide.ClearValue(FrameworkElement.CursorProperty);
			CurrentSlide.MouseLeave -= CurrentSlide_MouseLeave;
			_eraserMask = null;
		}

		public static FrameworkElement MakeDefaultCursorElement(Size pointEraserSize)
		{
			return null;
		}

		private void UpdateEraserCursor()
		{
			EraserCursor.Width = PointEraserBoardSettings.PointEraserSize.Width;
			EraserCursor.Height = PointEraserBoardSettings.PointEraserSize.Height;
		}

		private void MoveEraserCursorTo(Point point)
		{
			if (VisualTreeHelper.GetParent(EraserCursor) == null)
			{
				CurrentSlide.AddUtilities(EraserCursor);
				UpdateEraserCursor();
			}
			EraserCursor.BeginAnimation(UIElement.OpacityProperty, null);
			EraserCursor.Opacity = 1.0;
			TranslateTransform translateTransform = EraserCursor.RenderTransform as TranslateTransform;
			if (translateTransform == null)
			{
				translateTransform = new TranslateTransform();
				EraserCursor.RenderTransform = translateTransform;
			}
			translateTransform.X = point.X - PointEraserBoardSettings.PointEraserSize.Width / 2.0;
			translateTransform.Y = point.Y - PointEraserBoardSettings.PointEraserSize.Height / 2.0;
			_autoHideEraserTask.Restart();
		}

		private void CurrentSlide_MouseLeave(object sender, MouseEventArgs e)
		{
			CurrentSlide.RemoveUtilities(EraserCursor);
		}

		private void HideEraserCursor()
		{
			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				_autoHideEraserTask.Restart();
				return;
			}
			DoubleAnimation animation = new DoubleAnimation
			{
				To = 0.0,
				Duration = TimeSpan.FromMilliseconds(200.0)
			};
			EraserCursor.BeginAnimation(UIElement.OpacityProperty, animation);
		}
	}
}
