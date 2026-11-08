using System;

namespace Cvte.Mcu
{
	[Flags]
	public enum Digcf
	{
		DigcfDefault = 1,
		DigcfPresent = 2,
		DigcfAllclasses = 4,
		DigcfProfile = 8,
		DigcfDeviceinterface = 0x10
	}
}
