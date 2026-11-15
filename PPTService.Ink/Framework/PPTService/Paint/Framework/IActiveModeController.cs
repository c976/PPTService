using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Paint.Framework
{
	public interface IActiveModeController
	{
		Mode ActiveMode { get; }

		void SetActiveMode([NotNull] Mode mode);

		void InvalidateActiveMode();
	}
}
