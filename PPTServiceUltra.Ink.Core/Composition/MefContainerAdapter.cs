using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.Composition
{
	
	
	public class MefContainerAdapter : IContainerAdapter
	{
		private ConcurrentDictionary<Type, Lazy<object>> LazyDictionary { get; } = new ConcurrentDictionary<Type, Lazy<object>>();

		private ConcurrentDictionary<Type, TaskCompletionSource<object>> AsyncDictionary { get; } = new ConcurrentDictionary<Type, TaskCompletionSource<object>>();

		public object Get(Type type)
		{
			if ((object)type == null)
			{
				throw new ArgumentNullException("type");
			}
			Lazy<object> value;
			if (LazyDictionary.TryGetValue(type, out value))
			{
				return value.Value;
			}
			TaskCompletionSource<object> value2;
			if (AsyncDictionary.TryGetValue(type, out value2) && value2.Task.IsCompleted)
			{
				return value2.Task.Result;
			}
			throw new ExportNotFoundException(type);
		}

		public TService Get<TService>()
		{
			Type typeFromHandle = typeof(TService);
			return (TService)Get(typeFromHandle);
		}

		public async Task<TService> GetAsync<TService>()
		{
			Type typeFromHandle = typeof(TService);
			Lazy<object> value;
			if (LazyDictionary.TryGetValue(typeFromHandle, out value))
			{
				return (TService)value.Value;
			}
			return (TService)(await AsyncDictionary.AddOrUpdate(typeFromHandle, (Type o) => new TaskCompletionSource<object>(), (Type t, TaskCompletionSource<object> o) => o).Task.ConfigureAwait(false));
		}

		public void Set<T>(Func<T> creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException("creator");
			}
			Lazy<object> lazy = new Lazy<object>(() => creator(), LazyThreadSafetyMode.ExecutionAndPublication);
			LazyDictionary[typeof(T)] = lazy;
			TaskCompletionSource<object> taskCompletionSource;
			if (AsyncDictionary.TryGetValue(typeof(T), out taskCompletionSource) && !taskCompletionSource.Task.IsCompleted)
			{
				Task.Run(() =>
				{
					taskCompletionSource.SetResult(lazy.Value);
				});
			}
		}

		public Task Set<T>(Func<Task<T>> asyncCreator)
		{
			if (asyncCreator == null)
			{
				throw new ArgumentNullException("asyncCreator");
			}
			TaskCompletionSource<object> taskCompletionSource = AsyncDictionary.AddOrUpdate(typeof(T), (Type o) => new TaskCompletionSource<object>(), (Type t, TaskCompletionSource<object> o) => o);
			return asyncCreator().ContinueWith( (Task<T> t) =>
			{
				taskCompletionSource.SetResult(t.Result);
			});
		}

		public void LazySet<T>() where T : new()
		{
			LazyDictionary[typeof(T)] = new Lazy<object>(() => new T(), LazyThreadSafetyMode.ExecutionAndPublication);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持带有 contractName 的 Get 方法，请使用无 contractName 的重载替代。", true)]
		public object Get(Type type, string contractName)
		{
			throw new PlatformNotSupportedException("跨平台版本的 Container 不再支持带有 contractName 的 Get 方法，请使用无 contractName 的重载替代。");
		}

		[Obsolete("跨平台版本的 Container 不再支持带有 contractName 的 Get 方法，请使用无 contractName 的重载替代。", true)]
		public TService Get<TService>(string contractName)
		{
			throw new PlatformNotSupportedException("跨平台版本的 Container 不再支持带有 contractName 的 Get 方法，请使用无 contractName 的重载替代。");
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持带有 contractName 的 Get 方法，请使用无 contractName 的重载替代。", true)]
		public object Get(Type type, Type metadataType, string contractName)
		{
			throw new PlatformNotSupportedException("跨平台版本的 Container 不再支持带有 contractName 的 Get 方法，请使用无 contractName 的重载替代。");
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持 GetAll 方法，请使用 dotnetCampus.Telescope 替代。", true)]
		public IEnumerable<TService> GetAll<TService>()
		{
			throw new PlatformNotSupportedException("跨平台版本的 Container 不再支持 GetAll 方法，请使用 dotnetCampus.Telescope 替代。");
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持 GetAll 方法，请使用 dotnetCampus.Telescope 替代。", true)]
		public IEnumerable<object> GetAll(Type type)
		{
			throw new PlatformNotSupportedException("跨平台版本的 Container 不再支持 GetAll 方法，请使用 dotnetCampus.Telescope 替代。");
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持 GetAll 方法，请使用 dotnetCampus.Telescope 替代。", true)]
		public IEnumerable<object> GetAll(Type type, Type metadataType, string contractName)
		{
			throw new PlatformNotSupportedException("跨平台版本的 Container 不再支持 GetAll 方法，请使用 dotnetCampus.Telescope 替代。");
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持此 Set<T> 方法，请使用委托版本的重载替代。", true)]
		public void Set<T>(T value)
		{
			throw new PlatformNotSupportedException("跨平台版本的 Container 不再支持此 Set<T> 方法，请使用委托版本的重载替代。");
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持带有 contractName 的 Set 方法，请使用委托版本的重载替代。", true)]
		public void Set<T>(string contractName, T value)
		{
			throw new PlatformNotSupportedException("跨平台版本的 Container 不再支持带有 contractName 的 Set 方法，请使用委托版本的重载替代。");
		}
	}
}
