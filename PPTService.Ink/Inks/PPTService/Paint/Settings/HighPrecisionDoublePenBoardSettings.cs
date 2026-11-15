using System;
using System.Windows.Input;
using PPTService.Paint.Framework.Settings;
using PPTService.Paint.Inks.HighPrecision;

namespace PPTService.Paint.Settings
{
	public class HighPrecisionDoublePenBoardSettings : BoardSettings
	{
		private InkBoardSettings _thickPenSetting = new InkBoardSettings
		{
			InkColor = InkBoardSettings.DefaultRedColor,
			InkThickness = 6.0
		};

		public InkBoardSettings ThickPenSetting
		{
			get
			{
				return _thickPenSetting;
			}
			set
			{
				if (_thickPenSetting != value)
				{
					_thickPenSetting = value;
					OnPropertyChanged("ThickPenSetting");
				}
			}
		}

		public Func<StylusPoint, HighPrecisionDoublePenInputType> GetDoublePenInputTypeByStylusPointFunc { get; set; } = HighPrecisionInputHelper.GetDoublePenInputTypeByStylusPointForFlagFrogTouchFrame;
	}
}
