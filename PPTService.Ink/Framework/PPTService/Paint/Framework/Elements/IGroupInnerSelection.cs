using PPTService.Paint.Framework.SelectionBoxs;

namespace PPTService.Paint.Framework.Elements
{
	public interface IGroupInnerSelection
	{
		ISelectionOperator GetSelectionOperator(Element element);
	}
}
