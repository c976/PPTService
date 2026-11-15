using System.Collections.Generic;

namespace PPTService.Paint.Framework.SelectionBoxs
{
	public interface ISelectionBox
	{
		IList<Element> SelectedElements { get; set; }
	}
}
