using System;
using PPTService.Paint.Framework;

namespace PPTService.Paint.DesktopMode
{
	public interface IDesktopBoardModesProvider : IDisposable
	{
		Mode PenMode { get; }

		Mode EraserMode { get; }
	}
}
