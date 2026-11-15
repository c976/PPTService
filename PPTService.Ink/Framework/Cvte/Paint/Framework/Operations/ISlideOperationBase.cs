using PPTService.Ink.Core.Documents;

namespace Cvte.Paint.Framework.Operations
{
	internal interface ISlideOperationBase : IInjectBoardOperation, IOperation
	{
		string SlideId { get; }

		Slide GetSlide();
	}
}
