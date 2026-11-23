using System;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	[Flags]
	public enum AcceptingInputTypes
	{
		Number = 1,
		LowerLetters = 0x10,
		UpperLetters = 0x100,
		Letters = LowerLetters | UpperLetters,
		English = 0x1000110,
		Hanzi = 0x10000,
		NewLine = 0x100000,
		Blank = 0x1000000,
		All = English | Number | Hanzi | NewLine
	}
}
