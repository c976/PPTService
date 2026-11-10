using System.Diagnostics;

namespace Cvte.ComponentModel
{
	/// <summary>
	/// Reconstructed shim. Cvte.Net.dll contains a TypeRef to
	/// [Cvte.Core.cc]Cvte.ComponentModel.DebuggingProperties, but the shipped
	/// Cvte.Core.cc.dll no longer defines the type. The original member only
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
