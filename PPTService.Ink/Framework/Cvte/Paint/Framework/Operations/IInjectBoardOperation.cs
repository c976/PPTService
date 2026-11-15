using PPTService.Ink.Core.Documents;

namespace Cvte.Paint.Framework.Operations
{
	public interface IInjectBoardOperation : IOperation
	{
		Board Board { get; set; }
	}
}
