using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class Selection : ISelection
	{
		private CaretOffset _endOffset;

		private CaretOffset _startOffset;

		public CaretOffset FrontOffset { get; private set; }

		public CaretOffset BehindOffset { get; private set; }

		public CaretOffset StartOffset
		{
			get
			{
				return _startOffset;
			}
			set
			{
				_startOffset = value;
				UpdateProperty();
			}
		}

		public int Length { get; private set; }

		public CaretOffset EndOffset
		{
			get
			{
				return _endOffset;
			}
			set
			{
				_endOffset = value;
				UpdateProperty();
			}
		}

		public bool IsEmpty => Length == 0;

		[DebuggerStepThrough]
		public Selection(CaretOffset startOffset, CaretOffset endOffset)
		{
			_startOffset = startOffset;
			_endOffset = endOffset;
			UpdateProperty();
		}

		public Selection(CaretOffset startOffset, int length)
			: this(startOffset, new CaretOffset
			{
				Offset = startOffset.Offset + length
			})
		{
		}

		private void UpdateProperty()
		{
			Length = Math.Abs(EndOffset.Offset - StartOffset.Offset);
			if (StartOffset.Offset < EndOffset.Offset)
			{
				FrontOffset = StartOffset;
				BehindOffset = EndOffset;
			}
			else
			{
				FrontOffset = EndOffset;
				BehindOffset = StartOffset;
			}
		}

		public override bool Equals(object obj)
		{
			Selection selection = obj as Selection;
			if (selection == null)
			{
				return false;
			}
			if (StartOffset.Equals(selection.StartOffset))
			{
				return EndOffset.Equals(selection.EndOffset);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		[DebuggerStepThrough]
		public ISegment ToSegment()
		{
			return new SimpleSegment(FrontOffset.Offset + 1, Length);
		}

		public static ISelection GetFullSelection(TextEditor textEditor)
		{
			return new Selection(CaretOffset.Default, new CaretOffset
			{
				Offset = textEditor.DocumentManager.CharsManager.CharCount - 1
			});
		}
	}
}
