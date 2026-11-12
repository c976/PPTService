namespace Cvte.Paint.Framework.Operations
{
	public abstract class ElementOperation : ElementOperationBase
	{
		public bool ShouldGotoSlide { get; protected set; } = true;

		protected ElementOperation(string slideId)
			: base(slideId)
		{
		}

		protected ElementOperation(Slide slide)
			: base(slide)
		{
		}

		protected ElementOperation(Element element)
			: base(element)
		{
			if (Slide.GetSlide(element) == null)
			{
				ShouldGotoSlide = false;
			}
		}
	}
}
