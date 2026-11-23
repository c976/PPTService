using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.ComponentModel
{
	public static class DebuggingProperties
	{
		private static bool? _isDebug;

		public static bool IsDebug
		{
			get
			{
				if (!_isDebug.HasValue)
				{
					Assembly assembly = Assembly.GetEntryAssembly();
					if (assembly == null)
					{
						assembly = Assembly.GetCallingAssembly();
					}
					_isDebug = _003Cget_IsDebug_003Eg__GetDebuggableAttribute_007C1_0(assembly) || _003Cget_IsDebug_003Eg__GetDebuggableAttribute_007C1_0(new StackTrace().GetFrames().Last().GetMethod()
						.Module.Assembly);
				}
				return _isDebug.Value;
			}
		}

		public static void SetIsDebug(bool isDebug)
		{
			_isDebug = isDebug;
		}

		
		[CompilerGenerated]
		internal static bool _003Cget_IsDebug_003Eg__GetDebuggableAttribute_007C1_0(Assembly assembly)
		{
			return assembly.GetCustomAttribute<DebuggableAttribute>().DebuggingFlags.HasFlag(DebuggableAttribute.DebuggingModes.EnableEditAndContinue);
		}
	}
}
