using System;
using PPTService.Paint.Framework;
using PPTService.Paint.Framework.Settings;

namespace PPTService.Paint.Settings
{
	public class SmartPenSelectionButtonSettings : BoardFilterSettings
	{
		public Predicate<Mode> OnCanSelect { get; set; }
	}
}
