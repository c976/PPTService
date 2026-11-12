using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Inks
{
	[PublicAPI]
	public abstract class EraseEventArgs : OperationSynchronousEventArgs
	{
		public EraserHandler CurrentEraser { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }
	}
}
