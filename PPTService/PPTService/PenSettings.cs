using System.Windows.Media;

namespace PPTService
{
	/// <summary>
	/// 笔设置常量，移植自桌面批注 DesktopAnnotation.PenSettings。
	/// 三档笔迹粗细与「滑动清屏」阈值保持一致，便于工具栏与白板引擎共用。
	/// </summary>
	public static class PenSettings
	{
		public static double PenLightWeight = 2.0;

		public static double PenWeight = 4.0;

		public static double PenBoldWeight = 8.0;

		/// <summary>橡皮滑杆滑到该比例以上才清屏，避免误触。</summary>
		public static double EraserSlideThreshold = 0.8;

		/// <summary>默认笔色（红）。</summary>
		public static readonly Color DefaultColor = Colors.Red;
	}
}
