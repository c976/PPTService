using System;

namespace PPTService.Ink.Core.Windows.Native
{
	[Obsolete("请使用 Win32.MonitorDpiType")]
	internal enum MonitorDpiType
	{
		EffectiveDpi = 0,
		AngularDpi = 1,
		RawDpi = 2,
		Default = EffectiveDpi
	}
}
