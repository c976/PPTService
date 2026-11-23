using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public interface ILoadingScreen
	{
		void Show(string loadingMessage);

		void Hide();

		void Dispose();

		void SetLocation(Rect bounds);
	}
}
