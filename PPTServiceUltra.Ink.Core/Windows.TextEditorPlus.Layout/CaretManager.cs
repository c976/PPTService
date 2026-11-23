using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Layout
{
	public class CaretManager
	{
		private bool _isNeedShowing;

		private bool _isShowing;

		private readonly CaretLayer _caretLayer;

		private readonly DocumentManager _documentManager;

		private readonly RenderManager _renderManager;

		private DispatcherTimer _caretBlinkTimer;

		private bool _isUpdating;

		private readonly object _locker = new object();

		private Brush _caretBrush = Brushes.Black;

		private bool _isCaretKeyBoardFocusWithin;

		internal CaretType CaretType { get; private set; }

		internal bool IsCaretKeyBoardFocusWithin
		{
			get
			{
				return _isCaretKeyBoardFocusWithin;
			}
			set
			{
				_isCaretKeyBoardFocusWithin = value;
				if (!_isCaretKeyBoardFocusWithin)
				{
					_caretLayer.HideCaret();
				}
			}
		}

		internal CaretManager(DocumentManager documentManager, RenderManager renderManager, CaretLayer caretLayer)
		{
			_documentManager = documentManager;
			_renderManager = renderManager;
			_documentManager.ArrangingTypeChanged += DocumentManger_OnOnArrangingTypeChanged;
			_documentManager.CaretInfo.ArrangingTypeChanged += DocumentManger_OnCaretInfoOnArrangingTypeChanged;
			_documentManager.CaretInfo.OffsetChanged += DocumentManger_OnCaretInfoOnOffsetChanged;
			_documentManager.CaretInfo.BrushChanged += DocumentManger_OnCaretInfoOnBrushChanged;
			_documentManager.PropertyManager.CurrentRunPropertyChanged += DocumentManger_OnCurrentRunPropertyChanged;
			_documentManager.PropertyManager.CurrentParagraphPropertyChanged += DocumentManger_OnCurrentParagraphPropertyChanged;
			_documentManager.SelectionChanged += DocumentManager_OnSelectionChanged;
			_renderManager.RenderCompleted += RenderManager_OnRenderCompleted;
			_caretLayer = caretLayer;
		}

		private void DocumentManger_OnOnArrangingTypeChanged(object sender, ValueChangeEventArgs<ArrangingType> e)
		{
			_documentManager.CaretInfo.ArrangingType = e.NewValue;
		}

		private void DocumentManager_OnSelectionChanged(object sender, ValueChangeEventArgs<ISelection> e)
		{
			if (e.NewValue.IsEmpty && _documentManager.IsEditable)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}

		private void DocumentManger_OnCurrentParagraphPropertyChanged(object sender, ValueChangeEventArgs<ParagraphProperty> e)
		{
			Show();
		}

		private void DocumentManger_OnCurrentRunPropertyChanged(object sender, ValueChangeEventArgs<RunProperty> e)
		{
			Show();
		}

		private void DocumentManger_OnCaretInfoOnBrushChanged(object sender, ValueChangeEventArgs<Brush> e)
		{
			Show();
		}

		private void DocumentManger_OnCaretInfoOnOffsetChanged(object sender, ValueChangeEventArgs<CaretOffset> e)
		{
			Show();
		}

		private void DocumentManger_OnCaretInfoOnArrangingTypeChanged(object sender, ValueChangeEventArgs<ArrangingType> e)
		{
			Show();
		}

		private void RenderManager_OnRenderCompleted(object sender, List<ParagraphLayoutData> obj)
		{
			Show();
		}

		private void ShowInner(bool withNotify)
		{
			if (_isNeedShowing && _documentManager.CurrentFeature.HasFeature(Features.CaretInner))
			{
				UpdateCaret(withNotify);
				CaretInfo caretInfo = _documentManager.CaretInfo;
				if (caretInfo.Bounds.Equals(Rect.Empty) || (caretInfo.Brush.Equals(Brushes.Transparent) && !caretInfo.IsAutoBrush))
				{
					StopBlink();
				}
				else
				{
					_caretLayer.SetCaret(caretInfo);
					StartBlink();
				}
				_isNeedShowing = false;
			}
		}

		[DllImport("user32.dll")]
		private static extern int GetCaretBlinkTime();

		private void StartBlink()
		{
			_caretBlinkTimer?.Stop();
			if (IsCaretKeyBoardFocusWithin)
			{
				_caretLayer.ShowCaret();
			}
			_caretBlinkTimer = new DispatcherTimer();
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(GetCaretBlinkTime());
			_caretBlinkTimer.Interval = ((timeSpan.TotalMilliseconds > 0.0) ? timeSpan : TimeSpan.FromMilliseconds(553.0));
			_caretBlinkTimer.Tick += (object s, EventArgs e) =>
			{
				if (IsCaretKeyBoardFocusWithin)
				{
					_caretLayer.ReverseCaretVisibility();
				}
				else
				{
					_caretLayer.HideCaret();
				}
			};
			_caretBlinkTimer.Start();
		}

		private void StopBlink()
		{
			_caretBlinkTimer?.Stop();
			_caretLayer.HideCaret();
		}

		private void UpdateCaret(bool withNotify)
		{
			if (_isUpdating)
			{
				return;
			}
			lock (_locker)
			{
				if (_isUpdating)
				{
					return;
				}
				_isUpdating = true;
			}
			try
			{
				SetCaretScale(1.0 / TextHelperInner.GetScalingRatioToDevice(_caretLayer).Width);
				UpdateCaretBounds(withNotify);
				UpdateCaretBrush(withNotify);
			}
			finally
			{
				lock (_locker)
				{
					_isUpdating = false;
				}
			}
		}

		private void UpdateCaretBounds(bool withNotify)
		{
			CaretOffset currentCaretOffset = _documentManager.CaretInfo.CurrentCaretOffset;
			Rect runBoundsByCaretOffset = _renderManager.GetRunBoundsByCaretOffset(currentCaretOffset);
			FlowDirection runDirectionByCaretOffset = _renderManager.GetRunDirectionByCaretOffset(currentCaretOffset);
			ParagraphNode paragraphNodeByDocumentOffset = _documentManager.CharsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(currentCaretOffset.Offset + 1);
			double paragraphBefore = _documentManager.PropertyManager.GetParagraphProperty(paragraphNodeByDocumentOffset.ParagraphNumber - 1).ParagraphBefore;
			double x = ((runDirectionByCaretOffset == FlowDirection.LeftToRight) ? runBoundsByCaretOffset.Right : runBoundsByCaretOffset.Left);
			double y = ((runDirectionByCaretOffset == FlowDirection.LeftToRight) ? runBoundsByCaretOffset.Bottom : runBoundsByCaretOffset.Top);
			switch (_documentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				_documentManager.CaretInfo.UpdateCaretBounds(new Rect(x, runBoundsByCaretOffset.Y + ((paragraphNodeByDocumentOffset.ParagraphNumber == 1) ? 0.0 : paragraphBefore), _documentManager.CaretInfo.Weight, runBoundsByCaretOffset.Height), withNotify);
				break;
			case ArrangingType.Vertical:
				_documentManager.CaretInfo.UpdateCaretBounds(new Rect(runBoundsByCaretOffset.Left, y, runBoundsByCaretOffset.Width, _documentManager.CaretInfo.Weight), withNotify);
				break;
			case ArrangingType.Mongolian:
				_documentManager.CaretInfo.UpdateCaretBounds(new Rect(runBoundsByCaretOffset.Left, y, runBoundsByCaretOffset.Width, _documentManager.CaretInfo.Weight), withNotify);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void UpdateCaretBrush(bool withNotify)
		{
			CaretType caretType = CaretType;
			Brush brush;
			if (caretType != CaretType.Normal)
			{
				if (caretType != CaretType.DragOver)
				{
					goto IL_004d;
				}
				brush = InnerTextContext.CaretDragOverBrush;
			}
			else
			{
				if (!_documentManager.Selection.IsEmpty)
				{
					goto IL_004d;
				}
				brush = (_documentManager.CaretInfo.IsAutoBrush ? GetCurrentCaretBrush() : _caretBrush);
			}
			goto IL_0053;
			IL_004d:
			brush = Brushes.Transparent;
			goto IL_0053;
			IL_0053:
			_documentManager.CaretInfo.UpdateCaretBrush(brush, withNotify);
		}

		private Brush GetCurrentCaretBrush()
		{
			RunProperty currentRunProperty = _documentManager.PropertyManager.CurrentRunProperty;
			if (currentRunProperty.Hyperlink != null)
			{
				if (!currentRunProperty.IsHyperlinkHit)
				{
					return InnerTextContext.HyperlinkBrush;
				}
				return InnerTextContext.HyperlinkHitBrush;
			}
			Brush brush = currentRunProperty.EffectsArgs.OfType<FillEffectArgs>().FirstOrDefault()?.FillBrush;
			if (brush != null)
			{
				return brush;
			}
			return currentRunProperty.Foreground;
		}

		public void Hide()
		{
			_isShowing = false;
			_isNeedShowing = false;
			StopBlink();
			TextTracer.Debug("隐藏光标", "Cursor");
		}

		public void Show()
		{
			Show(true);
		}

		private void Show(bool withNotify)
		{
			if (_documentManager.IsEditable)
			{
				_isShowing = true;
				_isNeedShowing = true;
				if (!_documentManager.IsDocumentBuilding && _renderManager.Paragraphs.Any())
				{
					ShowInner(withNotify);
					TextTracer.Debug("显示光标", "Cursor");
				}
			}
		}

		public async void InvalidCaretScale()
		{
			await _caretLayer.Dispatcher.InvokeAsync((Func<Task>)(async () =>
			{
				double oldScale = _documentManager.CaretInfo.CaretScale;
				for (int i = 0; i < 5; i++)
				{
					await Dispatcher.Yield();
					double d = 1.0 / TextHelperInner.GetScalingRatioToDevice(_caretLayer).Width;
					if (!Nearly.Equals(oldScale, d, 0.001))
					{
						break;
					}
				}
				UpdateCaret(false);
			}));
		}

		public bool GetIsShowing()
		{
			return _isShowing;
		}

		[Obsolete("禁止外部设置光标宽度", true)]
		public void SetCaretWeight(double weight)
		{
		}

		internal void SetCaretScale(double scale)
		{
			_documentManager.CaretInfo.CaretScale = scale;
		}

		public void SetCaretBrush(Brush caretBrush)
		{
			_caretBrush = caretBrush;
		}

		public void SetCaretType(CaretType caretType)
		{
			CaretType = caretType;
		}
	}
}
