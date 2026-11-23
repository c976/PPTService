using PPTServiceUltra.Ink.Core.Documents;

namespace PPTServiceUltra.Paint.Framework.Operations
{
	internal interface IBoardOperationBase : IOperation
	{
		Board Board { get; }
	}
}
