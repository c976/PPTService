using System.Collections.Generic;
using Cvte.Paint.Framework.Level;

namespace Cvte.Paint.Framework
{
	public interface IElementContainer
	{
		IList<ILevelElement> GetLevelElementList();
	}
}
