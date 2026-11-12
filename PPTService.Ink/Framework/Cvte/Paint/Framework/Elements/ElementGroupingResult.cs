using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.Elements
{
	public abstract class ElementGroupingResult
	{
		[CanBeNull]
		public string SlideId { get; }

		protected ElementGroupingResult([CanBeNull] Slide slide)
		{
			SlideId = slide?.Id;
		}

		public abstract void Undo(ElementGroupOperation elementGroupOperation);

		public abstract void Redo(ElementGroupOperation elementGroupOperation);
	}
}
