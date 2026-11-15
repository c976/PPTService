using System;
using System.Windows;

namespace PPTService.Paint.Framework
{
	public interface IModeInputSource
	{
		event ModeInputEventHandler InputDown;

		event ModeInputEventHandler InputMove;

		event ModeInputEventHandler InputHover;

		event ModeInputEventHandler InputUp;

		void Attach(UIElement eventSource, Func<UIElement> getContainer = null);

		void Detach();
	}
}
