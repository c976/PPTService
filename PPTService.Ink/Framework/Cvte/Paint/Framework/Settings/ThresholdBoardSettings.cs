using System.Collections.Generic;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework.Settings
{
	public class ThresholdBoardSettings : BoardSettings, IUpdateBoardSettings
	{
		private double _alignmentThreshold = 5.0;

		private readonly Dictionary<string, object> _settingDictionary;

		private const string DragToleranceKey = "DragTolerance";

		private const string HitTestToleranceKey = "HitTestTolerance";

		private const string ManipulationTranslateThresholdKey = "ManipulationTranslateThreshold";

		private const string ManipulationScaleThresholdKey = "ManipulationScaleThreshold";

		private const string ManipulationRotateThresholdKey = "ManipulationRotateThreshold";

		private const string ManipulationRotateAlignThresholdKey = "ManipulationRotateAlignThreshold";

		private const string AlignThresholdKey = "AlignThreshold";

		public DeviceInputDoubleDifferentialValue DragTolerance
		{
			get
			{
				return (DeviceInputDoubleDifferentialValue)_settingDictionary["DragTolerance"];
			}
			set
			{
				_settingDictionary["DragTolerance"] = value;
			}
		}

		public DeviceInputDoubleDifferentialValue HitTestTolerance
		{
			get
			{
				return (DeviceInputDoubleDifferentialValue)_settingDictionary["HitTestTolerance"];
			}
			set
			{
				_settingDictionary["HitTestTolerance"] = value;
			}
		}

		public double ManipulationTranslateThreshold
		{
			get
			{
				return (double)_settingDictionary["ManipulationTranslateThreshold"];
			}
			set
			{
				_settingDictionary["ManipulationTranslateThreshold"] = value;
			}
		}

		public double ManipulationScaleThreshold
		{
			get
			{
				return (double)_settingDictionary["ManipulationScaleThreshold"];
			}
			set
			{
				_settingDictionary["ManipulationScaleThreshold"] = value;
			}
		}

		public double ManipulationRotateThreshold
		{
			get
			{
				return (double)_settingDictionary["ManipulationRotateThreshold"];
			}
			set
			{
				_settingDictionary["ManipulationRotateThreshold"] = value;
			}
		}

		public double ManipulationRotateAlignThreshold
		{
			get
			{
				return (double)_settingDictionary["ManipulationRotateAlignThreshold"];
			}
			set
			{
				_settingDictionary["ManipulationRotateAlignThreshold"] = value;
			}
		}

		public double AlignThreshold
		{
			get
			{
				return (double)_settingDictionary["AlignThreshold"];
			}
			set
			{
				_settingDictionary["AlignThreshold"] = value;
			}
		}

		public double AlignmentThreshold
		{
			get
			{
				return _alignmentThreshold;
			}
			set
			{
				if (!value.Equals(_alignmentThreshold))
				{
					_alignmentThreshold = value;
					OnPropertyChanged("AlignmentThreshold");
				}
			}
		}

		public ThresholdBoardSettings()
		{
			_settingDictionary = new Dictionary<string, object>
			{
				{
					"DragTolerance",
					new DeviceInputDoubleDifferentialValue(1.0, 16.0, 4.0)
				},
				{
					"HitTestTolerance",
					new DeviceInputDoubleDifferentialValue(3.0, 10.0, 5.0)
				},
				{ "ManipulationTranslateThreshold", 6.666666666666667 },
				{ "ManipulationScaleThreshold", 1.3333333333333333 },
				{ "ManipulationRotateThreshold", 360.0 },
				{ "ManipulationRotateAlignThreshold", 10.0 },
				{ "AlignThreshold", 5.0 }
			};
		}

		public void UpdateBoardSettings(BoardSettings boardSettings)
		{
			ThresholdBoardSettings thresholdBoardSettings = (ThresholdBoardSettings)boardSettings;
			AlignmentThreshold = thresholdBoardSettings.AlignmentThreshold;
		}
	}
}
