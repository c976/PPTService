using System;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	[PublicAPI]
	public static class PointEraseEventHelper
	{
		public static EventHandler<SelectEventArgs> PointEraseStartedEvent;

		public static EventHandler<SelectEventArgs> PointErasingEvent;

		public static EventHandler<SelectEventArgs> PointEraseCompletedEvent;

		public static EventHandler<SelectEventArgs> PointEraseCancelEvent;

		public static event EventHandler<OperationSynchronousEventArgs> OperationSynchronous;

		internal static void OnOperationSynchronous(object sender, OperationSynchronousEventArgs args)
		{
			OperationSynchronous?.Invoke(sender, args);
		}

		internal static void OnPointEraseStarted(object sender, SelectEventArgs e)
		{
			PointEraseStartedEvent?.Invoke(sender, e);
		}

		internal static void OnPointErasing(object sender, SelectEventArgs e)
		{
			PointErasingEvent?.Invoke(sender, e);
		}

		internal static void OnPointEraseCompleted(object sender, SelectEventArgs e)
		{
			PointEraseCompletedEvent?.Invoke(sender, e);
		}

		internal static void OnPointEraseCanceled(object sender, SelectEventArgs e)
		{
			PointEraseCancelEvent?.Invoke(sender, e);
		}
	}
}
