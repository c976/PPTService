using System.Text;

namespace Cvte.Mcu
{
	internal static class Analyzer
	{
		public static bool IsMatchCVTouchVersion(byte[] data)
		{
			if (data[6] == 198)
			{
				return data[7] == 2;
			}
			return false;
		}

		public static int GetCVTouchSize(byte[] data)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 16; i < data.Length; i++)
			{
				if (IsNumber(data[i]))
				{
					stringBuilder.Append((data[i] - 48).ToString());
					flag = true;
				}
				if (!IsNumber(data[i]) & flag)
				{
					return int.Parse(stringBuilder.ToString());
				}
			}
			return -1;
		}

		private static bool IsNumber(byte data)
		{
			if (data >= 48)
			{
				return data <= 57;
			}
			return false;
		}
	}
}
