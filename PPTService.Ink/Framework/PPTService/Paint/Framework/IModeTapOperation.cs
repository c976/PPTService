namespace PPTService.Paint.Framework
{
	public interface IModeTapOperation : IModeOperation
	{
		void OnTap(ModeInputEventArgs e);
	}
}
