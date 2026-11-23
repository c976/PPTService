using System;
using PPTServiceUltra.Paint.Framework;
using PPTServiceUltra.Paint.Framework.Settings;

namespace PPTServiceUltra.Paint.Settings
{
	public class SmartPenSettings : BoardFilterSettings
	{
		public Predicate<Mode> OnCanWrite { get; set; }

		[Obsolete("请使用 SmartPenSelectionButtonSettings")]
		public Predicate<Mode> OnCanSelect { get; set; }
	}
}
