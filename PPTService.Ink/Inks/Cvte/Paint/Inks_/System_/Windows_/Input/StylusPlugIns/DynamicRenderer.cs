using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Threading;

namespace Cvte.Paint.Inks_.System_.Windows_.Input.StylusPlugIns
{
	[Obsolete("这个类是反编译微软源代码，只能用于调试")]
	public class DynamicRenderer : StylusPlugIn
	{
		public class MediaContextReflection
		{
			private static readonly MemberInfo _removeRenderComplete;

			public static MethodInfo AddRenderComplete { get; set; }

			public static Type MediaContext { get; }

			public static MethodInfo From { get; }

			public static EventInfo RenderComplete { get; }

			static MediaContextReflection()
			{
				MediaContext = Assembly.GetAssembly(typeof(StylusPlugIn)).GetType("System.Windows.Media.MediaContext");
				From = Bindf.GetMethod(MediaContext, "From", null);
				RenderComplete = MediaContext.GetEvent("RenderComplete", BindingFlags.Instance | BindingFlags.NonPublic);
				AddRenderComplete = RenderComplete.GetAddMethod(true);
				_removeRenderComplete = RenderComplete.GetRemoveMethod(true);
			}

			public static void RemoveRenderComplete(object mediaContext, EventHandler onRenderComplete)
			{
			}
		}

		public class StrokeNodeIteratorReflection
		{
			public static MethodInfo GetIteratorForNextSegment { get; }

			public static Type TStrokeNodeIterator { get; }

			static StrokeNodeIteratorReflection()
			{
				TStrokeNodeIterator = Assembly.GetAssembly(typeof(StylusPlugIn)).GetType("MS.Internal.Ink.StrokeNodeIterator");
				GetIteratorForNextSegment = Bindf.GetMethod(TStrokeNodeIterator, "GetIteratorForNextSegment", new Type[1] { typeof(StylusPointCollection) });
				MediaContextReflection.MediaContext.GetType();
			}
		}

		public class StrokeRendererReflection
		{
			public static Type TStrokeRenderer { get; }

			public static MethodInfo CalcGeometryAndBounds { get; }

			public static FieldInfo SolidStrokeAlpha { get; }

			static StrokeRendererReflection()
			{
				TStrokeRenderer = Assembly.GetAssembly(typeof(StylusPlugIn)).GetType("MS.Internal.Ink.StrokeRenderer");
				CalcGeometryAndBounds = Bindf.GetMethod(TStrokeRenderer, "CalcGeometryAndBounds", null);
				SolidStrokeAlpha = TStrokeRenderer.GetField("SolidStrokeAlpha", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
			}
		}

		private class StrokeInfo
		{
			private int _stylusId;

			private int _startTime;

			private int _lastTime;

			private ContainerVisual _strokeCV;

			private ContainerVisual _strokeRTICV;

			private bool _seenUp;

			private bool _isReset;

			private SolidColorBrush _fillBrush;

			private DrawingAttributes _drawingAttributes;

			private object _strokeNodeIterator;

			private double _opacity;

			private DynamicRendererHostVisual _strokeHV;

			public int StylusId => _stylusId;

			public int StartTime => _startTime;

			public int LastTime
			{
				get
				{
					return _lastTime;
				}
				set
				{
					_lastTime = value;
				}
			}

			public ContainerVisual StrokeCV
			{
				get
				{
					return _strokeCV;
				}
				set
				{
					_strokeCV = value;
				}
			}

			public ContainerVisual StrokeRTICV
			{
				get
				{
					return _strokeRTICV;
				}
				set
				{
					_strokeRTICV = value;
				}
			}

			public bool SeenUp
			{
				get
				{
					return _seenUp;
				}
				set
				{
					_seenUp = value;
				}
			}

			public bool IsReset
			{
				get
				{
					return _isReset;
				}
				set
				{
					_isReset = value;
				}
			}

			public object StrokeNodeIterator
			{
				get
				{
					return _strokeNodeIterator;
				}
				set
				{
					if (value == null)
					{
						throw new ArgumentNullException("StrokeNodeIterator");
					}
					_strokeNodeIterator = value;
				}
			}

