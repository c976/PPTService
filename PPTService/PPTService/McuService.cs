using PPTService.Core.Mcu;

namespace PPTService
{
	public static class McuService
	{
		public static bool FindMcu()
		{
			return Mcu.FindMcu();
		}

		public static bool ForbiddenAndroidPen()
		{
			return Mcu.ForbiddenAndroidPen();
		}

		public static bool OpenAndroidPen()
		{
			return Mcu.OpenAndroidPen();
		}
	}
}
