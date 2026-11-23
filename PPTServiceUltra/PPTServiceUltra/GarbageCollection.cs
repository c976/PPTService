using System;

namespace PPTServiceUltra
{
	public static class GarbageCollection
	{
		public static void Collect()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
	}
}
