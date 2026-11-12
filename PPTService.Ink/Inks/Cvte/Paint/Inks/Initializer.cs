using Cvte.Paint.Framework;
using Cvte.Paint.Inks.Hardware.SmartPen;

namespace Cvte.Paint.Inks
{
	public class Initializer : IPaintInitializer
	{
		void IPaintInitializer.Initialize()
		{
		}

		void IPaintInitializer.Initialize(Board board)
		{
			board.AddFilter(new EraserGestureFilter());
			board.AddFilter(new SmartPenSelectionButtonFilter());
			board.AddFilter(new SmartPenPressureFilter());
		}
	}
}
