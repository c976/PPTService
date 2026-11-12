namespace Cvte.Paint.Framework
{
	public interface IExtensionCloneable
	{
		void OnCloned(int seed);

		void CloneFinalState();
	}
}
