using System.Windows;

namespace PPTServiceUltra.Paint.Framework
{
	public interface IUtilitiesContainer
	{
		void AddUtilities(UIElement utilitiesElement);

		void RemoveUtilities(UIElement utilitiesElement);

		bool ContainsUtilities(UIElement utilitiesElement);
	}
}
