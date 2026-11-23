using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Diagnostics
{
	
	
	[Obsolete("MemoryState 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
	public static class MemoryState
	{
		
		[Obsolete("MemoryState 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
		public static string GetMemoryStateDescription( string processName = null)
		{
			throw new PlatformNotSupportedException("MemoryState 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。");
		}

		[Obsolete("MemoryState 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
		public static double GetWorkingSetInMb(string processName = null)
		{
			throw new PlatformNotSupportedException("MemoryState 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。");
		}

		[Obsolete("MemoryState 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
		public static double GetPrivateBytesInMb(string processName = null)
		{
			throw new PlatformNotSupportedException("MemoryState 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。");
		}

		[Obsolete("MemoryState 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
		public static double GetVirtualBytesInMb(string processName = null)
		{
			throw new PlatformNotSupportedException("MemoryState 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。");
		}

		[Obsolete("MemoryState 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
		public static double GetManagedHeapSizeInMb(string processName = null)
		{
			throw new PlatformNotSupportedException("MemoryState 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。");
		}
	}
}
