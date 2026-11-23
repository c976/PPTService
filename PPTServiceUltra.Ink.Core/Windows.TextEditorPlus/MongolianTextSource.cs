using System;
using System.Windows.Media.TextFormatting;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus
{
	internal class MongolianTextSource : TextSource
	{
		private string _text;

		private MongolianTextRunProperties _currentRendering;

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public MongolianTextRunProperties FontRendering
		{
			get
			{
				return _currentRendering;
			}
			set
			{
				_currentRendering = value;
			}
		}

		public override TextRun GetTextRun(int textSourceCharacterIndex)
		{
			if (textSourceCharacterIndex >= _text.Length)
			{
				return new TextEndOfParagraph(1);
			}
			if (textSourceCharacterIndex < _text.Length)
			{
				return new TextCharacters(_text, textSourceCharacterIndex, 1, _currentRendering);
			}
			return new TextEndOfParagraph(1);
		}

		public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
		{
			throw new NotImplementedException();
		}

		public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
		{
			throw new NotImplementedException();
		}
	}
}
