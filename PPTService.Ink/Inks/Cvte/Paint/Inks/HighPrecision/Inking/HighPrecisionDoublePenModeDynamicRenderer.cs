using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks.HighPrecision.Inking
{
	internal class HighPrecisionDoublePenModeDynamicRenderer : HardPenInputFromBoardDynamicRenderer
	{
		private readonly HashSet<int> _currentPoints = new HashSet<int>();

		private readonly InkBoardSettings _inkBoardSettings;

		private readonly HighPrecisionDoublePenBoardSettings _doublePenSettings;

		private readonly bool _handleFinger;

		public HighPrecisionDoublePenModeDynamicRenderer(InkCanvas inkCanvas, Board board, bool handleFinger)
			: base(inkCanvas)
		{
			_inkBoardSettings = board.GetSettings<InkBoardSettings>();
			_doublePenSettings = board.GetSettings<HighPrecisionDoublePenBoardSettings>();
			_handleFinger = handleFinger;
		}

		protected override void OnStylusDown(RawStylusInput rawStylusInput)
		{
			StylusPoint arg = rawStylusInput.GetStylusPoints().First();
			InkBoardSettings inkBoardSettings;
			switch (_doublePenSettings.GetDoublePenInputTypeByStylusPointFunc(arg))
			{
			case HighPrecisionDoublePenInputType.ThinPen:
				inkBoardSettings = _inkBoardSettings;
				break;
			case HighPrecisionDoublePenInputType.SimulatePressure:
				inkBoardSettings = _inkBoardSettings;
				break;
			case HighPrecisionDoublePenInputType.Finger:
				if (_handleFinger)
				{
					inkBoardSettings = _inkBoardSettings;
					break;
				}
				goto default;
			case HighPrecisionDoublePenInputType.ThickPen:
				inkBoardSettings = _doublePenSettings.ThickPenSetting;
				break;
			default:
				inkBoardSettings = null;
				break;
			}
			InkBoardSettings inkBoardSettings2 = inkBoardSettings;
			if (inkBoardSettings2 != null && inkBoardSettings2.InkType == InkType.Pen)
			{
				if (InkBoardSettings.InkColor != inkBoardSettings2.InkColor || InkBoardSettings.InkThickness != inkBoardSettings2.InkThickness)
				{
					UpdateInkBoardSettings(inkBoardSettings2);
				}
				_currentPoints.Add(rawStylusInput.StylusDeviceId);
				base.OnStylusDown(rawStylusInput);
			}
		}

		protected override void OnStylusMove(RawStylusInput rawStylusInput)
		{
			if (!_currentPoints.Contains(rawStylusInput.StylusDeviceId))
			{
				return;
			}
			StylusPoint arg = rawStylusInput.GetStylusPoints().First();
			switch (_doublePenSettings.GetDoublePenInputTypeByStylusPointFunc(arg))
			{
			case HighPrecisionDoublePenInputType.Finger:
				if (!_handleFinger)
				{
					break;
				}
				goto case HighPrecisionDoublePenInputType.ThinPen;
			case HighPrecisionDoublePenInputType.ThinPen:
			case HighPrecisionDoublePenInputType.ThickPen:
			case HighPrecisionDoublePenInputType.SimulatePressure:
				base.OnStylusMove(rawStylusInput);
				break;
			}
		}

		protected override void OnStylusUp(RawStylusInput rawStylusInput)
		{
			if (!_currentPoints.Contains(rawStylusInput.StylusDeviceId))
			{
				return;
			}
			StylusPoint stylusPoint = rawStylusInput.GetStylusPoints().First();
			StylusPointDescription description = stylusPoint.Description;
			if (description == null || !description.HasProperty(StylusPointProperties.Width))
			{
				return;
			}
			StylusPointDescription description2 = stylusPoint.Description;
			if (description2 != null && description2.HasProperty(StylusPointProperties.Height))
			{
				int propertyValue = stylusPoint.GetPropertyValue(StylusPointProperties.Width);
				int propertyValue2 = stylusPoint.GetPropertyValue(StylusPointProperties.Height);
				if (propertyValue == 0 && propertyValue2 == 0)
				{
					_currentPoints.Remove(rawStylusInput.StylusDeviceId);
					base.OnStylusUp(rawStylusInput);
				}
			}
		}
	}
}
