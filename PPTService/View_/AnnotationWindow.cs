using System;
using System.Windows;
using Cvte.Paint.DesktopMode;

namespace PPTService
{
	/// <summary>
	/// 批注窗口。直接继承桌面批注（net45）原版白板窗口 DesktopBoardWindow，
	/// 原样保留其 InkBoard 白板、压感软笔（SoftPenInputFromBoardWritingMode）、
	/// 手势橡皮（EraserMode）、逐页独立笔迹（GoToSlide）与鼠标穿透（WS_EX_TRANSPARENT）。
	/// </summary>
	public class AnnotationWindow : DesktopBoardWindow
	{
		public AnnotationWindow()
		{
			ShowInTaskbar = false;
			// 初始为鼠标模式：窗口穿透，点击落到 PPT。
			GotoSelect();
			Activated += AnnotationWindow_Activated;
			Loaded += AnnotationWindow_Loaded;
		}

		private void AnnotationWindow_Loaded(object sender, RoutedEventArgs e)
		{
			AnnotationController.KeepBelowToolbar();
		}

		private void AnnotationWindow_Activated(object sender, EventArgs e)
		{
			// 原版窗口为 Topmost，激活后可能盖住小工具的工具栏 / 翻页按钮，
			// 这里始终把它压回主窗口之下（按钮仍可点击，空白区仍可落笔）。
			AnnotationController.KeepBelowToolbar();
		}
	}
}
