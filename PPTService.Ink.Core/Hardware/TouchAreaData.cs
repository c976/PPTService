using System.Windows;

namespace PPTService.Ink.Hardware.Core
{
	/// <summary>
	/// ITouchArea 的通用实现，供普通 WPF TouchPoint / RawStylusInput 路径构造触摸区域。
	/// 替代原 Hardware 程序集里的 TouchArea，使 Ink 本体不依赖希沃硬件程序集。
	/// </summary>
	public class TouchAreaData : ITouchArea
	{
		public Point Position { get; set; }
		public Size Size { get; set; }

		public override string ToString()
		{
			return $"TouchArea{{ Position={Position}, Size={Size}}}";
		}
	}
}
