namespace PPTServiceUltra.Paint.Framework
{
	public interface IElementSaveInfoProvider
	{
		void SetSaveInfo(ElementSaveInfo elementSaveInfo);

		ElementSaveInfo GetSaveInfo();
	}
}
