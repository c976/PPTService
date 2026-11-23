using System;

namespace PPTServiceUltra.Ink.Core.Windows.Media
{
	public interface IDestroyableImages : IDisposable
	{
		void DestroyAgain();
	}
}
