using System;
using System.Collections.Generic;
using PPTService.Paint.Framework;

namespace PPTService.Paint.Inks.HighPrecision.Erasing
{
	public class HighPrecisionEraserMode : CompositeMode, IEraserMode
	{
		private readonly HighPrecisionEraserModeOperation _modeOperation;

		public event EventHandler<ErasingCompletedEventArgs> ErasingCompleted
		{
			add
			{
				_modeOperation.ErasingCompleted += value;
			}
			remove
			{
				_modeOperation.ErasingCompleted -= value;
			}
		}

		public HighPrecisionEraserMode(HighPrecisionEraserModeOperation modeOperation = null, bool isElementInteractionEnabled = false)
			: base(new ModeSettings
			{
				IsElementInteractionEnabled = isElementInteractionEnabled
			})
		{
			_modeOperation = modeOperation ?? new HighPrecisionEraserModeOperation();
			ModeOperations = new List<IModeOperation> { _modeOperation };
		}
	}
}