			public SolidColorBrush FillBrush
			{
				get
				{
					return _fillBrush;
				}
				set
				{
					_fillBrush = value;
				}
			}

			public DrawingAttributes DrawingAttributes => _drawingAttributes;

			public double Opacity => _opacity;

			public DynamicRendererHostVisual StrokeHV => _strokeHV;

			public StrokeInfo(DrawingAttributes drawingAttributes, int stylusDeviceId, int startTimestamp, DynamicRendererHostVisual hostVisual)
			{
				_stylusId = stylusDeviceId;
				_startTime = startTimestamp;
				_lastTime = _startTime;
				_drawingAttributes = drawingAttributes.Clone();
				_strokeNodeIterator = StrokeNodeIteratorReflection.TStrokeNodeIterator.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[1].Invoke(new object[1] { _drawingAttributes });
				Color color = _drawingAttributes.Color;
				byte b = (byte)StrokeRendererReflection.SolidStrokeAlpha.GetValue(null);
				_opacity = (_drawingAttributes.IsHighlighter ? 0.0 : ((double)(int)color.A / (double)(int)b));
				color.A = b;
				SolidColorBrush solidColorBrush = new SolidColorBrush(color);
				solidColorBrush.Freeze();
				_fillBrush = solidColorBrush;
				_strokeHV = hostVisual;
				hostVisual.AddStrokeInfoRef(this);
			}

			public bool IsTimestampWithin(int timestamp)
			{
				if (!SeenUp)
				{
					return true;
				}
				if (StartTime < LastTime)
				{
					if (timestamp >= StartTime)
					{
						return timestamp <= LastTime;
					}
					return false;
				}
				if (timestamp < StartTime)
				{
					return timestamp <= LastTime;
				}
				return true;
			}

			public bool IsTimestampAfter(int timestamp)
			{
				if (SeenUp)
				{
					return false;
				}
				if (LastTime >= StartTime)
				{
					if (timestamp >= LastTime)
					{
						return true;
					}
					if (LastTime > 0)
					{
						return timestamp < 0;
					}
					return false;
				}
				if (timestamp >= LastTime)
				{
					return timestamp <= StartTime;
				}
				return false;
			}
		}

		private class DynamicRendererHostVisual : HostVisual
		{
			private List<StrokeInfo> _strokeInfoList = new List<StrokeInfo>();

			private VisualTarget _visualTarget;

			internal bool InUse => _strokeInfoList.Count > 0;

			internal bool HasSingleReference => _strokeInfoList.Count == 1;

			internal VisualTarget VisualTarget
			{
				[SecuritySafeCritical]
				get
				{
					if (_visualTarget == null)
					{
						_visualTarget = new VisualTarget(this);
						_visualTarget.RootVisual = new ContainerVisual();
					}
					return _visualTarget;
				}
			}

			internal void AddStrokeInfoRef(StrokeInfo si)
			{
				_strokeInfoList.Add(si);
			}

			internal void RemoveStrokeInfoRef(StrokeInfo si)
			{
				_strokeInfoList.Remove(si);
			}
		}

		private DrawingAttributes _drawAttrsSource = new DrawingAttributes();

		private List<StrokeInfo> _strokeInfoList = new List<StrokeInfo>();

		private object __siLock = new object();

		private Queue<StrokeInfo> _renderCompleteDRThreadStrokeInfoList = new Queue<StrokeInfo>();

		private Dispatcher _applicationDispatcher;

		private Geometry _zeroSizedFrozenRect;

		private ContainerVisual _mainContainerVisual;

		private ContainerVisual _mainRawInkContainerVisual;

		private DynamicRendererHostVisual _rawInkHostVisual1;

		private DynamicRendererHostVisual _rawInkHostVisual2;

		private DynamicRendererHostVisual _currentHostVisual;

		private EventHandler _onRenderComplete;

		private bool _waitingForRenderComplete;

		private StrokeInfo _renderCompleteStrokeInfo;

		private DynamicRendererThreadManager _renderingThread;

		private EventHandler _onDRThreadRenderComplete;

		private bool _waitingForDRThreadRenderComplete;

		public Visual RootVisual
		{
			get
			{
				if (_mainContainerVisual == null)
				{
					CreateInkingVisuals();
				}
				return _mainContainerVisual;
			}
		}

