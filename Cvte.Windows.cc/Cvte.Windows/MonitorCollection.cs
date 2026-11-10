using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Cvte.Windows
{
	public sealed class MonitorCollection : ICollection<MonitorDevice>, IEnumerable<MonitorDevice>, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private readonly List<MonitorDevice> _devices;

		public MonitorDevice Primary => _devices.First((MonitorDevice device) => object.Equals(device.Win32Screen, Screen.PrimaryScreen));

		public MonitorDevice Secondary => _devices.FirstOrDefault((MonitorDevice device) => !object.Equals(device.Win32Screen, Screen.PrimaryScreen));

		public int Count => _devices.Count;

		bool ICollection<MonitorDevice>.IsReadOnly => true;

		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add
			{
				CollectionChanged += value;
			}
			remove
			{
				CollectionChanged -= value;
			}
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				PropertyChanged += value;
			}
			remove
			{
				PropertyChanged -= value;
			}
		}

		private event NotifyCollectionChangedEventHandler CollectionChanged;

		private event PropertyChangedEventHandler PropertyChanged;

		internal MonitorCollection(IEnumerable<MonitorDevice> devices)
		{
			_devices = new List<MonitorDevice>(devices);
		}

		internal void Update(IEnumerable<MonitorDevice> devices)
		{
			IList list = _devices.ToArray();
			_devices.Clear();
			_devices.AddRange(devices);
			IList list2 = _devices.ToArray();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, list2, list));
			if (list.Count != list2.Count)
			{
				OnPropertyChanged("Count");
			}
			OnPropertyChanged("Item[]");
		}

		bool ICollection<MonitorDevice>.Contains(MonitorDevice item)
		{
			return _devices.Contains(item);
		}

		void ICollection<MonitorDevice>.Add(MonitorDevice item)
		{
			throw new NotSupportedException();
		}

		bool ICollection<MonitorDevice>.Remove(MonitorDevice item)
		{
			throw new NotSupportedException();
		}

		void ICollection<MonitorDevice>.Clear()
		{
			throw new NotSupportedException();
		}

		void ICollection<MonitorDevice>.CopyTo(MonitorDevice[] array, int arrayIndex)
		{
			_devices.CopyTo(array, arrayIndex);
		}

		IEnumerator<MonitorDevice> IEnumerable<MonitorDevice>.GetEnumerator()
		{
			return _devices.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _devices.GetEnumerator();
		}

		private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged?.Invoke(this, e);
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
