using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;

namespace Cvte.Paint.Inks
{
	internal class AddInkElementOperation : AddElementsOperation
	{
		public AddInkElementOperation(Slide slide, Element element)
			: base(slide, element)
		{
		}
	}
}
