using PPTService.Ink.Core.Documents;

namespace PPTService.Paint.Framework.Operations
{
	internal interface IBoardOperationBase : IOperation
	{
		Board Board { get; }
	}
}
