using System.Collections.Generic;
using System.Globalization;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Culture
{
	internal class ZhCHTLanguage : TextLanguage
	{
		public ZhCHTLanguage()
			: base(new CultureInfo("zh-CHT"))
		{
		}

		protected override Dictionary<string, string> Read()
		{
			return new Dictionary<string, string>
			{
				{ "Lang.TextEditor.Copy", "複製" },
				{ "Lang.TextEditor.Cut", "剪切" },
				{ "Lang.TextEditor.Paste", "粘貼" },
				{ "Lang.TextEditor.AddHyperlink", "添加超鏈接" },
				{ "Lang.TextEditor.EditHyperlink", "編輯超鏈接" },
				{ "Lang.TextEditor.OpenHyperlink", "打開超鏈接" },
				{ "Lang.TextEditor.RemoveHyperlink", "取消超鏈接" }
			};
		}
	}
}
