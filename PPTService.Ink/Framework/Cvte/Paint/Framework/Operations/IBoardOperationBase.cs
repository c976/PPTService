using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	internal interface IBoardOperationBase : IOperation
	{
		Board Board { get; }
	}
}
