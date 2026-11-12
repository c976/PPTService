using System;
using System.Collections.Generic;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.Elements
{
	public interface IGroupElement
	{
		IEnumerable<Element> InnerSelections { get; }

		ICollection<Element> Elements { get; }

		ElementGroupingResult Group(bool isAutoAddToSlide);

		ElementGroupingResult Ungroup(bool isNewLevelRequired, bool isAutoSelect = true);

		[Obsolete("请使用 ElementGroupResult Group(bool isAutoAddToSlide) 代替")]
		void Group();

		[Obsolete("请使用 ElementGroupResult Ungroup(bool isElementUseNewLevel, bool isAutoSelect = true) 代替")]
		IList<Element> Ungroup();

		void RemoveInnerSelectionBoxes();

		void InnerSelect([CanBeNull] Element innerElement);
	}
}
