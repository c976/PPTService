using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	
	internal struct LineOffset
	{
		public DocumentOffset DocumentOffset { get; }

		public int Offset { get; }

		public LineOffset(int offset, int lineStartOffset)
		{
			Offset = offset;
			DocumentOffset = new DocumentOffset(offset + lineStartOffset);
		}

		public LineOffset(DocumentOffset documentOffset, int offset)
		{
			DocumentOffset = new DocumentOffset((int)documentOffset + offset);
			Offset = offset;
		}
	}
}
