using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal interface ITextGeometryRender
	{
		TextGeometryRenderResult BuildCharGeometry(RunProperty runProperty, ICharObject textChar);

		TextGeometryRenderResult BuildTextGeometry(RunProperty runProperty, string text);
	}
}
