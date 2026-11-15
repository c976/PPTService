using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace PPTService.Ink.Core.Windows.WindowResizeBorder
{
	internal class ExtendedResizeController
	{
		private readonly ExtendedResizeWindow _bottom;

		private readonly ExtendedResizeWindow _left;

		private readonly ExtendedResizeWindow _right;

		private readonly ExtendedResizeWindow _top;

		private readonly List<ExtendedResizeWindow> _shadows;

		private bool _hasInitialized;

		private bool _isClosed;

		private readonly Window _window;

		private readonly Stub _windowInitStub;

		internal static readonly DependencyProperty ShadowControllerProperty = DependencyProperty.RegisterAttached("ExtendedResizeController", typeof(ExtendedResizeController), typeof(ExtendedResizeController), new PropertyMetadata((object)null));

		public ExtendedResizeController(Window window)
		{
			_window = window;
			_window.Closed += Window_Closed;
			if (ExtendedResizeBorder.GetExtendedResizeBorder(_window) != null)
			{
				_left = new ExtendedResizeWindow(Direction.Left);
				_top = new ExtendedResizeWindow(Direction.Top);
				_right = new ExtendedResizeWindow(Direction.Right);
				_bottom = new ExtendedResizeWindow(Direction.Bottom);
				_shadows = new List<ExtendedResizeWindow> { _left, _top, _right, _bottom };
				_windowInitStub = window.OnInitialized(Initialize);
			}
		}

		public void Close()
		{
			if (_isClosed)
			{
				return;
			}
			try
			{
				if (_hasInitialized)
				{
					RemoveEventListeners();
				}
				_shadows.ForEach((ExtendedResizeWindow x) =>
				{
					x.Owner = null;
					x.Close();
				});
			}
			finally
			{
				_isClosed = true;
			}
		}

		private void WindowOnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}

		private void Window_LocationChanged(object sender, EventArgs e)
		{
			if (_window.WindowState == WindowState.Normal)
			{
				UpdateBounds();
			}
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateBounds();
		}

		private void ShadowBorder_RadiusShrinkChanged(object sender, RoutedEventArgs e)
		{
			_window.Dispatcher.InvokeAsync(UpdateBounds);
		}

		private async void Window_StateChanged(object sender, EventArgs e)
		{
			if (_window.WindowState == WindowState.Normal)
			{
				await Task.Delay(TimeSpan.FromMilliseconds(200.0));
				UpdateBounds();
				Show();
			}
			else
			{
				Hide();
			}
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			UpdateBounds();
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			_shadows.ForEach((ExtendedResizeWindow x) =>
			{
				x.Topmost = !x.Topmost;
				x.Topmost = !x.Topmost;
			});
			UpdateBounds();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Close();
		}

		private void Initialize()
		{
			try
			{
				AddEventListeners();
				_shadows.ForEach((ExtendedResizeWindow x) =>
				{
					Window window = _window;
					if (window != null && window.IsLoaded)
					{
						x.Owner = _window;
					}
				});
				if (_window.ResizeMode == ResizeMode.CanMinimize || _window.ResizeMode == ResizeMode.NoResize)
				{
					_shadows.ForEach((ExtendedResizeWindow x) =>
					{
						x.IsHitTestVisible = false;
					});
				}
				if (_window.IsVisible && _window.WindowState == WindowState.Normal)
				{
					UpdateBounds();
					Show();
				}
			}
			finally
			{
				if (_isClosed)
				{
					RemoveEventListeners();
				}
				else
				{
					_hasInitialized = true;
				}
			}
		}

		private void UpdateBounds()
		{
			if (_window.WindowState != WindowState.Normal)
			{
				return;
			}
			ExtendedResizeBorder shadowBorder = ExtendedResizeBorder.GetExtendedResizeBorder(_window);
			if (shadowBorder != null)
			{
				Rect bounds = new Rect(_window.Left, _window.Top, _window.ActualWidth, _window.ActualHeight);
				_shadows.ForEach((ExtendedResizeWindow x) =>
				{
					x.Render(bounds, shadowBorder);
				});
			}
		}

		private void AddEventListeners()
		{
			_window.LocationChanged += Window_LocationChanged;
			_window.SizeChanged += Window_SizeChanged;
			_window.Activated += Window_Activated;
			_window.Deactivated += Window_Deactivated;
			_window.StateChanged += Window_StateChanged;
			_window.IsVisibleChanged += WindowOnIsVisibleChanged;
			_window.AddHandler(ExtendedResizeBorder.RadiusChangedEvent, new RoutedEventHandler(ShadowBorder_RadiusShrinkChanged));
		}

		private void RemoveEventListeners()
		{
			_windowInitStub.IsEnabled = false;
			_window.Activated -= Window_Activated;
			_window.Deactivated -= Window_Deactivated;
			_window.StateChanged -= Window_StateChanged;
			_window.IsVisibleChanged -= WindowOnIsVisibleChanged;
			_window.Closed -= Window_Closed;
			_window.LocationChanged -= Window_LocationChanged;
			_window.SizeChanged -= Window_SizeChanged;
			_window.RemoveHandler(ExtendedResizeBorder.RadiusChangedEvent, new RoutedEventHandler(ShadowBorder_RadiusShrinkChanged));
		}

		private void Hide()
		{
			_shadows.ForEach((ExtendedResizeWindow x) =>
			{
				x.Hide();
				x.Owner = null;
			});
		}

		private void Show()
		{
			Window window = _window;
			if (window == null || !window.IsLoaded)
			{
				return;
			}
			try
			{
				if (_window.WindowState != WindowState.Normal)
				{
					return;
				}
				_shadows.ForEach((ExtendedResizeWindow x) =>
				{
					Window window2 = _window;
					if (window2 != null && window2.IsLoaded)
					{
						x.Show();
						x.Owner = _window;
					}
				});
			}
			catch
			{
			}
		}
	}
}
