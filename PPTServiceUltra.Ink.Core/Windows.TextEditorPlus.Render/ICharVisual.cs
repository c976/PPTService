using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal interface ICharVisual
	{
		Rect Bounds { get; set; }

		CharFlowDirection Direction { get; }

		FlowDirection ActualDirection { get; set; }
	}
}
