namespace PPTServiceUltra.Ink.Core.Documents
{
	public interface IOperation
	{
		void Undo();

		void Redo();
	}
}
