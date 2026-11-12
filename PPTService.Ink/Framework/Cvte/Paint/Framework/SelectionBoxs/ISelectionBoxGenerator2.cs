namespace Cvte.Paint.Framework.SelectionBoxs
{
	public interface ISelectionBoxGenerator2
	{
		ISelectionBox GenerateSelectionBox(Element element);

		void ShowSelectionBox(Element element);

		void HideSelectionBox(Element element);
	}
}
