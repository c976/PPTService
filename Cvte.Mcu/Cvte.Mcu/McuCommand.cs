namespace Cvte.Mcu
{
	internal static class McuCommand
	{
		public static byte[] ForbiddenAndroidPenCommand
		{
			get
			{
				byte[] array = new byte[64];
				array[0] = 252;
				array[1] = 165;
				array[2] = 5;
				array[3] = 59;
				array[4] = 0;
				array[5] = 251;
				array[6] = 42;
				array[7] = 0;
				array[8] = 1;
				array[9] = 0;
				array[10] = 0;
				return array;
			}
		}

		public static byte[] OpenAndroidPenCommand
		{
			get
			{
				byte[] array = new byte[64];
				array[0] = 252;
				array[1] = 165;
				array[2] = 5;
				array[3] = 59;
				array[4] = 0;
				array[5] = 251;
				array[6] = 42;
				array[7] = 0;
				array[8] = 1;
				array[9] = 0;
				array[10] = 1;
				return array;
			}
		}

		public static byte[] GetMcuVersionCommand
		{
			get
			{
				byte[] array = new byte[64];
				array[0] = 252;
				array[1] = 165;
				array[2] = 5;
				array[3] = 59;
				array[4] = 0;
				array[5] = 251;
				array[6] = 22;
				array[7] = 2;
				return array;
			}
		}
	}
}
