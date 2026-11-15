using System;
using System.Windows;
using PPTService.Ink.Core.Windows.Input;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Editing;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Extensions.ExtensionalCharInput
{
	public class TextEditorInputEventArgs : EventArgs
	{
		private readonly Lazy<Point> _position;

		public Point Position => _position.Value;

		public DeviceInputEventArgs DeviceInputEventArgs { get; }

		public TextArea TextArea { get; }

		public DocumentOffset DocumentOffset { get; }

		public Rect? RenderBounds { get; }

		public IInputInteropExtensionalChar ExtensionalChar { get; }

		public TextEditorInputEventArgs(DeviceInputEventArgs deviceInputEventArgs, TextArea textArea, DocumentOffset documentOffset, Rect? renderBounds = null, IInputInteropExtensionalChar extensionalChar = null)
		{
			DeviceInputEventArgs = deviceInputEventArgs;
			TextArea = textArea;
			DocumentOffset = documentOffset;
			RenderBounds = renderBounds;
			ExtensionalChar = extensionalChar;
			_position = new Lazy<Point>(GetExtensionalCharPosition);
		}

		private Point GetExtensionalCharPosition()
		{
			Point position = DeviceInputEventArgs.Position;
			Point topLeft = ((!RenderBounds.HasValue) ? TextArea.RenderManager.GetRunBoundsByDocumentOffset(DocumentOffset) : RenderBounds.Value).TopLeft;
			Vector vector = position - topLeft;
			return new Point(vector.X, vector.Y);
		}
	}
}
