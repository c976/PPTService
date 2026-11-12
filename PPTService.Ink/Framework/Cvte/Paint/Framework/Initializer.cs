namespace Cvte.Paint.Framework
{
	public class Initializer : IPaintInitializer
	{
		void IPaintInitializer.Initialize()
		{
		}

		void IPaintInitializer.Initialize(Board board)
		{
			board.AddFilter(new ExclusiveModeFilter());
		}
	}
}
