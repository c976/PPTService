using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.ComponentModel;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public class HardPenBoardWritingMode : WritingPenMode, IPenMode, IDisposable
	{
		private DependencyPropertyDescriptor _boardContentScaleProperty;

		private DependencyPropertyDescriptor _boardContentOffsetProperty;

		private InkToStaticInk _theLastInk;

		private int _dryingCount;

		private bool _startDry;

		private Board CurrentBoard { get; set; }

		protected InkCanvas InkCanvas { get; set; }

		protected Matrix TransformFromBoard { get; set; } = Matrix.Identity;

		private InkBoardSettings InkBoardSettings { get; set; }

		protected override void OnSwitchOn()
		{
			Board board = (CurrentBoard = GetBoard());
			Board board3 = board;
			InkBoardSettings = board3.GetSettings<InkBoardSettings>();
			if (InkCanvas == null)
			{
				InkCanvas = new InkCanvas(false);
				AddStylusPlugInSource(board3, InkCanvas);
				InkCanvas.DynamicRenderer = GetDynamicRenderer(InkCanvas);
				board3.AddBoardUtilitiesElement(InkCanvas);
				InkCanvas.Init();
				InkCanvas.StrokesCollected += InkCanvas_StrokesCollected;
			}
			else if (!board3.ContainsBoardUtilitiesElement(InkCanvas))
			{
				board3.AddBoardUtilitiesElement(InkCanvas);
				InkCanvas.Start();
			}
			InkCanvas.SetValue(Slide.SlideProperty, CurrentSlide);
			InkBoardSettings.PropertyChanged += InkBoardSettings_OnPropertyChanged;
			UpdateInkBoardSettings();
			UpdateBoardMatrix();
			_boardContentScaleProperty = DependencyPropertyDescriptor.FromProperty(Board.ContentScaleProperty, typeof(Board));
			_boardContentScaleProperty.AddValueChanged(board3, UpdateBoardContentLocation);
			_boardContentOffsetProperty = DependencyPropertyDescriptor.FromProperty(Board.ContentOffsetProperty, typeof(Board));
			_boardContentOffsetProperty.AddValueChanged(board3, UpdateBoardContentLocation);
			base.OnSwitchOn();
		}

		protected override void OnSwitchOff()
		{
			InkCanvas.Stop();
			Board currentBoard = CurrentBoard;
			currentBoard.RemoveBoardUtilitiesElement(InkCanvas);
			InkBoardSettings.PropertyChanged -= InkBoardSettings_OnPropertyChanged;
			_boardContentScaleProperty.RemoveValueChanged(currentBoard, UpdateBoardContentLocation);
			_boardContentOffsetProperty.RemoveValueChanged(currentBoard, UpdateBoardContentLocation);
			_theLastInk?.Rest();
			base.OnSwitchOff();
		}

		protected virtual DynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return new HardPenBoardDynamicRenderer(inkCanvas);
		}

		private void UpdateBoardMatrix()
		{
			double contentScale = Board.GetContentScale(CurrentSlide);
			Vector contentOffset = Board.GetContentOffset(CurrentSlide);
			Matrix transformFromBoard = default(Matrix);
			transformFromBoard.Translate(0.0 - contentOffset.X, 0.0 - contentOffset.Y);
			transformFromBoard.Scale(1.0 / contentScale, 1.0 / contentScale);
			TransformFromBoard = transformFromBoard;
			((HardPenBoardDynamicRenderer)InkCanvas.DynamicRenderer).TransformFromBoard = transformFromBoard;
		}

		private void UpdateBoardContentLocation(object sender, EventArgs e)
		{
			UpdateBoardMatrix();
		}

		private void InkBoardSettings_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateInkBoardSettings();
		}

		private void AddStylusPlugInSource(Board board, InkCanvas inkCanvas)
		{
			IStylusPlugInSource stylusPlugInSource = Board.GetRenderSource(board) as IStylusPlugInSource;
			if (stylusPlugInSource != null)
			{
				inkCanvas.EventSource = stylusPlugInSource;
			}
		}

		protected virtual void InkCanvas_StrokesCollected(object sender, InkSynchronizer e)
		{
			_startDry = true;
			_dryingCount++;
			InkToStaticInk inkToStaticInk = null;
			foreach (InkSynchronizer.StrokeSynchronizer item in e.BeginDry())
			{
				Ink ink = AsInk(item);
				if (ink != null)
				{
					inkToStaticInk = new InkToStaticInk(ink, CurrentSlide);
					inkToStaticInk.AddToSlide();
				}
			}
			if (inkToStaticInk != null)
			{
				_theLastInk = inkToStaticInk;
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
			combinedStroke.Transform(TransformFromBoard, true);
			combinedStroke.GetGeometry(combinedStroke.DrawingAttributes);
			return new Ink(combinedStroke);
		}

		protected void UpdateInkBoardSettings()
		{
			InkCanvas.DynamicRenderer.UpdateInkBoardSettings(InkBoardSettings);
		}

		public void Dispose()
		{
			if (IsOn)
			{
				if (DebuggingProperties.IsDebug)
				{
					throw new InvalidOperationException("禁止在当前 " + GetType().FullName + " Mode 使用过程中，调用 Dispose 进行释放，请先在 Board 中将此 Mode 切走。仅调试下抛出，如有问题请找德熙");
				}
				Board board = GetBoard();
				if (board != null)
				{
					board.Mode = new DisposedMode();
				}
			}
			InkCanvas?.Dispose();
		}
	}
}
