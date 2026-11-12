namespace Cvte.Paint.Framework.SelectionBoxs
{
	public interface IBoardSensitiveSelectionBoxGenerator : ISelectionBoxGenerator
	{
		Board Board { get; set; }
	}
}
