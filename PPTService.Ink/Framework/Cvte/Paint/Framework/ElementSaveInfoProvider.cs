using System;
using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	public class ElementSaveInfoProvider<T> : IElementSaveInfoProvider where T : ElementSaveInfo
	{
		private readonly Action<T> _setElementSaveInfo;

		private readonly Func<T> _getElementSaveInfo;

		public ElementSaveInfoProvider(ISavable<T> savable)
			: this((Action<T>)savable.SetSaveInfo, (Func<T>)savable.GetSaveInfo)
		{
		}

		private ElementSaveInfoProvider(Action<T> setElementSaveInfo, Func<T> getElementSaveInfo)
		{
			_setElementSaveInfo = setElementSaveInfo;
			_getElementSaveInfo = getElementSaveInfo;
		}

		public void SetSaveInfo(ElementSaveInfo elementSaveInfo)
		{
			_setElementSaveInfo((T)elementSaveInfo);
		}

		public ElementSaveInfo GetSaveInfo()
		{
			return _getElementSaveInfo();
		}
	}
}
