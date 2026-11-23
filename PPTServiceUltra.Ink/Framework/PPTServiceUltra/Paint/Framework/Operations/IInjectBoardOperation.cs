using PPTServiceUltra.Ink.Core.Documents;

namespace PPTServiceUltra.Paint.Framework.Operations
{
	public interface IInjectBoardOperation : IOperation
	{
		Board Board { get; set; }
	}
}