		public DrawingAttributes DrawingAttributes
		{
			get
			{
				return _drawAttrsSource;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_drawAttrsSource = value;
				OnDrawingAttributesReplaced();
			}
		}

		public DynamicRenderer()
		{
			_zeroSizedFrozenRect = new RectangleGeometry(new Rect(0.0, 0.0, 0.0, 0.0));
			_zeroSizedFrozenRect.Freeze();
		}

		public virtual void Reset(StylusDevice stylusDevice, StylusPointCollection stylusPoints)
		{
			if (_mainContainerVisual == null || _applicationDispatcher == null || !IsActiveForInput)
			{
				return;
			}
			_applicationDispatcher.VerifyAccess();
			if (stylusDevice?.InAir ?? (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Released))
			{
				throw new ArgumentException("stylusDevice");
			}
			using (_applicationDispatcher.DisableProcessing())
			{
				lock (__siLock)
				{
					AbortAllStrokes();
					StrokeInfo strokeInfo = new StrokeInfo(DrawingAttributes, stylusDevice?.Id ?? 0, Environment.TickCount, GetCurrentHostVisual());
					_strokeInfoList.Add(strokeInfo);
					strokeInfo.IsReset = true;
					if (stylusPoints != null)
					{
						RenderPackets(stylusPoints, strokeInfo);
					}
				}
			}
		}

		protected override void OnAdded()
		{
			_applicationDispatcher = Element.Dispatcher;
			if (IsActiveForInput)
			{
				CreateRealTimeVisuals();
			}
		}

		protected override void OnRemoved()
		{
			DestroyRealTimeVisuals();
			_applicationDispatcher = null;
		}

		protected override void OnIsActiveForInputChanged()
		{
			if (IsActiveForInput)
			{
				CreateRealTimeVisuals();
			}
			else
			{
				DestroyRealTimeVisuals();
			}
		}

		protected override void OnStylusEnter(RawStylusInput rawStylusInput, bool confirmed)
		{
			HandleStylusEnterLeave(rawStylusInput, true, confirmed);
		}

		protected override void OnStylusLeave(RawStylusInput rawStylusInput, bool confirmed)
		{
			HandleStylusEnterLeave(rawStylusInput, false, confirmed);
		}

		private void HandleStylusEnterLeave(RawStylusInput rawStylusInput, bool isEnter, bool isConfirmed)
		{
			if (isConfirmed)
			{
				StrokeInfo strokeInfo = FindStrokeInfo(rawStylusInput.Timestamp);
				if (strokeInfo != null && rawStylusInput.StylusDeviceId == strokeInfo.StylusId && ((isEnter && rawStylusInput.Timestamp > strokeInfo.StartTime) || (!isEnter && !strokeInfo.SeenUp)))
				{
					TransitionStrokeVisuals(strokeInfo, true);
				}
			}
		}

		protected override void OnEnabledChanged()
		{
			if (!Enabled)
			{
				AbortAllStrokes();
			}
		}

		protected override void OnStylusDown(RawStylusInput rawStylusInput)
		{
			if (_mainContainerVisual == null)
			{
				return;
			}
			StrokeInfo strokeInfo;
			lock (__siLock)
			{
				strokeInfo = FindStrokeInfo(rawStylusInput.Timestamp);
				if (strokeInfo != null)
				{
					return;
				}
				strokeInfo = new StrokeInfo(DrawingAttributes, rawStylusInput.StylusDeviceId, rawStylusInput.Timestamp, GetCurrentHostVisual());
				_strokeInfoList.Add(strokeInfo);
			}
			rawStylusInput.NotifyWhenProcessed(strokeInfo);
			RenderPackets(rawStylusInput.GetStylusPoints(), strokeInfo);
		}

