using System.Collections.Generic;

namespace Cvte.Paint.Framework.Level
{
	public interface ILevelStorage : ILevelManager, IComparer<ILevelElement>
	{
		void AddElement(ILevelElement element);
	}
}
