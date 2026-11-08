namespace Cvte.Documents
{
	public interface IOperation
	{
		void Undo();

		void Redo();
	}
}
