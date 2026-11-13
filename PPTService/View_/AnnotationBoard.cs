using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Cvte.Paint.DesktopMode;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks;
using Cvte.Paint.Settings;

namespace PPTService
{
	/// <summary>
	/// 自实现批注的绘制层：直接复用桌面批注（net45）原版白板 Board 控件，
	/// 但不再包一层独立 Window，而是把 Board 作为 MainWindow 视觉树里最底层的一个子元素
	/// （工具栏按钮网格在其之上）。这样 WPF 单窗口命中测试自然让工具栏按钮永远可点、
	/// 空白处才落到 Board，彻底消除「批注层盖住按钮 / 画到工具栏就点不动」的问题，
	/// 也无需再用 SetWindowPos 跨窗口抢 z 序（顺带消除了此前触控画第一笔卡死的互锁）。
	///
	/// 鼠标（选择）模式：Board.IsHitTestVisible=false，点击穿透到下方 PPT（按钮仍在其上可点）；
	/// 笔 / 橡皮模式：Board.IsHitTestVisible=true，空白处的笔迹输入被 Board 吃下。
	/// </summary>
	internal sealed class AnnotationBoard
	{
		private readonly IDesktopBoardModesProvider _modesProvider = new DefaultDesktopBoardModesProvider();

		// 鼠标也能书写 / 擦除的笔模式（原版只认触屏 / 压感笔）。复用同一实例，
		// 与 DefaultDesktopBoardModesProvider.PenMode 的 Lazy 单例用法保持一致。
		private readonly Mode _penMode = new MouseEnabledPenWritingMode();

		public Board InkBoard { get; }

		public AnnotationBoard()
		{
			Board board = new Board
			{
				Background = Brushes.Transparent,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};
			board.Slides[0].Background = Brushes.Transparent;
			InkBoard = board;
			((Cvte.Paint.Framework.IPaintInitializer)new Cvte.Paint.Inks.Initializer()).Initialize(board);
			SetInkColor(Color.FromRgb(242, 24, 24));
			// 默认鼠标模式：Board 不命中，点击穿透到下方 PPT（工具栏按钮仍在其之上可点）。
			board.IsHitTestVisible = false;
			// 主题 BAML 未被嵌入（或资源名不匹配）时 Board 没有模板、输入层不存在，
			// 表现为「怎么都画不出笔迹」，这里用代码模板兜底。
			if (board.TryFindResource(typeof(Board)) == null)
			{
				InkBoardStyleFallback.Apply(board);
			}
		}

		public void GotoSelect()
		{
			// 鼠标（选择）模式：Board 不命中，点击穿透到下方 PPT（工具栏按钮仍在其上可点）。
			// 原版用内部 DesktopMode 切换模式 + 窗口级 WS_EX_TRANSPARENT 穿透；合并进单窗口后
			// 改为元素级 IsHitTestVisible=false，输入层不再接收指针事件，等效于原版穿透。
			InkBoard.IsHitTestVisible = false;
		}

		public void GotoInk()
		{
			InkBoard.Mode = _penMode;
			InkBoard.IsHitTestVisible = true;
		}

		public void GotoEraser()
		{
			InkBoard.Mode = _modesProvider.EraserMode;
			InkBoard.IsHitTestVisible = true;
		}

		public void CleanInk()
		{
			InkBoard.CurrentSlide.RemoveAll<Ink>();
		}

		public void Undo()
		{
			InkBoard.UndoRedoProvider.Undo();
		}

		public void Redo()
		{
			InkBoard.UndoRedoProvider.Redo();
		}

		public void SetInkColor(Color color)
		{
			InkBoard.GetSettings<InkBoardSettings>().InkColor = color;
		}

		public void SetInkThickness(double thickness)
		{
			InkBoard.GetSettings<InkBoardSettings>().InkThickness = thickness;
		}

		public void GoToSlide(int index)
		{
			int count = InkBoard.Slides.Count;
			if (count <= index)
			{
				InkBoard.Insert((from x in Enumerable.Repeat(0, index - count + 1)
					select new Slide
					{
						Background = Brushes.Transparent
					}).ToList());
			}
			InkBoard.GoTo(index);
		}

		public void ClearSlides()
		{
			InkBoard.Clear(new Slide
			{
				Background = Brushes.Transparent
			});
		}
	}
}
