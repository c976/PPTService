using PPTService.Ink.Hardware.Core;
using PPTService.Paint.Inks.Hardware.SmartPen;
using SmartPenDevice = PPTService.Paint.Inks.Hardware.SmartPen.SmartPen;

namespace PPTService.Paint.Inks.Hardware
{
	/// <summary>
	/// 希沃硬件程序集的注册入口。Core 的 HardwareServices 在首次访问时通过反射调用
	/// RegisterAll()，把专有实现注给 Ink 本体。本类只在希沃硬件程序集内存在。
	/// </summary>
	public static class HardwareRegistration
	{
		public static void RegisterAll()
		{
			HardwareServices.Register(new SmartPenProvider(), new CVTouchProvider());
		}
	}

	/// <summary>把静态 SmartPen / EmrPenManager 适配成 ISmartPenProvider。</summary>
	internal sealed class SmartPenProvider : ISmartPenProvider
	{
		public double? Pressure { get { return SmartPenDevice.Pressure; } }
		public bool IsSquareButtonPressed { get { return SmartPenDevice.IsSquareButtonPressed; } }
		public bool IsEmrEnabled
		{
			get { return SmartPenDevice.IsEmrEnabled; }
			set { SmartPenDevice.IsEmrEnabled = value; }
		}
		public bool IsEmrPenConnected
		{
			get { return SmartPenDevice.IsEmrPenConnected; }
			set { SmartPenDevice.IsEmrPenConnected = value; }
		}
		public bool IsEmrPenWritable { get { return SmartPenDevice.IsEmrPenWritable; } }
		public System.Action OnEmrPenConnected { get { return SmartPenDevice.OnEmrPenConnected; } }
		public void Enable(System.Func<System.Windows.Interop.HwndSource> hwndSourceCreator)
		{
			SmartPenDevice.Enable(hwndSourceCreator);
		}
		public bool IsPen(System.Windows.Input.InputEventArgs e)
		{
			return EmrPenManager.IsPen(e);
		}
		public bool IsSelectedButtonPressed(System.Windows.Input.StylusEventArgs e)
		{
			return EmrPenManager.IsSelectedButtonPressed(e);
		}
	}

	/// <summary>把静态 CVTouchInfoProvider / RawTouch 适配成 ICVTouchProvider。</summary>
	internal sealed class CVTouchProvider : ICVTouchProvider
	{
		public bool HasInitialized { get { return RawTouchEngine.CVTouchInfoProvider.HasInitialized; } }
		public bool? IsCVTouchFrame { get { return RawTouchEngine.CVTouchInfoProvider.IsCVTouchFrame; } }
		public ITouchArea GetTouchArea(System.Windows.Point screenPoint)
		{
			RawTouchEngine.TouchArea area = RawTouchEngine.RawTouch.GetTouchArea(screenPoint);
			if (area == null)
			{
				return null;
			}
			return new TouchAreaData
			{
				Position = area.Position,
				Size = area.Size
			};
		}
	}
}
