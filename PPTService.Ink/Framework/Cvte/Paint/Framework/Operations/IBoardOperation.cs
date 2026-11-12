using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	public interface IBoardOperation : IOperation
	{
		string SlideId { get; }

		Board Board { get; set; }
	}
}
