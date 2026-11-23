using System.Collections.Generic;

namespace PPTServiceUltra.Paint.Framework.SelectionBoxs
{
	public interface ISelectionBox
	{
		IList<Element> SelectedElements { get; set; }
	}
}
