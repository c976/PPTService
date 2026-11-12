using System;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public abstract class ModeInputSource : IModeInputSource
	{
		private Func<UIElement> _getContainer;

		protected UIElement EventSource { get; private set; }

		protected UIElement Container
		{
			get
			{
				if (_getContainer != null)
				{
					return _getContainer();
				}
				return EventSource;
			}
		}

		private event ModeInputEventHandler InputDown;

		private event ModeInputEventHandler InputMove;

		private event ModeInputEventHandler InputHover;

		private event ModeInputEventHandler InputUp;

		event ModeInputEventHandler IModeInputSource.InputDown
		{
			add
			{
				InputDown += value;
			}
			remove
			{
				InputDown -= value;
			}
		}

		event ModeInputEventHandler IModeInputSource.InputMove
		{
			add
			{
				InputMove += value;
			}
			remove
			{
				InputMove -= value;
			}
		}

		event ModeInputEventHandler IModeInputSource.InputHover
		{
			add
			{
				InputHover += value;
			}
			remove
			{
				InputHover -= value;
			}
		}

		event ModeInputEventHandler IModeInputSource.InputUp
		{
			add
			{
				InputUp += value;
			}
			remove
			{
				InputUp -= value;
			}
		}

		public void Attach(UIElement eventSource, Func<UIElement> getContainer = null)
		{
			_getContainer = getContainer;
			OnAttached(eventSource);
		}

		public void Detach()
		{
			OnDetaching();
			EventSource = null;
		}

		protected virtual void OnAttached(UIElement eventSource)
		{
		}

		protected virtual void OnDetaching()
		{
		}

		protected void RaiseInputDown(ModeInputEventArgs e)
		{
			InputDown?.Invoke(this, e);
		}

		protected void RaiseInputMove(ModeInputEventArgs e)
		{
			InputMove?.Invoke(this, e);
		}

		protected void RaiseInputHover(ModeInputEventArgs e)
		{
			InputHover?.Invoke(this, e);
		}

		protected void RaiseInputUp(ModeInputEventArgs e)
		{
			InputUp?.Invoke(this, e);
		}
	}
}
