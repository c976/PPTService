using System.Windows.Media;

namespace PPTServiceUltra
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

		/// <summary>
		/// 默认笔色（与自实现批注参考色板首色 #FFF14527 对齐）。
		/// 此前这里是旧色板的 #FFFF1200，与 9 色板首色不一致，导致放映开始时
		/// 笔是旧红、且色盘没有一项被选中。
		/// </summary>
		public static readonly Color DefaultColor = Color.FromRgb(0xF1, 0x45, 0x27);

		/// <summary>Office 原生批注色盘首色（沿用其原始 8 色）。</summary>
		public static readonly Color OfficeDefaultColor = Color.FromRgb(255, 18, 0);

		/// <summary>按当前批注模式返回色盘首色，供色盘勾选态复位使用。</summary>
		public static Color GetDefaultColor(bool selfAnnotation)
		{
			return selfAnnotation ? DefaultColor : OfficeDefaultColor;
		}
	}
}
