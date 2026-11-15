using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cache
{
	
	
	public sealed class CachePoolValueMap< TSource,  TCache>
	{
		private readonly Func<TSource, TCache> _converter;

		private readonly Dictionary<TSource, Func<TSource, TCache>> _additionalConverters = new Dictionary<TSource, Func<TSource, TCache>>();

		public Func<TSource, TCache> this[TSource source]
		{
			get
			{
				Func<TSource, TCache> value;
				if (!_additionalConverters.TryGetValue(source, out value))
				{
					return _converter;
				}
				return value;
			}
			set
			{
				Dictionary<TSource, Func<TSource, TCache>> additionalConverters = _additionalConverters;
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				additionalConverters[source] = value;
			}
		}

		public CachePoolValueMap(Func<TSource, TCache> defaultConverter)
		{
			if (defaultConverter == null)
			{
				throw new ArgumentNullException("defaultConverter");
			}
			_converter = defaultConverter;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal TCache Convert(TSource source)
		{
			return this[source](source);
		}

		public static implicit operator CachePool<TSource, TCache>(CachePoolValueMap<TSource, TCache> converter)
		{
			if (converter != null)
			{
				return new CachePool<TSource, TCache>(converter);
			}
			return null;
		}

		public CachePool<TSource, TCache> ToCachePool()
		{
			return new CachePool<TSource, TCache>(this);
		}
	}
}
