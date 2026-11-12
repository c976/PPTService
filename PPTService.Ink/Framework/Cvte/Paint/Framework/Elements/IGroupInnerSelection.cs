using Cvte.Paint.Framework.SelectionBoxs;

namespace Cvte.Paint.Framework.Elements
{
	public interface IGroupInnerSelection
	{
		ISelectionOperator GetSelectionOperator(Element element);
	}
}
