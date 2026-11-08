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
			double num = GetPerformanceValue("Process", "Working Set", processName ?? CurrentProcessName);
			if (0 == 0)
			{
				double num2 = 1024.0;
				do
				{
					num /= num2;
					num2 = 1024.0;
				}
				while (5 == 0 || 2 == 0);
				num /= num2;
			}
			return num;
		}

		public static double GetPrivateBytesInMb(string processName = null)
		{
			double num = GetPerformanceValue("Process", "Private Bytes", processName ?? CurrentProcessName);
			if (0 == 0)
			{
				double num2 = 1024.0;
				do
				{
					num /= num2;
					num2 = 1024.0;
				}
				while (5 == 0 || 2 == 0);
				num /= num2;
			}
			return num;
		}

		public static double GetVirtualBytesInMb(string processName = null)
		{
			double num = GetPerformanceValue("Process", "Virtual Bytes", processName ?? CurrentProcessName);
			if (0 == 0)
			{
				double num2 = 1024.0;
				do
				{
					num /= num2;
					num2 = 1024.0;
				}
				while (5 == 0 || 2 == 0);
				num /= num2;
			}
			return num;
		}

		public static double GetManagedHeapSizeInMb(string processName = null)
		{
			double num = GetPerformanceValue(".NET CLR Memory", "Gen 0 heap size", processName ?? CurrentProcessName);
			while (true)
			{
				if (-1 == 0)
				{
					goto IL_0030;
				}
				double num2 = 1024.0;
				double num3;
				if (0 == 0)
				{
					num /= num2;
					num3 = 1024.0;
					goto IL_002f;
				}
				goto IL_0046;
				IL_002f:
				num /= num3;
				goto IL_0030;
				IL_0046:
				num3 = num2 / 1024.0;
				if (4 != 0)
				{
					num3 /= 1024.0;
					if (4 != 0)
					{
						num += num3;
						if (0 == 0)
						{
							break;
						}
						continue;
					}
				}
				goto IL_002f;
				IL_0030:
				num2 = GetPerformanceValue(".NET CLR Memory", "# Bytes in all Heaps", processName ?? CurrentProcessName);
				goto IL_0046;
			}
			return num;
		}

		private static double GetPerformanceValue(string categoryName, string counterName, string instanceName)
		{
			if (PerformanceCounterCategory.Exists(categoryName) && 0 == 0)
			{
				bool flag = PerformanceCounterCategory.CounterExists(counterName, categoryName);
				while (flag)
				{
					flag = PerformanceCounterCategory.InstanceExists(instanceName, categoryName);
					if (false)
					{
						continue;
					}
					if (!flag)
					{
						break;
					}
					string key = categoryName + counterName + instanceName;
					while (true)
					{
						PerformanceCounter value;
						if (!PerformanceCounters.TryGetValue(key, out value))
						{
							value = new PerformanceCounter(categoryName, counterName, instanceName);
							if (3 == 0)
							{
								break;
							}
							value.NextSample();
							if (false ? true : false)
							{
								continue;
							}
							PerformanceCounters.Add(key, value);
						}
						return value.NextValue();
					}
					break;
				}
			}
			return double.NaN;
		}
	}
}
