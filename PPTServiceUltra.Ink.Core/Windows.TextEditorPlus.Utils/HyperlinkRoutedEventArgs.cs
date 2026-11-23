using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	public class HyperlinkRoutedEventArgs : RoutedEventArgs
	{
		public string Hyperlink { get; }

		public HyperlinkRoutedEventArgs(string hyperlink, RoutedEvent e)
			: base(e)
		{
			Hyperlink = hyperlink;
		}
	}
}