		protected override void OnStylusMove(RawStylusInput rawStylusInput)
		{
			if (_mainContainerVisual == null)
			{
				return;
			}
			StrokeInfo strokeInfo = FindStrokeInfo(rawStylusInput.Timestamp);
			if (strokeInfo != null && strokeInfo.StylusId == rawStylusInput.StylusDeviceId && strokeInfo.IsTimestampAfter(rawStylusInput.Timestamp))
			{
				strokeInfo.LastTime = rawStylusInput.Timestamp;
				RenderPackets(rawStylusInput.GetStylusPoints(), strokeInfo);
			}
			if (strokeInfo == null || strokeInfo.StylusId != rawStylusInput.StylusDeviceId || !strokeInfo.IsTimestampAfter(rawStylusInput.Timestamp))
			{
				if (strokeInfo == null)
				{
					Log("strokeInfo 空");
				}
				else if (strokeInfo.StylusId != rawStylusInput.StylusDeviceId)
				{
					Log("strokeInfo.StylusId != rawStylusInput.StylusDeviceId " + _strokeInfoList[0].StylusId);
				}
				else if (!strokeInfo.IsTimestampAfter(rawStylusInput.Timestamp))
				{
					Log("!strokeInfo.IsTimestampAfter(rawStylusInput.Timestamp)");
				}
			}
		}

		private static void Log(string str)
		{
		}

		protected override void OnStylusUp(RawStylusInput rawStylusInput)
		{
			if (_mainContainerVisual != null)
			{
				StrokeInfo strokeInfo = FindStrokeInfo(rawStylusInput.Timestamp);
				if (strokeInfo != null && (strokeInfo.StylusId == rawStylusInput.StylusDeviceId || (rawStylusInput.StylusDeviceId == 0 && (strokeInfo.IsReset || (strokeInfo.IsTimestampAfter(rawStylusInput.Timestamp) && IsStylusUp(strokeInfo.StylusId))))))
				{
					strokeInfo.SeenUp = true;
					strokeInfo.LastTime = rawStylusInput.Timestamp;
					rawStylusInput.NotifyWhenProcessed(strokeInfo);
				}
			}
		}

		private bool IsStylusUp(int stylusId)
		{
			TabletDeviceCollection tabletDevices = Tablet.TabletDevices;
			for (int i = 0; i < tabletDevices.Count; i++)
			{
				TabletDevice tabletDevice = tabletDevices[i];
				for (int j = 0; j < tabletDevice.StylusDevices.Count; j++)
				{
					StylusDevice stylusDevice = tabletDevice.StylusDevices[j];
					if (stylusId == stylusDevice.Id)
					{
						return stylusDevice.InAir;
					}
				}
			}
			return true;
		}

		private void OnRenderComplete()
		{
			StrokeInfo renderCompleteStrokeInfo = _renderCompleteStrokeInfo;
			if (renderCompleteStrokeInfo != null)
			{
				if (renderCompleteStrokeInfo.StrokeHV.Clip == null)
				{
					TransitionComplete(renderCompleteStrokeInfo);
					_renderCompleteStrokeInfo = null;
				}
				else
				{
					RemoveDynamicRendererVisualAndNotifyWhenDone(renderCompleteStrokeInfo);
				}
			}
		}

		private void RemoveDynamicRendererVisualAndNotifyWhenDone(StrokeInfo si)
		{
			if (si == null)
			{
				return;
			}
			DynamicRendererThreadManager renderingThread = _renderingThread;
			if (renderingThread == null)
			{
				return;
			}
			DynamicRendererThreadManager.GetThreadDispatcher(renderingThread).InvokeAsync(() =>
			{
				if (si.StrokeRTICV != null)
				{
					if (_onDRThreadRenderComplete == null)
					{
						_onDRThreadRenderComplete = OnDRThreadRenderComplete;
					}
					_renderCompleteDRThreadStrokeInfoList.Enqueue(si);
					if (!_waitingForDRThreadRenderComplete)
					{
						((ContainerVisual)si.StrokeHV.VisualTarget.RootVisual).Children.Remove(si.StrokeRTICV);
						si.StrokeRTICV = null;
						object obj = MediaContextReflection.From.Invoke(null, new object[1] { DynamicRendererThreadManager.GetThreadDispatcher(renderingThread) });
						_waitingForDRThreadRenderComplete = true;
						MethodInfo addRenderComplete = MediaContextReflection.AddRenderComplete;
						object[] parameters = new EventHandler[1] { _onDRThreadRenderComplete.Invoke };
						addRenderComplete.Invoke(obj, parameters);
					}
				}
				else
				{
					NotifyAppOfDRThreadRenderComplete(si);
				}
				return (object)null;
			}, DispatcherPriority.Send);
		}

