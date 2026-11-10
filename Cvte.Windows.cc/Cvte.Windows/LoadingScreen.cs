using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Cvte.Windows.Threading;

namespace Cvte.Windows
{
	public class LoadingScreen<T> : IDisposable where T : Visual
	{
		private EventHandler _handler;

		private WindowLocationWatcher _watcher;

		private Window _loadingWindow;

		private Dispatcher _dispatcher;

		private Rect _windowBounds;

		private bool _disposed;

		private bool _initialized;

		private AutoResetEvent _eventObj;

		public Func<T> OnCreatingLoadingUI { get; set; }

		public static LoadingScreen<T> Current { get; set; }

		public LoadingScreen(Func<T> createLoadingUI)
		{
			Action action = null;
			OnCreatingLoadingUI = createLoadingUI;
			_eventObj = new AutoResetEvent(false);
			Dispatcher dispatcher = Application.Current.Dispatcher;
			action = () =>
			{
				Application.Current.Exit += (object sender, ExitEventArgs e) =>
				{
					Dispose();
				};
			};
			dispatcher.Do(action);
			Initialize();
		}

		public LoadingScreen()
			: this((Func<T>)Activator.CreateInstance<T>)
		{
		}

		public void Show(bool resetLoadingUI = true)
		{
			CheckInitialized();
			CheckDisposed();
			Invoke(() =>
			{
				if (resetLoadingUI)
				{
					ResetLoadingUI();
				}
				UpdateWindowBounds();
				_loadingWindow.Show();
			});
		}

		public void Show(Rect bounds, bool resetLoadingUI = true)
		{
			CheckInitialized();
			CheckDisposed();
			_windowBounds = bounds;
			Show(resetLoadingUI);
		}

		public void Hide()
		{
			CheckInitialized();
			CheckDisposed();
			Invoke(() =>
			{
				_loadingWindow.Hide();
			});
		}

		public void Dispose()
		{
			CheckInitialized();
			_disposed = true;
			Invoke(() =>
			{
				_loadingWindow.Close();
				_dispatcher.InvokeShutdown();
				_loadingWindow = null;
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
			CheckDisposed();
			Invoke(() =>
			{
				if (_loadingWindow.Content == null)
				{
					ResetLoadingUI();
				}
				operation(_loadingWindow.Content as T);
			});
		}

		public void SetLocation(Rect bounds)
		{
			CheckDisposed();
			if (_watcher != null)
			{
				_watcher.WindowBoundsChanged -= _handler;
			}
			_windowBounds = bounds;
			if (_initialized)
			{
				Invoke(UpdateWindowBounds);
			}
		}

		internal void SetLocation(Window windowAttatchedTo)
		{
			CheckDisposed();
			if (_watcher != null)
			{
				_watcher.WindowBoundsChanged -= _handler;
			}
			_watcher = new WindowLocationWatcher(windowAttatchedTo);
			_handler = (object sender, EventArgs e) =>
			{
				Point point = windowAttatchedTo.PointToScreen(default(Point));
				Rect windowBounds = new Rect(point.X, point.Y, windowAttatchedTo.ActualWidth, windowAttatchedTo.ActualHeight);
				_windowBounds = windowBounds;
				if (_initialized)
				{
					Invoke(UpdateWindowBounds);
				}
			};
			_watcher.WindowBoundsChanged += _handler;
		}

		private void UpdateWindowBounds()
		{
			if (!(_windowBounds == Rect.Empty))
			{
				_loadingWindow.Top = _windowBounds.Top;
				_loadingWindow.Left = _windowBounds.Left;
				_loadingWindow.Width = _windowBounds.Width;
				_loadingWindow.Height = _windowBounds.Height;
			}
		}

		private void Invoke(Action operation)
		{
			if (!_initialized)
			{
				throw new InvalidOperationException("LoadingScreen has not initialized yet...");
			}
			if (_dispatcher != null)
			{
				_dispatcher.Do(operation);
			}
		}

		private void Initialize()
		{
			Thread thread = new Thread(() =>
			{
				try
				{
					_loadingWindow = new Window
					{
						WindowStyle = WindowStyle.None,
						WindowState = WindowState.Normal,
						AllowsTransparency = true,
						Background = Brushes.Transparent,
						Topmost = true,
						ShowInTaskbar = false,
						Title = "Loading..."
					};
					_loadingWindow.Closing += (object sender, CancelEventArgs e) =>
					{
						e.Cancel = true;
					};
					_dispatcher = _loadingWindow.Dispatcher;
					UpdateWindowBounds();
					_loadingWindow.Show();
					_loadingWindow.Hide();
					_initialized = true;
					_eventObj.Set();
					_eventObj.Dispose();
					_eventObj = null;
					Dispatcher.PushFrame(new DispatcherFrame());
				}
				catch (Exception ex)
				{
					Console.WriteLine("LoadingScreen Error:" + ex);
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start();
		}

		private void ResetLoadingUI()
		{
			Func<T> onCreatingLoadingUI = OnCreatingLoadingUI;
			if (onCreatingLoadingUI != null)
			{
				_loadingWindow.Content = onCreatingLoadingUI();
				return;
			}
			_loadingWindow.Content = new Border
			{
				Background = new SolidColorBrush(Color.FromArgb(102, 0, 0, 0))
			};
		}

		[DebuggerStepThrough]
		private void CheckDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("Already disposed...");
			}
		}

		private void CheckInitialized()
		{
			if (!_initialized && _eventObj != null)
			{
				_eventObj.WaitOne();
			}
		}
	}
}
