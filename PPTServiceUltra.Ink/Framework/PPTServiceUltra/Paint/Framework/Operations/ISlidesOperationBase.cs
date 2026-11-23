using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Documents;

namespace PPTServiceUltra.Paint.Framework.Operations
{
	internal interface ISlidesOperationBase : IBoardOperationBase, IOperation
	{
		IReadOnlyList<string> SlideIdList { get; }
	}
}
