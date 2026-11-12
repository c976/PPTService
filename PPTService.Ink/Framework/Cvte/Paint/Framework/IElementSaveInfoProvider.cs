namespace Cvte.Paint.Framework
{
	public interface IElementSaveInfoProvider
	{
		void SetSaveInfo(ElementSaveInfo elementSaveInfo);

		ElementSaveInfo GetSaveInfo();
	}
}
