using PPTServiceUltra.Paint.Framework;
using PPTServiceUltra.Paint.Framework.Operations;

namespace PPTServiceUltra.Paint.Inks
{
	internal class AddInkElementOperation : AddElementsOperation
	{
		public AddInkElementOperation(Slide slide, Element element)
			: base(slide, element)
		{
		}
	}
}
