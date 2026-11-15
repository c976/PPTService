using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using PPTService.Ink.Core.Windows.Threading;

namespace PPTService.Ink.Core.Windows
{
	public class LoadingScreen<T> : IDisposable, ILoadingScreen where T : Visual
	{
		private Window _loadingWindow;

		private Dispatcher _dispatcher;

		private Rect _windowBounds;

		private bool _disposed;

		private bool _initialized;

		private AutoResetEvent _eventObj;

		public bool IsShown => _loadingWindow != null;

		public Func<T> OnCreatingLoadingUI { get; set; }

		public Action<string, T> OnUpdateLoadingMessage { get; set; }

		public LoadingScreenLocatingMode LocatingMode { get; set; }

		public AnimationTimeline FadeInAnimation { get; set; }

		public AnimationTimeline FadeOutAnimation { get; set; }

		public LoadingScreen(Func<T> createLoadingUI)
		{
			LocatingMode = LoadingScreenLocatingMode.CoversActiveWindow;
			OnCreatingLoadingUI = createLoadingUI;
			_eventObj = new AutoResetEvent(false);
			Initialize();
		}

		public LoadingScreen()
			: this((Func<T>)Activator.CreateInstance<T>)
		{
		}

		public void Show(string loadingMessage)
		{
			CheckInitialized();
			InvokeOnLoadingThread(() =>
			{
				if (IsShown)
				{
					UpdateWindowBounds();
					_loadingWindow.Content = CreateLoadingUIContent();
					UpdateLoadingMessage(loadingMessage);
				}
				else
				{
					ResetWindow();
					UpdateLoadingMessage(loadingMessage);
					UpdateWindowBounds();
					if (FadeInAnimation != null)
					{
						if (!FadeInAnimation.CheckAccess())
						{
							DispatcherSwitcher.SwitchTo(FadeInAnimation, _dispatcher);
						}
						_loadingWindow.BeginAnimation(UIElement.OpacityProperty, FadeInAnimation);
					}
					TryOperatingWindow(_loadingWindow.Show);
				}
			});
		}

		public void Hide()
		{
			CheckInitialized();
			InvokeOnLoadingThread(async () =>
			{
				if (IsShown)
				{
					Window window = _loadingWindow;
					_loadingWindow = null;
					if (FadeOutAnimation != null)
					{
						if (!FadeOutAnimation.CheckAccess())
						{
							DispatcherSwitcher.SwitchTo(FadeOutAnimation, _dispatcher);
						}
						window.BeginAnimation(UIElement.OpacityProperty, FadeOutAnimation);
						await Task.Delay(FadeOutAnimation.Duration.TimeSpan);
						window.Tag = true;
						TryOperatingWindow(window.Close);
					}
					else
					{
						window.Tag = true;
						TryOperatingWindow(window.Close);
					}
				}
			});
		}

		public void Dispose()
		{
			CheckInitialized();
			if (_disposed)
			{
				return;
			}
			_disposed = true;
			if (Application.Current != null)
			{
				Application.Current.Exit -= OnCurrentAppExit;
			}
			InvokeOnLoadingThread(() =>
			{
				if (IsShown)
				{
					_loadingWindow.Tag = true;
					TryOperatingWindow(_loadingWindow.Close);
				}
				_loadingWindow = null;
				_dispatcher.InvokeShutdown();
				_dispatcher = null;
			});
		}

		public void UsingLoadingThread(Action<T> operation)
		{
			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}
			CheckInitialized();
			if (!IsShown)
			{
				throw new NotSupportedException("Cannot invoke UsingLoadingThread method while LoadingScreen is not shown.");
			}
			InvokeOnLoadingThread(() =>
			{
				if (!IsShown)
				{
					throw new NotSupportedException("Cannot invoke UsingLoadingThread method while LoadingScreen is not shown.");
				}
				operation(_loadingWindow.Content as T);
			});
		}

		public void SetLocation(Rect bounds)
		{
			if (LocatingMode != LoadingScreenLocatingMode.Manual)
			{
				throw new NotSupportedException("SetLocationManually method can only be invoked while LocatingMode property being set to Manual");
			}
			_windowBounds = bounds;
			if (_initialized)
			{
				InvokeOnLoadingThread(UpdateWindowBounds);
			}
		}