		private void NotifyAppOfDRThreadRenderComplete(StrokeInfo si)
		{
			_applicationDispatcher?.InvokeAsync(() =>
			{
				if (si == _renderCompleteStrokeInfo)
				{
					if (si.StrokeHV.Clip != null)
					{
						si.StrokeHV.Clip = null;
						NotifyOnNextRenderComplete();
					}
					else
					{
						TransitionComplete(si);
					}
				}
				else
				{
					TransitionComplete(si);
				}
				return (object)null;
			}, DispatcherPriority.Send);
		}

		private void OnDRThreadRenderComplete(object sender, EventArgs e)
		{
			DynamicRendererThreadManager renderingThread = _renderingThread;
			if (renderingThread == null)
			{
				return;
			}
			Dispatcher threadDispatcher = DynamicRendererThreadManager.GetThreadDispatcher(renderingThread);
			if (threadDispatcher == null)
			{
				return;
			}
			if (_renderCompleteDRThreadStrokeInfoList.Count > 0)
			{
				NotifyAppOfDRThreadRenderComplete(_renderCompleteDRThreadStrokeInfoList.Dequeue());
			}
			if (_renderCompleteDRThreadStrokeInfoList.Count == 0)
			{
				MediaContextReflection.RemoveRenderComplete(MediaContextReflection.From.Invoke(null, new object[1] { threadDispatcher }), _onDRThreadRenderComplete.Invoke);
				_waitingForDRThreadRenderComplete = false;
				return;
			}
			StrokeInfo siNext = _renderCompleteDRThreadStrokeInfoList.Peek();
			if (siNext.StrokeRTICV != null)
			{
				threadDispatcher.InvokeAsync(() =>
				{
					((ContainerVisual)siNext.StrokeHV.VisualTarget.RootVisual).Children.Remove(siNext.StrokeRTICV);
					siNext.StrokeRTICV = null;
					return (object)null;
				}, DispatcherPriority.Send);
			}
		}

		protected override void OnStylusDownProcessed(object callbackData, bool targetVerified)
		{
			StrokeInfo strokeInfo = callbackData as StrokeInfo;
			if (!((strokeInfo == null) | targetVerified))
			{
				TransitionStrokeVisuals(strokeInfo, true);
			}
		}

		protected override void OnStylusUpProcessed(object callbackData, bool targetVerified)
		{
			StrokeInfo strokeInfo = callbackData as StrokeInfo;
			if (strokeInfo != null)
			{
				TransitionStrokeVisuals(strokeInfo, !targetVerified);
			}
		}

		private void OnInternalRenderComplete(object sender, EventArgs e)
		{
			MediaContextReflection.RemoveRenderComplete(MediaContextReflection.From.Invoke(null, new object[1] { _applicationDispatcher }), _onRenderComplete.Invoke);
			_waitingForRenderComplete = false;
			using (_applicationDispatcher.DisableProcessing())
			{
				OnRenderComplete();
			}
		}

		private void NotifyOnNextRenderComplete()
		{
			if (_applicationDispatcher != null)
			{
				_applicationDispatcher.VerifyAccess();
				if (_onRenderComplete == null)
				{
					_onRenderComplete = OnInternalRenderComplete;
				}
				if (!_waitingForRenderComplete)
				{
					object obj = MediaContextReflection.From.Invoke(null, new object[1] { _applicationDispatcher });
					MethodInfo addRenderComplete = MediaContextReflection.AddRenderComplete;
					object[] parameters = new EventHandler[1] { _onRenderComplete.Invoke };
					addRenderComplete.Invoke(obj, parameters);
					_waitingForRenderComplete = true;
				}
			}
		}

		protected virtual void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints, Geometry geometry, Brush fillBrush)
		{
			if (drawingContext == null)
			{
				throw new ArgumentNullException("drawingContext");
			}
			drawingContext.DrawGeometry(fillBrush, null, geometry);
		}

		protected virtual void OnDrawingAttributesReplaced()
		{
		}

		protected Dispatcher GetDispatcher()
		{
			if (_renderingThread == null)
			{
				return null;
			}
			return DynamicRendererThreadManager.GetThreadDispatcher(_renderingThread);
		}

