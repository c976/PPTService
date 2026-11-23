using System.Collections.Generic;

namespace PPTServiceUltra.Paint.Framework.Level
{
	public interface ILevelStorage : ILevelManager, IComparer<ILevelElement>
	{
		void AddElement(ILevelElement element);
	}
}
