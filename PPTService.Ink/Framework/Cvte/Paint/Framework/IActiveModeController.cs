using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public interface IActiveModeController
	{
		Mode ActiveMode { get; }

		void SetActiveMode([NotNull] Mode mode);

		void InvalidateActiveMode();
	}
}