		private void RenderPackets(StylusPointCollection stylusPoints, StrokeInfo si)
		{
			if (stylusPoints.Count == 0 || _applicationDispatcher == null)
			{
				return;
			}
			si.StrokeNodeIterator = StrokeNodeIteratorReflection.GetIteratorForNextSegment.Invoke(si.StrokeNodeIterator, new object[1] { stylusPoints });
			if (si.StrokeNodeIterator == null)
			{
				return;
			}
			Geometry strokeGeometry = null;
			object[] array = new object[5]
			{
				si.StrokeNodeIterator,
				si.DrawingAttributes,
				false,
				null,
				default(Rect)
			};
			StrokeRendererReflection.CalcGeometryAndBounds.Invoke(null, array);
			strokeGeometry = (Geometry)array[3];
			Rect rect = (Rect)array[4];
			if (_applicationDispatcher.CheckAccess())
			{
				if (si.StrokeCV == null)
				{
					si.StrokeCV = new ContainerVisual();
					if (!si.DrawingAttributes.IsHighlighter)
					{
						si.StrokeCV.Opacity = si.Opacity;
					}
					_mainRawInkContainerVisual.Children.Add(si.StrokeCV);
				}
				DrawingVisual drawingVisual = new DrawingVisual();
				DrawingContext drawingContext = drawingVisual.RenderOpen();
				try
				{
					OnDraw(drawingContext, stylusPoints, strokeGeometry, si.FillBrush);
				}
				finally
				{
					drawingContext.Close();
				}
				if (si.StrokeCV != null)
				{
					si.StrokeCV.Children.Add(drawingVisual);
				}
				return;
			}
			DynamicRendererThreadManager renderingThread = _renderingThread;
			((renderingThread != null) ? DynamicRendererThreadManager.GetThreadDispatcher(renderingThread) : null)?.InvokeAsync(() =>
			{
				SolidColorBrush fillBrush = si.FillBrush;
				if (fillBrush != null)
				{
					if (si.StrokeRTICV == null)
					{
						si.StrokeRTICV = new ContainerVisual();
						if (!si.DrawingAttributes.IsHighlighter)
						{
							si.StrokeRTICV.Opacity = si.Opacity;
						}
						((ContainerVisual)si.StrokeHV.VisualTarget.RootVisual).Children.Add(si.StrokeRTICV);
					}
					DrawingVisual drawingVisual2 = new DrawingVisual();
					DrawingContext drawingContext2 = drawingVisual2.RenderOpen();
					try
					{
						OnDraw(drawingContext2, stylusPoints, strokeGeometry, fillBrush);
					}
					finally
					{
						drawingContext2.Close();
					}
					si.StrokeRTICV.Children.Add(drawingVisual2);
				}
			}, DispatcherPriority.Send);
		}

		private void AbortAllStrokes()
		{
			lock (__siLock)
			{
				while (_strokeInfoList.Count > 0)
				{
					TransitionStrokeVisuals(_strokeInfoList[0], true);
				}
			}
		}

