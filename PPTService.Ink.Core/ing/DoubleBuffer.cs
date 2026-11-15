using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace dotnetCampus.Threading
{
	
	
	internal class DoubleBuffer<T,  TU> where T : class, ICollection<TU>
	{
		private readonly object _lock = new object();

		private T CurrentList { get; set; }

		private T AList { get; }

		private T BList { get; }

		public DoubleBuffer(T aList, T bList)
		{
			AList = aList;
			BList = bList;
			CurrentList = AList;
		}

		public void Add(TU t)
		{
			lock (_lock)
			{
				CurrentList.Add(t);
			}
		}

		public T SwitchBuffer()
		{
			lock (_lock)
			{
				if (CurrentList == AList)
				{
					CurrentList = BList;
					return AList;
				}
				CurrentList = AList;
				return BList;
			}
		}

		public void DoAll(Action<T> action)
		{
			while (true)
			{
				T val = SwitchBuffer();
				if (val.Count != 0)
				{
					action(val);
					val.Clear();
					continue;
				}
				break;
			}
		}

		public async Task DoAllAsync(Func<T, Task> action)
		{
			while (true)
			{
				T buffer = SwitchBuffer();
				if (buffer.Count == 0)
				{
					break;
				}
				await action(buffer).ConfigureAwait(false);
				buffer.Clear();
			}
		}
	}
	
	
	internal class DoubleBuffer<T> : DoubleBuffer<List<T>, T>
	{
		public DoubleBuffer()
			: base(new List<T>(), new List<T>())
		{
		}
	}
}
