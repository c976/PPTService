using System;
using PPTServiceUltra.Paint.Framework;

namespace PPTServiceUltra.Paint.DesktopMode
{
	public interface IDesktopBoardModesProvider : IDisposable
	{
		Mode PenMode { get; }

		Mode EraserMode { get; }
	}
}
