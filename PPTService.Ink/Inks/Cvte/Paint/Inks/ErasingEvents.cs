using System;
using System.Windows;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	public static class ErasingEvents
	{
		public static readonly RoutedEvent ErasingStartingEvent = EventManager.RegisterRoutedEvent("ErasingStarting", RoutingStrategy.Bubble, typeof(EventHandler<ErasingStartingRoutedEventArgs>), typeof(Board));

		public static readonly RoutedEvent ErasingStartedEvent = EventManager.RegisterRoutedEvent("ErasingStarted", RoutingStrategy.Bubble, typeof(EventHandler<ErasingStartedRoutedEventArgs>), typeof(Board));

		public static readonly RoutedEvent ErasingCompletedEvent = EventManager.RegisterRoutedEvent("ErasingCompleted", RoutingStrategy.Bubble, typeof(EventHandler<ErasingCompletedRoutedEventArgs>), typeof(Board));

		public static readonly RoutedEvent ErasingCanceledEvent = EventManager.RegisterRoutedEvent("ErasingCanceled", RoutingStrategy.Bubble, typeof(EventHandler<ErasingCanceledRoutedEventArgs>), typeof(Board));

		public static void AddErasingStartingHandler(UIElement element, EventHandler<ErasingStartingRoutedEventArgs> handler)
		{
			element.AddHandler(ErasingStartingEvent, handler);
		}

		public static void AddErasingStartedHandler(UIElement element, EventHandler<ErasingStartedRoutedEventArgs> handler)
		{
			element.AddHandler(ErasingStartedEvent, handler);
		}

		public static void AddErasingCompletedHandler(UIElement element, EventHandler<ErasingCompletedRoutedEventArgs> handler)
		{
			element.AddHandler(ErasingCompletedEvent, handler);
		}

		public static void AddErasingCanceledHandler(UIElement element, EventHandler<ErasingCanceledRoutedEventArgs> handler)
		{
			element.AddHandler(ErasingCanceledEvent, handler);
		}
	}
}
