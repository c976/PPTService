using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class AttachedDecorationData
	{
		public TextEditorDecoration TextEditorDecoration { get; set; }

		public Rect Bounds { get; set; }

		public Brush Foreground { get; set; }
	}
}
