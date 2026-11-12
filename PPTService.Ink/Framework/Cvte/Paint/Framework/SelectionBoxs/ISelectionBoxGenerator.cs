using System.Collections.Generic;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.SelectionBoxs
{
	public interface ISelectionBoxGenerator
	{
		void GenerateSelectionBox([NotNull] Slide slide, [NotNull] IList<Element> elements);

		void DegenerateSelectionBox([NotNull] Slide slide, [NotNull] IList<Element> elements);
	}
}
