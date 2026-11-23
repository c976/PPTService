using PPTServiceUltra.Paint.Framework.SelectionBoxs;

namespace PPTServiceUltra.Paint.Framework.Elements
{
	public interface IGroupInnerSelection
	{
		ISelectionOperator GetSelectionOperator(Element element);
	}
}
