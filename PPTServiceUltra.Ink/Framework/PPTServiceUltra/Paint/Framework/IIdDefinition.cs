using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Paint.Framework
{
	public interface IIdDefinition
	{
		string Id { get; }

		void SetId([NotNull] string id);
	}
}
