using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Threading;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;
using Cvte.Windows.Threading;

namespace Cvte.Paint.Inks
{
	public class InkCanvas : UIElement, IStylusPlugInSource, IInputElement, IDisposable
	{
		private class InkLocker
		{
			private volatile bool _locker;

			public void Wait()
			{
				while (!_locker)
				{
					Thread.Sleep(0);
				}
			}

			public void Set()
			{
				_locker = true;
			}
		}

		private bool _needInput = true;

		private readonly DispatcherRequiring _invokeStrokesCollected;

		private readonly List<DryingDrawingVisual> _dryingDrawingVisualList = new List<DryingDrawingVisual>();

		private readonly EventHandler _mainRenderComplete;

		private readonly HostVisual _hostVisual = new HostVisual();

		private DynamicRenderer _dynamicRenderer;

		private Rect _hitTestRect = Rect.Empty;

		private bool _isInit;

		private bool _isStarted;

		private VisualTarget _visualTarget;

		private IStylusPlugInSource _eventSource;

		public IStylusPlugInSource EventSource
		{
			get
			{
				return _eventSource ?? this;
			}
			set
			{
				if (_isInit)
				{
					throw new InvalidOperationException("禁止在初始化之后设置输入源");
				}
				IStylusPlugInSource eventSource = _eventSource;
				if (eventSource != value)
				{
					DynamicRenderer dynamicRenderer = DynamicRenderer;
					if (dynamicRenderer != null && eventSource != null)
					{
						eventSource.RemoveStylusPlugIns(dynamicRenderer);
						value.AddStylusPlugIns(dynamicRenderer);
					}
					_eventSource = value;
					if (value != null && value != this)
					{
						_needInput = false;
					}
				}
			}
		}

		public DynamicRenderer DynamicRenderer
		{
			get
			{
				return _dynamicRenderer;
			}
			set
			{
				if (_dynamicRenderer != value)
				{
					if (_dynamicRenderer != null)
					{
						_dynamicRenderer.Stop();
						EventSource.RemoveStylusPlugIns(_dynamicRenderer);
						_dynamicRenderer.StrokesCollected -= DynamicRenderer_StrokesCollected;
					}
					_dynamicRenderer = value;
					InkSynchronizer = new InkSynchronizer(value);
					if (_isInit)
					{
						InitRender();
					}
					else
					{
						Inited += InkCanvas_Inited;
					}
					if (_isStarted)
					{
						EventSource.AddStylusPlugIns(value);
					}
					_dynamicRenderer.StrokesCollected -= DynamicRenderer_StrokesCollected;
					_dynamicRenderer.StrokesCollected += DynamicRenderer_StrokesCollected;
					DynamicRendererChanged?.Invoke(this, value);
				}
			}
		}

		public InkSynchronizer InkSynchronizer { get; private set; }

		public ContainerVisual InkFromTouchContainerVisual { get; private set; }

		public ContainerVisual InkFromMouseContainerVisual { get; private set; }

		protected override int VisualChildrenCount => 2;

		private DynamicRendererThreadManager DynamicRendererThreadManager { get; } = new DynamicRendererThreadManager();

		[NotNull]
		private IInkCanvasThreadManager InkCanvasThreadManager { get; }

		public event EventHandler<InkSynchronizer> StrokesCollected;

		public event EventHandler<DynamicRenderer> DynamicRendererChanged;

		private event EventHandler Started;

		private event EventHandler Inited;

		public InkCanvas()
			: this(true)
		{
		}

		public InkCanvas(bool withInit = true)
			: this(withInit, null)
		{
		}

		internal InkCanvas(bool withInit, IInkCanvasThreadManager inkCanvasThreadManager)
		{
			if (inkCanvasThreadManager == null)
			{
				inkCanvasThreadManager = new InkCanvasThreadManager(this, DynamicRendererThreadManager);
			}
			InkCanvasThreadManager = inkCanvasThreadManager;
			if (withInit)
			{
				Init();
			}
			_mainRenderComplete = InternalRenderComplete;
			_invokeStrokesCollected = new DispatcherRequiring(() =>
			{
				StrokesCollected?.Invoke(this, InkSynchronizer);
			}, DispatcherPriority.Background);
		}

		private void DynamicRenderer_StrokesCollected(object sender, EventArgs e)
		{
			Dispatcher.InvokeAsync(OnStrokesCollected);
		}

		public void Init()
		{
			if (_isInit)
			{
				throw new InvalidOperationException("已经初始化不能再次调用此函数");
			}
			_isInit = true;
			DynamicRendererThreadManager.StartUp();
			AddVisualChild(_hostVisual);
			RunInTouchDispatcher(() =>
			{
				_visualTarget = new VisualTarget(_hostVisual);
				ContainerVisual rootVisual = (InkFromTouchContainerVisual = new ContainerVisual());
				_visualTarget.RootVisual = rootVisual;
			});
			InkFromMouseContainerVisual = new ContainerVisual();
			AddVisualChild(InkFromMouseContainerVisual);
			if (DynamicRenderer == null)
			{
				DynamicRenderer = new DynamicRenderer(this);
			}
			Start();
			Inited?.Invoke(this, null);
		}

		public void Start()
		{
			if (!_isStarted)
			{
				IStylusPlugInSource eventSource = EventSource;
				if (!eventSource.ContainsStylusPlugIns(DynamicRenderer))
				{
					eventSource.AddStylusPlugIns(DynamicRenderer);
				}
				eventSource.StylusUp += Source_StylusUp;
				eventSource.MouseUp += Source_MouseUp;
				DynamicRenderer.Start();
				IsHitTestVisible = _needInput;
				_isStarted = true;
				Started?.Invoke(this, null);
				if (DebuggingProperties.IsDebug)
				{
					InkCanvasVisualBrushDebugger.AttachDebugInfo(this);
				}
			}
		}

