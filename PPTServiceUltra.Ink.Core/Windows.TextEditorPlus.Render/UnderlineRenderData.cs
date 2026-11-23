using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class UnderlineRenderData
	{
		internal System.Windows.Rect UnderlineBounds { get; set; }

		internal PPTServiceUltra.Ink.Core.Storage.Standard.Brush Foreground { get; set; }

		internal System.Windows.Media.DashStyle DashStyle { get; set; }
	}
}
