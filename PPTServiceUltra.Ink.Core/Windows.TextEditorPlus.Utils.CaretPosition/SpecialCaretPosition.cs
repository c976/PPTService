using System;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.CaretPosition
{
	[Flags]
	public enum SpecialCaretPosition
	{
		DocumentNone = 0,
		DocumentStart = 0x110,
		DocumentTop = 0x100,
		DocumentEnd = 0x11000,
		DocumentBottom = 0x10000,
		DocumentAll = DocumentStart | DocumentEnd
	}
}
