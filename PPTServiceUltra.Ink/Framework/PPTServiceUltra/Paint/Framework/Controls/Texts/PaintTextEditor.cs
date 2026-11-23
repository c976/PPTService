using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;

namespace PPTServiceUltra.Paint.Framework.Controls.Texts
{
	public class PaintTextEditor : TextEditor
	{
		public PaintTextEditor(bool intoEditAfterLoaded)
			: base(intoEditAfterLoaded)
		{
		}

		public PaintTextEditor()
			: this(true)
		{
		}

		public override void SetRichText(RichTextSaveInfo saveInfo, bool withForceRedraw = true)
		{
			PreSetSaveInfo(saveInfo);
			base.SetRichText(saveInfo, withForceRedraw);
		}

		public override RichTextSaveInfo GetRichText()
		{
			RichTextSaveInfo richText = base.GetRichText();
			PreGetSaveInfo(richText);
			return richText;
		}

		private void PreSetSaveInfo(RichTextSaveInfo saveInfo)
		{
		}

		private void PreGetSaveInfo(RichTextSaveInfo saveInfo)
		{
		}
	}
}
