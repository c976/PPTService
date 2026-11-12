using System.Collections.Generic;

namespace Cvte.Paint.Framework.SelectionBoxs
{
	public interface ISelectionBox
	{
		IList<Element> SelectedElements { get; set; }
	}
}
