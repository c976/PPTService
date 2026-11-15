using System.Collections;
using System.Collections.Generic;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	public class ParagraphLinesInfo : IReadOnlyList<IReadonlyLineProperty>, IReadOnlyCollection<IReadonlyLineProperty>, IEnumerable<IReadonlyLineProperty>, IEnumerable
	{
		private readonly List<LineProperty> _list;

		public int Count => _list.Count;

		public IReadonlyLineProperty this[int index] => _list[index];

		internal ParagraphLinesInfo(List<LineProperty> list)
		{
			_list = list;
		}

		public IEnumerator<IReadonlyLineProperty> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_list).GetEnumerator();
		}
	}
}
