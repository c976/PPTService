using System;
using PPTService.Paint.Framework;
using PPTService.Paint.Framework.Settings;

namespace PPTService.Paint.Settings
{
	public class SmartPenSettings : BoardFilterSettings
	{
		public Predicate<Mode> OnCanWrite { get; set; }

		[Obsolete("请使用 SmartPenSelectionButtonSettings")]
		public Predicate<Mode> OnCanSelect { get; set; }
	}
}
