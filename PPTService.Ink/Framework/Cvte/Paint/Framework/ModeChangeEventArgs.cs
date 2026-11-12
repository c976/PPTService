namespace Cvte.Paint.Framework
{
	public class ModeChangeEventArgs : ValueChangeEventArgs<Mode>
	{
		public ModeChangeEventArgs(Mode oldMode, Mode newMode)
			: base(oldMode, newMode)
		{
		}
	}
}
