using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
	public class HardPenInputFromBoardWritingMode : WritingPenMode, IPenMode, IDisposable
	{
		private readonly List<int> _inputStylusIdList = new List<int>();

		private DependencyPropertyDescriptor _boardContentOffsetProperty;

		private DependencyPropertyDescriptor _boardContentScaleXProperty;

		private DependencyPropertyDescriptor _boardContentScaleYProperty;

		private Board _currentBoard;

		private int _dryingCount;

		private bool _isOn;

		private bool _startDry;

		private InkToStaticInk _theLastInk;

		protected InkCanvas InkCanvas { get; set; }

		protected Matrix TransformFromBoard { get; set; } = Matrix.Identity;

		private bool AllowsMouseInput => InkCanvas.DynamicRenderer.AllowsMouseInput;

		private Board CurrentBoard => _currentBoard ?? (_currentBoard = GetBoard());

		protected InkBoardSettings InkBoardSettings { get; set; }

		public HardPenInputFromBoardWritingMode()
		{
		}

		protected HardPenInputFromBoardWritingMode(ModeSettings modeSettings)
			: base(modeSettings)
		{
		}

		protected override void OnSwitchOn()
		{
			Board currentBoard = CurrentBoard;
			Slide currentSlide = CurrentSlide;
			InkBoardSettings = currentBoard.GetSettings<InkBoardSettings>();
			if (InkCanvas == null)
			{
				InkCanvas = new InkCanvas(false);
				AddStylusPlugInSource(currentBoard, InkCanvas);
				InkCanvas.DynamicRenderer = GetDynamicRenderer(InkCanvas);
				InkCanvas.DynamicRenderer.AllowsMouseInput = false;
				currentSlide.AddUtilities(InkCanvas);
				InkCanvas.Init();
				InkCanvas.StrokesCollected += InkCanvas_StrokesCollected;
				InkCanvas.DynamicRendererChanged += InkCanvas_OnDynamicRendererChanged;
			}
			else if (!currentSlide.ContainsUtilities(InkCanvas))
			{
				Slide slide = Slide.GetSlide(InkCanvas);
				if (slide != null)
				{
					if (DebuggingProperties.IsDebug)
					{
						Board board = slide.Parent as Board;
						if (board != null && board != currentBoard)
						{
							throw new ArgumentException(string.Format("跨Board使用笔模式，一个笔模式实例只能在一个 Board 使用，当前的笔模式已经在 Board = {0} {1} 使用{2}，此时在 Board = {3} {4} 上使用了这个笔模式，如果你不知道当前发生了什么，请将德熙叫过去", board.Name, board.GetHashCode(), _isOn ? "，同时这个笔模式还在开启过程中" : "", currentBoard.Name, currentBoard.GetHashCode()));
						}
						throw new ArgumentException("在重新打开笔模式，发现切换模式" + (_isOn ? "不成对，上一次的笔模式没有关闭" : "成对，上一次的笔模式已经关闭但没有预期从页面移除"))
						{
							Data = 
							{
								{
									(object)"笔迹所在的页面",
									(object)slide.Id
								},
								{
									(object)"当前模式所在的页面",
									(object)currentSlide.Id
								}
							}
						};
					}
					slide.RemoveUtilities(InkCanvas);
				}
				currentSlide.AddUtilities(InkCanvas);
				InkCanvas.Start();
			}
			InkCanvas.SetValue(Slide.SlideProperty, CurrentSlide);
			InkBoardSettings.PropertyChanged += InkBoardSettings_OnPropertyChanged;
			UpdateInkBoardSettings();
			UpdateBoardMatrix();
			_boardContentScaleXProperty = DependencyPropertyDescriptor.FromProperty(Board.ContentScaleXProperty, typeof(Board));
			_boardContentScaleXProperty.AddValueChanged(currentBoard, UpdateBoardContentLocation);
			_boardContentScaleYProperty = DependencyPropertyDescriptor.FromProperty(Board.ContentScaleYProperty, typeof(Board));
			_boardContentScaleYProperty.AddValueChanged(currentBoard, UpdateBoardContentLocation);
			_boardContentOffsetProperty = DependencyPropertyDescriptor.FromProperty(Board.ContentOffsetProperty, typeof(Board));
			_boardContentOffsetProperty.AddValueChanged(currentBoard, UpdateBoardContentLocation);
			_isOn = true;
			base.OnSwitchOn();
		}

		protected override void OnSwitchOff()
		{
			InkCanvas.Stop();
			Board currentBoard = CurrentBoard;
			CurrentSlide.RemoveUtilities(InkCanvas);
			InkBoardSettings.PropertyChanged -= InkBoardSettings_OnPropertyChanged;
			_boardContentScaleXProperty.RemoveValueChanged(currentBoard, UpdateBoardContentLocation);
			_boardContentScaleYProperty.RemoveValueChanged(currentBoard, UpdateBoardContentLocation);
			_boardContentOffsetProperty.RemoveValueChanged(currentBoard, UpdateBoardContentLocation);
			_theLastInk?.Rest();
			_inputStylusIdList.Clear();
			InkCanvas.ClearValue(Slide.SlideProperty);
			_isOn = false;
			base.OnSwitchOff();
		}

		public void LostCapture()
		{
			InkCanvas.LostCapture();
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

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			int id = e.Id;
			if (IsMouse(id))
			{
				StylusPointCollection stylusPointCollection = CollectPoints(e.GetStylusPoints());
				InkCanvas.DynamicRenderer.Down(id, -1, stylusPointCollection);
			}
			_inputStylusIdList.Add(id);
			base.OnDeviceDown(e);
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			int id = e.Id;
			if (IsMouse(id))
			{
				StylusPointCollection stylusPointCollection = CollectPoints(e.GetStylusPoints());
				InkCanvas.DynamicRenderer.Move(id, -1, stylusPointCollection);
			}
			base.OnDeviceMove(e);
		}

		protected override void OnDeviceUp(ModeInputEventArgs e)
		{
			int id = e.Id;
			if (IsMouse(id))
			{
				StylusPointCollection stylusPointCollection = CollectPoints(e.GetStylusPoints());
				InkCanvas.DynamicRenderer.Up(id, -1, stylusPointCollection);
			}
			base.OnDeviceUp(e);
		}

		private void UpdateBoardMatrix()
		{
			Slide currentSlide = CurrentSlide;
			if (currentSlide != null)
			{
				double contentScaleX = Board.GetContentScaleX(currentSlide);
				double contentScaleY = Board.GetContentScaleY(currentSlide);
				Vector contentOffset = Board.GetContentOffset(currentSlide);
				Matrix transformFromBoard = default(Matrix);
				transformFromBoard.Translate(0.0 - contentOffset.X, 0.0 - contentOffset.Y);
				transformFromBoard.Scale(1.0 / contentScaleX, 1.0 / contentScaleY);
				TransformFromBoard = transformFromBoard;
				((HardPenInputFromBoardDynamicRenderer)InkCanvas.DynamicRenderer).TransformFromBoard = transformFromBoard;
			}
		}

		private static bool IsMouse(int id)
		{
			return id == -1;
		}

		private void UpdateBoardContentLocation(object sender, EventArgs e)
		{
			UpdateInkBoardSettings();
			UpdateBoardMatrix();
		}

		private void InkBoardSettings_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateInkBoardSettings();
		}

		private StylusPointCollection CollectPoints(IEnumerable<StylusPoint> stylusPointList)
		{
			StylusPointCollection stylusPointCollection = new StylusPointCollection(stylusPointList);
			Matrix transformFromBoard = TransformFromBoard;
			transformFromBoard.Invert();
			for (int i = 0; i < stylusPointCollection.Count; i++)
			{
				StylusPoint value = stylusPointCollection[i];
				Point point = value.ToPoint();
				point = transformFromBoard.Transform(point);
				value.X = point.X;
				value.Y = point.Y;
				stylusPointCollection[i] = value;
			}
			return stylusPointCollection;
		}

		private void AddStylusPlugInSource(Board board, InkCanvas inkCanvas)
		{
			IStylusPlugInSource stylusPlugInSource = Board.GetRenderSource(board) as IStylusPlugInSource;
			if (stylusPlugInSource != null)
			{
				inkCanvas.EventSource = stylusPlugInSource;
			}
			else if (DebuggingProperties.IsDebug)
			{
				throw new InvalidOperationException("Board 没有提供注入的输入，要求 Board 的输入层是 BoardEventSource 将样式的 PART_DecoratorSource 修改为 BoardEventSource 类");
			}
		}

		protected virtual HardPenInputFromBoardDynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return new HardPenInputFromBoardDynamicRenderer(inkCanvas);
		}

		private void InkCanvas_OnDynamicRendererChanged(object sender, DynamicRenderer e)
		{
			if (IsOn)
			{
				UpdateInkBoardSettings();
				UpdateBoardMatrix();
				e.AllowsMouseInput = false;
			}
		}

		protected virtual void InkCanvas_StrokesCollected(object sender, InkSynchronizer e)
		{
			if (!_isOn)
			{
				return;
			}
			_startDry = true;
			_dryingCount++;
			InkToStaticInk inkToStaticInk = null;
			foreach (InkSynchronizer.StrokeSynchronizer item in e.BeginDry())
			{
				bool flag = _inputStylusIdList.Contains(item.StylusDeviceId);
				StylusPointCollection stylusPointCollection = item.StylusPointCollection;
				bool flag2 = stylusPointCollection != null && stylusPointCollection.Count > 50;
				bool lostCapture = item.LostCapture;
				if (flag | flag2 | lostCapture)
				{
					Element element = StrokeSynchronizerToElement(item);
					if (element != null)
					{
						inkToStaticInk = new InkToStaticInk(element, CurrentSlide);
						inkToStaticInk.AddToSlide();
					}
					_inputStylusIdList.Remove(item.StylusDeviceId);
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
				EndDry(e);
			}
		}

		protected virtual void EndDry(InkSynchronizer synchronizer)
		{
			synchronizer.EndDry();
		}

		protected virtual Element StrokeSynchronizerToElement(InkSynchronizer.StrokeSynchronizer synchronizer)
		{
			return AsInk(synchronizer);
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
			combinedStroke.GetGeometry(combinedStroke.DrawingAttributes);
			return new Ink(combinedStroke);
		}

		protected virtual void UpdateInkBoardSettings()
		{
			Board currentBoard = CurrentBoard;
			InkBoardSettings.InkScale = 1.0 / currentBoard.ContentScale;
			InkCanvas.DynamicRenderer.UpdateInkBoardSettings(InkBoardSettings);
		}

		[Conditional("DEBUG")]
		private void Trace(string str)
		{
		}
	}
}
