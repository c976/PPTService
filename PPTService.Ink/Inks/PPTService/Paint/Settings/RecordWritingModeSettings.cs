using PPTService.Paint.Framework.Settings;

namespace PPTService.Paint.Settings
{
	public class RecordWritingModeSettings : BoardSettings
	{
		private bool _needCollectStrokeDetails;

		public bool NeedCollectStrokeDetails
		{
			get
			{
				return _needCollectStrokeDetails;
			}
			set
			{
				if (value != _needCollectStrokeDetails)
				{
					_needCollectStrokeDetails = value;
					OnPropertyChanged("NeedCollectStrokeDetails");
				}
			}
		}
	}
}
