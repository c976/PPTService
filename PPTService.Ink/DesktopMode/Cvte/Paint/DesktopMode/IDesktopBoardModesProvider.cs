using System;
using Cvte.Paint.Framework;

namespace Cvte.Paint.DesktopMode
{
	public interface IDesktopBoardModesProvider : IDisposable
	{
		Mode PenMode { get; }

		Mode EraserMode { get; }
	}
}
