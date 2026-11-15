using System.Collections.Generic;
using PPTService.Ink.Core.Documents;

namespace Cvte.Paint.Framework.Operations
{
	internal interface ISlidesOperationBase : IBoardOperationBase, IOperation
	{
		IReadOnlyList<string> SlideIdList { get; }
	}
}
