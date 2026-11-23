using System;
using PPTServiceUltra.Paint.Framework;
using PPTServiceUltra.Paint.Framework.Settings;

namespace PPTServiceUltra.Paint.Settings
{
	public class SmartPenSelectionButtonSettings : BoardFilterSettings
	{
		public Predicate<Mode> OnCanSelect { get; set; }
	}
}
