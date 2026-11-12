using System;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;

namespace Cvte.Paint.Settings
{
	public class SmartPenSelectionButtonSettings : BoardFilterSettings
	{
		public Predicate<Mode> OnCanSelect { get; set; }
	}
}
