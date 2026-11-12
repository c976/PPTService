using System.Windows;

namespace Cvte.Paint.Framework
{
	public interface IUtilitiesContainer
	{
		void AddUtilities(UIElement utilitiesElement);

		void RemoveUtilities(UIElement utilitiesElement);

		bool ContainsUtilities(UIElement utilitiesElement);
	}
}
