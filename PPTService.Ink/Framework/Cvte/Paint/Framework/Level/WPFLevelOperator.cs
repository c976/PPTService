using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Cvte.Windows.Threading;

namespace Cvte.Paint.Framework.Level
{
	internal class WPFLevelOperator : ILevelOperator
	{
		private DispatcherRequiring _dispatcherRequiring;

		private void ApplyLevel(ILevelElement element, int index)
		{
			Panel.SetZIndex((UIElement)element, index);
		}

		public void ApplyLevel(ILevelStorage levelStorage)
		{
			if (_dispatcherRequiring == null)
			{
				DispatcherRequiring dispatcherRequiring = new DispatcherRequiring(() =>
				{
					List<ILevelElement> levelSortedElements = levelStorage.GetLevelSortedElements();
					for (int i = 0; i < levelSortedElements.Count; i++)
					{
						ApplyLevel(levelSortedElements[i], i);
					}
				});
				_dispatcherRequiring = dispatcherRequiring;
			}
			_dispatcherRequiring.Require();
		}
	}
}
