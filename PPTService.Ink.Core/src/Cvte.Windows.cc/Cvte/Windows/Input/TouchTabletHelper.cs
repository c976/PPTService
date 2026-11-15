using System;
using System.Reflection;
using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	public static class TouchTabletHelper
	{
		private static bool HasRemovedDevices { get; set; }

		public static void DisableWPFTabletSupport()
		{
			if (HasRemovedDevices)
			{
				return;
			}
			object obj = typeof(InputManager).InvokeMember("StylusLogic", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null, InputManager.Current, null);
			if (obj == null)
			{
				return;
			}
			Type type = obj.GetType();
			FieldInfo field = type.GetField("_lastSeenDeviceCount", BindingFlags.Instance | BindingFlags.NonPublic);
			while (Tablet.TabletDevices.Count > 0)
			{
				if (field != null)
				{
					field.SetValue(obj, 1 + (int)field.GetValue(obj));
				}
				int num = Tablet.TabletDevices.Count - 1;
				type.InvokeMember("OnTabletRemoved", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, obj, new object[1] { (uint)num });
				HasRemovedDevices = true;
			}
		}
	}
}
