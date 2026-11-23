namespace PPTServiceUltra.Paint.Framework
{
	public interface IDynamicSavableElement
	{
		void SetSaveInfo(ElementSaveInfo elementSaveInfo);

		ElementSaveInfo GetSaveInfo();
	}
}
