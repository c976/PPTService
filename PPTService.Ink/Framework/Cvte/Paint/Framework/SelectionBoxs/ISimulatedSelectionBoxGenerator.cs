namespace Cvte.Paint.Framework.SelectionBoxs
{
	public interface ISimulatedSelectionBoxGenerator : ISelectionBoxGenerator
	{
		bool IsSimulated { get; }

		ISimulatedSelectionBoxGenerator GetSimulatedGeneratorCopy();
	}
}
