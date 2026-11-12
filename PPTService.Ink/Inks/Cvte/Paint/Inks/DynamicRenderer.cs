using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Threading;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Internal.Ink;
using Cvte.Paint.Inks.Visual_;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public class DynamicRenderer : StylusPlugIn, DynamicRenderer.IInkStrokeTipBuilderBuilder, IInkDryCanvas
	{
		public class HardPenInkStrokeTipBuilder : InkStrokeTipBuilder
		{
			private StylusPointCollection _stylusPointCollection;

			private StylusPointQueue _stylusPointQueue;

			public override StylusPointCollection StrokeTipCollection
			{
				get
				{
					if (_stylusPointQueue == null || _stylusPointQueue.Count == 0)
					{
						return null;
					}
					if (_stylusPointCollection == null)
					{
						_stylusPointCollection = new StylusPointCollection(_stylusPointQueue[0].Description, 18);
					}
					StylusPointCollection stylusPointCollection = _stylusPointCollection;
					_stylusPointQueue.CopyToStylusPointCollection(stylusPointCollection);
					return stylusPointCollection;
				}
			}

			public override StylusPointCollection AddPoint(StylusPoint stylusPoint)
			{
				if (_stylusPointQueue == null)
				{
					_stylusPointQueue = new StylusPointQueue(18);
					_stylusPointQueue.Enqueue(stylusPoint);
					return null;
				}
				StylusPointCollection stylusPointCollection = null;
				if (_stylusPointQueue.Count == 16)
				{
					stylusPointCollection = new StylusPointCollection(stylusPoint.Description);
					for (int i = 0; i < 8; i++)
					{
						stylusPointCollection.Add(_stylusPointQueue.Dequeue());
					}
					stylusPointCollection.Add(_stylusPointQueue[0]);
				}
				_stylusPointQueue.Enqueue(stylusPoint);
				return stylusPointCollection;
			}

			public override void Redraw()
			{
				if (_stylusPointQueue != null && _stylusPointQueue.Count != 0)
				{
					base.Redraw();
				}
			}
		}

		public interface IInkStrokeTipBuilderBuilder
		{
			InkStrokeTipBuilder GetInkStrokeTipBuilder();
		}

		public class InkRenderer
		{
			private readonly bool _shouldUseCustomStrokeVisualCreator;

			public InkRenderer()
			{
				_shouldUseCustomStrokeVisualCreator = (object)GetType() != typeof(InkRenderer);
			}

			public void InitStrokeTipBuilder(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				StrokeInfo strokeInfo = stylusPointCollectionInfo.StrokeInfo;
				InkRenderInfo inkRenderInfo = strokeInfo.InkRenderInfo;
				InkStrokeTipBuilder inkStrokeTipBuilder = inkRenderInfo.InkStrokeTipBuilderBuilder.GetInkStrokeTipBuilder();
				inkStrokeTipBuilder.Brush = strokeInfo.FillSolidColorBrush;
				inkStrokeTipBuilder.DrawingAttributes = strokeInfo.DrawingAttributes;
				inkRenderInfo.StrokeTipBuilder = inkStrokeTipBuilder;
				ContainerVisual containerVisual = inkRenderInfo.ContainerVisual;
				DrawingVisual drawingVisual = inkRenderInfo.StrokeTipBuilder.CreateStroke();
				containerVisual.Children.Add(drawingVisual);
				inkRenderInfo.StrokeVisualList.StrokeTipVisual = drawingVisual;
				RenderPackets(stylusPointCollectionInfo);
			}

			public void RenderPackets(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				StrokeInfo strokeInfo = stylusPointCollectionInfo.StrokeInfo;
				InkRenderInfo inkRenderInfo = strokeInfo.InkRenderInfo;
				foreach (StylusPoint item in stylusPointCollectionInfo.StylusPlugInCollection)
				{
					StylusPointCollection stylusPointCollection = inkRenderInfo.StrokeTipBuilder.AddPoint(item);
					if (stylusPointCollection != null)
					{
						Cvte.Paint.Inks.Visual_.StrokeVisual strokeVisual = ((!_shouldUseCustomStrokeVisualCreator) ? new Cvte.Paint.Inks.Visual_.StrokeVisual(strokeInfo.FillSolidColorBrush, strokeInfo.DrawingAttributes, stylusPointCollection) : CreateStrokeVisual(strokeInfo, stylusPointCollection));
						inkRenderInfo.ContainerVisual.Children.Add(strokeVisual);
						inkRenderInfo.StrokeVisualList.AddLast(strokeVisual);
						strokeVisual.Redraw();
					}
				}
				inkRenderInfo.StrokeTipBuilder.Redraw();
			}

			public void CleanStrokeTipBuilder(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				RenderPackets(stylusPointCollectionInfo);
			}

			protected virtual Cvte.Paint.Inks.Visual_.StrokeVisual CreateStrokeVisual(StrokeInfo strokeInfo, StylusPointCollection stylusPointCollection)
			{
				return new Cvte.Paint.Inks.Visual_.StrokeVisual(strokeInfo.FillSolidColorBrush, strokeInfo.DrawingAttributes, stylusPointCollection);
			}
		}

		public class InkRenderInfo
		{
			private InkStrokeTipBuilder _strokeTipBuilder;

			[NotNull]
			public ContainerVisual ContainerVisual { get; }

			[NotNull]
			public StrokeVisualList StrokeVisualList { get; } = new StrokeVisualList();

			[CanBeNull]
			public InkStrokeTipBuilder StrokeTipBuilder
			{
				get
				{
					return _strokeTipBuilder;
				}
				set
				{
					_strokeTipBuilder = value;
				}
			}

			[NotNull]
			public IInkStrokeTipBuilderBuilder InkStrokeTipBuilderBuilder { get; }

			[NotNull]
			public Dispatcher Dispatcher { get; set; }

			public InkRenderInfo([NotNull] IInkStrokeTipBuilderBuilder inkStrokeTipBuilderBuilder, [NotNull] ContainerVisual containerVisual)
			{
				if (inkStrokeTipBuilderBuilder == null)
				{
					throw new ArgumentNullException("inkStrokeTipBuilderBuilder");
				}
				if (containerVisual == null)
				{
					throw new ArgumentNullException("containerVisual");
				}
				InkStrokeTipBuilderBuilder = inkStrokeTipBuilderBuilder;
				ContainerVisual = containerVisual;
				Dispatcher = containerVisual.Dispatcher;
			}
		}

		public abstract class InkStrokeTipBuilder : IInkStrokeTipBuilder
		{
			public const int RearPointCount = 8;

			public abstract StylusPointCollection StrokeTipCollection { get; }

			protected StrokeTipVisual BuildingStroke { get; set; }

			public DrawingAttributes DrawingAttributes { get; set; }

			public SolidColorBrush Brush { get; set; }

			public abstract StylusPointCollection AddPoint(StylusPoint stylusPoint);

			public virtual DrawingVisual CreateStroke()
			{
				BuildingStroke = new StrokeTipVisual(Brush, DrawingAttributes);
				return BuildingStroke;
			}

			public virtual void Redraw()
			{
				BuildingStroke.Redraw(StrokeTipCollection);
			}

			[Obsolete("此方法毫无作用，请不要再次使用")]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public DrawingVisual GetBuildingStroke()
			{
				return BuildingStroke;
			}
		}

		public class StrokeInfo
		{
			public bool SeenUp { get; set; }

			public bool LostCapture { get; set; }

			public int StylusDeviceId { get; }

			[NotNull]
			public DrawingAttributes DrawingAttributes { get; }

			[NotNull]
			public SolidColorBrush FillSolidColorBrush { get; }

			public int StartTime { get; }

			public int LastTime { get; set; }

			internal StrokeNodeIterator StrokeNodeIterator { get; set; }

			[NotNull]
			public InkRenderInfo InkRenderInfo { get; }

			public StrokeInfo(int stylusDeviceId, [NotNull] DrawingAttributes drawingAttributes, [NotNull] SolidColorBrush fillSolidColorBrush, int startTime, [NotNull] InkRenderInfo inkRenderInfo)
			{
				if ((object)drawingAttributes == null)
				{
					throw new ArgumentNullException("drawingAttributes");
				}
				if (fillSolidColorBrush == null)
				{
					throw new ArgumentNullException("fillSolidColorBrush");
				}
				if (inkRenderInfo == null)
				{
					throw new ArgumentNullException("inkRenderInfo");
				}
				StylusDeviceId = stylusDeviceId;
				DrawingAttributes = drawingAttributes;
				FillSolidColorBrush = fillSolidColorBrush;
				StartTime = startTime;
				LastTime = startTime;
				StrokeNodeIterator = new StrokeNodeIterator(drawingAttributes);
				InkRenderInfo = inkRenderInfo;
			}

			public bool IsTimestampAfter(int timestamp)
			{
				if (!SeenUp)
				{
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
				return false;
			}
		}

		public class StylusPointCollectionInfo
		{
			[NotNull]
			public StrokeInfo StrokeInfo { get; }

			[NotNull]
			public StylusPointCollection StylusPlugInCollection { get; }

			public int Timestamp { get; }

			public StylusPointCollectionInfo([NotNull] StrokeInfo strokeInfo, [NotNull] StylusPointCollection stylusPoints, int timestamp)
			{
				if (strokeInfo == null)
				{
					throw new ArgumentNullException("strokeInfo");
				}
				if (stylusPoints == null)
				{
					throw new ArgumentNullException("stylusPoints");
				}
				StrokeInfo = strokeInfo;
				StylusPlugInCollection = stylusPoints;
				Timestamp = timestamp;
			}
		}

		[Obsolete("此接口毫无作用，请不要再次使用")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public interface IInkStrokeTipBuilder
		{
			[Obsolete("此方法毫无作用，请不要再次使用")]
			[EditorBrowsable(EditorBrowsableState.Never)]
			DrawingVisual GetBuildingStroke();
		}

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass74_0
		{
			public StringBuilder log;
		}

		private static readonly SolidColorBrush DefaultFillSolidColorBrush;

		public const int MouseId = -1;

		private bool _isInitializing;

		private DrawingAttributes _drawingAttributes;

		private SolidColorBrush _fillSolidColorBrush = DefaultFillSolidColorBrush;

		private InkBoardSettings _inkBoardSettings;

		private IInkSynchronizer _inkSynchronizer;

		private readonly Dictionary<int, StrokeInfo> _strokeInfoList = new Dictionary<int, StrokeInfo>(50);

		private InkRenderer _render;

		private const int MaxDynamicRenderer = 50;

		public bool IsStarted { get; private set; }

		protected InkRenderer Render
		{
			get
			{
				return _render;
			}
			set
			{
				if (!_isInitializing && DebuggingProperties.IsDebug)
				{
					throw new InvalidOperationException("禁止在初始化调用之前赋值，请在 OnInit 方法里面赋值。仅调试下输出，请找德熙");
				}
				_render = value;
			}
		}

		public bool AllowsMouseInput { get; set; } = true;

		public InkCanvas InkCanvas { get; set; }

		private bool IsInitialized => IsStarted;

		[CanBeNull]
		private StrokeInfo MouseStrokeInfo { get; set; }

		IInkSynchronizer IInkDryCanvas.InkSynchronizer
		{
			get
			{
				return _inkSynchronizer;
			}
			set
			{
				_inkSynchronizer = value;
			}
		}

		protected DrawingAttributes DrawingAttributes
		{
			get
			{
				return _drawingAttributes;
			}
			set
			{
				_drawingAttributes = value;
			}
		}

		protected InkBoardSettings InkBoardSettings => _inkBoardSettings;

		internal event EventHandler StrokesCollected;

		static DynamicRenderer()
		{
			DefaultFillSolidColorBrush = Brushes.Black;
		}

		public DynamicRenderer(InkCanvas inkCanvas)
		{
			InkCanvas = inkCanvas;
			if (_inkBoardSettings == null)
			{
				InkBoardSettings inkBoardSettings = new InkBoardSettings
				{
					InkColor = DefaultFillSolidColorBrush.Color,
					InkThickness = 3.0
				};
				UpdateInkBoardSettingsInternal(inkBoardSettings);
			}
		}

		public virtual void Start()
		{
		}

		public virtual void Stop()
		{
			Rest();
		}

		public void Init()
		{
			_isInitializing = true;
			Render = new InkRenderer();
			OnInit();
			IsStarted = true;
		}

		public void Rest()
		{
			_strokeInfoList.Clear();
			InkCanvas.RunInTouchDispatcher((ContainerVisual visual) =>
			{
				visual.Children.Clear();
			});
			InkCanvas.RunInMouseDispatcher((ContainerVisual visual) =>
			{
				visual.Children.Clear();
			});
		}

		public InkBoardSettings CopyInkBoardSettings()
		{
			if (!IsStarted)
			{
				throw new InvalidOperationException("在启动完成之前禁止修改设置，请调用 Start 方法启动");
			}
			InkBoardSettings inkBoardSettings = new InkBoardSettings();
			inkBoardSettings.SetBoardSettingsFrom(inkBoardSettings);
			return inkBoardSettings;
		}

		public virtual void UpdateInkBoardSettings(InkBoardSettings inkBoardSettings)
		{
			UpdateInkBoardSettingsInternal(inkBoardSettings);
		}

		public void Down(int stylusDeviceId, int timestamp, StylusPointCollection stylusPointCollection)
		{
			if (IsMouse(stylusDeviceId))
			{
				MouseDown(timestamp, stylusPointCollection);
				return;
			}
			StrokeInfo value;
			if (_strokeInfoList.TryGetValue(stylusDeviceId, out value))
			{
				bool seenUp = value.SeenUp;
				return;
			}
			value = new StrokeInfo(stylusDeviceId, _drawingAttributes, _fillSolidColorBrush, timestamp, new InkRenderInfo(this, GetContainerVisual(stylusDeviceId)));
			_strokeInfoList[stylusDeviceId] = value;
			StylusPointCollectionInfo stylusPointCollectionInfo = new StylusPointCollectionInfo(value, stylusPointCollection, timestamp);
			Run(value, () =>
			{
				OnDown(stylusPointCollectionInfo);
			});
		}

		public void Move(int stylusDeviceId, int timestamp, StylusPointCollection stylusPointCollection)
		{
			StrokeInfo value;
			if (IsMouse(stylusDeviceId))
			{
				MouseMove(timestamp, stylusPointCollection);
			}
			else if (_strokeInfoList.TryGetValue(stylusDeviceId, out value) && value != null && value.IsTimestampAfter(timestamp))
			{
				StylusPointCollectionInfo stylusPointCollectionInfo = new StylusPointCollectionInfo(value, stylusPointCollection, timestamp);
				Run(value, () =>
				{
					OnMove(stylusPointCollectionInfo);
				});
				value.LastTime = timestamp;
			}
		}

		public void Up(int stylusDeviceId, int timestamp, StylusPointCollection stylusPointCollection)
		{
			StrokeInfo strokeInfo;
			if (IsMouse(stylusDeviceId))
			{
				MouseUp(timestamp, stylusPointCollection);
			}
			else if (_strokeInfoList.TryGetValue(stylusDeviceId, out strokeInfo))
			{
				if (strokeInfo == null || strokeInfo.SeenUp)
				{
					return;
				}
				strokeInfo.SeenUp = true;
				strokeInfo.LastTime = timestamp;
				StylusPointCollectionInfo stylusPointCollectionInfo = new StylusPointCollectionInfo(strokeInfo, stylusPointCollection, timestamp);
				Run(strokeInfo, () =>
				{
					try
					{
						OnUp(stylusPointCollectionInfo);
					}
					catch (Exception)
					{
						if (Debugger.IsAttached)
						{
							Debugger.Break();
						}
					}
					CollectStrokes(strokeInfo);
				});
				_strokeInfoList.Remove(stylusDeviceId);
			}
			else
			{
				IsMouse(stylusDeviceId);
			}
		}

		private void MouseDown(int timestamp, StylusPointCollection stylusPointCollection)
		{
			if (MouseStrokeInfo != null)
			{
				OnUp(new StylusPointCollectionInfo(MouseStrokeInfo, new StylusPointCollection(), timestamp));
			}
			stylusPointCollection = CleanStylusPointDescription(stylusPointCollection);
			StrokeInfo strokeInfo = new StrokeInfo(-1, _drawingAttributes, _fillSolidColorBrush, timestamp, new InkRenderInfo(this, GetContainerVisual(0)));
			StylusPointCollectionInfo stylusPointCollectionInfo = new StylusPointCollectionInfo(strokeInfo, stylusPointCollection, timestamp);
			OnDown(stylusPointCollectionInfo);
			MouseStrokeInfo = strokeInfo;
		}

		private void MouseMove(int timestamp, StylusPointCollection stylusPointCollection)
		{
			StrokeInfo mouseStrokeInfo = MouseStrokeInfo;
			if (mouseStrokeInfo != null)
			{
				stylusPointCollection = CleanStylusPointDescription(stylusPointCollection);
				StylusPointCollectionInfo stylusPointCollectionInfo = new StylusPointCollectionInfo(mouseStrokeInfo, stylusPointCollection, timestamp);
				OnMove(stylusPointCollectionInfo);
			}
		}

		private void MouseUp(int timestamp, StylusPointCollection stylusPointCollection)
		{
			StrokeInfo mouseStrokeInfo = MouseStrokeInfo;
			if (mouseStrokeInfo != null)
			{
				stylusPointCollection = CleanStylusPointDescription(stylusPointCollection);
				StylusPointCollectionInfo stylusPointCollectionInfo = new StylusPointCollectionInfo(mouseStrokeInfo, stylusPointCollection, timestamp);
				OnUp(stylusPointCollectionInfo);
				MouseStrokeInfo = null;
				CollectStrokes(mouseStrokeInfo);
			}
		}

		private StylusPointCollection CleanStylusPointDescription(StylusPointCollection stylusPointCollection)
		{
			StylusPointCollection stylusPointCollection2 = new StylusPointCollection(stylusPointCollection.Count);
			foreach (StylusPoint item2 in stylusPointCollection)
			{
				StylusPoint item = new StylusPoint(item2.X, item2.Y, item2.PressureFactor);
				stylusPointCollection2.Add(item);
			}
			return stylusPointCollection2;
		}

		public virtual InkStrokeTipBuilder GetInkStrokeTipBuilder()
		{
			return new HardPenInkStrokeTipBuilder();
		}

		private void CollectStrokes(StrokeInfo strokeInfo)
		{
			_inkSynchronizer.AddRawStroke(strokeInfo);
			OnStrokesCollected();
		}

		private void UpdateInkBoardSettingsInternal(InkBoardSettings inkBoardSettings)
		{
			if (_inkBoardSettings == null)
			{
				_inkBoardSettings = new InkBoardSettings();
			}
			_inkBoardSettings.InkColor = inkBoardSettings.InkColor;
			_inkBoardSettings.InkThickness = inkBoardSettings.InkThickness * inkBoardSettings.InkScale;
			_drawingAttributes = UpdateDrawingAttributes(_inkBoardSettings);
			_fillSolidColorBrush = UpdateInkColor(inkBoardSettings);
		}

		protected virtual DrawingAttributes UpdateDrawingAttributes(InkBoardSettings inkBoardSettings)
		{
			return new DrawingAttributes
			{
				FitToCurve = false,
				IgnorePressure = false,
				StylusTip = StylusTip.Ellipse,
				Color = inkBoardSettings.InkColor,
				Width = inkBoardSettings.InkThickness,
				Height = inkBoardSettings.InkThickness
			};
		}

		protected virtual SolidColorBrush UpdateInkColor(InkBoardSettings inkBoardSettings)
		{
			SolidColorBrush solidColorBrush = new SolidColorBrush(inkBoardSettings.InkColor);
			solidColorBrush.Freeze();
			return solidColorBrush;
		}

		private void UseStrokeToDraw(StylusPointCollectionInfo stylusPointCollectionInfo, ContainerVisual visual)
		{
			Stroke stroke = new Stroke(stylusPointCollectionInfo.StylusPlugInCollection, _drawingAttributes);
			DrawingVisual drawingVisual = new DrawingVisual();
			DrawingContext drawingContext = drawingVisual.RenderOpen();
			using (drawingContext)
			{
				stroke.Draw(drawingContext);
			}
			visual.Children.Add(drawingVisual);
		}

		protected virtual void OnInit()
		{
		}

		protected override void OnStylusDown(RawStylusInput rawStylusInput)
		{
			if (rawStylusInput.StylusDeviceId != 0 || AllowsMouseInput)
			{
				Down(rawStylusInput.StylusDeviceId, rawStylusInput.Timestamp, rawStylusInput.GetStylusPoints());
				rawStylusInput.NotifyWhenProcessed(rawStylusInput.StylusDeviceId);
				base.OnStylusDown(rawStylusInput);
			}
		}

		protected override void OnStylusMove(RawStylusInput rawStylusInput)
		{
			if (rawStylusInput.StylusDeviceId != 0 || AllowsMouseInput)
			{
				Move(rawStylusInput.StylusDeviceId, rawStylusInput.Timestamp, rawStylusInput.GetStylusPoints());
				base.OnStylusMove(rawStylusInput);
			}
		}

		protected override void OnStylusUp(RawStylusInput rawStylusInput)
		{
			if (rawStylusInput.StylusDeviceId != 0 || AllowsMouseInput)
			{
				Up(rawStylusInput.StylusDeviceId, rawStylusInput.Timestamp, rawStylusInput.GetStylusPoints());
				rawStylusInput.NotifyWhenProcessed(rawStylusInput.StylusDeviceId);
				base.OnStylusUp(rawStylusInput);
			}
		}

		protected virtual void OnDown(StylusPointCollectionInfo stylusPointCollectionInfo)
		{
			Render.InitStrokeTipBuilder(stylusPointCollectionInfo);
		}

		protected virtual void OnMove(StylusPointCollectionInfo stylusPointCollectionInfo)
		{
			Render.RenderPackets(stylusPointCollectionInfo);
		}

		protected virtual void OnUp(StylusPointCollectionInfo stylusPointCollectionInfo)
		{
			Render.RenderPackets(stylusPointCollectionInfo);
		}

		protected virtual ContainerVisual GetContainerVisual(int stylusDeviceId)
		{
			if (IsMouse(stylusDeviceId))
			{
				return InkCanvas.InkFromMouseContainerVisual;
			}
			return InkCanvas.InkFromTouchContainerVisual;
		}

		protected override void OnStylusDownProcessed(object callbackData, bool targetVerified)
		{
			if (!targetVerified && callbackData is int)
			{
				int id = (int)callbackData;
				if (IsMouse(id))
				{
					return;
				}
				InkCanvas.RunInTouchDispatcher(() =>
				{
					StrokeInfo value;
					if (_strokeInfoList.TryGetValue(id, out value))
					{
						_strokeInfoList.Remove(id);
						foreach (DrawingVisual item in InkRenderVisual.GetInkRenderDryingDrawVisual(value.InkRenderInfo))
						{
							value.InkRenderInfo.ContainerVisual.Children.Remove(item);
						}
					}
				});
			}
			base.OnStylusDownProcessed(callbackData, targetVerified);
		}

		protected override void OnStylusMoveProcessed(object callbackData, bool targetVerified)
		{
			base.OnStylusMoveProcessed(callbackData, targetVerified);
		}

		protected override void OnStylusUpProcessed(object callbackData, bool targetVerified)
		{
			base.OnStylusUpProcessed(callbackData, targetVerified);
		}

		protected override void OnStylusLeave(RawStylusInput rawStylusInput, bool confirmed)
		{
			if (_strokeInfoList.Count > 0)
			{
				List<KeyValuePair<int, StrokeInfo>> list = null;
				try
				{
					list = _strokeInfoList.ToList();
					_strokeInfoList.Clear();
				}
				catch (InvalidOperationException)
				{
					return;
				}
				_003C_003Ec__DisplayClass74_0 obj = default(_003C_003Ec__DisplayClass74_0);
				obj.log = new StringBuilder();
				foreach (KeyValuePair<int, StrokeInfo> item in list)
				{
					StrokeInfo value = item.Value;
					if (value == null)
					{
						if (DebuggingProperties.IsDebug)
						{
							throw new ArgumentException("strokeInfo 为空请将德熙叫过来");
						}
						continue;
					}
					value.LostCapture = true;
					InkRenderInfo inkRenderInfo = value.InkRenderInfo;
					if (inkRenderInfo == null)
					{
						_003COnStylusLeave_003Eg__Trace_007C74_0("inkRender 为空", ref obj);
					}
					else if (inkRenderInfo.StrokeVisualList.Count > 0)
					{
						_003COnStylusLeave_003Eg__AddRawStroke_007C74_1(value, ref obj);
					}
					else
					{
						_003COnStylusLeave_003Eg__CleanStrokeDrawingVisual_007C74_2(inkRenderInfo, ref obj);
					}
				}
				InkCanvas?.RunInTouchDispatcher(OnStrokesCollected);
				if (obj.log.Length > 0)
				{
					InkTracer.LogTracer.Error(obj.log.ToString());
					if (DebuggingProperties.IsDebug)
					{
						throw new ArgumentNullException(obj.log.ToString());
					}
				}
			}
			base.OnStylusLeave(rawStylusInput, confirmed);
		}

		protected void OnStrokesCollected()
		{
			StrokesCollected?.Invoke(this, EventArgs.Empty);
		}

		[Conditional("DEBUG")]
		private void Trace(string str)
		{
		}

		private static bool IsMouse(int id)
		{
			if (id != -1)
			{
				return id == 0;
			}
			return true;
		}

		private void Run(StrokeInfo strokeInfo, Action action)
		{
			strokeInfo.InkRenderInfo.Dispatcher.InvokeAsync(() =>
			{
				try
				{
					action();
				}
				catch (Exception)
				{
					if (Debugger.IsAttached)
					{
						Debugger.Break();
					}
				}
			}, DispatcherPriority.Send);
		}

		[CompilerGenerated]
		private static void _003COnStylusLeave_003Eg__Trace_007C74_0(string str, ref _003C_003Ec__DisplayClass74_0 P_1)
		{
			P_1.log.Append("OnStylusLeave " + str + "\r\n");
		}

		[CompilerGenerated]
		private void _003COnStylusLeave_003Eg__AddRawStroke_007C74_1(StrokeInfo strokeInfo, ref _003C_003Ec__DisplayClass74_0 P_1)
		{
			IInkSynchronizer inkSynchronizer = _inkSynchronizer;
			if (inkSynchronizer == null)
			{
				_003COnStylusLeave_003Eg__Trace_007C74_0("_inkSynchronizer 为空", ref P_1);
			}
			else
			{
				inkSynchronizer.AddRawStroke(strokeInfo);
			}
		}

		[CompilerGenerated]
		private void _003COnStylusLeave_003Eg__CleanStrokeDrawingVisual_007C74_2(InkRenderInfo inkRender, ref _003C_003Ec__DisplayClass74_0 P_1)
		{
			DryingDrawingVisual dryingDrawingVisual = new DryingDrawingVisual(inkRender.ContainerVisual, InkRenderVisual.GetInkRenderDryingDrawVisual(inkRender));
			InkCanvas inkCanvas = InkCanvas;
			if (inkCanvas == null)
			{
				_003COnStylusLeave_003Eg__Trace_007C74_0("inkCanvas 为空", ref P_1);
			}
			else
			{
				inkCanvas.AddDryingDrawingVisualList(dryingDrawingVisual);
			}
		}
	}
}