		private void UpdateWindowBounds()
		{
			Rect boundsInternal = Rect.Empty;
			switch (LocatingMode)
			{
			case LoadingScreenLocatingMode.CoversActiveWindow:
				boundsInternal = WindowHelper.GetWindowBounds(WindowHelper.CurrentActiveWindowHandle);
				break;
			case LoadingScreenLocatingMode.CoversActiveWindowClientArea:
				boundsInternal = WindowHelper.GetWindowClientBounds(WindowHelper.CurrentActiveWindowHandle);
				break;
			case LoadingScreenLocatingMode.CoversScreen:
				boundsInternal = WindowMonitorHelper.GetMonitorDeviceFromWindow(Process.GetCurrentProcess().MainWindowHandle).PixelBounds;
				break;
			case LoadingScreenLocatingMode.Manual:
				boundsInternal = _windowBounds;
				break;
			}
			if (!_windowBounds.IsEmpty)
			{
				SetBoundsInternal(boundsInternal);
			}
		}

		private void SetBoundsInternal(Rect rect)
		{
			if (object.Equals(Dpi.System.FactorX, 0.0))
			{
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
				throw new InvalidOperationException("Dpi.System.FactorX 是 0");
			}
			if (double.IsInfinity(rect.Left))
			{
				if (_initialized)
				{
					if (Debugger.IsAttached)
					{
						Debugger.Break();
					}
					throw new InvalidOperationException("rect.Left 竟然是正无穷");
				}
				rect.X = -1.0;
				rect.Y = -1.0;
			}
			_loadingWindow.Left = rect.Left / Dpi.System.FactorX;
			_loadingWindow.Top = rect.Top / Dpi.System.FactorY;
			_loadingWindow.Width = rect.Width / Dpi.System.FactorX;
			_loadingWindow.Height = rect.Height / Dpi.System.FactorY;
		}

		private void UpdateLoadingMessage(string message)
		{
			if (OnUpdateLoadingMessage != null)
			{
				OnUpdateLoadingMessage(message, (T)_loadingWindow.Content);
			}
		}

		private void InvokeOnLoadingThread(Action operation)
		{
			if (!_initialized)
			{
				throw new InvalidOperationException("LoadingScreen has not initialized yet...");
			}
			_dispatcher?.Continue(operation);
		}

		private async void Initialize()
		{
			Thread thread = new Thread(() =>
			{
				try
				{
					_dispatcher = Dispatcher.CurrentDispatcher;
					_dispatcher.UnhandledException += Dispatcher_UnhandledException;
					_initialized = true;
					_eventObj.Set();
					_eventObj.Dispose();
					_eventObj = null;
					Dispatcher.PushFrame(new DispatcherFrame());
				}
				catch (Exception)
				{
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start();
			Application current = Application.Current;
			if (current != null)
			{
				await current.Dispatcher.Continue(() =>
				{
					Application.Current.Exit += OnCurrentAppExit;
				});
			}
		}

		private void ResetWindow()
		{
			_loadingWindow = new Window
			{
				WindowStyle = WindowStyle.None,
				WindowState = WindowState.Normal,
				AllowsTransparency = true,
				Background = Brushes.Transparent,
				Topmost = true,
				ShowInTaskbar = false,
				Title = "Loading...",
				ShowActivated = false
			};
			WindowHelper.SetWindowNoActivate(_loadingWindow);
			_loadingWindow.Closing += (object sender, CancelEventArgs e) =>
			{
				Window window = (Window)sender;
				if (!object.Equals(true, window.Tag))
				{
					e.Cancel = true;
				}
			};
			_loadingWindow.Content = CreateLoadingUIContent();
		}

		private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			e.Handled = true;
		}

		private void OnCurrentAppExit(object sender, ExitEventArgs e)
		{
			if (!_disposed)
			{
				Dispose();
			}
		}

		private T CreateLoadingUIContent()
		{
			Func<T> onCreatingLoadingUI = OnCreatingLoadingUI;
			if (onCreatingLoadingUI == null)
			{
				return null;
			}
			return onCreatingLoadingUI();
		}

		private void CheckInitialized()
		{
			if (!_initialized)
			{
				_eventObj.WaitOne();
			}
		}

		private void TryOperatingWindow(Action operation)
		{
			try
			{
				operation();
			}
			catch (InvalidOperationException)
			{
			}
		}
	}
}
