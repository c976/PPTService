using System;
using System.Windows.Input;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Editing
{
	internal class TextInputManager
	{
		private string _emojiCache = string.Empty;

		private DocumentManager DocumentManager { get; }

		public TextInputManager(DocumentManager documentManager)
		{
			DocumentManager = documentManager;
		}

		public void PerformTextInput(TextCompositionEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (e.Handled || string.IsNullOrEmpty(e.Text) || e.Text == "\u001b" || e.Text == "\b" || e.Text == "\ufe0f")
			{
				return;
			}
			if (RegexPatterns.Utf16SurrogatesPattern.ContainInRange(e.Text))
			{
				if (string.IsNullOrEmpty(_emojiCache))
				{
					_emojiCache += e.Text;
					return;
				}
				_emojiCache += e.Text;
				DocumentManager.EditChar(_emojiCache);
				_emojiCache = string.Empty;
			}
			else
			{
				_emojiCache = string.Empty;
				DocumentManager.EditChar(e.Text);
			}
		}
	}
}
