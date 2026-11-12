using System;
using System.ComponentModel;
using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	public interface IExtension<in T, TInfo> : IExtensionSavable<TInfo>, ISavable, IExtension<T>, IExtension where T : IExtensible<T> where TInfo : SaveInfo
	{
	}
	public interface IExtension<in T> : IExtension where T : IExtensible<T>
	{
		void Attached(T owner);

		void Detached(T owner);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("仅仅只是框架内部使用，用来提升反射性能，请不要直接继承此接口。请使用 IExtension<T> 接口")]
	public interface IExtension
	{
	}
}
