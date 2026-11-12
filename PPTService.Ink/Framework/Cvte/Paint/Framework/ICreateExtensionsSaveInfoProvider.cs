using System.Collections.Generic;
using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	public interface ICreateExtensionsSaveInfoProvider
	{
		IList<IExtension<T>> CreateExtensions<T>(SaveInfo saveInfo) where T : IExtensible<T>;

		void CreateExtensionsAndSetSaveInfoAndAddToExtensionCollection<T>(SaveInfo saveInfo, ExtensionCollection<T> extensions) where T : IExtensible<T>;
	}
}
