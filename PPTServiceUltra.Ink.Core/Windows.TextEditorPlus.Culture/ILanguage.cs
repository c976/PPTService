using System.Collections.Generic;
using System.Globalization;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Culture
{
	internal interface ILanguage
	{
		CultureInfo Culture { get; }

		Dictionary<string, string> Language { get; }
	}
}
