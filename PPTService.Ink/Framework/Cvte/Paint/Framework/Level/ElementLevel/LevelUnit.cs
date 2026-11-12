using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.ElementLevel;

namespace Cvte.Paint.Framework.Level.ElementLevel
{
	public static class LevelUnit
	{
		[Obsolete("不支持的方法", true)]
		public static int GetElementLevel(this Slide slide, ILevelElement element)
		{
			throw new NotSupportedException();
		}

		[NotNull]
		[Obsolete("请使用 LevelManager.GetLevelSortedElements 代替")]
		public static List<Element> GetLevelSortedElements([NotNull] this Slide slide, [NotNull] IEnumerable<Element> elements)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			return slide.LevelStorage.GetLevelSortedElements(((IEnumerable<ILevelElement>)elements).ToArray()).Cast<Element>().ToList();
		}

		[Obsolete("请使用 LevelManager.GetTopElement 代替")]
		public static Element GetTopElement([NotNull] this Slide slide, [NotNull][ItemNotNull] IList<Element> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			if (!elements.Any())
			{
				throw new ArgumentException("传入的elements不能为空");
			}
			return LevelManager.GetTopElement(slide, elements);
		}

		public static ChangeLayerOperation BringElementForward([NotNull] this ILevelContainer elementLevelContainer, params ILevelElement[] elements)
		{
			return GetChangeLayerOperation(Slide.GetSlide((DependencyObject)elementLevelContainer), elementLevelContainer.LevelStorage.MoveUp(elements));
		}

		public static ChangeLayerOperation SendElementBackward([NotNull] this ILevelContainer elementLevelContainer, params ILevelElement[] elements)
		{
			return GetChangeLayerOperation(Slide.GetSlide((DependencyObject)elementLevelContainer), elementLevelContainer.LevelStorage.MoveDown(elements));
		}

		[CanBeNull]
		public static ChangeLayerOperation BringElementToFront([NotNull] this ILevelContainer elementLevelContainer, params ILevelElement[] elements)
		{
			return GetChangeLayerOperation(Slide.GetSlide((DependencyObject)elementLevelContainer), elementLevelContainer.LevelStorage.MoveToTop(elements));
		}

		public static ChangeLayerOperation SendElementToBack([NotNull] this ILevelContainer elementLevelContainer, params ILevelElement[] elements)
		{
			return GetChangeLayerOperation(Slide.GetSlide((DependencyObject)elementLevelContainer), elementLevelContainer.LevelStorage.MoveToBottom(elements));
		}

		private static ChangeLayerOperation GetChangeLayerOperation(Slide slide, LevelMoveResultCollection levelMoveResult)
		{
			return new ChangeLayerOperation(slide, levelMoveResult);
		}
	}
}
