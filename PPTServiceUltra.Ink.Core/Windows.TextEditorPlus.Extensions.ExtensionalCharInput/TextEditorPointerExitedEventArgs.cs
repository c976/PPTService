using System;
using PPTServiceUltra.Ink.Core.Windows.Input;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Editing;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions.ExtensionalCharInput
{
	public class TextEditorPointerExitedEventArgs : EventArgs
	{
		public IInputInteropExtensionalChar ExtensionalChar { get; }

		public DeviceInputEventArgs DeviceInputEventArgs { get; }

		public TextArea TextArea { get; }

		public TextEditorPointerExitedEventArgs(IInputInteropExtensionalChar extensionalChar, DeviceInputEventArgs deviceInputEventArgs, TextArea textArea)
		{
			ExtensionalChar = extensionalChar;
			DeviceInputEventArgs = deviceInputEventArgs;
			TextArea = textArea;
		}
	}
}
