using System.Windows.Input;

namespace Cvte.Paint.Framework.Settings
{
	public class CursorBoardSettings : BoardSettings
	{
		public Cursor DragableCursor { get; set; } = CustomCursors.GetCursor(CursorStyle.Move);

		public Cursor DraggingCursor { get; set; } = CustomCursors.GetCursor(CursorStyle.Move);

		public Cursor RotateCursor { get; set; } = CustomCursors.GetCursor(CursorStyle.Rotate);
	}
}
