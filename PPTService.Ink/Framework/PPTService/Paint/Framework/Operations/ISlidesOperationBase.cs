using System.Collections.Generic;
using PPTService.Ink.Core.Documents;

namespace PPTService.Paint.Framework.Operations
{
	internal interface ISlidesOperationBase : IBoardOperationBase, IOperation
	{
		IReadOnlyList<string> SlideIdList { get; }
	}
}
