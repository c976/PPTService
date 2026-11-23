using System.Collections.Generic;
using System.Globalization;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Culture
{
	internal class ZhCHSLanguage : TextLanguage
	{
		public ZhCHSLanguage()
			: base(new CultureInfo("zh-CHS"))
		{
		}

		protected override Dictionary<string, string> Read()
		{
			return new Dictionary<string, string>
			{
				{ "Lang.TextEditor.Copy", "复制" },
				{ "Lang.TextEditor.Cut", "剪切" },
				{ "Lang.TextEditor.Paste", "粘贴" },
				{ "Lang.TextEditor.AddHyperlink", "添加超链接" },
				{ "Lang.TextEditor.EditHyperlink", "编辑超链接" },
				{ "Lang.TextEditor.OpenHyperlink", "打开超链接" },
				{ "Lang.TextEditor.RemoveHyperlink", "取消超链接" }
			};
		}
	}
}
