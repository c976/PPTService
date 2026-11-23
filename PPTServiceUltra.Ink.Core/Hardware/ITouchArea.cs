using System.Windows;

namespace PPTServiceUltra.Ink.Hardware.Core
{
	/// <summary>
	/// 触摸区域的通用抽象（Position/Size）。
	/// 手势橡皮等通用逻辑只依赖本接口，不再直接引用希沃硬件程序集。
	/// </summary>
	public interface ITouchArea
	{
		Point Position { get; }
		Size Size { get; }
	}
}
