using Cvte.Paint.Inks;

namespace PPTService
{
	/// <summary>
	/// 允许鼠标输入的笔模式。
	/// 原版 HardPenInputFromBoardWritingMode 在 OnSwitchOn / DynamicRendererChanged 里把
	/// DynamicRenderer.AllowsMouseInput 强制为 false（只认触屏 / 压感笔，忽略鼠标）。
	/// 自实现批注要在没有触屏的机器上（例如 /debug 用鼠标测试）也能书写，这里在切换开启后
	/// 把 AllowsMouseInput 改回 true。橡皮模式复用同一条 DynamicRenderer 管线，因此也会一并支持鼠标。
	///
	/// 仅 PPTService 批注使用本子类，不改动 PPTService.Ink 库源码，因此不影响 DesktopAnnotation 等其它使用者。
	///
	/// 触笔双写风险说明：开启后，WPF 会把「真实手写笔 / 触屏」以手写笔设备（StylusDeviceId != 0）上报，
	/// 把「鼠标」以鼠标设备（StylusDeviceId == 0）上报，二者互不相干，故正常硬件上不会重复成笔；
	/// 仅当某硬件把真实笔也经鼠标设备（id == 0）双重上报时才会出现双笔，Seewo 数位屏通常以真实笔设备上报，无此问题。
	/// 普通 PC 只有鼠标，亦无重复。
	/// </summary>
	internal class MouseEnabledPenWritingMode : SoftPenInputFromBoardWritingMode
	{
		protected override void OnSwitchOn()
		{
			base.OnSwitchOn();
			if (InkCanvas != null && InkCanvas.DynamicRenderer != null)
			{
				InkCanvas.DynamicRenderer.AllowsMouseInput = true;
			}
		}
	}
}
