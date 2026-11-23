using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions
{
	public class ExtensionalCharRoutedEventArgs : RoutedEventArgs
	{
		public ExtensionalChar ExtensionalChar { get; }

		public ExtensionalCharRoutedEventArgs(RoutedEvent routedEvent, ExtensionalChar extensionalChar)
			: base(routedEvent)
		{
			ExtensionalChar = extensionalChar;
		}
	}
}
