using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public sealed class PopupRootContentCollection : Collection<IPopupRootContent>
	{
		private readonly Action<IPopupRootContent> _onAdded;

		private readonly Action<IPopupRootContent> _onRemoved;

		private readonly Action _onFirstAdded;

		private readonly Action _onLastRemoving;

		private readonly Action _onLastRemoved;

		internal PopupRootContentCollection(Action<IPopupRootContent> onAdded, Action<IPopupRootContent> onRemoved, Action onFirstAdded = null, Action onLastRemoving = null, Action onLastRemoved = null)
		{
			_onAdded = onAdded;
			_onRemoved = onRemoved;
			_onFirstAdded = onFirstAdded;
			_onLastRemoving = onLastRemoving;
			_onLastRemoved = onLastRemoved;
		}

		protected override void InsertItem(int index, IPopupRootContent item)
		{
			bool flag = Count == 0;
			base.InsertItem(index, item);
			try
			{
				if (flag)
				{
					_onFirstAdded?.Invoke();
				}
			}
			finally
			{
				_onAdded?.Invoke(item);
			}
		}

		protected override void RemoveItem(int index)
		{
			IPopupRootContent obj = Items[index];
			base.RemoveItem(index);
			bool flag = Count == 0;
			_onLastRemoving?.Invoke();
			try
			{
				_onRemoved(obj);
			}
			finally
			{
				if (flag)
				{
					_onLastRemoved?.Invoke();
				}
			}
		}

		protected override void SetItem(int index, IPopupRootContent item)
		{
			IPopupRootContent obj = Items[index];
			base.SetItem(index, item);
			try
			{
				_onRemoved?.Invoke(obj);
			}
			finally
			{
				_onAdded?.Invoke(item);
			}
		}

		protected override void ClearItems()
		{
			IPopupRootContent[] array = Items.ToArray();
			base.ClearItems();
			List<Exception> list = new List<Exception>();
			try
			{
				_onLastRemoving?.Invoke();
			}
			catch (Exception item)
			{
				list.Add(item);
			}
			try
			{
				IPopupRootContent[] array2 = array;
				foreach (IPopupRootContent obj in array2)
				{
					try
					{
						_onRemoved?.Invoke(obj);
					}
					catch (Exception item2)
					{
						list.Add(item2);
					}
				}
				if (list.Count == 1)
				{
					ExceptionDispatchInfo.Capture(list.First()).Throw();
				}
				else if (list.Any())
				{
					throw new AggregateException(list);
				}
			}
			finally
			{
				_onLastRemoved?.Invoke();
			}
		}
	}
}
