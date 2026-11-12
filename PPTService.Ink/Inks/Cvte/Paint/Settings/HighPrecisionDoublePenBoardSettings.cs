using System;
using System.Windows.Input;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks.HighPrecision;

namespace Cvte.Paint.Settings
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
