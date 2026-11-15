using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PPTService.Ink.Core.Windows
{
	[Obsolete("这个类还没稳定，暂时不对外开放")]
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
					WeakReference weakReference = _handlers.FirstOrDefault((WeakReference x) => x.Target == value);
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
			_window.ContentRendered += Window_ContentRendered;
			_window.StateChanged += Window_StateChanged;
			_window.LocationChanged += Window_LocationChanged;
			_window.Closed += Window_Closed;
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			_window.ContentRendered -= Window_ContentRendered;
			_window.StateChanged -= Window_StateChanged;
			_window.LocationChanged -= Window_LocationChanged;
			_window.Closed -= Window_Closed;
		}

		private void Window_LocationChanged(object sender, EventArgs e)
		{
			RaiseWindowBoundsChanged();
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{
			RaiseWindowBoundsChanged();
		}

		private void Window_ContentRendered(object sender, EventArgs e)
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
