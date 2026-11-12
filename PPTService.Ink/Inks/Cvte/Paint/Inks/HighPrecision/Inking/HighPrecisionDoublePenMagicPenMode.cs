using System.Linq;
using System.Windows.Input;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks.HighPrecision.Inking
{
	public class HighPrecisionDoublePenMagicPenMode : MagicPenMode
	{
		private readonly bool _handleFinger;

		private InkBoardSettings _inkBoardSetting;

		public HighPrecisionDoublePenMagicPenMode(bool handleFinger = false)
			: base(new ModeSettings
			{
				IsElementInteractionEnabled = true,
				IsTouchCursorVisible = false
			})
		{
			_handleFinger = handleFinger;
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			Board board = GetBoard();
			if (e.Id == -1)
			{
				_inkBoardSetting = board.GetSettings<InkBoardSettings>();
				return;
			}
			StylusPoint arg = e.GetStylusPoints().First();
			InkBoardSettings inkBoardSetting;
			switch (board.GetSettings<HighPrecisionDoublePenBoardSettings>().GetDoublePenInputTypeByStylusPointFunc(arg))
			{
			case HighPrecisionDoublePenInputType.ThinPen:
				inkBoardSetting = board.GetSettings<InkBoardSettings>();
				break;
			case HighPrecisionDoublePenInputType.SimulatePressure:
				inkBoardSetting = board.GetSettings<InkBoardSettings>();
				break;
			case HighPrecisionDoublePenInputType.Finger:
				if (_handleFinger)
				{
					inkBoardSetting = board.GetSettings<InkBoardSettings>();
					break;
				}
				goto default;
			case HighPrecisionDoublePenInputType.ThickPen:
				inkBoardSetting = board.GetSettings<HighPrecisionDoublePenBoardSettings>().ThickPenSetting;
				break;
			default:
				inkBoardSetting = board.GetSettings<InkBoardSettings>();
				break;
			}
			_inkBoardSetting = inkBoardSetting;
		}

		protected override DynamicInk CreateDynamicInk()
		{
			return CreateDynamicInkBySetting(_inkBoardSetting);
		}
	}
}
