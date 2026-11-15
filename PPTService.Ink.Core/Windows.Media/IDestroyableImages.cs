using System;

namespace PPTService.Ink.Core.Windows.Media
{
	public interface IDestroyableImages : IDisposable
	{
		void DestroyAgain();
	}
}
