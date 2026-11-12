using Cvte.Windows.TextEditorPlus;
using Cvte.Windows.TextEditorPlus.Storage;

namespace Cvte.Paint.Framework.Controls.Texts
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
