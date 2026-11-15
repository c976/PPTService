using System;
using System.ComponentModel;
using PPTService.Paint.Framework;

namespace PPTService.Paint.Settings
{
	[Obsolete("请使用 BoardFilterProvider")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class FilterSettingsUtil
	{
		[Obsolete("请使用 ModetoFilterCanBeActivated")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static Predicate<ActiveModeFilter> DelegateTo(Func<Predicate<Mode>> delegated)
		{
			return (ActiveModeFilter filter) => delegated()?.Invoke(filter.CurrentMode) ?? true;
		}
	}
}
