namespace Cvte.Paint.Framework
{
	public interface IElementSaveInfoFilter
	{
		void BeforeGetElementSaveInfo(Element element);

		void AfterGetElementSaveInfo(Element element, ref ElementSaveInfo saveInfo);

		void BeforeSetElementSaveInfo(Element element, ref ElementSaveInfo saveInfo);

		void AfterSetElementSaveInfo(Element element, ElementSaveInfo saveInfo);
	}
}