		public void Stop()
		{
			if (_isStarted)
			{
				DynamicRenderer.Stop();
				IStylusPlugInSource eventSource = EventSource;
				eventSource.StylusUp -= Source_StylusUp;
				eventSource.MouseUp -= Source_MouseUp;
				eventSource.RemoveStylusPlugIns(DynamicRenderer);
			}
			IsHitTestVisible = false;
			InkSynchronizer.Clean();
			_isStarted = false;
		}

		public void Close()
		{
			Stop();
			DynamicRendererThreadManager.Close();
		}

		public void RunInTouchDispatcher(Action action)
		{
			InkCanvasThreadManager.RunInTouchDispatcher(action);
		}

		public void RunInTouchDispatcher(Action<ContainerVisual> action)
		{
			InkCanvasThreadManager.RunInTouchDispatcher(() =>
			{
				action(InkFromTouchContainerVisual);
			});
		}

		public void RunInMouseDispatcher(Action<ContainerVisual> action)
		{
			ContainerVisual inkFromMouseContainerVisual = InkFromMouseContainerVisual;
			InkCanvasThreadManager.RunInMainDispatcher(() =>
			{
				action(inkFromMouseContainerVisual);
			});
		}

		public void LostCapture()
		{
			DynamicRenderer.Rest();
		}

		public void Dispose()
		{
			RunInTouchDispatcher(() =>
			{
				_visualTarget?.Dispose();
			});
			Close();
		}

		protected virtual void OnStrokesCollected()
		{
			_invokeStrokesCollected.Require();
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			if (!_needInput)
			{
				return null;
			}
			if (_hitTestRect.Contains(hitTestParameters.HitPoint))
			{
				return new PointHitTestResult(this, hitTestParameters.HitPoint);
			}
			return base.HitTestCore(hitTestParameters);
		}

		protected override Size MeasureCore(Size availableSize)
		{
			if (availableSize.Height > 0.0 && availableSize.Width > 0.0 && !double.IsInfinity(availableSize.Height) && !double.IsInfinity(availableSize.Width))
			{
				return availableSize;
			}
			return new Size(1920.0, 1080.0);
		}

		protected override void ArrangeCore(Rect finalRect)
		{
			RenderSize = finalRect.Size;
			_hitTestRect = finalRect;
		}

		protected override Visual GetVisualChild(int index)
		{
			if (index == 0)
			{
				return _hostVisual;
			}
			return InkFromMouseContainerVisual;
		}

		public void AddDryingDrawingVisualList(DryingDrawingVisual dryingDrawingVisual)
		{
			lock (_dryingDrawingVisualList)
			{
				_dryingDrawingVisualList.Add(dryingDrawingVisual);
			}
		}

		public void NotifyOnNextRenderComplete()
		{
			DynamicRendererThreadManager.MainMediaContextRenderComplete += _mainRenderComplete;
		}

		private void Source_MouseUp(object sender, MouseButtonEventArgs e)
		{
		}

		private void Source_StylusUp(object sender, StylusEventArgs e)
		{
		}

		private void InternalRenderComplete(object sender, EventArgs e)
		{
			DynamicRendererThreadManager.MainMediaContextRenderComplete -= _mainRenderComplete;
			List<DryingDrawingVisual> source;
			lock (_dryingDrawingVisualList)
			{
				source = _dryingDrawingVisualList.ToList();
				_dryingDrawingVisualList.Clear();
			}
			foreach (IGrouping<ContainerVisual, DryingDrawingVisual> dryingDrawingVisual in from temp in source
				group temp by temp.ContainerVisual)
			{
				ContainerVisual containerVisual = dryingDrawingVisual.Key;
				if (containerVisual.CheckAccess())
				{
					RemoveDrawingVisual(dryingDrawingVisual, containerVisual);
					continue;
				}
				containerVisual.Dispatcher.InvokeAsync(() =>
				{
					RemoveDrawingVisual(dryingDrawingVisual, containerVisual);
				}, DispatcherPriority.Background);
			}
		}

		private static void RemoveDrawingVisual(IEnumerable<DryingDrawingVisual> dryingDrawingVisualList, ContainerVisual containerVisual)
		{
			foreach (DryingDrawingVisual dryingDrawingVisual in dryingDrawingVisualList)
			{
				foreach (DrawingVisual dryingDrawingVisual2 in dryingDrawingVisual.DryingDrawingVisualList)
				{
					containerVisual.Children.Remove(dryingDrawingVisual2);
				}
			}
		}

		private void InkCanvas_Inited(object sender, EventArgs e)
		{
			Inited -= InkCanvas_Inited;
			InitRender();
		}

		private void InitRender()
		{
			InkLocker inkLocker = new InkLocker();
			RunInTouchDispatcher(() =>
			{
				DynamicRenderer.Init();
				if (_isStarted)
				{
					DynamicRenderer.Start();
				}
				inkLocker.Set();
			});
			inkLocker.Wait();
		}

		public void AddStylusPlugIns(StylusPlugIn stylusPlugIns)
		{
			StylusPlugIns.Add(stylusPlugIns);
		}

		public void RemoveStylusPlugIns(StylusPlugIn stylusPlugIns)
		{
			StylusPlugIns.Remove(stylusPlugIns);
		}

		public bool ContainsStylusPlugIns(StylusPlugIn stylusPlugIns)
		{
			return StylusPlugIns.Contains(stylusPlugIns);
		}
	}
}
