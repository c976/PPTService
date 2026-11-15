using System.Collections.Generic;
using System.Globalization;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Culture
{
	internal abstract class TextLanguage : ILanguage
	{
		private Dictionary<string, string> _language;

		public CultureInfo Culture { get; }

		public Dictionary<string, string> Language => _language ?? (_language = Read());

		protected TextLanguage(CultureInfo culture)
		{
			Culture = culture;
		}

		protected abstract Dictionary<string, string> Read();
	}
}
