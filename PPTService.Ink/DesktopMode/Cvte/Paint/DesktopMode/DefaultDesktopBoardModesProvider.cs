using System;
using Cvte.Paint.Framework;
using Cvte.Paint.Inks;

namespace Cvte.Paint.DesktopMode
{
	public class DefaultDesktopBoardModesProvider : IDesktopBoardModesProvider, IDisposable
	{
		private readonly Lazy<EraserMode> _eraserMode = new Lazy<EraserMode>(() => new EraserMode());

		private readonly Lazy<SoftPenInputFromBoardWritingMode> _penMode = new Lazy<SoftPenInputFromBoardWritingMode>(() => new SoftPenInputFromBoardWritingMode());

		private bool disposedValue;

		public Mode PenMode => _penMode.Value;

		public Mode EraserMode => _eraserMode.Value;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing && _penMode.IsValueCreated)
				{
					_penMode.Value.SwitchOff();
					_penMode.Value.Dispose();
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
