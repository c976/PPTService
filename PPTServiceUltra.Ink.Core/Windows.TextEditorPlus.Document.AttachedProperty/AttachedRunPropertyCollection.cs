using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.AttachedProperty
{
	internal class AttachedRunPropertyCollection : List<IAttachedRunProperty>, IEquatable<AttachedRunPropertyCollection>
	{
		public AttachedRunPropertyCollection()
		{
		}

		private AttachedRunPropertyCollection(int capacity)
			: base(capacity)
		{
		}

		public void Reset(List<IAttachedRunProperty> attachedRunPropertyList)
		{
			Clear();
			Capacity = Math.Max(attachedRunPropertyList.Count, Capacity);
			AddRange(attachedRunPropertyList);
		}

		public AttachedRunPropertyCollection Clone()
		{
			AttachedRunPropertyCollection attachedRunPropertyCollection = new AttachedRunPropertyCollection(Count);
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IAttachedRunProperty current = enumerator.Current;
					attachedRunPropertyCollection.Add(current.Clone());
				}
				return attachedRunPropertyCollection;
			}
		}

		[CanBeNull]
		public T GetProperty<T>() where T : IAttachedRunProperty
		{
			return (T)this.FirstOrDefault((IAttachedRunProperty temp) => temp is T);
		}

		public T GetOrCreateProperty<T>() where T : IAttachedRunProperty, new()
		{
			IAttachedRunProperty attachedRunProperty = this.FirstOrDefault((IAttachedRunProperty temp) => temp is T);
			if (attachedRunProperty == null)
			{
				attachedRunProperty = new T();
				Add(attachedRunProperty);
			}
			return (T)attachedRunProperty;
		}

		public bool Equals(AttachedRunPropertyCollection other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Count == 0 && other.Count == 0)
			{
				return true;
			}
			if (Count != other.Count)
			{
				return false;
			}
			for (int i = 0; i < Count; i++)
			{
				if (!base[i].Equals(other[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((AttachedRunPropertyCollection)obj);
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}
	}
}
