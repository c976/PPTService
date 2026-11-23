using System.Diagnostics;

namespace PPTServiceUltra.Core.ComponentModel
{
	/// <summary>
	/// Reconstructed shim. PPTServiceUltra.Core.Net.dll contains a TypeRef to
	/// [PPTServiceUltra.Core.Core.cc]PPTServiceUltra.Core.ComponentModel.DebuggingProperties, but the shipped
	/// PPTServiceUltra.Core.Core.cc.dll no longer defines the type. The original member only
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
