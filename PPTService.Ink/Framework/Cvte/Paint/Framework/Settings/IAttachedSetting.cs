namespace Cvte.Paint.Framework.Settings
{
	public interface IAttachedSetting<out T> where T : BoardFilterSettings
	{
		T Settings { get; }
	}
}
