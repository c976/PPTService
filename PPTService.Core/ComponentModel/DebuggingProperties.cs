using System.Diagnostics;

namespace PPTService.Core.ComponentModel
{
	/// <summary>
	/// Reconstructed shim. PPTService.Core.Net.dll contains a TypeRef to
	/// [PPTService.Core.Core.cc]PPTService.Core.ComponentModel.DebuggingProperties, but the shipped
	/// PPTService.Core.Core.cc.dll no longer defines the type. The original member only
	/// exposes an IsDebug flag, so it is reproduced here as source.
	/// </summary>
	public static class DebuggingProperties
	{
		public static bool IsDebug
		{
			get
			{
#if DEBUG
				return true;
#else
				return Debugger.IsAttached;
#endif
			}
		}
	}
}