		private void TransitionStrokeVisuals(StrokeInfo si, bool abortStroke)
		{
			RemoveStrokeInfo(si);
			if (si.StrokeCV != null)
			{
				if (_mainRawInkContainerVisual != null)
				{
					_mainRawInkContainerVisual.Children.Remove(si.StrokeCV);
				}
				si.StrokeCV = null;
			}
			si.FillBrush = null;
			if (_rawInkHostVisual1 == null)
			{
				return;
			}
			bool flag = false;
			if (!abortStroke && _renderCompleteStrokeInfo == null)
			{
				using (_applicationDispatcher.DisableProcessing())
				{
					lock (__siLock)
					{
						if (si.StrokeHV.HasSingleReference)
						{
							si.StrokeHV.Clip = _zeroSizedFrozenRect;
							_renderCompleteStrokeInfo = si;
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				NotifyOnNextRenderComplete();
			}
			else
			{
				RemoveDynamicRendererVisualAndNotifyWhenDone(si);
			}
		}

		private DynamicRendererHostVisual GetCurrentHostVisual()
		{
			if (_currentHostVisual == null)
			{
				_currentHostVisual = _rawInkHostVisual1;
			}
			else
			{
				HostVisual hostVisual = ((_renderCompleteStrokeInfo != null) ? _renderCompleteStrokeInfo.StrokeHV : null);
				if (_currentHostVisual.InUse)
				{
					if (_currentHostVisual == _rawInkHostVisual1)
					{
						if (!_rawInkHostVisual2.InUse || _rawInkHostVisual1 == hostVisual)
						{
							_currentHostVisual = _rawInkHostVisual2;
						}
					}
					else if (!_rawInkHostVisual1.InUse || _rawInkHostVisual2 == hostVisual)
					{
						_currentHostVisual = _rawInkHostVisual1;
					}
				}
			}
			return _currentHostVisual;
		}

		private void TransitionComplete(StrokeInfo si)
		{
			using (_applicationDispatcher.DisableProcessing())
			{
				lock (__siLock)
				{
					si.StrokeHV.RemoveStrokeInfoRef(si);
				}
			}
		}

		private void RemoveStrokeInfo(StrokeInfo si)
		{
			lock (__siLock)
			{
				_strokeInfoList.Remove(si);
			}
		}

		private StrokeInfo FindStrokeInfo(int timestamp)
		{
			lock (__siLock)
			{
				for (int i = 0; i < _strokeInfoList.Count; i++)
				{
					StrokeInfo strokeInfo = _strokeInfoList[i];
					if (strokeInfo.IsTimestampWithin(timestamp))
					{
						return strokeInfo;
					}
				}
			}
			return null;
		}

		private void CreateInkingVisuals()
		{
			if (_mainContainerVisual == null)
			{
				_mainContainerVisual = new ContainerVisual();
				_mainRawInkContainerVisual = new ContainerVisual();
				_mainContainerVisual.Children.Add(_mainRawInkContainerVisual);
			}
			if (!IsActiveForInput)
			{
				return;
			}
			using (Element.Dispatcher.DisableProcessing())
			{
				CreateRealTimeVisuals();
			}
		}

		private void CreateRealTimeVisuals()
		{
			if (_mainContainerVisual != null && _rawInkHostVisual1 == null)
			{
				_rawInkHostVisual1 = new DynamicRendererHostVisual();
				_rawInkHostVisual2 = new DynamicRendererHostVisual();
				_currentHostVisual = null;
				_mainContainerVisual.Children.Add(_rawInkHostVisual1);
				_mainContainerVisual.Children.Add(_rawInkHostVisual2);
				_renderingThread = DynamicRendererThreadManager.GetCurrentThreadInstance();
			}
		}

		private void DestroyRealTimeVisuals()
		{
			if (_mainContainerVisual == null || _rawInkHostVisual1 == null)
			{
				return;
			}
			if (_waitingForRenderComplete)
			{
				MediaContextReflection.RemoveRenderComplete(MediaContextReflection.From.Invoke(null, new object[1] { _applicationDispatcher }), _onRenderComplete.Invoke);
				_waitingForRenderComplete = false;
			}
			_mainContainerVisual.Children.Remove(_rawInkHostVisual1);
			_mainContainerVisual.Children.Remove(_rawInkHostVisual2);
			_renderCompleteStrokeInfo = null;
			DynamicRendererThreadManager renderingThread = _renderingThread;
			Dispatcher drDispatcher = ((renderingThread != null) ? DynamicRendererThreadManager.GetThreadDispatcher(renderingThread) : null);
			if (drDispatcher != null)
			{
				drDispatcher.InvokeAsync(() =>
				{
					_renderCompleteDRThreadStrokeInfoList.Clear();
					drDispatcher = DynamicRendererThreadManager.GetThreadDispatcher(renderingThread);
					if (drDispatcher != null && _waitingForDRThreadRenderComplete)
					{
						MediaContextReflection.RemoveRenderComplete(MediaContextReflection.From.Invoke(null, new object[1] { drDispatcher }), _onDRThreadRenderComplete.Invoke);
					}
					_waitingForDRThreadRenderComplete = false;
					return (object)null;
				}, DispatcherPriority.Send);
			}
			_renderingThread = null;
			_rawInkHostVisual1 = null;
			_rawInkHostVisual2 = null;
			_currentHostVisual = null;
			AbortAllStrokes();
		}
	}
}
