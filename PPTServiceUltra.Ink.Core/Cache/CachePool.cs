using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Cache
{
	
	
	public sealed class CachePool< TSource,  TCache>
	{
		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass7_0
		{
			
			public CachePool<TSource, TCache> _003C_003E4__this;

			
			public TSource source;
		}

		private readonly object _locker;

		private readonly CachePoolValueMap<TSource, TCache> _converter;

		private readonly Dictionary<TSource, TCache> _cacheDictionary = new Dictionary<TSource, TCache>();

		public TCache this[TSource source] => GetOrCacheValue(source);

		public CachePool(Func<TSource, TCache> conversion, bool threadSafe = false)
			: this(new CachePoolValueMap<TSource, TCache>(conversion), threadSafe)
		{
		}

		public CachePool(CachePoolValueMap<TSource, TCache> conversion, bool threadSafe = false)
		{
			if (conversion == null)
			{
				throw new ArgumentNullException("conversion");
			}
			_converter = conversion;
			_locker = (threadSafe ? new object() : null);
		}

		private TCache GetOrCacheValue(TSource source)
		{
			_003C_003Ec__DisplayClass7_0 obj = default(_003C_003Ec__DisplayClass7_0);
			obj._003C_003E4__this = this;
			obj.source = source;
			if (_locker == null)
			{
				return _003CGetOrCacheValue_003Eg__GetOrCacheValue_007C7_0(ref obj);
			}
			lock (_locker)
			{
				return _003CGetOrCacheValue_003Eg__GetOrCacheValue_007C7_0(ref obj);
			}
		}

		
		[CompilerGenerated]
		
		private TCache _003CGetOrCacheValue_003Eg__GetOrCacheValue_007C7_0(ref _003C_003Ec__DisplayClass7_0 P_0)
		{
			TCache value;
			if (!_cacheDictionary.TryGetValue(P_0.source, out value))
			{
				value = _converter.Convert(P_0.source);
				_cacheDictionary[P_0.source] = value;
			}
			return value;
		}
	}
}
