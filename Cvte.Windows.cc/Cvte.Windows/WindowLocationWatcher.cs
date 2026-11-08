using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Cvte.Windows
{
	internal class WindowLocationWatcher
	{
		private List<WeakReference> _handlers;

		private Window _window;

		public event EventHandler WindowBoundsChanged
		{
			add
			{
				while (true)
				{
					if (_handlers == null)
					{
						_handlers = new List<WeakReference>();
					}
					_handlers.Add(new WeakReference(value));
					if (3 != 0)
					{
						value(_window, EventArgs.Empty);
						if (7 != 0)
						{
							break;
						}
					}
				}
			}
			remove
			{
				WeakReference weakReference2 = default(WeakReference);
				while (true)
				{
					if (0 == 0)
					{
						if (false)
						{
							break;
						}
						EventHandler value2 = value;
						if (_handlers == null)
						{
							return;
						}
						WeakReference weakReference = _handlers.FirstOrDefault((WeakReference x) => object.ReferenceEquals(x.Target, value2));
						if (0 == 0)
						{
							weakReference2 = weakReference;
						}
						if (5 == 0)
						{
							continue;
						}
						if (weakReference2 == null)
						{
							break;
						}
					}
					if (7 != 0)
					{
						_handlers.Remove(weakReference2);
						break;
					}
				}
				if (1 == 0)
				{
				}
			}
		}

		internal WindowLocationWatcher(Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window");
			}
			_window = window;
			_window.ContentRendered += window_ContentRendered;
			_window.StateChanged += window_StateChanged;
			_window.LocationChanged += window_LocationChanged;
			_window.Closed += window_Closed;
		}

		private void window_Closed(object sender, EventArgs e)
		{
			while (true)
			{
				_window.ContentRendered -= window_ContentRendered;
				while (true)
				{
					if (0 == 0)
					{
						_window.StateChanged -= window_StateChanged;
						if (1 == 0)
						{
							continue;
						}
						_window.LocationChanged -= window_LocationChanged;
						goto IL_0051;
					}
					goto IL_0068;
					IL_0068:
					if (false)
					{
						break;
					}
					if (0 == 0)
					{
						if (1 == 0)
						{
							break;
						}
						return;
					}
					goto IL_0051;
					IL_0051:
					_window.Closed -= window_Closed;
					goto IL_0068;
				}
			}
		}

		private void window_LocationChanged(object sender, EventArgs e)
		{
			RaiseWindowBoundsChanged();
		}

		private void window_StateChanged(object sender, EventArgs e)
		{
			RaiseWindowBoundsChanged();
		}

		private void window_ContentRendered(object sender, EventArgs e)
		{
			RaiseWindowBoundsChanged();
		}

		private void RaiseWindowBoundsChanged()
		{
			if (_handlers == null || 4 == 0)
			{
				return;
			}
			foreach (WeakReference handler in _handlers)
			{
				(handler.Target as EventHandler)?.Invoke(_window, EventArgs.Empty);
			}
		}
	}
}
