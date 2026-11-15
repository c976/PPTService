using System;
using System.Windows;
using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	public class TouchCursor
	{
		public enum CursorMode
		{
			Unspecified,
			Hide
		}

		public static readonly DependencyProperty IsTouchCursorVisibleProperty = DependencyProperty.RegisterAttached("IsTouchCursorVisible", typeof(CursorMode), typeof(TouchCursor), new PropertyMetadata(CursorMode.Unspecified, OnIsTouchCursorVisibleChanged));

		private static readonly StylusEventHandler OnElmenetStylusUp = Element_StylusUp;

		private static readonly StylusEventHandler OnElmenetStylusMove = Element_StylusMove;

		private static readonly StylusDownEventHandler OnElmenetStylusDown = Element_StylusDown;

		private static readonly MouseButtonEventHandler OnElmenetMouseUp = Element_MouseUp;

		private static readonly MouseEventHandler OnElmenetMouseMove = ElementOnMouseMove;

		private static readonly MouseButtonEventHandler OnElmenetMouseDown = ElementOnMouseDown;

		private static readonly StylusEventHandler OnElmenetStylusInAirMove = ElmenetStylusInAirMove;

		private static readonly StylusEventHandler OnElmenetStylusOutOfRange = ElmenetStylusOutOfRange;

		private static void OnIsTouchCursorVisibleChanged(DependencyObject s, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			if (dependencyPropertyChangedEventArgs.NewValue == null)
			{
				return;
			}
			UIElement uIElement = s as UIElement;
			if (uIElement == null)
			{
				return;
			}
			if ((CursorMode)dependencyPropertyChangedEventArgs.NewValue == CursorMode.Hide)
			{
				uIElement.AddHandler(UIElement.MouseUpEvent, OnElmenetMouseUp, true);
				uIElement.AddHandler(UIElement.MouseMoveEvent, OnElmenetMouseMove, true);
				uIElement.AddHandler(UIElement.MouseDownEvent, OnElmenetMouseDown, true);
				uIElement.AddHandler(UIElement.StylusDownEvent, OnElmenetStylusDown, true);
				uIElement.AddHandler(UIElement.StylusMoveEvent, OnElmenetStylusMove, true);
				uIElement.AddHandler(UIElement.StylusUpEvent, OnElmenetStylusUp, true);
				uIElement.AddHandler(UIElement.StylusInAirMoveEvent, OnElmenetStylusInAirMove, true);
				uIElement.AddHandler(UIElement.StylusOutOfRangeEvent, OnElmenetStylusOutOfRange, true);
				Window window = s as Window;
				if (window != null)
				{
					window.Deactivated -= Window_Deactivated;
					window.Deactivated += Window_Deactivated;
				}
			}
			else if ((CursorMode)dependencyPropertyChangedEventArgs.NewValue == CursorMode.Unspecified)
			{
				uIElement.RemoveHandler(UIElement.MouseUpEvent, OnElmenetMouseUp);
				uIElement.RemoveHandler(UIElement.MouseMoveEvent, OnElmenetMouseMove);
				uIElement.RemoveHandler(UIElement.MouseDownEvent, OnElmenetMouseDown);
				uIElement.RemoveHandler(UIElement.StylusDownEvent, OnElmenetStylusDown);
				uIElement.RemoveHandler(UIElement.StylusMoveEvent, OnElmenetStylusMove);
				uIElement.RemoveHandler(UIElement.StylusUpEvent, OnElmenetStylusUp);
				uIElement.RemoveHandler(UIElement.StylusInAirMoveEvent, OnElmenetStylusInAirMove);
				uIElement.RemoveHandler(UIElement.StylusOutOfRangeEvent, OnElmenetStylusOutOfRange);
				Window window2 = s as Window;
				if (window2 != null)
				{
					window2.Deactivated -= Window_Deactivated;
				}
			}
		}

		private static void ElmenetStylusInAirMove(object sender, StylusEventArgs e)
		{
			WindowsCursorsManager.ShowCursor();
		}

		private static void ElmenetStylusOutOfRange(object sender, StylusEventArgs e)
		{
			InputTouchStylus();
		}

		private static void Element_StylusUp(object sender, StylusEventArgs e)
		{
			InputTouchStylus();
		}

		private static void Element_StylusMove(object sender, StylusEventArgs e)
		{
			InputTouchStylus();
		}

		private static void Element_StylusDown(object sender, StylusEventArgs e)
		{
			InputTouchStylus();
		}

		private static void InputTouchStylus()
		{
			WindowsCursorsManager.HideCursor();
		}

		private static void Window_Deactivated(object sender, EventArgs e)
		{
			WindowsCursorsManager.ShowCursor();
		}

		private static void Element_MouseUp(object sender, MouseEventArgs e)
		{
			InputMouse(e);
		}

		private static void ElementOnMouseMove(object sender, MouseEventArgs e)
		{
			InputMouse(e);
		}

		private static void ElementOnMouseDown(object sender, MouseEventArgs e)
		{
			InputMouse(e);
		}

		private static void InputMouse(MouseEventArgs e)
		{
			if (e.StylusDevice == null)
			{
				WindowsCursorsManager.ShowCursor();
			}
		}
	}
}
