using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.Composition
{
	
	public interface IContainerAdapter
	{
		object Get(Type type);

		T Get<T>();

		Task<TService> GetAsync<TService>();

		void Set<T>(Func<T> creator);

		Task Set<T>(Func<Task<T>> asyncCreator);

		void LazySet<T>() where T : new();

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持带有 contractName 的 Get 方法，请使用无 contractName 的重载替代。", true)]
		object Get(Type type, string contractName);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持带有 contractName 的 Get 方法，请使用无 contractName 的重载替代。", true)]
		T Get<T>(string contractName);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持 GetAll 方法，请使用 dotnetCampus.Telescope 替代。", true)]
		IEnumerable<object> GetAll(Type type);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持 GetAll 方法，请使用 dotnetCampus.Telescope 替代。", true)]
		IEnumerable<T> GetAll<T>();

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持此 Set<T> 方法，请使用委托版本的重载替代。", true)]
		void Set<T>(T value);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("跨平台版本的 Container 不再支持带有 contractName 的 Set 方法，请使用委托版本的重载替代。", true)]
		void Set<T>(string contractName, T value);
	}
}
