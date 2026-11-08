namespace PPTService
{
	public static class LongExtension
	{
		public static string ConvertMilliseconds(this long totalMilliseconds)
		{
			if (totalMilliseconds < 100)
			{
				return "0.1秒以内";
			}
			if (totalMilliseconds >= 100 && totalMilliseconds < 500)
			{
				return "0.1~0.5秒";
			}
			if (totalMilliseconds >= 500 && totalMilliseconds < 1000)
			{
				return "0.5~1秒";
			}
			if (totalMilliseconds >= 1000 && totalMilliseconds < 1500)
			{
				return "1~1.5秒";
			}
			if (totalMilliseconds >= 1500 && totalMilliseconds < 2000)
			{
				return "1.5~2秒";
			}
			if (totalMilliseconds >= 2000 && totalMilliseconds < 2500)
			{
				return "2~2.5秒";
			}
			if (totalMilliseconds >= 2500 && totalMilliseconds < 3000)
			{
				return "2.5~3秒";
			}
			if (totalMilliseconds >= 3000 && totalMilliseconds < 4000)
			{
				return "3~4秒";
			}
			if (totalMilliseconds >= 4000 && totalMilliseconds < 5000)
			{
				return "4~5秒";
			}
			if (totalMilliseconds >= 5000 && totalMilliseconds < 10000)
			{
				return "5~10秒";
			}
			if (totalMilliseconds >= 10000 && totalMilliseconds < 15000)
			{
				return "10~15秒";
			}
			if (totalMilliseconds >= 15000 && totalMilliseconds < 20000)
			{
				return "15~20秒";
			}
			if (totalMilliseconds >= 20000 && totalMilliseconds < 25000)
			{
				return "20~25秒";
			}
			if (totalMilliseconds >= 25000 && totalMilliseconds < 30000)
			{
				return "20~30秒";
			}
			if (totalMilliseconds >= 30000 && totalMilliseconds < 40000)
			{
				return "30~40秒";
			}
			if (totalMilliseconds >= 40000 && totalMilliseconds < 50000)
			{
				return "40~50秒";
			}
			if (totalMilliseconds >= 50000 && totalMilliseconds < 60000)
			{
				return "50~60秒";
			}
			if (totalMilliseconds >= 60000 && totalMilliseconds < 120000)
			{
				return "60~120秒";
			}
			if (totalMilliseconds >= 120000 && totalMilliseconds < 180000)
			{
				return "120~180秒";
			}
			return "180秒以上";
		}
	}
}
