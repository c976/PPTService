using PPTServiceUltra.Ink.Core.Documents;

namespace PPTServiceUltra.Paint.Framework.Operations
{
	internal interface ISlideOperationBase : IInjectBoardOperation, IOperation
	{
		string SlideId { get; }

		Slide GetSlide();
	}
}
