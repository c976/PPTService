using PPTService.Ink.Core.Documents;

namespace PPTService.Paint.Framework.Operations
{
	public interface IBoardOperation : IOperation
	{
		string SlideId { get; }

		Board Board { get; set; }
	}
}
