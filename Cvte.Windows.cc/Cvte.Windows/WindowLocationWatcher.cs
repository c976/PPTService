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
				if (_handlers == null)
				{
					_handlers = new List<WeakReference>();
				}
				_handlers.Add(new WeakReference(value));
				value(_window, EventArgs.Empty);
			}
			remove
			{
				if (_handlers != null)
				{
					WeakReference weakReference = _handlers.FirstOrDefault((WeakReference x) => object.ReferenceEquals(x.Target, value));
					if (weakReference != null)
					{
						_handlers.Remove(weakReference);
					}
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
			_window.ContentRendered -= window_ContentRendered;
			_window.StateChanged -= window_StateChanged;
			_window.LocationChanged -= window_LocationChanged;
			_window.Closed -= window_Closed;
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
			if (_handlers == null)
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
