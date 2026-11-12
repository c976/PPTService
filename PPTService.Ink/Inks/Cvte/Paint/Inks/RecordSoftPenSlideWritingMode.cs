using System;
using System.ComponentModel;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public class RecordSoftPenSlideWritingMode : SoftPenSlideWritingMode
	{
		private class RecordSoftPenDynamicRenderer : SoftPenSlideDynamicRenderer
		{
			public bool NeedCollectStrokeDetails { get; set; }

			private WritingInkRecorder WritingInkRecorder { get; }

			public RecordSoftPenDynamicRenderer(InkCanvas inkCanvas, WritingInkRecorder writingInkRecorder)
				: base(inkCanvas)
			{
				WritingInkRecorder = writingInkRecorder;
			}

			protected override void OnDown(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				CollectStrokePointDetails(stylusPointCollectionInfo);
				base.OnDown(stylusPointCollectionInfo);
			}

			protected override void OnMove(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				CollectStrokePointDetails(stylusPointCollectionInfo);
				base.OnMove(stylusPointCollectionInfo);
			}

			protected override void OnUp(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				CollectStrokePointDetails(stylusPointCollectionInfo);
				base.OnUp(stylusPointCollectionInfo);
			}

			private void CollectStrokePointDetails(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				if (NeedCollectStrokeDetails)
				{
					WritingInkRecorder.CollectStrokePointDetails(stylusPointCollectionInfo.StrokeInfo.StylusDeviceId, stylusPointCollectionInfo.StylusPlugInCollection);
				}
			}
		}

		private bool _needCollectStrokeDetails = true;

		private RecordSoftPenDynamicRenderer _recordSoftPenDynamicRenderer;

		private RecordWritingModeSettings _recordSoftPenWritingModeSettings;

		[Obsolete("请使用 RecordSoftPenWritingModeSettings 设置")]
		public bool NeedCollectStrokeDetails
		{
			get
			{
				return _recordSoftPenDynamicRenderer?.NeedCollectStrokeDetails ?? _needCollectStrokeDetails;
			}
			set
			{
				if (value != _needCollectStrokeDetails)
				{
					if (_recordSoftPenDynamicRenderer != null)
					{
						_recordSoftPenDynamicRenderer.NeedCollectStrokeDetails = value;
					}
					else if (value && InkCanvas != null)
					{
						_recordSoftPenDynamicRenderer = new RecordSoftPenDynamicRenderer(InkCanvas, WritingInkRecorder)
						{
							NeedCollectStrokeDetails = true
						};
						InkCanvas.DynamicRenderer = _recordSoftPenDynamicRenderer;
						UpdateInkBoardSettings();
					}
					_needCollectStrokeDetails = value;
					if (_recordSoftPenWritingModeSettings != null)
					{
						_recordSoftPenWritingModeSettings.NeedCollectStrokeDetails = value;
					}
				}
			}
		}

		private WritingInkRecorder WritingInkRecorder { get; } = new WritingInkRecorder();

		protected override DynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			SoftPenSlideDynamicRenderer softPenSlideDynamicRenderer = _recordSoftPenDynamicRenderer;
			if (softPenSlideDynamicRenderer == null)
			{
				if (NeedCollectStrokeDetails)
				{
					_recordSoftPenDynamicRenderer = new RecordSoftPenDynamicRenderer(inkCanvas, WritingInkRecorder)
					{
						NeedCollectStrokeDetails = true
					};
					softPenSlideDynamicRenderer = _recordSoftPenDynamicRenderer;
				}
				else
				{
					softPenSlideDynamicRenderer = new SoftPenSlideDynamicRenderer(inkCanvas);
				}
			}
			else
			{
				_recordSoftPenDynamicRenderer.NeedCollectStrokeDetails = _needCollectStrokeDetails;
			}
			return softPenSlideDynamicRenderer;
		}

		protected override void OnSwitchOn()
		{
			_recordSoftPenWritingModeSettings = CurrentSlide.GetSettings<RecordWritingModeSettings>();
			NeedCollectStrokeDetails = _recordSoftPenWritingModeSettings.NeedCollectStrokeDetails;
			_recordSoftPenWritingModeSettings.PropertyChanged += RecordSoftPenWritingModeSettings_PropertyChanged;
			base.OnSwitchOn();
		}

		protected override void OnSwitchOff()
		{
			_recordSoftPenWritingModeSettings.PropertyChanged -= RecordSoftPenWritingModeSettings_PropertyChanged;
			base.OnSwitchOff();
		}

		protected override void AsInk(InkSynchronizer.StrokeSynchronizer synchronizer)
		{
			HardPenStroke hardPenStroke = new HardPenStroke(synchronizer.StylusPointCollection, synchronizer.DrawingAttributes.Color, synchronizer.DrawingAttributes.Width);
			if (_recordSoftPenDynamicRenderer != null)
			{
				hardPenStroke.StrokePointDetailsList = WritingInkRecorder.PopStrokePointDetails(synchronizer.StylusDeviceId);
			}
			Ink element = new Ink(hardPenStroke);
			CurrentSlide.Add(element);
		}

		private void RecordSoftPenWritingModeSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NeedCollectStrokeDetails = _recordSoftPenWritingModeSettings.NeedCollectStrokeDetails;
		}
	}
}
