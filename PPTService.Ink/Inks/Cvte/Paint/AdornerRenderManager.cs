using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.Paint.Framework;
using Cvte.Paint.Inks;

namespace Cvte.Paint
{
	public static class AdornerRenderManager
	{
		private static Dictionary<Type, IAdornerRender> _adornerRenderDic;

		static AdornerRenderManager()
		{
			_adornerRenderDic = new Dictionary<Type, IAdornerRender>();
		}

		public static IAdornerRender GetAdornerRender(Type renderType)
		{
			if (_adornerRenderDic.ContainsKey(renderType))
			{
				return _adornerRenderDic[renderType];
			}
			IAdornerRender adornerRender = renderType.Create<IAdornerRender>();
			_adornerRenderDic.Add(renderType, adornerRender);
			return adornerRender;
		}

		public static void ClearAdorner()
		{
			foreach (IAdornerRender value in _adornerRenderDic.Values)
			{
				value.InterruptRender();
			}
		}

		public static void ClearWrittingAdornerByClientId(string clientID)
		{
			WritingAdornerRender writingAdornerRender = GetAdornerRender(typeof(WritingAdornerRender)) as WritingAdornerRender;
			IEnumerable<DynamicInk> enumerable = writingAdornerRender?.GetAllDynamicInks().Where((DynamicInk x) => x.ClientID == clientID);
			if (enumerable == null)
			{
				return;
			}
			foreach (DynamicInk item in enumerable)
			{
				writingAdornerRender.DeleteDynamicInk(item);
			}
		}

		public static void ClearWrittingAdornerByDeviceId(string deviceId)
		{
			WritingAdornerRender writingAdornerRender = GetAdornerRender(typeof(WritingAdornerRender)) as WritingAdornerRender;
			if (writingAdornerRender?.GetDynamicInk(deviceId) != null)
			{
				writingAdornerRender.DeleteDynamicInk(deviceId);
			}
		}
	}
}
