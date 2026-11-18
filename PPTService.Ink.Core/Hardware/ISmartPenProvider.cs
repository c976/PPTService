using System;
using System.Windows.Input;
using System.Windows.Interop;

namespace PPTService.Ink.Hardware.Core
{
	/// <summary>
	/// 希沃智能笔 / 电磁笔硬件状态的抽象。
	/// 实现位于 PPTService.Ink.Hardware（专有硬件程序集）；Ink 本体只依赖本接口。
	/// 普通电脑上由 HardwareServices 的空实现接管，所有状态为 false/null，不会误触发。
	/// </summary>
	public interface ISmartPenProvider
	{
		double? Pressure { get; }
		bool IsSquareButtonPressed { get; }
		bool IsEmrEnabled { get; set; }
		bool IsEmrPenConnected { get; set; }
		bool IsEmrPenWritable { get; }
		Action OnEmrPenConnected { get; }
		void Enable(Func<HwndSource> hwndSourceCreator);
		bool IsPen(InputEventArgs e);
		bool IsSelectedButtonPressed(StylusEventArgs e);
	}
}
