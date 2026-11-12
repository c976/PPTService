using System;
using System.Linq;
using System.Windows;
using Cvte.Paint.Framework.ElementLockedActions;

namespace Cvte.Paint.Framework
{
	public static class ElementActionExtensions
	{
		public static bool GetIsLocked(this Element element)
		{
			return element.GetActionExtension().IsLocked;
		}

		public static void SetIsLocked(this Element element, bool value)
		{
			element.GetActionExtension().IsLocked = value;
		}

		public static void AddElementIsLockedChangedEvent(this Element element, EventHandler<ElementIsLockedChangedRoutedEventArgs> eventHandler)
		{
			AddElementIsLockedChangedEvent((UIElement)element, eventHandler);
		}

		public static void AddElementIsLockedChangedEvent(UIElement element, EventHandler<ElementIsLockedChangedRoutedEventArgs> eventHandler)
		{
			element.AddHandler(ElementActionExtension.ElementIsLockedChangedEvent, eventHandler);
		}

		public static ElementActionExtension GetActionExtension(this Element element)
		{
			ElementActionExtension elementActionExtension = element.Extensions.OfType<ElementActionExtension>().FirstOrDefault();
			if (elementActionExtension == null)
			{
				elementActionExtension = new ElementActionExtension();
				element.Extensions.Add(elementActionExtension);
			}
			return elementActionExtension;
		}
	}
}
