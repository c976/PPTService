using System.Collections;
using System.Collections.Generic;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public interface IFeature : IReadOnlyCollection<string>, IEnumerable<string>, IEnumerable
	{
		bool HasFeature(IFeature feature);
	}
}
