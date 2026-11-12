using System.Windows;
using System.Windows.Input;
using Cvte.Windows;

namespace Cvte.Paint.Framework
{
	public sealed class SlideFocusModeOperation : IModePrepareOperation, IModeOperation, IModeInputPrepareOperation
	{
		public Slide CurrentSlide { get; set; }

		public void OnSwitchOn()
		{
		}

		public void OnSwitchOff()
		{
		}

		public void OnInputStarting(ModeInputEventArgs e)
		{
			if (Keyboard.FocusedElement == null)
			{
				CurrentSlide.Focus();
				return;
			}
			Element element = (Keyboard.FocusedElement as Element) ?? ((DependencyObject)Keyboard.FocusedElement).VisualAncestor<Element>();
			if (element == null)
			{
				CurrentSlide.Focus();
			}
			else if (!object.Equals(element.TopGroupElement(), e.Element) || !e.Element.GetIsSelected())
			{
				CurrentSlide.Focus();
			}
		}

		void IModeInputPrepareOperation.OnInputCompleted(ModeInputEventArgs e)
		{
		}
	}
}
