using PPTServiceUltra.Ink.Core.Storage;

namespace PPTServiceUltra.Paint.Framework
{
	public static class ElementSaveInfoProviderExtensions
	{
		public static ElementSaveInfoProvider<T> GetElementSaveInfoProvider<T>(this ISavable<T> savable) where T : ElementSaveInfo
		{
			return new ElementSaveInfoProvider<T>(savable);
		}
	}
}
