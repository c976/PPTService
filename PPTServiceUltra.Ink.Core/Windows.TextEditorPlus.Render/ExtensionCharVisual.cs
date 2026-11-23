using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class ExtensionCharVisual : ICharVisual
	{
		internal UIElement View { get; set; }

		public Rect Bounds { get; set; }

		public CharFlowDirection Direction { get; }

		public FlowDirection ActualDirection { get; set; }

		internal Point LogicalCenter { get; set; }

		internal bool IsNeedRotationUnderVertical { get; set; }

		internal bool IsHitTestVisible { get; set; }
	}
}
