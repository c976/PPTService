using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Diagnostics
{
	
	
	[Obsolete("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
	public static class InstanceWatcher
	{
		[Obsolete("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
		public static void Watch(object instance)
		{
			throw new PlatformNotSupportedException("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。");
		}

		[Obsolete("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
		public static void Watch(object instance, string tag)
		{
			throw new PlatformNotSupportedException("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。");
		}

		[Obsolete("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
		public static int CountSurvivedInstances()
		{
			throw new PlatformNotSupportedException("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。");
		}

		[Obsolete("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
		public static int CountSurvivedInstances(string key)
		{
			throw new PlatformNotSupportedException("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。");
		}

		[Obsolete("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
		private static void WathcerInternal(string tag, object obj)
		{
			throw new PlatformNotSupportedException("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。");
		}

		[Obsolete("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。", true)]
		public static void Reset()
		{
			throw new PlatformNotSupportedException("InstanceWatcher 功能无法在跨平台的核心库实现，请使用 CoreLib.Windows 库中的同名类型替代。");
		}
	}
}
