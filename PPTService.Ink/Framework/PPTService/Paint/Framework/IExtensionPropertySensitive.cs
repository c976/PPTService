using PPTService.Ink.Core;
namespace PPTService.Paint.Framework
{
	public interface IExtensionPropertySensitive
	{
		void IsLockedPropertyChanged(ValueChangeEventArgs<bool> args);
	}
}
