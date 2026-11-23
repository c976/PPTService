using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Paint.Framework
{
	public interface IActiveModeController
	{
		Mode ActiveMode { get; }

		void SetActiveMode([NotNull] Mode mode);

		void InvalidateActiveMode();
	}
}
