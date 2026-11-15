using System;

namespace PPTService.Ink.Core.Windows
{
	[Flags]
	public enum SingleInstanceOptions
	{
		None = 0,
		AllowSetForegroundWindow = 1
	}
}
