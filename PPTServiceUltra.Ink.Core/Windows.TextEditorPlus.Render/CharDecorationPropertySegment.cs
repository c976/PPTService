using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class CharDecorationPropertySegment : ISegment
	{
		public int StartOffset { get; }

		public int Length { get; set; } = 1;

		public int EndOffset => StartOffset + Length;

		public int FrontOffset
		{
			get
			{
				if (StartOffset <= EndOffset)
				{
					return StartOffset;
				}
				return EndOffset;
			}
		}

		public int BehindOffset
		{
			get
			{
				if (StartOffset <= EndOffset)
				{
					return EndOffset;
				}
				return StartOffset;
			}
		}

		public CharDecorationPropertySegment(int startOffset)
		{
			StartOffset = startOffset;
		}
	}
}
