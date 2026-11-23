using System;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Paint.Framework.Level;
using PPTServiceUltra.Paint.Framework.Operations;

namespace PPTServiceUltra.Paint.Framework.ElementLevel
{
	public class ChangeLayerOperation : ElementOperation
	{
		private readonly LevelMoveResultCollection _levelMoveResult;

		public bool AreAnyLayersChanged => _levelMoveResult.AreAnyLayersChanged;

		public ChangeLayerOperation([NotNull] Slide slide, [NotNull] LevelMoveResultCollection levelMoveResult)
			: base(slide)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (levelMoveResult == null)
			{
				throw new ArgumentNullException("levelMoveResult");
			}
			_levelMoveResult = levelMoveResult;
		}

		protected override void OnRedo()
		{
			_levelMoveResult.Redo();
		}

		protected override void OnUndo()
		{
			_levelMoveResult.Undo();
		}
	}
}
