using PPTService.Paint.Framework;

namespace PPTService.Paint.Inks
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
