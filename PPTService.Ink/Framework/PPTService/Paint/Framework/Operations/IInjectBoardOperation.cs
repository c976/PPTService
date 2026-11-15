using PPTService.Ink.Core.Documents;

namespace PPTService.Paint.Framework.Operations
{
	public interface IInjectBoardOperation : IOperation
	{
		Board Board { get; set; }
	}
}
