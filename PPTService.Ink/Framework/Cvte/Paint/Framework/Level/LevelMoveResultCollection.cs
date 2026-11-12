using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.Level
{
	public class LevelMoveResultCollection
	{
		internal static readonly LevelMoveResultCollection Empty = new LevelMoveResultCollection();

		private ILevelErratic LevelErratic { get; }

		public bool AreAnyLayersChanged { get; } = true;

		private IReadOnlyList<LevelMoveResult> LevelMoveResultList { get; }

		internal LevelMoveResultCollection(IReadOnlyList<LevelMoveResult> levelMoveResultList, ILevelErratic levelErratic)
		{
			LevelMoveResultList = levelMoveResultList;
			LevelErratic = levelErratic;
		}

		internal LevelMoveResultCollection(LevelMoveResult levelMoveResult, ILevelErratic levelErratic)
		{
			LevelMoveResultList = new LevelMoveResult[1] { levelMoveResult };
			LevelErratic = levelErratic;
		}

		private LevelMoveResultCollection()
		{
			AreAnyLayersChanged = false;
		}

		public void Undo()
		{
			if (!AreAnyLayersChanged)
			{
				return;
			}
			foreach (LevelMoveResult levelMoveResult in LevelMoveResultList)
			{
				levelMoveResult.Undo(LevelErratic);
			}
			LevelErratic.EndMove();
		}

		public void Redo()
		{
			if (!AreAnyLayersChanged)
			{
				return;
			}
			foreach (LevelMoveResult levelMoveResult in LevelMoveResultList)
			{
				levelMoveResult.Redo(LevelErratic);
			}
			LevelErratic.EndMove();
		}

		public static LevelMoveResultCollection Combine([CanBeNull] LevelMoveResultCollection a, [CanBeNull] LevelMoveResultCollection b)
		{
			if (a == null && b == null)
			{
				if (DebuggingProperties.IsDebug)
				{
					throw new ArgumentNullException("不能合并两个空的元素");
				}
				return null;
			}
			if (a != null && b == null)
			{
				return a;
			}
			if (a == null && b != null)
			{
				return b;
			}
			if (a.LevelErratic != b.LevelErratic)
			{
				throw new ArgumentException("不能合并两个不同容器");
			}
			List<LevelMoveResult> list = a.LevelMoveResultList.ToList();
			list.AddRange(b.LevelMoveResultList);
			return new LevelMoveResultCollection(list, a.LevelErratic);
		}
	}
}
