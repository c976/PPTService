using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class UiElementCharVisual : ICharVisual
	{
		internal UIElement CharView { get; set; }

		public Rect Bounds { get; set; }

		public CharFlowDirection Direction { get; } = CharFlowDirection.LeftToRight;

		public FlowDirection ActualDirection { get; set; }
	}
}
