using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Paint.Framework.SelectionBoxs
{
	public interface ISelectionBoxGenerator
	{
		void GenerateSelectionBox([NotNull] Slide slide, [NotNull] IList<Element> elements);

		void DegenerateSelectionBox([NotNull] Slide slide, [NotNull] IList<Element> elements);
	}
}
