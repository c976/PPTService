using PPTService.Paint.Framework;
using PPTService.Paint.Framework.Operations;

namespace PPTService.Paint.Inks
{
	internal class AddInkElementOperation : AddElementsOperation
	{
		public AddInkElementOperation(Slide slide, Element element)
			: base(slide, element)
		{
		}
	}
}
