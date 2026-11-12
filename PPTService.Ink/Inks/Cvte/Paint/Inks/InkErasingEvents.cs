using System;
using System.Windows;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	public static class InkErasingEvents
	{
		public static readonly RoutedEvent InkErasingCompleteEvent = EventManager.RegisterRoutedEvent("InkErasingComplete", RoutingStrategy.Bubble, typeof(EventHandler<InkErasingCompleteRoutedEventArgs>), typeof(Board));

		public static readonly RoutedEvent InkErasingCanceledEvent = EventManager.RegisterRoutedEvent("InkErasingCanceled", RoutingStrategy.Bubble, typeof(EventHandler<InkErasingCanceledRoutedEventArgs>), typeof(Board));

		public static readonly RoutedEvent InkErasingStartedEvent = EventManager.RegisterRoutedEvent("InkErasingStarted", RoutingStrategy.Bubble, typeof(EventHandler<InkErasingStartedRoutedEventArgs>), typeof(Board));

		public static void AddInkErasingStartedHandler(UIElement element, EventHandler<InkErasingStartedRoutedEventArgs> handler)
		{
			element.AddHandler(InkErasingStartedEvent, handler);
		}

		public static void AddInkErasingCompletedHandler(UIElement element, EventHandler<InkErasingCompleteRoutedEventArgs> handler)
		{
			element.AddHandler(InkErasingCompleteEvent, handler);
		}

		public static void AddInkErasingCanceledHandler(UIElement element, EventHandler<InkErasingCanceledRoutedEventArgs> handler)
		{
			element.AddHandler(InkErasingCanceledEvent, handler);
		}
	}
}
