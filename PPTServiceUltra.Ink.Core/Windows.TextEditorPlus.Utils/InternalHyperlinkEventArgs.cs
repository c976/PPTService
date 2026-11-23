using System;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal class InternalHyperlinkEventArgs : EventArgs
	{
		public string Hyperlink { get; set; }

		public InternalHyperlinkEventArgs(string hyperlink)
		{
			Hyperlink = hyperlink;
		}
	}
}
