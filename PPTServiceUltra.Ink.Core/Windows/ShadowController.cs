using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;
using PPTServiceUltra.Ink.Core.Windows.Threading;

namespace PPTServiceUltra.Ink.Core.Windows
{
	internal class ShadowController
	{
		private readonly Shadow _bottom;

		private readonly Shadow _left;

		private readonly Shadow _right;

		private readonly Shadow _top;

		private readonly Shadow _topLeft;

		private readonly Shadow _topRight;

		private readonly Shadow _bottomLeft;

		private readonly Shadow _bottomRight;

		private readonly List<Shadow> _shadows;

		private readonly Binding _activeBorderBinding;

		private readonly Binding _inactiveBorderBinding;

		private readonly Binding _radiusBinding;

		private readonly Binding _opacityBinding;

		private readonly Binding _shrinkBinding;

		private readonly Window _window;

		private readonly Stub _windowInitStub;

		private readonly DispatcherTask _fadeInTask;

		private bool _isFading;

		internal static readonly DependencyProperty ShadowControllerProperty = DependencyProperty.RegisterAttached("ShadowController", typeof(ShadowController), typeof(ShadowController), new PropertyMetadata((object)null));

		public ShadowController(Window window)
		{
			_window = window;
			ShadowBorder shadowBorder = ShadowBorder.GetShadowBorder(_window);
			if (shadowBorder == null)
			{
				return;
			}
			_left = new Shadow(Direction.Left, shadowBorder.CanResize);
			_top = new Shadow(Direction.Top, shadowBorder.CanResize);
			_right = new Shadow(Direction.Right, shadowBorder.CanResize);
			_bottom = new Shadow(Direction.Bottom, shadowBorder.CanResize);
			_topLeft = new Shadow(Direction.TopLeft, shadowBorder.CanResize);
			_topRight = new Shadow(Direction.TopRight, shadowBorder.CanResize);
			_bottomLeft = new Shadow(Direction.BottomLeft, shadowBorder.CanResize);
			_bottomRight = new Shadow(Direction.BottomRight, shadowBorder.CanResize);
			_shadows = new List<Shadow> { _left, _top, _right, _bottom, _topLeft, _topRight, _bottomLeft, _bottomRight };
			_windowInitStub = window.OnInitialized(Initialize);
			_fadeInTask = new DispatcherTask(() =>
			{
				_isFading = false;
				UpdateBounds();
				DoubleAnimation animation = new DoubleAnimation
				{
					From = 0.0,
					Duration = new Duration(TimeSpan.FromMilliseconds(3000.0)),
					To = 1.0
				};
				_shadows.ForEach((Shadow x) =>
				{
					x.BeginAnimation(UIElement.OpacityProperty, animation);
				});
			});
		}

		public void Close()
		{
			RemoveEventListeners();
			_shadows.ForEach((Shadow x) =>
			{
				x.Owner = null;
				x.Close();
			});
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
				FadeOut();
			}
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateBounds();
			FadeOut();
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
			_shadows.ForEach((Shadow x) =>
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
			AddEventListeners();
			_shadows.ForEach((Shadow x) =>
			{
				Window window = _window;
				if (window != null && window.IsLoaded)
				{
					x.Owner = _window;
				}
			});
			if (_window.ResizeMode == ResizeMode.CanMinimize || _window.ResizeMode == ResizeMode.NoResize)
			{
				_shadows.ForEach((Shadow x) =>
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

		private void UpdateBounds()
		{
			if (_window.WindowState != WindowState.Normal || _isFading)
			{
				return;
			}
			ShadowBorder shadowBorder = ShadowBorder.GetShadowBorder(_window);
			if (shadowBorder != null)
			{
				Rect bounds = new Rect(_window.Left, _window.Top, _window.ActualWidth, _window.ActualHeight);
				_shadows.ForEach((Shadow x) =>
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
			_window.Closed += Window_Closed;
			_window.AddHandler(ShadowBorder.RadiusChangedEvent, new RoutedEventHandler(ShadowBorder_RadiusShrinkChanged));
			_window.AddHandler(ShadowBorder.ShrinkChangedEvent, new RoutedEventHandler(ShadowBorder_RadiusShrinkChanged));
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
			_window.RemoveHandler(ShadowBorder.RadiusChangedEvent, new RoutedEventHandler(ShadowBorder_RadiusShrinkChanged));
			_window.RemoveHandler(ShadowBorder.ShrinkChangedEvent, new RoutedEventHandler(ShadowBorder_RadiusShrinkChanged));
		}

		private void Hide()
		{
			_shadows.ForEach((Shadow x) =>
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
				_shadows.ForEach((Shadow x) =>
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

		private void FadeOut()
		{
			_fadeInTask.Restart();
			if (!_isFading)
			{
				_isFading = true;
				DoubleAnimation animation = new DoubleAnimation
				{
					Duration = new Duration(TimeSpan.FromMilliseconds(0.0)),
					To = 0.0
				};
				_shadows.ForEach((Shadow x) =>
				{
					x.BeginAnimation(UIElement.OpacityProperty, animation);
				});
			}
		}
	}
}
