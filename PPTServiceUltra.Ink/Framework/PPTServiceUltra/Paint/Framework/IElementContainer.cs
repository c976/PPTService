using System.Collections.Generic;
using PPTServiceUltra.Paint.Framework.Level;

namespace PPTServiceUltra.Paint.Framework
{
	public interface IElementContainer
	{
		IList<ILevelElement> GetLevelElementList();
	}
}
