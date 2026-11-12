using System.ComponentModel;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public class RecordSoftPenBoardWritingMode : SoftPenBoardWritingMode
	{
		private class RecordSoftPenDynamicRenderer : SoftPenBoardDynamicRenderer
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
					WritingInkRecorder.CollectStrokePointDetails(stylusPointCollectionInfo);
				}
			}
		}

		private bool _needCollectStrokeDetails;

		private RecordSoftPenDynamicRenderer _recordSoftPenDynamicRenderer;

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
						_recordSoftPenDynamicRenderer = new RecordSoftPenDynamicRenderer(InkCanvas, WritingInkRecorder)
						{
							NeedCollectStrokeDetails = true
						};
						InkCanvas.DynamicRenderer?.Stop();
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

		protected override DynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			DynamicRenderer dynamicRenderer = _recordSoftPenDynamicRenderer;
			if (dynamicRenderer == null)
			{
				if (NeedCollectStrokeDetails)
				{
					_recordSoftPenDynamicRenderer = new RecordSoftPenDynamicRenderer(inkCanvas, WritingInkRecorder)
					{
						NeedCollectStrokeDetails = true
					};
					dynamicRenderer = _recordSoftPenDynamicRenderer;
				}
				else
				{
					dynamicRenderer = new SoftPenBoardDynamicRenderer(inkCanvas);
				}
			}
			else
			{
				_recordSoftPenDynamicRenderer.NeedCollectStrokeDetails = NeedCollectStrokeDetails;
			}
			return dynamicRenderer;
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
	}
}
