namespace Cvte.Paint.Framework.Level
{
	internal class LevelMoveResult
	{
		private string Element { get; }

		private LevelInfo NewLevel { get; }

		private LevelInfo OldLevel { get; }

		internal LevelMoveResult(ILevelElement element, LevelInfo newLevel, LevelInfo oldLevel)
		{
			Element = element.Id;
			NewLevel = newLevel;
			OldLevel = oldLevel;
		}

		public void Undo(ILevelErratic LevelErratic)
		{
			LevelErratic.MoveTo(Element, OldLevel);
		}

		public void Redo(ILevelErratic LevelErratic)
		{
			LevelErratic.MoveTo(Element, NewLevel);
		}
	}
}
