using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Inks.Visual_;

namespace Cvte.Paint.Inks
{
	public class InkSynchronizer : IInkSynchronizer
	{
		public class StrokeSynchronizer
		{
			public StylusPointCollection StylusPointCollection { get; }

			public int StylusDeviceId { get; }

			public DrawingAttributes DrawingAttributes { get; }

			public bool LostCapture { get; set; }

			internal StrokeSynchronizer(StylusPointCollection stylusPointCollection, int stylusDeviceId, DrawingAttributes drawingAttributes)
			{
				StylusPointCollection = stylusPointCollection;
				StylusDeviceId = stylusDeviceId;
				DrawingAttributes = drawingAttributes;
			}
		}

		private readonly object _lockObject = new object();

		private readonly List<DynamicRenderer.StrokeInfo> _strokeInfoList = new List<DynamicRenderer.StrokeInfo>();

		private readonly List<DrawingVisual> _dryingDrawingVisualList = new List<DrawingVisual>();

		private readonly IInkDryCanvas _dynamicRenderer;

		internal InkSynchronizer(IInkDryCanvas dynamicRenderer)
		{
			_dynamicRenderer = dynamicRenderer;
			dynamicRenderer.InkSynchronizer = this;
		}

		void IInkSynchronizer.AddRawStroke([NotNull] DynamicRenderer.StrokeInfo strokeInfo)
		{
			if (strokeInfo == null)
			{
				throw new ArgumentNullException("strokeInfo");
			}
			lock (_lockObject)
			{
				_strokeInfoList.Add(strokeInfo);
			}
		}

		public List<StrokeSynchronizer> BeginDry()
		{
			List<DynamicRenderer.StrokeInfo> list;
			lock (_lockObject)
			{
				list = _strokeInfoList.ToList();
				_strokeInfoList.Clear();
			}
			List<StrokeSynchronizer> list2 = new List<StrokeSynchronizer>();
			foreach (DynamicRenderer.StrokeInfo item in list)
			{
				if (item != null)
				{
					DynamicRenderer.InkRenderInfo inkRenderInfo = item.InkRenderInfo;
					StylusPointCollection inkRenderInfoStylusPointList = GetInkRenderInfoStylusPointList(inkRenderInfo);
					if (inkRenderInfoStylusPointList != null && inkRenderInfoStylusPointList.Any())
					{
						list2.Add(new StrokeSynchronizer(inkRenderInfoStylusPointList, item.StylusDeviceId, item.DrawingAttributes)
						{
							LostCapture = item.LostCapture
						});
					}
					DryingDrawingVisual dryingDrawingVisual = new DryingDrawingVisual(inkRenderInfo.ContainerVisual, InkRenderVisual.GetInkRenderDryingDrawVisual(inkRenderInfo));
					_dynamicRenderer.InkCanvas.AddDryingDrawingVisualList(dryingDrawingVisual);
				}
			}
			return list2;
		}

		public void EndDry()
		{
			_dynamicRenderer.InkCanvas.NotifyOnNextRenderComplete();
		}

		internal void Clean()
		{
			lock (_lockObject)
			{
				_strokeInfoList.Clear();
			}
		}

		private StylusPointCollection GetInkRenderInfoStylusPointList(DynamicRenderer.InkRenderInfo inkRender)
		{
			StylusPointCollection stylusPointCollection = GetStylusPointsFromStrokeVisualList(inkRender.StrokeVisualList);
			DynamicRenderer.InkStrokeTipBuilder strokeTipBuilder = inkRender.StrokeTipBuilder;
			if (strokeTipBuilder != null)
			{
				StylusPointCollection strokeTipCollection = strokeTipBuilder.StrokeTipCollection;
				if (strokeTipCollection != null && strokeTipCollection.Any())
				{
					if (stylusPointCollection != null)
					{
						stylusPointCollection.Add(strokeTipCollection);
					}
					else
					{
						try
						{
							stylusPointCollection = CloneStylusPointCollection(strokeTipCollection);
						}
						catch (Exception)
						{
						}
					}
				}
			}
			return stylusPointCollection;
		}

		private StylusPointCollection GetStylusPointsFromStrokeVisualList(LinkedList<Cvte.Paint.Inks.Visual_.StrokeVisual> strokeVisualList)
		{
			StylusPointCollection stylusPointCollection = null;
			foreach (Cvte.Paint.Inks.Visual_.StrokeVisual strokeVisual in strokeVisualList)
			{
				StylusPointCollection stylusPoints = strokeVisual.Stroke.StylusPoints;
				if (stylusPointCollection == null)
				{
					stylusPointCollection = new StylusPointCollection(stylusPoints.Description);
				}
				stylusPointCollection.Add(stylusPoints);
			}
			return stylusPointCollection;
		}

		private StylusPointCollection CloneStylusPointCollection(StylusPointCollection spc)
		{
			StylusPointCollection stylusPointCollection = new StylusPointCollection(spc.Description, spc.Count);
			foreach (StylusPoint item in spc)
			{
				stylusPointCollection.Add(item);
			}
			return stylusPointCollection;
		}
	}
}
