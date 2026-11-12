using System.Collections.Generic;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.Level
{
	public interface ILevelManager : IComparer<ILevelElement>
	{
		LevelMoveResultCollection MoveUp(ILevelElement moveElement, ILevelElement targetElement);

		LevelMoveResultCollection MoveDown(ILevelElement moveElement, ILevelElement targetElement);

		LevelMoveResultCollection MoveUp(params ILevelElement[] elementList);

		LevelMoveResultCollection MoveDown(params ILevelElement[] elementList);

		LevelMoveResultCollection MoveToTop(params ILevelElement[] elementList);

		LevelMoveResultCollection MoveToBottom(params ILevelElement[] elementList);

		LevelMoveResultCollection ExchangeLevel(ILevelElement sourceElement, ILevelElement targetElement);

		[CanBeNull]
		ILevelElement GetUpLevelElement(ILevelElement targetElement);

		[CanBeNull]
		ILevelElement GetDownLevelElement(ILevelElement targetElement);

		List<ILevelElement> GetLevelSortedElements(IEnumerable<ILevelElement> elements);

		List<ILevelElement> GetLevelSortedElements();
	}
}
