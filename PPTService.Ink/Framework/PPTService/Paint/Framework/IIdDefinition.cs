using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Paint.Framework
{
	public interface IIdDefinition
	{
		string Id { get; }

		void SetId([NotNull] string id);
	}
}
