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

		// 输入捕获说明（关键）：
		// MainWindow 是 AllowsTransparency 的分层窗口（Style.Window：Background="#00FFFFFF"，alpha=0），
		// 分层窗口的 alpha=0 像素默认会「穿透」到下方 PPT —— 这正是合并后「笔 / 橡皮无效」的根因：
		// Board 哪怕 IsHitTestVisible=true，其空白区（alpha=0）仍把所有输入漏给 PPT。
		// 原独立 AnnotationWindow 用「窗口级 WS_EX_TRANSPARENT 切换」来捕获；合并进单窗口后，
		// 笔 / 橡皮模式下把 Board 背景设为「alpha=1 的近透明黑」（CaptureBackground）：
		// 对分层窗口而言 alpha>0 即「实心像素」，会捕获输入交给 Board，
		// 视觉上近乎全透明（PPT 仍可见，仅极淡色偏），触控 / 压感笔 / 橡皮 / 鼠标均可落到笔迹管线；
		// 选择（鼠标）模式用回 alpha=0 背景，恢复穿透到 PPT，工具栏按钮（alpha>0）仍可点。
		//
		// 注：曾用 MainWindow 的 WM_NCHITTEST 钩子（笔 / 橡皮模式返回 HTCLIENT）尝试捕获，
		// 但触控 / 压感笔在部分设备上层窗口的命中测试会绕过该消息而失效，故回退到 alpha 方案。
		private static readonly Brush CaptureBackground = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));

		/// <summary>
		/// 笔 / 橡皮模式是否用 alpha=1 背景来捕获输入。
		/// MainWindow 是全透明覆盖在 PPT 之上的分层窗口，必须为 true；
		/// 板中板有白色不透明底板（InkCanvasBorder），像素 alpha 已为 255，
		/// 输入天然会被窗口捕获，传 false 可让白板保持纯白、零色偏。
		/// </summary>
		private readonly bool _captureInputWithAlpha;

		public Board InkBoard { get; }

		public AnnotationBoard(bool captureInputWithAlpha = true)
		{
			_captureInputWithAlpha = captureInputWithAlpha;
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
			// 分层窗口 alpha=0 像素默认穿透，等效于原版 WS_EX_TRANSPARENT 穿透；
			// 选择模式下不应在 Board 上捕获任何输入。
			InkBoard.IsHitTestVisible = false;
			InkBoard.Background = Brushes.Transparent;
		}

		private Brush WritingBackground
		{
			// 板中板场景：窗口已有白色不透明底板，Board 直接铺白，既保证 WPF 命中测试
			// 命中 Board 而非底板，也不引入任何半透明色偏（板中板本来就是白板）。
			get { return _captureInputWithAlpha ? CaptureBackground : Brushes.White; }
		}

		public void GotoInk()
		{
			InkBoard.Mode = _penMode;
			// 笔模式：MainWindow 场景用 alpha=1 背景让分层窗口在该区域捕获输入
			// （视觉仍近乎透明，PPT 可见）；板中板有白底，保持纯透明即可。
			// 触摸 / 压感笔 / 橡皮 / 鼠标均可落到 Board 的笔迹管线。
			InkBoard.IsHitTestVisible = true;
			InkBoard.Background = WritingBackground;
		}

		public void GotoEraser()
		{
			InkBoard.Mode = _modesProvider.EraserMode;
			// 橡皮模式：与笔模式同理，共享同一条 DynamicRenderer 管线。
			InkBoard.IsHitTestVisible = true;
			InkBoard.Background = WritingBackground;
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
