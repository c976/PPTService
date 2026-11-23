using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Composition
{
	
	
	[Obsolete("基础库 CoreLib.Core 不具备实现 MefExportedTypeDetector 的能力，请寻找其他替代方案。", true)]
	public class MefExportedTypeDetector : ITypeDetector
	{
		event EventHandler<NewTypesDetectedEventArgs> ITypeDetector.NewTypesDetected
		{
			add
			{
				throw new PlatformNotSupportedException("基础库 CoreLib.Core 不具备实现 MefExportedTypeDetector 的能力，请寻找其他替代方案。");
			}
			remove
			{
				throw new PlatformNotSupportedException("基础库 CoreLib.Core 不具备实现 MefExportedTypeDetector 的能力，请寻找其他替代方案。");
			}
		}

		void ITypeDetector.Detect(Assembly assembly)
		{
			throw new PlatformNotSupportedException("基础库 CoreLib.Core 不具备实现 MefExportedTypeDetector 的能力，请寻找其他替代方案。");
		}

		ReadOnlyCollection<T> ITypeDetector.GetDetectedObjects< T>()
		{
			throw new PlatformNotSupportedException("基础库 CoreLib.Core 不具备实现 MefExportedTypeDetector 的能力，请寻找其他替代方案。");
		}
	}
}
