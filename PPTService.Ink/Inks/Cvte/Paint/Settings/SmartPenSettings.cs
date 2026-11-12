using System;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;

namespace Cvte.Paint.Settings
{
	public class SmartPenSettings : BoardFilterSettings
	{
		public Predicate<Mode> OnCanWrite { get; set; }

		[Obsolete("请使用 SmartPenSelectionButtonSettings")]
		public Predicate<Mode> OnCanSelect { get; set; }
	}
}
