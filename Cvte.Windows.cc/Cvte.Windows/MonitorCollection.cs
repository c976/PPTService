using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Cvte.Windows
{
	public sealed class MonitorCollection : ICollection<MonitorDevice>, IEnumerable<MonitorDevice>, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private readonly List<MonitorDevice> _devices;

		private NotifyCollectionChangedEventHandler m_CollectionChanged;

		private PropertyChangedEventHandler m_PropertyChanged;

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

		private event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add
			{
				NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						notifyCollectionChangedEventHandler = this.m_CollectionChanged;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler2 = default(NotifyCollectionChangedEventHandler);
				NotifyCollectionChangedEventHandler value2 = (NotifyCollectionChangedEventHandler)Delegate.Combine(notifyCollectionChangedEventHandler2, value);
				notifyCollectionChangedEventHandler = Interlocked.CompareExchange(ref this.m_CollectionChanged, value2, notifyCollectionChangedEventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler3 = notifyCollectionChangedEventHandler;
				if (6u != 0)
				{
					notifyCollectionChangedEventHandler2 = notifyCollectionChangedEventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)notifyCollectionChangedEventHandler != notifyCollectionChangedEventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
			remove
			{
				NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						notifyCollectionChangedEventHandler = this.m_CollectionChanged;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler2 = default(NotifyCollectionChangedEventHandler);
				NotifyCollectionChangedEventHandler value2 = (NotifyCollectionChangedEventHandler)Delegate.Remove(notifyCollectionChangedEventHandler2, value);
				notifyCollectionChangedEventHandler = Interlocked.CompareExchange(ref this.m_CollectionChanged, value2, notifyCollectionChangedEventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler3 = notifyCollectionChangedEventHandler;
				if (6u != 0)
				{
					notifyCollectionChangedEventHandler2 = notifyCollectionChangedEventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)notifyCollectionChangedEventHandler != notifyCollectionChangedEventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
		}

		private event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				PropertyChangedEventHandler propertyChangedEventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						propertyChangedEventHandler = this.m_PropertyChanged;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				PropertyChangedEventHandler propertyChangedEventHandler2 = default(PropertyChangedEventHandler);
				PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Combine(propertyChangedEventHandler2, value);
				propertyChangedEventHandler = Interlocked.CompareExchange(ref this.m_PropertyChanged, value2, propertyChangedEventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				PropertyChangedEventHandler propertyChangedEventHandler3 = propertyChangedEventHandler;
				if (6u != 0)
				{
					propertyChangedEventHandler2 = propertyChangedEventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)propertyChangedEventHandler != propertyChangedEventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
			remove
			{
				PropertyChangedEventHandler propertyChangedEventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						propertyChangedEventHandler = this.m_PropertyChanged;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				PropertyChangedEventHandler propertyChangedEventHandler2 = default(PropertyChangedEventHandler);
				PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Remove(propertyChangedEventHandler2, value);
				propertyChangedEventHandler = Interlocked.CompareExchange(ref this.m_PropertyChanged, value2, propertyChangedEventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				PropertyChangedEventHandler propertyChangedEventHandler3 = propertyChangedEventHandler;
				if (6u != 0)
				{
					propertyChangedEventHandler2 = propertyChangedEventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)propertyChangedEventHandler != propertyChangedEventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
		}

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
			this.m_CollectionChanged?.Invoke(this, e);
		}

		private void OnPropertyChanged(string propertyName)
		{
			while (true)
			{
				PropertyChangedEventHandler propertyChanged;
				if (0 == 0)
				{
					propertyChanged = this.m_PropertyChanged;
				}
				while (true)
				{
					if (propertyChanged != null)
					{
						if (false)
						{
							break;
						}
						if (5 != 0)
						{
							propertyChanged(this, new PropertyChangedEventArgs(propertyName));
						}
					}
					if (false)
					{
						continue;
					}
					return;
				}
			}
		}
	}
}
