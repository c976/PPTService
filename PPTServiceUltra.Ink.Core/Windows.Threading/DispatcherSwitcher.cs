using System;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;

namespace PPTServiceUltra.Ink.Core.Windows.Threading
{
	internal static class DispatcherSwitcher
	{
		private static readonly Lazy<FieldInfo> DispatcherFieldInfo = new Lazy<FieldInfo>(() => typeof(DispatcherObject).GetField("_dispatcher", BindingFlags.Instance | BindingFlags.NonPublic), LazyThreadSafetyMode.None);

		internal static void SwitchTo(DispatcherObject d, Dispatcher dispatcher)
		{
			if (d.Dispatcher != dispatcher)
			{
				FieldInfo value = DispatcherFieldInfo.Value;
				if (value == null)
				{
					throw new FieldAccessException();
				}
				value.SetValue(d, dispatcher);
			}
		}
	}
}
