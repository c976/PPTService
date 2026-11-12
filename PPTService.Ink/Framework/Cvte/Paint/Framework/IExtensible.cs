namespace Cvte.Paint.Framework
{
	public interface IExtensible<T> where T : IExtensible<T>
	{
		ExtensionCollection<T> Extensions { get; }
	}
}
