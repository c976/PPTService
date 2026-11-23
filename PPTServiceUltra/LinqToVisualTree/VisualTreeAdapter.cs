using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace LinqToVisualTree
{
	public class VisualTreeAdapter : ILinqTree<DependencyObject>
	{
		private DependencyObject _item;

		public DependencyObject Parent => VisualTreeHelper.GetParent(_item);

		public VisualTreeAdapter(DependencyObject item)
		{
			_item = item;
		}

		public IEnumerable<DependencyObject> Children()
		{
			int childrenCount = VisualTreeHelper.GetChildrenCount(_item);
			for (int i = 0; i < childrenCount; i++)
			{
				yield return VisualTreeHelper.GetChild(_item, i);
			}
		}
	}
}
