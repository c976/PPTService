using System.Windows.Input;

namespace PPTService.Paint.Framework
{
	public interface IModeKeyboardSensitive
	{
		ModifierKeys Modifiers { get; }
	}
}
