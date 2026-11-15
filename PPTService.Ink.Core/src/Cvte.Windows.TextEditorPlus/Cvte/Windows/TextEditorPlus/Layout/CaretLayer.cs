using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Core;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Layout
{
	internal class CaretLayer : Layer, ILazyInitialize
	{
		private CaretInfo _lastCaretInfo;

		private readonly CaretElement _caretElement;

		private readonly List<UIElement> _children = new List<UIElement>();

		protected override int VisualChildrenCount => _children.Count;

		internal CaretLayer()
			: base(KnownLayer.Caret)
		{
			_caretElement = new CaretElement();
			_children.Add(_caretElement);
		}

		public void InitializeUntilEditable()
		{
			AddVisualChild(_caretElement);
		}

		public void InitializeUntilNotEmpty()
		{
		}

		internal void SetCaret(CaretInfo caretInfo)
		{
			_lastCaretInfo = caretInfo;
			ArrangeCaret();
			_caretElement.Visibility = Visibility.Hidden;
			_caretElement.SetCaretInfo(caretInfo);
		}

		internal void ShowCaret()
		{
			_caretElement.Visibility = Visibility.Visible;
		}

		internal void HideCaret()
		{
			_caretElement.Visibility = Visibility.Collapsed;
		}

		internal void ReverseCaretVisibility()
		{
			switch (_caretElement.Visibility)
			{
			case Visibility.Collapsed:
				_caretElement.Visibility = Visibility.Visible;
				break;
			case Visibility.Visible:
				_caretElement.Visibility = Visibility.Collapsed;
				break;
			case Visibility.Hidden:
				_caretElement.Visibility = Visibility.Visible;
				break;
			}
		}

		private void ArrangeCaret()
		{
			if (!_caretElement.IsArrangeValid)
			{
				Dispatcher.InvokeAsync(() =>
				{
					_caretElement.Arrange(_lastCaretInfo.Bounds);
				});
			}
			else
			{
				_caretElement.Arrange(_lastCaretInfo.Bounds);
			}
		}

		protected override Visual GetVisualChild(int index)
		{
			return _children[index];
		}
	}
}
