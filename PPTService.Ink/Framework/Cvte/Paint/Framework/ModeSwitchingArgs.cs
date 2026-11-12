namespace Cvte.Paint.Framework
{
	public sealed class ModeSwitchingArgs
	{
		public Mode SourceMode { get; internal set; }

		public Mode TargetMode { get; internal set; }

		public bool SuppressSourceModeUnselect { get; set; }
	}
}
