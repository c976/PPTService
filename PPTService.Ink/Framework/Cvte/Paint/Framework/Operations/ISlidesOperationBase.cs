using System.Collections.Generic;
using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	internal interface ISlidesOperationBase : IBoardOperationBase, IOperation
	{
		IReadOnlyList<string> SlideIdList { get; }
	}
}
