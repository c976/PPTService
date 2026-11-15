using PPTService.Ink.Core;
namespace PPTService.Paint.Framework
{
	public class ModeChangeEventArgs : ValueChangeEventArgs<Mode>
	{
		public ModeChangeEventArgs(Mode oldMode, Mode newMode)
			: base(oldMode, newMode)
		{
		}
	}
}
