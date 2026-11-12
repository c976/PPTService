using System;
using System.Reflection;
using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	public interface ISaveInfoProvider
	{
		void Load(Assembly assembly);

		void RegisterCreateRule<TSaveInfo>(Func<TSaveInfo, object> createFunc) where TSaveInfo : SaveInfo;

		void RegisterCreateRule<TSaveInfo>(Predicate<TSaveInfo> predicate, Func<TSaveInfo, object> createFunc) where TSaveInfo : SaveInfo;

		void SetExtensions<T>(SaveInfo saveInfo, ExtensionCollection<T> extensions) where T : IExtensible<T>;

		[Obsolete("这个方法做的事情有点多，请使用 ICreateExtensionsSaveInfoProvider 中的 CreateExtensions 方法，或者使用完全等价的 CreateExtensionsAndSetSaveInfoAndAddToExtensionCollection 方法")]
		void CreateExtensions<T>(SaveInfo saveInfo, ExtensionCollection<T> extensions) where T : IExtensible<T>;

		bool TryGetSaveInfo<TTarget, TSaveInfo>(TTarget value, out TSaveInfo saveInfo) where TTarget : new() where TSaveInfo : SaveInfo;

		void SetSaveInfo<TSaveInfo>(object value, TSaveInfo saveInfo) where TSaveInfo : SaveInfo;

		bool TryCreate<TTarget, TSaveInfo>(TSaveInfo saveInfo, out TTarget value) where TTarget : class, new() where TSaveInfo : SaveInfo;
	}
}
