using System;
using System.Reflection;

namespace PPTServiceUltra
{
	/// <summary>
	/// 希沃 MCU 硬件调用的保留门面：方法名与返回值语义保持原样，
	/// 真正的 USB/HID 实现已拆到可选插件 PPTServiceUltra.Hardware.dll 中（命名空间 PPTServiceUltra.Hardware）。
	/// 插件不存在、加载失败或调用异常时一律返回默认 false，等同于「不做实际操作」。
	/// </summary>
	public static class McuService
	{
		private const string McuTypeName = "PPTServiceUltra.Hardware.Mcu, PPTServiceUltra.Hardware";

		private static readonly object LoadLock = new object();

		private static bool _triedLoad;

		private static Type _mcuType;

		private static Type McuType
		{
			get
			{
				if (!_triedLoad)
				{
					lock (LoadLock)
					{
						if (!_triedLoad)
						{
							_triedLoad = true;
							try
							{
								// false = 程序集找不到时不要抛异常；缺插件属于正常降级分支。
								_mcuType = Type.GetType(McuTypeName, false);
							}
							catch
							{
								_mcuType = null;
							}
						}
					}
				}
				return _mcuType;
			}
		}

		private static bool Invoke(string methodName, bool defaultValue)
		{
			Type type = McuType;
			if (type == null)
			{
				return defaultValue;
			}
			try
			{
				MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
				if (method == null)
				{
					return defaultValue;
				}
				object result = method.Invoke(null, null);
				return result is bool ? (bool)result : defaultValue;
			}
			catch
			{
				return defaultValue;
			}
		}

		public static bool FindMcu()
		{
			return Invoke("FindMcu", false);
		}

		public static bool ForbiddenAndroidPen()
		{
			return Invoke("ForbiddenAndroidPen", false);
		}

		public static bool OpenAndroidPen()
		{
			return Invoke("OpenAndroidPen", false);
		}
	}
}
