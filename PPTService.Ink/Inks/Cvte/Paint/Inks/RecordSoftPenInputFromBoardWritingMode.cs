using System.ComponentModel;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public class RecordSoftPenInputFromBoardWritingMode : SoftPenInputFromBoardWritingMode
	{
		private class RecordSoftPenInputFromBoardDynamicRenderer : SoftPenInputFromBoardDynamicRenderer
		{
			public bool NeedCollectStrokeDetails { get; set; }

			private WritingInkRecorder WritingInkRecorder { get; }

			public RecordSoftPenInputFromBoardDynamicRenderer(InkCanvas inkCanvas, WritingInkRecorder writingInkRecorder)
				: base(inkCanvas)
			{
				WritingInkRecorder = writingInkRecorder;
			}

			protected override void OnDown(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				WritingInkRecorder.RemoveCollectStrokePointDetail(stylusPointCollectionInfo.StrokeInfo.StylusDeviceId);
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

			protected override void OnStylusDownProcessed(object callbackData, bool targetVerified)
			{
				if (!targetVerified && callbackData is int)
				{
					int strokeInfoStylusDeviceId = (int)callbackData;
					WritingInkRecorder.RemoveCollectStrokePointDetail(strokeInfoStylusDeviceId);
				}
				base.OnStylusDownProcessed(callbackData, targetVerified);
			}

			private void CollectStrokePointDetails(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				if (NeedCollectStrokeDetails)
				{
					WritingInkRecorder.CollectStrokePointDetails(stylusPointCollectionInfo.StrokeInfo.StylusDeviceId, stylusPointCollectionInfo.StylusPlugInCollection);
				}
			}
		}

		private bool _needCollectStrokeDetails;

		private RecordSoftPenInputFromBoardDynamicRenderer _recordSoftPenDynamicRenderer;

		private RecordWritingModeSettings _recordSoftPenWritingModeSettings;

		private WritingInkRecorder WritingInkRecorder { get; } = new WritingInkRecorder();

		private bool NeedCollectStrokeDetails
		{
			get
			{
				return _needCollectStrokeDetails;
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
						_recordSoftPenDynamicRenderer = new RecordSoftPenInputFromBoardDynamicRenderer(InkCanvas, WritingInkRecorder)
						{
							NeedCollectStrokeDetails = true
						};
						InkCanvas.DynamicRenderer?.Stop();
						InkCanvas.DynamicRenderer = _recordSoftPenDynamicRenderer;
					}
					_needCollectStrokeDetails = value;
					if (_recordSoftPenWritingModeSettings != null)
					{
						_recordSoftPenWritingModeSettings.NeedCollectStrokeDetails = value;
					}
				}
			}
		}

		private void RecordSoftPenWritingModeSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NeedCollectStrokeDetails = _recordSoftPenWritingModeSettings.NeedCollectStrokeDetails;
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

		protected override Ink AsInk(InkSynchronizer.StrokeSynchronizer synchronizer)
		{
			Ink ink = base.AsInk(synchronizer);
			if (NeedCollectStrokeDetails)
			{
				StrokeBase strokeBase = ink.InnerStroke as StrokeBase;
				if (strokeBase != null)
				{
					strokeBase.StrokePointDetailsList = WritingInkRecorder.PopStrokePointDetails(synchronizer.StylusDeviceId);
				}
			}
			return ink;
		}

		protected override HardPenInputFromBoardDynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			HardPenInputFromBoardDynamicRenderer hardPenInputFromBoardDynamicRenderer = _recordSoftPenDynamicRenderer;
			if (hardPenInputFromBoardDynamicRenderer == null)
			{
				if (NeedCollectStrokeDetails)
				{
					_recordSoftPenDynamicRenderer = new RecordSoftPenInputFromBoardDynamicRenderer(inkCanvas, WritingInkRecorder)
					{
						NeedCollectStrokeDetails = true
					};
					hardPenInputFromBoardDynamicRenderer = _recordSoftPenDynamicRenderer;
				}
				else
				{
					hardPenInputFromBoardDynamicRenderer = new SoftPenInputFromBoardDynamicRenderer(inkCanvas);
				}
			}
			else
			{
				_recordSoftPenDynamicRenderer.NeedCollectStrokeDetails = NeedCollectStrokeDetails;
			}
			return hardPenInputFromBoardDynamicRenderer;
		}
	}
}
