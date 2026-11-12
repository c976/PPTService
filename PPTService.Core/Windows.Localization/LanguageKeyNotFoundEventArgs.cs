using System;
using System.Diagnostics;
using System.Globalization;

namespace PPTService.Core.Windows.Localization
{
	[DebuggerDisplay("Culture={Culture}, Key={Key}")]
	public class LanguageKeyNotFoundEventArgs : EventArgs
	{
		public string Key { get; private set; }

		public CultureInfo Culture { get; private set; }

		internal LanguageKeyNotFoundEventArgs(string key, CultureInfo culture)
		{
			Key = key;
			Culture = culture;
		}
	}
}
