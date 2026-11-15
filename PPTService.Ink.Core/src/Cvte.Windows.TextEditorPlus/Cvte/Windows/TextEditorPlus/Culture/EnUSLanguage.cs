using System.Collections.Generic;
using System.Globalization;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Culture
{
	internal class EnUSLanguage : TextLanguage
	{
		public EnUSLanguage()
			: base(new CultureInfo("en-US"))
		{
		}

		protected override Dictionary<string, string> Read()
		{
			return new Dictionary<string, string>
			{
				{ "Lang.TextEditor.Copy", "Copy" },
				{ "Lang.TextEditor.Cut", "Cut" },
				{ "Lang.TextEditor.Paste", "Paste" },
				{ "Lang.TextEditor.AddHyperlink", "Add Hyperlink" },
				{ "Lang.TextEditor.EditHyperlink", "Edit Hyperlink" },
				{ "Lang.TextEditor.OpenHyperlink", "Open Hyperlink" },
				{ "Lang.TextEditor.RemoveHyperlink", "Remove Hyperlink" }
			};
		}
	}
}
