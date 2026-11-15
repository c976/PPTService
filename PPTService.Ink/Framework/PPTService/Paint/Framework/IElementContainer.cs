using System.Collections.Generic;
using PPTService.Paint.Framework.Level;

namespace PPTService.Paint.Framework
{
	public interface IElementContainer
	{
		IList<ILevelElement> GetLevelElementList();
	}
}
