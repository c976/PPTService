namespace Cvte.Paint.Inks
{
	public class SoftPenInputFromBoardWritingMode : HardPenInputFromBoardWritingMode
	{
		protected override HardPenInputFromBoardDynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return new SoftPenInputFromBoardDynamicRenderer(inkCanvas);
		}
	}
}
