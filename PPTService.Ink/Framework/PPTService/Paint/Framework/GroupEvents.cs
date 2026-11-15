using System.Windows;
using PPTService.Paint.Framework.Elements;

namespace PPTService.Paint.Framework
{
	public class GroupEvents
	{
		public static readonly RoutedEvent GroupingEvent = EventManager.RegisterRoutedEvent("ElementGrouping", RoutingStrategy.Bubble, typeof(ElementGroupChangedRoutedEventHandler), typeof(IGroupElement));

		public static readonly RoutedEvent GroupedEvent = EventManager.RegisterRoutedEvent("ElementGrouped", RoutingStrategy.Bubble, typeof(ElementGroupChangedRoutedEventHandler), typeof(IGroupElement));

		public static readonly RoutedEvent UngroupingEvent = EventManager.RegisterRoutedEvent("ElementUngrouping", RoutingStrategy.Bubble, typeof(ElementGroupChangedRoutedEventHandler), typeof(IGroupElement));

		public static readonly RoutedEvent UngroupedEvent = EventManager.RegisterRoutedEvent("ElementUngrouped", RoutingStrategy.Bubble, typeof(ElementGroupChangedRoutedEventHandler), typeof(IGroupElement));
	}
}
