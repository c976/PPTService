using System.Windows;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal interface ICharVisual
	{
		Rect Bounds { get; set; }

		CharFlowDirection Direction { get; }

		FlowDirection ActualDirection { get; set; }
	}
}
