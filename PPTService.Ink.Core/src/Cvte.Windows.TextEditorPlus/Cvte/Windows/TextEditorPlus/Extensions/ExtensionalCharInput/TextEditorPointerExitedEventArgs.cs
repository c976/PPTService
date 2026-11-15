using System;
using PPTService.Ink.Core.Windows.Input;
using PPTService.Ink.Core.Windows.TextEditorPlus.Editing;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Extensions.ExtensionalCharInput
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
