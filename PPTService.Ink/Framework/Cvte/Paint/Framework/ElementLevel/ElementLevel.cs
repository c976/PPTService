using System.Collections.Generic;
using System.Linq;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Level;

namespace Cvte.Paint.Framework.ElementLevel
{
	public static class ElementLevel
	{
		[NotNull]
		public static ChangeLayerOperation BringElementForward([NotNull] this Slide slide, [NotNull] Element element)
		{
			return slide.BringElementForward(new Element[1] { element });
		}

		[NotNull]
		public static ChangeLayerOperation SendElementBackward([NotNull] this Slide slide, [NotNull] Element element)
		{
			return slide.SendElementBackward(new Element[1] { element });
		}

		[NotNull]
		public static ChangeLayerOperation BringElementToFront([NotNull] this Slide slide, [NotNull] Element element)
		{
			return slide.BringElementToFront(new Element[1] { element });
		}

		[CanBeNull]
		public static ChangeLayerOperation SendElementToBack([NotNull] this Slide slide, [NotNull] Element element)
		{
			return slide.SendElementToBack(new Element[1] { element });
		}

		[NotNull]
		public static ChangeLayerOperation BringElementForward(this Slide slide, IEnumerable<Element> elements)
		{
			return GetChangeLayerOperation(slide, slide.LevelStorage.MoveUp(((IEnumerable<ILevelElement>)elements).ToArray()));
		}

		[NotNull]
		public static ChangeLayerOperation SendElementBackward(this Slide slide, IEnumerable<Element> elements)
		{
			return GetChangeLayerOperation(slide, slide.LevelStorage.MoveDown(((IEnumerable<ILevelElement>)elements).ToArray()));
		}

		[CanBeNull]
		public static ChangeLayerOperation BringElementToFront(this Slide slide, IEnumerable<Element> elements)
		{
			return GetChangeLayerOperation(slide, slide.LevelStorage.MoveToTop(((IEnumerable<ILevelElement>)elements).ToArray()));
		}

		[CanBeNull]
		public static ChangeLayerOperation SendElementToBack(this Slide slide, IEnumerable<Element> elements)
		{
			return GetChangeLayerOperation(slide, slide.LevelStorage.MoveToBottom(((IEnumerable<ILevelElement>)elements).ToArray()));
		}

		private static ChangeLayerOperation GetChangeLayerOperation(Slide slide, LevelMoveResultCollection levelMoveResult)
		{
			return new ChangeLayerOperation(slide, levelMoveResult);
		}
	}
}
