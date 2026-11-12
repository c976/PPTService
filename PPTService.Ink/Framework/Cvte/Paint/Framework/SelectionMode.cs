using System.Collections.Generic;
using System.Windows.Input;

namespace Cvte.Paint.Framework
{
	public class SelectionMode : CompositeMode, IModeKeyboardSensitive
	{
		ModifierKeys IModeKeyboardSensitive.Modifiers => ModifierKeys.Control;

		public SelectionMode()
		{
			ModeOperations = new List<IModeOperation>
			{
				new SlideFocusModeOperation(),
				new SelectionCursorModeOperation(),
				new HitTestModeOperation(new SelectionForElementModeOperation(), new SelectionForBoardModeOperation
				{
					IsSimulateEnabled = true
				})
			};
		}
	}
}
