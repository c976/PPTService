using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace PPTServiceUltra.Ink.Hardware.Core
{
	/// <summary>
	/// 硬件能力服务定位器：Ink 本体的智能笔 / CV 触控能力全部通过这里读取接口，
	/// 编译期不再依赖 PPTServiceUltra.Ink.Hardware。
	/// 实现由 Hardware 程序集在首次访问时通过反射注册（见 HardwareRegistration.RegisterAll）；
	/// 普通电脑没有该程序集或注册失败时，保留空实现，所有硬件状态为 false/null，
	/// 手势橡皮自动走 WPF 普通触控路径，批注功能不受影响。
	/// </summary>
	public static class HardwareServices
	{
		private static ISmartPenProvider _smartPen = NullSmartPenProvider.Instance;
		private static ICVTouchProvider _cvTouch = NullCVTouchProvider.Instance;
		private static bool _triedLoad;

		public static ISmartPenProvider SmartPen
		{
			get
			{
				EnsureLoaded();
				return _smartPen;
			}
		}

		public static ICVTouchProvider CVTouch
		{
			get
			{
				EnsureLoaded();
				return _cvTouch;
			}
		}

		/// <summary>供 Hardware 程序集注册其专有实现。</summary>
		public static void Register(ISmartPenProvider smartPen, ICVTouchProvider cvTouch)
		{
			if (smartPen != null)
			{
				_smartPen = smartPen;
			}
			if (cvTouch != null)
			{
				_cvTouch = cvTouch;
			}
		}

		private static void EnsureLoaded()
		{
			if (_triedLoad)
			{
				return;
			}
			_triedLoad = true;
			try
			{
				Type type = Type.GetType("PPTServiceUltra.Paint.Inks.Hardware.HardwareRegistration, PPTServiceUltra.Ink.Hardware");
				MethodInfo method = type?.GetMethod("RegisterAll", BindingFlags.Public | BindingFlags.Static);
				method?.Invoke(null, null);
			}
			catch (Exception)
			{
				// 普通电脑无希沃硬件程序集 / 加载失败：静默保留空实现。
			}
		}
	}

	/// <summary>智能笔空实现：普通电脑上所有状态为无。</summary>
	internal sealed class NullSmartPenProvider : ISmartPenProvider
	{
		public static readonly NullSmartPenProvider Instance = new NullSmartPenProvider();

		private NullSmartPenProvider()
		{
		}

		public double? Pressure => null;
		public bool IsSquareButtonPressed => false;
		public bool IsEmrEnabled { get; set; }
		public bool IsEmrPenConnected { get; set; }
		public bool IsEmrPenWritable => false;
		public Action OnEmrPenConnected => () => { };
		public void Enable(Func<HwndSource> hwndSourceCreator) { }
		public bool IsPen(InputEventArgs e) => false;
		public bool IsSelectedButtonPressed(StylusEventArgs e) => false;
	}

	/// <summary>CV 触控空实现：普通电脑上视为无希沃触控。</summary>
	internal sealed class NullCVTouchProvider : ICVTouchProvider
	{
		public static readonly NullCVTouchProvider Instance = new NullCVTouchProvider();

		private NullCVTouchProvider()
		{
		}

		public bool HasInitialized => false;
		public bool? IsCVTouchFrame => false;
		public ITouchArea GetTouchArea(Point screenPoint) => null;
	}
}
