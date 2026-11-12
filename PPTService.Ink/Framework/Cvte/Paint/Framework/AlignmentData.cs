using System;

namespace Cvte.Paint.Framework
{
	public abstract class AlignmentData
	{
		public virtual bool Merge(AlignmentData data, double threshold)
		{
			return false;
		}
	}
	public abstract class AlignmentData<T> : AlignmentData where T : AlignmentData
	{
		protected AlignmentData()
		{
			if (typeof(T) == typeof(AlignmentData))
			{
				throw new InvalidOperationException("不允许构造 AlignmentData 泛型参数的 AlignmentData 对象。");
			}
		}

		public sealed override bool Merge(AlignmentData data, double threshold)
		{
			T val = data as T;
			if (val != null)
			{
				return Merge(val, threshold);
			}
			return false;
		}

		protected abstract bool Merge(T data, double threshold);
	}
}
