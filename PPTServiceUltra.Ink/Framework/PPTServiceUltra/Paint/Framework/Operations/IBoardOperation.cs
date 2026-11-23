using PPTServiceUltra.Ink.Core.Documents;

namespace PPTServiceUltra.Paint.Framework.Operations
{
	public interface IBoardOperation : IOperation
	{
		string SlideId { get; }

		Board Board { get; set; }
	}
}
