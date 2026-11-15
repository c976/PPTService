using System.Collections.Generic;

namespace PPTService.Paint.Framework.Level
{
	public interface ILevelStorage : ILevelManager, IComparer<ILevelElement>
	{
		void AddElement(ILevelElement element);
	}
}
