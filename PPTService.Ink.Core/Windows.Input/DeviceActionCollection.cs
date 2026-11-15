using System.Collections.ObjectModel;
using System.Windows;

namespace PPTService.Ink.Core.Windows.Input
{
	public sealed class DeviceActionCollection : ObservableCollection<DeviceAction>
	{
		public DependencyObject AssociatedObject { get; }

		internal DeviceActionCollection(DependencyObject associatedObject)
		{
			AssociatedObject = associatedObject;
		}

		protected override void InsertItem(int index, DeviceAction item)
		{
			base.InsertItem(index, item);
			item.Attach(AssociatedObject);
		}

		protected override void RemoveItem(int index)
		{
			Items[index].Detach();
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, DeviceAction item)
		{
			Items[index].Detach();
			item.Attach(AssociatedObject);
			base.SetItem(index, item);
		}
	}
}
