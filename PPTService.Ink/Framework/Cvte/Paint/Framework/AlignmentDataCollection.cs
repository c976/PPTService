using System;
using System.Collections;
using System.Collections.Generic;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public sealed class AlignmentDataCollection : ICollection<AlignmentData>, IEnumerable<AlignmentData>, IEnumerable
	{
		private readonly List<AlignmentData> _data;

		public static double Threshold { get; } = 0.375;

		public int Count => _data.Count;

		bool ICollection<AlignmentData>.IsReadOnly => false;

		public AlignmentDataCollection()
		{
			_data = new List<AlignmentData>();
		}

		internal AlignmentDataCollection(IEnumerable<AlignmentData> collection)
		{
			_data = new List<AlignmentData>(collection);
		}

		public void Merge([NotNull] AlignmentData item, bool deleteWhenMergeFailed = false)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			bool flag = false;
			foreach (AlignmentData datum in _data)
			{
				flag = datum.Merge(item, Threshold);
				if (flag)
				{
					break;
				}
			}
			if (!flag && !deleteWhenMergeFailed)
			{
				_data.Add(item);
			}
		}

		public void Merge(IEnumerable<AlignmentData> items)
		{
			foreach (AlignmentData item in items)
			{
				Merge(item);
			}
		}

		public void Clear()
		{
			_data.Clear();
		}

		void ICollection<AlignmentData>.Add(AlignmentData item)
		{
			Merge(item);
		}

		bool ICollection<AlignmentData>.Remove(AlignmentData item)
		{
			throw new NotSupportedException();
		}

		bool ICollection<AlignmentData>.Contains(AlignmentData item)
		{
			throw new NotSupportedException();
		}

		void ICollection<AlignmentData>.CopyTo(AlignmentData[] array, int arrayIndex)
		{
			_data.CopyTo(array, arrayIndex);
		}

		IEnumerator<AlignmentData> IEnumerable<AlignmentData>.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _data.GetEnumerator();
		}
	}
}
