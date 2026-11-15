using PPTService.Ink.Core.Documents;

namespace PPTService.Paint.Framework.Operations
{
	internal interface ISlideOperationBase : IInjectBoardOperation, IOperation
	{
		string SlideId { get; }

		Slide GetSlide();
	}
}
