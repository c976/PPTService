using PPTServiceUltra.Ink.Core;
namespace PPTServiceUltra.Paint.Framework
{
	public interface IExtensionPropertySensitive
	{
		void IsLockedPropertyChanged(ValueChangeEventArgs<bool> args);
	}
}
