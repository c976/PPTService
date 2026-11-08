using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Cvte.Windows.Threading;

namespace Cvte.Windows
{
	public class LoadingScreen<SA_T0> : IDisposable where SA_T0 : Visual
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4
		{
			public LoadingScreen<SA_T0> _003C_003E4__this;

			public bool resetLoadingUI;

			public void _003CShow_003Eb__3()
			{
				do
				{
					if (4 == 0)
					{
						return;
					}
					if (resetLoadingUI)
					{
						if (6 == 0)
						{
							return;
						}
						_003C_003E4__this.ResetLoadingUI();
					}
					if (0 == 0)
					{
						_003C_003E4__this.UpdateWindowBounds();
					}
				}
				while (6 == 0);
				_003C_003E4__this._loadingWindow.Show();
			}
		}

		private EventHandler _handler;

		private WindowLocationWatcher _watcher;

		private Window _loadingWindow;

		private Dispatcher _dispatcher;

		private Rect _windowBounds;

		private bool _disposed;

		private bool _initialized;

		private AutoResetEvent _eventObj;

		public Func<SA_T0> OnCreatingLoadingUI { get; set; }

		public static LoadingScreen<SA_T0> Current { get; set; }

		public LoadingScreen(Func<SA_T0> createLoadingUI)
		{
			OnCreatingLoadingUI = createLoadingUI;
			_eventObj = new AutoResetEvent(false);
			Dispatcher dispatcher = Application.Current.Dispatcher;
			Action action = () =>
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
			: this((Func<SA_T0>)Activator.CreateInstance<SA_T0>)
		{
		}

		public void Show(bool resetLoadingUI = true)
		{
			_003C_003Ec__DisplayClass4 obj2 = default(_003C_003Ec__DisplayClass4);
			while (true)
			{
				_003C_003Ec__DisplayClass4 obj = new _003C_003Ec__DisplayClass4();
				if (0 == 0)
				{
					obj2 = obj;
				}
				while (0 == 0)
				{
					obj2.resetLoadingUI = resetLoadingUI;
					if (4 != 0)
					{
						obj2._003C_003E4__this = this;
						CheckInitialized();
						CheckDisposed();
						Invoke(obj2._003CShow_003Eb__3);
						return;
					}
				}
			}
		}

		public void Show(Rect bounds, bool resetLoadingUI = true)
		{
			if (true)
			{
				if (0 == 0)
				{
					CheckInitialized();
				}
				goto IL_002e;
			}
			goto IL_0038;
			IL_002e:
			CheckDisposed();
			goto IL_0038;
			IL_0038:
			while (true)
			{
				_windowBounds = bounds;
				Show(resetLoadingUI);
				if (false)
				{
					break;
				}
				if (false)
				{
					continue;
				}
				return;
			}
			goto IL_002e;
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
			while (true)
			{
				if (0 == 0)
				{
					CheckInitialized();
				}
				while (true)
				{
					_disposed = true;
					if (false)
					{
						break;
					}
					if (5 != 0)
					{
						Invoke(() =>
						{
							do
							{
								if (8 == 0 || 5 == 0)
								{
									return;
								}
								Window loadingWindow = _loadingWindow;
								if (3u != 0)
								{
									loadingWindow.Close();
								}
							}
							while (false);
							_dispatcher.InvokeShutdown();
							if (2 != 0)
							{
								_loadingWindow = null;
							}
							_dispatcher = null;
						});
					}
					if (false)
					{
						continue;
					}
					return;
				}
			}
		}

		public void UsingLoadingThread(Action<SA_T0> operation)
		{
			while (true)
			{
				if (operation == null)
				{
					goto IL_002e;
				}
				if (-1 == 0)
				{
					continue;
				}
				CheckInitialized();
				if (5 == 0)
				{
					continue;
				}
				CheckDisposed();
				if (7 == 0)
				{
					continue;
				}
				Invoke(() =>
				{
					if (_loadingWindow.Content == null)
					{
						ResetLoadingUI();
					}
					operation(_loadingWindow.Content as SA_T0);
				});
				goto IL_0061;
				IL_0061:
				if (true)
				{
					break;
				}
				goto IL_002e;
				IL_002e:
				if (0 == 0)
				{
					throw new ArgumentNullException("operation");
				}
				goto IL_0061;
			}
		}

		public void SetLocation(Rect bounds)
		{
			while (true)
			{
				CheckDisposed();
				while (true)
				{
					if (_watcher != null)
					{
						goto IL_0010;
					}
					goto IL_0023;
					IL_0023:
					if (8 != 0)
					{
						_windowBounds = bounds;
					}
					if (!_initialized)
					{
						return;
					}
					if (3 != 0)
					{
						if (2 == 0)
						{
							break;
						}
						if (6 == 0)
						{
							continue;
						}
						Invoke(UpdateWindowBounds);
						return;
					}
					goto IL_0010;
					IL_0010:
					if (false)
					{
						break;
					}
					_watcher.WindowBoundsChanged -= _handler;
					goto IL_0023;
				}
			}
		}

		internal void SetLocation(Window windowAttatchedTo)
		{
			if (true)
			{
				CheckDisposed();
			}
			if (_watcher != null)
			{
				_watcher.WindowBoundsChanged -= _handler;
			}
			if (2 != 0)
			{
				_watcher = new WindowLocationWatcher(windowAttatchedTo);
				_handler = (object sender, EventArgs e) =>
				{
					Point point = default(Point);
					if (0 == 0 && -1 != 0)
					{
						point = windowAttatchedTo.PointToScreen(default(Point));
					}
					Rect windowBounds = new Rect(point.X, point.Y, windowAttatchedTo.ActualWidth, windowAttatchedTo.ActualHeight);
					_windowBounds = windowBounds;
					while (_initialized)
					{
						if (4 != 0)
						{
							Invoke(UpdateWindowBounds);
							break;
						}
					}
				};
			}
			_watcher.WindowBoundsChanged += _handler;
		}

		private void UpdateWindowBounds()
		{
			if (_windowBounds == Rect.Empty && 7 != 0)
			{
				return;
			}
			if (0 == 0)
			{
				_loadingWindow.Top = _windowBounds.Top;
				_loadingWindow.Left = _windowBounds.Left;
			}
			do
			{
				if (0 == 0)
				{
					_loadingWindow.Width = _windowBounds.Width;
				}
				_loadingWindow.Height = _windowBounds.Height;
			}
			while (1 == 0);
		}

		private void Invoke(Action operation)
		{
			do
			{
				if (4 == 0)
				{
					return;
				}
				if (!_initialized)
				{
					if (6 != 0)
					{
						throw new InvalidOperationException("LoadingScreen has not initialized yet...");
					}
					return;
				}
				if (0 == 0 && _dispatcher == null)
				{
					return;
				}
			}
			while (6 == 0);
			_dispatcher.Do(operation);
		}

		private void Initialize()
		{
			Thread thread2 = default(Thread);
			do
			{
				if (4 == 0)
				{
					return;
				}
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
						do
						{
							UpdateWindowBounds();
							_loadingWindow.Show();
							_loadingWindow.Hide();
							_initialized = true;
							_eventObj.Set();
							_eventObj.Dispose();
						}
						while (1 == 0);
						_eventObj = null;
						Dispatcher.PushFrame(new DispatcherFrame());
					}
					catch (Exception ex)
					{
						if (0 == 0)
						{
							Console.WriteLine("LoadingScreen Error:" + ex);
						}
					}
				});
				if (0 == 0)
				{
					thread2 = thread;
				}
				do
				{
					thread2.SetApartmentState(ApartmentState.STA);
				}
				while (false);
				thread2.IsBackground = true;
			}
			while (false);
			thread2.Start();
		}

		private void ResetLoadingUI()
		{
			Func<SA_T0> onCreatingLoadingUI = OnCreatingLoadingUI;
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
			bool flag;
			if (true)
			{
				if (0 == 0)
				{
					flag = _initialized;
					goto IL_000d;
				}
				goto IL_002c;
			}
			goto IL_002f;
			IL_002c:
			if (_eventObj != null)
			{
				goto IL_002f;
			}
			return;
			IL_002f:
			flag = _eventObj.WaitOne();
			if (0 == 0 && 8 != 0)
			{
				return;
			}
			goto IL_000d;
			IL_000d:
			if (!flag)
			{
				goto IL_002c;
			}
		}
	}
}
