using System;
using System.Windows;
using System.Windows.Input;
using Cvte.Paint.Framework.Settings;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework
{
	public sealed class SelectionCursorModeOperation : IModePrepareOperation, IModeOperation, IModeInputPrepareOperation, IModeHoverOperation
	{
		private Cursor _dragableCursor;

		private Cursor _draggingCursor;

		private Lazy<DeviceInputDoubleDifferentialValue> _hitTolerance;

		public Slide CurrentSlide { get; set; }

		public void OnSwitchOn()
		{
			BoardSettingsProvider boardSettings = CurrentSlide.Settings();
			_dragableCursor = boardSettings.GetSettings<CursorBoardSettings>().DragableCursor;
			_draggingCursor = boardSettings.GetSettings<CursorBoardSettings>().DraggingCursor;
			_hitTolerance = new Lazy<DeviceInputDoubleDifferentialValue>(() => boardSettings.GetSettings<ThresholdBoardSettings>().HitTestTolerance);
		}

		public void OnSwitchOff()
		{
			if (CurrentSlide?.Canvas == null)
			{
				throw new InvalidOperationException(string.Format("关闭 SelectionCursorModeOperation 时发现意外的环境，CurrentSlide={0}，CurrentSlide.Canvas={1}。", ((object)CurrentSlide) ?? ((object)"null"), ((object)CurrentSlide?.Canvas) ?? ((object)"null")));
			}
			CurrentSlide.Canvas.ClearValue(FrameworkElement.CursorProperty);
			_hitTolerance = null;
		}

		public void OnInputStarting(ModeInputEventArgs e)
		{
			if (e.Button != DeviceButton.Context && e.Button != DeviceButton.Eraser)
			{
				if (e.Type == DeviceType.Touch)
				{
					CurrentSlide.Canvas.ClearValue(FrameworkElement.CursorProperty);
				}
				else if (CurrentSlide.Canvas.Cursor == _dragableCursor)
				{
					CurrentSlide.Canvas.Cursor = _draggingCursor;
				}
				else
				{
					CurrentSlide.Canvas.ClearValue(FrameworkElement.CursorProperty);
				}
			}
		}

		public void OnInputCompleted(ModeInputEventArgs e)
		{
			if (e.Type == DeviceType.Touch)
			{
				CurrentSlide.Canvas.ClearValue(FrameworkElement.CursorProperty);
				return;
			}
			Element element = CurrentSlide.HitElement(e.Position, GetAllowance(e.Type));
			if (element != null && !element.GetIsLocked())
			{
				CurrentSlide.Canvas.Cursor = _dragableCursor;
			}
			else
			{
				CurrentSlide.Canvas.ClearValue(FrameworkElement.CursorProperty);
			}
		}

		public void OnHover(ModeInputEventArgs e)
		{
			if (e.Type == DeviceType.Touch)
			{
				CurrentSlide.Canvas.Cursor = Cursors.None;
				return;
			}
			Element element = e.Element;
			if (element == null || element.GetIsLocked())
			{
				CurrentSlide.Canvas.ClearValue(FrameworkElement.CursorProperty);
			}
			else
			{
				CurrentSlide.Canvas.Cursor = _dragableCursor;
			}
		}

		private double GetAllowance(DeviceType type)
		{
			DeviceInputDoubleDifferentialValue value = _hitTolerance.Value;
			switch (type)
			{
			case DeviceType.Mouse:
				return value.Mouse;
			case DeviceType.Touch:
				return value.Touch;
			case DeviceType.Stylus:
				return value.Stylus;
			default:
				return 0.0;
			}
		}
	}
}
