using System.Collections.Generic;
using System.Diagnostics;

namespace Cvte.Diagnostics
{
	public static class MemoryState
	{
		private static readonly string CurrentProcessName = Process.GetCurrentProcess().ProcessName;

		private static readonly Dictionary<string, PerformanceCounter> PerformanceCounters = new Dictionary<string, PerformanceCounter>();

		public static string GetMemoryStateDescription(string processName = null)
		{
			return $"Working Set:{GetWorkingSetInMb(processName):f1}MB, Private Bytes:{GetPrivateBytesInMb(processName):f1}MB, Virtual Bytes:{GetVirtualBytesInMb(processName):f1}MB, Managed Heap Size:{GetManagedHeapSizeInMb(processName):f1}MB";
		}

		public static double GetWorkingSetInMb(string processName = null)
		{
			return GetPerformanceValue("Process", "Working Set", processName ?? CurrentProcessName) / 1024.0 / 1024.0;
		}

		public static double GetPrivateBytesInMb(string processName = null)
		{
			return GetPerformanceValue("Process", "Private Bytes", processName ?? CurrentProcessName) / 1024.0 / 1024.0;
		}

		public static double GetVirtualBytesInMb(string processName = null)
		{
			return GetPerformanceValue("Process", "Virtual Bytes", processName ?? CurrentProcessName) / 1024.0 / 1024.0;
		}

		public static double GetManagedHeapSizeInMb(string processName = null)
		{
			return GetPerformanceValue(".NET CLR Memory", "Gen 0 heap size", processName ?? CurrentProcessName) / 1024.0 / 1024.0 + GetPerformanceValue(".NET CLR Memory", "# Bytes in all Heaps", processName ?? CurrentProcessName) / 1024.0 / 1024.0;
		}

		private static double GetPerformanceValue(string categoryName, string counterName, string instanceName)
		{
			if (PerformanceCounterCategory.Exists(categoryName) && PerformanceCounterCategory.CounterExists(counterName, categoryName) && PerformanceCounterCategory.InstanceExists(instanceName, categoryName))
			{
				string key = categoryName + counterName + instanceName;
				PerformanceCounter value;
				if (!PerformanceCounters.TryGetValue(key, out value))
				{
					value = new PerformanceCounter(categoryName, counterName, instanceName);
					value.NextSample();
					PerformanceCounters.Add(key, value);
				}
				return value.NextValue();
			}
			return double.NaN;
		}
	}
}
