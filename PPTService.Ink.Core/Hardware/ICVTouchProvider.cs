using System.Windows;

namespace PPTService.Ink.Hardware.Core
{
	/// <summary>
	/// 希沃 CV 原始触控（TouchSDKDll）的抽象。
	/// 实现位于 PPTService.Ink.Hardware；手势橡皮通过本接口取希沃增强触摸区域，
	/// 无硬件时返回 null，自动降级到 WPF 普通触控路径。
	/// </summary>
	public interface ICVTouchProvider
	{
		bool HasInitialized { get; }
		bool? IsCVTouchFrame { get; }
		ITouchArea GetTouchArea(Point screenPoint);
	}
}
