using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Editing
{
	public sealed class ImeSupport
	{
		private IntPtr _defaultImeWnd;

		private IntPtr _currentContext;

		private IntPtr _previousContext;

		private HwndSource _hwndSource;

		private readonly TextArea _textArea;

		private static bool _isSoftwarePinYinOverWin7;

		private bool _isUpdatingCompositionWindow;

		public ImeSupport(TextArea textArea)
		{
			if (textArea == null)
			{
				throw new ArgumentNullException("textArea");
			}
			_textArea = textArea;
			_textArea.GotKeyboardFocus += (object s, KeyboardFocusChangedEventArgs e) =>
			{
				if (_textArea.IsKeyboardFocused)
				{
					if (_hwndSource == null)
					{
						_isUpdatingCompositionWindow = true;
						ClearContext();
						CreateContext();
						UpdateCompositionWindow();
						_isUpdatingCompositionWindow = false;
					}
				}
				else
				{
					ClearContext();
				}
			};
			_textArea.LostKeyboardFocus += (object s, KeyboardFocusChangedEventArgs e) =>
			{
				if (!_isUpdatingCompositionWindow)
				{
					if (object.Equals(e.OldFocus, _textArea) && _currentContext != IntPtr.Zero)
					{
						ImeNative.ImmNotifyIME(_currentContext, 21, 4);
					}
					ClearContext();
				}
			};
			InputMethod.SetIsInputMethodSuspended(_textArea, true);
		}

		public void UpdateCompositionWindow()
		{
			if (!(_currentContext == IntPtr.Zero))
			{
				_isSoftwarePinYinOverWin7 = IsMsInputMethonOverWin7();
				if (_hwndSource != null)
				{
					SetCompositionFont(_hwndSource, _currentContext, _textArea);
					SetCompositionWindow(_hwndSource, _currentContext, _textArea);
				}
			}
		}

		private static bool IsMsInputMethonOverWin7()
		{
			string currentInputMethod = ImeHelper.GetCurrentInputMethod();
			if (string.IsNullOrEmpty(currentInputMethod) || (!currentInputMethod.Equals("微软拼音") && !currentInputMethod.Equals("微软五笔")))
			{
				return false;
			}
			Version version = Environment.OSVersion.Version;
			Version value = new Version("6.2");
			if (version.CompareTo(value) < 0)
			{
				return false;
			}
			return true;
		}

		private static void SetCompositionWindow(HwndSource source, IntPtr hIMC, TextArea textArea)
		{
			if (textArea == null)
			{
				throw new ArgumentNullException("textArea");
			}
			Rect bounds = ImeHelper.GetBounds(textArea, source);
			Rect bounds2 = textArea.DocumentManager.CaretInfo.Bounds;
			bounds2 = textArea.TransformToAncestor(source.RootVisual).TransformBounds(bounds2);
			Point point = new Point(bounds2.TopLeft.X / SystemParameters.CaretWidth, bounds2.TopLeft.Y / SystemParameters.CaretWidth);
			ImeNative.CompositionForm form = new ImeNative.CompositionForm
			{
				dwStyle = 2,
				ptCurrentPos = 
				{
					x = (int)Math.Max(point.X, bounds.Left),
					y = (int)Math.Max(point.Y, bounds.Top)
				}
			};
			if (_isSoftwarePinYinOverWin7)
			{
				form.ptCurrentPos.y += (int)bounds2.Height;
			}
			ImeNative.ImmSetCompositionWindow(hIMC, ref form);
		}

		private static void SetCompositionFont(HwndSource source, IntPtr hIMC, TextArea textArea)
		{
			if (textArea == null)
			{
				throw new ArgumentNullException("textArea");
			}
			ImeNative.LOGFONT font = default(ImeNative.LOGFONT);
			Rect bounds = textArea.DocumentManager.CaretInfo.Bounds;
			font.lfFaceName = textArea.DocumentManager.PropertyManager.CurrentRunProperty.FontFamily.Source;
			font.lfHeight = (int)bounds.Height;
			int dwIndex = 8;
			int num = ImeNative.ImmGetCompositionString(hIMC, dwIndex, null, 0);
			if (num > 0)
			{
				byte[] array = new byte[num];
				if (ImeNative.ImmGetCompositionString(hIMC, dwIndex, array, num) > 0 && string.IsNullOrWhiteSpace(Encoding.Default.GetString(array)))
				{
					font.lfWidth = 1;
				}
			}
			ImeNative.ImmSetCompositionFont(hIMC, ref font);
		}

		private void ClearContext()
		{
			if (_hwndSource != null)
			{
				ImeNative.ImmAssociateContext(_hwndSource.Handle, _previousContext);
				ImeNative.ImmReleaseContext(_defaultImeWnd, _currentContext);
				_currentContext = IntPtr.Zero;
				_defaultImeWnd = IntPtr.Zero;
				_hwndSource.RemoveHook(WndProc);
				_hwndSource = null;
			}
		}

		private void CreateContext()
		{
			_hwndSource = (HwndSource)PresentationSource.FromVisual(_textArea);
			if (_hwndSource != null)
			{
				_defaultImeWnd = ImeNative.ImmGetDefaultIMEWnd(IntPtr.Zero);
				_currentContext = ImeNative.ImmGetContext(_defaultImeWnd);
				_previousContext = ImeNative.ImmAssociateContext(_hwndSource.Handle, _currentContext);
				_hwndSource.AddHook(WndProc);
				ImeNative.GetTextFrameworkThreadManager()?.SetFocus(IntPtr.Zero);
			}
		}

		private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
			case 81:
				if (_hwndSource != null)
				{
					ClearContext();
					CreateContext();
				}
				break;
			case 271:
				UpdateCompositionWindow();
				break;
			}
			return IntPtr.Zero;
		}
	}
}
