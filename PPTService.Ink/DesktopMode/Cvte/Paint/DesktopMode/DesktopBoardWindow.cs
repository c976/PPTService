using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks;
using Cvte.Paint.Settings;
using Cvte.Windows.Shell;

namespace Cvte.Paint.DesktopMode
{
	public class DesktopBoardWindow : PerformanceDesktopTransparentWindow
	{
		public Mode _desktopMode = new DesktopMode();

		private readonly IDesktopBoardModesProvider _desktopBoardModesProvider;

		public bool IsClosing { get; private set; }

		public Board InkBoard { get; set; }

		public DesktopBoardWindow()
			: this(new DefaultDesktopBoardModesProvider())
		{
		}

		public DesktopBoardWindow(IDesktopBoardModesProvider desktopBoardModesProvider)
		{
			Loaded += DesktopBoardWindow_Loaded;
			Board board = new Board
			{
				Background = Brushes.Transparent
			};
			board.Slides[0].Background = Brushes.Transparent;
			InkBoard = board;
			Content = board;
			((IPaintInitializer)new Cvte.Paint.Inks.Initializer()).Initialize(board);
			SetInkColor(Color.FromRgb(242, 24, 24));
			_desktopBoardModesProvider = desktopBoardModesProvider;
			Topmost = true;
			ShowInTaskbar = false;
			new WindowInteropHelper(this).EnsureHandle();
			Closing += DesktopBoardWindow_Closing;
		}

		public void GotoSelect()
		{
			SetTransparentHitThrough();
			InkBoard.Mode = _desktopMode;
		}

		public void GotoInk()
		{
			InkBoard.Mode = _desktopBoardModesProvider.PenMode;
			SetTransparentNotHitThrough();
		}

		public void GotoEraser()
		{
			InkBoard.Mode = _desktopBoardModesProvider.EraserMode;
			SetTransparentNotHitThrough();
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

		public void SetInkColor(string color)
		{
			SetInkColor((Color)ColorConverter.ConvertFromString(color));
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

		public void UpdateWindowPosition(Point? point = null, Size? size = null)
		{
			FullScreenHelper.EndFullScreen(this);
			if (point.HasValue)
			{
				Left = point.Value.X;
				Top = point.Value.Y;
			}
			if (size.HasValue)
			{
				Width = size.Value.Width;
				Height = size.Value.Height;
			}
			FullScreenHelper.StartFullScreen(this);
		}

		private void DesktopBoardWindow_Closing(object sender, CancelEventArgs e)
		{
			IsClosing = true;
			_desktopBoardModesProvider.Dispose();
		}

		private void DesktopBoardWindow_Loaded(object sender, RoutedEventArgs e)
		{
			IsClosing = false;
			UpdateWindowPosition();
		}
	}
}
