using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks.HardPenSlideWritingMode_
{
	[Obsolete("这个类还没实现完成，暂时放在这里")]
	public class HardPenWritingMode2 : WritingPenMode, IPenMode, IDisposable
	{
		private DependencyPropertyDescriptor _boardContentScaleProperty;

		private DependencyPropertyDescriptor _boardContentOffsetProperty;

		private int _dryingCount;

		private bool _startDry;

		public bool IsInkInBoard { get; }

		protected Matrix TransformFromBoard { get; set; } = Matrix.Identity;

		protected InkCanvas InkCanvas { get; set; }

		private InkBoardSettings InkBoardSettings { get; set; }

		public HardPenWritingMode2(bool isInkInBoard)
		{
			IsInkInBoard = isInkInBoard;
		}

		protected override void OnSwitchOn()
		{
			Board board = GetBoard();
			InkBoardSettings = board.GetSettings<InkBoardSettings>();
			if (IsInkInBoard)
			{
				AddInkCanvasInBoard(board);
			}
			else
			{
				AddInkCanvasInSlide();
			}
			InkBoardSettings.PropertyChanged += InkBoardSettings_OnPropertyChanged;
			UpdateInkBoardSettings();
			base.OnSwitchOn();
		}

		protected override void OnSwitchOff()
		{
			InkCanvas.Stop();
			Board board = GetBoard();
			if (IsInkInBoard)
			{
				RemoveInkCanvasInBoard(board);
			}
			else
			{
				RemoveInkCanvasInSlide();
			}
			InkBoardSettings.PropertyChanged -= InkBoardSettings_OnPropertyChanged;
			base.OnSwitchOff();
		}

		private void AddInkCanvasInBoard(Board board)
		{
			if (InkCanvas == null)
			{
				InkCanvas = new InkCanvas(false);
				InkCanvas.DynamicRenderer = GetDynamicRenderer(InkCanvas);
				board.AddBoardUtilitiesElement(InkCanvas);
				InkCanvas.Init();
				InkCanvas.StrokesCollected += InkCanvas_StrokesCollected;
			}
			else if (!board.ContainsBoardUtilitiesElement(InkCanvas))
			{
				board.AddBoardUtilitiesElement(InkCanvas);
				InkCanvas.Start();
			}
			InkCanvas.SetValue(Slide.SlideProperty, CurrentSlide);
			UpdateBoardMatrix();
			_boardContentScaleProperty = DependencyPropertyDescriptor.FromProperty(Board.ContentScaleProperty, typeof(Board));
			_boardContentScaleProperty.AddValueChanged(board, UpdateBoardContentLocation);
			_boardContentOffsetProperty = DependencyPropertyDescriptor.FromProperty(Board.ContentOffsetProperty, typeof(Board));
			_boardContentOffsetProperty.AddValueChanged(board, UpdateBoardContentLocation);
		}

		private void RemoveInkCanvasInBoard(Board board)
		{
			board.RemoveBoardUtilitiesElement(InkCanvas);
			_boardContentScaleProperty.RemoveValueChanged(board, UpdateBoardContentLocation);
			_boardContentOffsetProperty.RemoveValueChanged(board, UpdateBoardContentLocation);
		}

		private void UpdateBoardMatrix()
		{
			double contentScale = Board.GetContentScale(CurrentSlide);
			Vector contentOffset = Board.GetContentOffset(CurrentSlide);
			Matrix transformFromBoard = default(Matrix);
			transformFromBoard.Translate(0.0 - contentOffset.X, 0.0 - contentOffset.Y);
			transformFromBoard.Scale(1.0 / contentScale, 1.0 / contentScale);
			TransformFromBoard = transformFromBoard;
			((HardPenDynamicRenderer)InkCanvas.DynamicRenderer).TransformFromBoard = transformFromBoard;
		}

		private void AddInkCanvasInSlide()
		{
			Slide currentSlide = CurrentSlide;
			if (InkCanvas == null)
			{
				Board board = GetBoard();
				InkCanvas = new InkCanvas(false);
				AddStylusPlugInSource(board, InkCanvas);
				InkCanvas.DynamicRenderer = GetDynamicRenderer(InkCanvas);
				currentSlide.AddUtilities(InkCanvas);
				InkCanvas.Init();
				InkCanvas.StrokesCollected += InkCanvas_StrokesCollected;
			}
			else if (!currentSlide.ContainsUtilities(InkCanvas))
			{
				currentSlide.AddUtilities(InkCanvas);
				InkCanvas.Start();
			}
		}

		private void RemoveInkCanvasInSlide()
		{
			CurrentSlide.RemoveUtilities(InkCanvas);
		}

		protected virtual DynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return new HardPenDynamicRenderer(inkCanvas, IsInkInBoard);
		}

		private void AddStylusPlugInSource(Board board, InkCanvas inkCanvas)
		{
			IStylusPlugInSource stylusPlugInSource = Board.GetRenderSource(board) as IStylusPlugInSource;
			if (stylusPlugInSource != null)
			{
				inkCanvas.EventSource = stylusPlugInSource;
			}
		}

		private void UpdateBoardContentLocation(object sender, EventArgs e)
		{
			UpdateBoardMatrix();
		}

		private void InkBoardSettings_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateInkBoardSettings();
		}

		protected virtual void InkCanvas_StrokesCollected(object sender, InkSynchronizer e)
		{
			_startDry = true;
			_dryingCount++;
			foreach (InkSynchronizer.StrokeSynchronizer item in e.BeginDry())
			{
				Ink ink = AsInk(item);
				if (ink != null)
				{
					CurrentSlide.Add(ink);
				}
			}
			_startDry = false;
			_dryingCount--;
			if (_dryingCount == 0 && !_startDry)
			{
				e.EndDry();
			}
		}

		protected virtual Ink AsInk(InkSynchronizer.StrokeSynchronizer synchronizer)
		{
			StylusPointCollection stylusPointCollection = synchronizer.StylusPointCollection;
			if (stylusPointCollection == null || !stylusPointCollection.Any())
			{
				return null;
			}
			DrawingAttributes drawingAttributes = synchronizer.DrawingAttributes.Clone();
			CombinedStroke combinedStroke = new CombinedStroke(stylusPointCollection, drawingAttributes);
			if (IsInkInBoard)
			{
				combinedStroke.Transform(TransformFromBoard, true);
			}
			combinedStroke.GetGeometry(combinedStroke.DrawingAttributes);
			return new Ink(combinedStroke);
		}

		protected void UpdateInkBoardSettings()
		{
			InkCanvas.DynamicRenderer.UpdateInkBoardSettings(InkBoardSettings);
		}

		public void Dispose()
		{
			InkCanvas?.Dispose();
		}
	}
}
