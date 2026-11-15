using System.Windows;

namespace PPTService.Paint.Framework
{
	public interface IUtilitiesContainer
	{
		void AddUtilities(UIElement utilitiesElement);

		void RemoveUtilities(UIElement utilitiesElement);

		bool ContainsUtilities(UIElement utilitiesElement);
	}
}
