using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public interface IIdDefinition
	{
		string Id { get; }

		void SetId([NotNull] string id);
	}
}
