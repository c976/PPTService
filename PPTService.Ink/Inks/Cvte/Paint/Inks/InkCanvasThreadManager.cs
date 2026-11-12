#define TRACE
using System;
using System.Diagnostics;
using System.Windows.Threading;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Inks
{
	public class InkCanvasThreadManager : IInkCanvasThreadManager
	{
		private Dispatcher InkDispatcher => DynamicRendererThreadManager.InkingDispatcher;

		private Dispatcher MainDispatcher => InkCanvas.Dispatcher;

		private DynamicRendererThreadManager DynamicRendererThreadManager { get; }

		private InkCanvas InkCanvas { get; }

		public InkCanvasThreadManager([NotNull] InkCanvas inkCanvas, [NotNull] DynamicRendererThreadManager dynamicRendererThreadManager)
		{
			if (inkCanvas == null)
			{
				throw new ArgumentNullException("inkCanvas");
			}
			if (dynamicRendererThreadManager == null)
			{
				throw new ArgumentNullException("dynamicRendererThreadManager");
			}
			InkCanvas = inkCanvas;
			DynamicRendererThreadManager = dynamicRendererThreadManager;
		}

		public void RunInTouchDispatcher(Action action)
		{
			InkDispatcher.InvokeAsync(() =>
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					Trace.TraceError(ex.Message);
					if (Debugger.IsAttached)
					{
						Debugger.Break();
					}
				}
			}, DispatcherPriority.Send);
		}

		public void RunInMainDispatcher(Action action)
		{
			MainDispatcher.InvokeAsync(() =>
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					Trace.TraceError(ex.Message);
					if (Debugger.IsAttached)
					{
						Debugger.Break();
					}
				}
			});
		}
	}
}
