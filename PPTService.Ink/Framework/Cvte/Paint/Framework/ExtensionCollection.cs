using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public class ExtensionCollection<T> : Collection<IExtension<T>> where T : IExtensible<T>
	{
		private readonly T _owner;

		public ExtensionCollection(T owner)
		{
			_owner = owner;
		}

		public ExtensionCollection([NotNull] IList<IExtension<T>> list, T owner)
			: base(list)
		{
			_owner = owner;
		}

		[CanBeNull]
		public TExtension GetExtension<TExtension>() where TExtension : IExtension<T>
		{
			return this.OfType<TExtension>().FirstOrDefault();
		}

		internal void AddWithoutAttached([NotNull] IExtension<T> item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			int count = Count;
			base.InsertItem(count, item);
		}

		protected override void InsertItem(int index, IExtension<T> item)
		{
			if (item != null)
			{
				base.InsertItem(index, item);
				item.Attached(_owner);
			}
		}

		protected override void RemoveItem(int index)
		{
			IExtension<T> extension = Items[index];
			base.RemoveItem(index);
			extension.Detached(_owner);
		}

		protected override void ClearItems()
		{
			foreach (IExtension<T> item in Items)
			{
				item.Detached(_owner);
			}
			base.ClearItems();
		}
	}
}
