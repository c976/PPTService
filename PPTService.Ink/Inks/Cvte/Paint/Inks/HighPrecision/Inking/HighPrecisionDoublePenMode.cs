using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks.HighPrecision.Inking
{
	public class HighPrecisionDoublePenMode : HardPenInputFromBoardWritingMode
	{
		private readonly bool _handleFinger;

		public HighPrecisionDoublePenMode(bool handleFinger = false)
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
			if (e.Id == -1)
			{
				InkBoardSettings settings = GetBoard().GetSettings<InkBoardSettings>();
				InkCanvas.DynamicRenderer.UpdateInkBoardSettings(settings);
			}
			base.OnDeviceDown(e);
		}

		protected override HardPenInputFromBoardDynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return new HighPrecisionDoublePenModeDynamicRenderer(inkCanvas, GetBoard(), _handleFinger);
		}

		protected override void UpdateInkBoardSettings()
		{
			Board board = GetBoard();
			board.GetSettings<HighPrecisionDoublePenBoardSettings>().ThickPenSetting.InkScale = 1.0 / board.ContentScale;
			base.UpdateInkBoardSettings();
		}
	}
}
