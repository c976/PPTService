namespace PPTService.Ink.Core.Windows.TextEditorPlus.Extensions.ExtensionalCharInput
{
	public interface IInputInteropExtensionalChar
	{
		void PointerDown(TextEditorInputEventArgs args);

		void PointerMove(TextEditorInputEventArgs args);

		void PointerUp(TextEditorInputEventArgs args);

		void PointerHover(TextEditorInputEventArgs args);

		void PointerEntered(TextEditorInputEventArgs args);

		void PointerExited(TextEditorPointerExitedEventArgs args);
	}
}
