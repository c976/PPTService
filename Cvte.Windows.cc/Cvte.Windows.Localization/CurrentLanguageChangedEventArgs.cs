using System;
using System.Diagnostics;
using System.Globalization;

namespace Cvte.Windows.Localization
{
	[DebuggerDisplay("Old={OldCulture}, New={NewCulture}")]
	public class CurrentLanguageChangedEventArgs : EventArgs
	{
		public CultureInfo OldCulture { get; private set; }

		public CultureInfo NewCulture { get; private set; }

		public CurrentLanguageChangedEventArgs(CultureInfo oldCulture, CultureInfo newCulture)
		{
			OldCulture = oldCulture;
			NewCulture = newCulture;
		}
	}
}
