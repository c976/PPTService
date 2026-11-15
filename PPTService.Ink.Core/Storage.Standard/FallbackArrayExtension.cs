using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public static class FallbackArrayExtension
	{
		public static FallbackArray<T> ToFallbackArray< T>(IList<T> list) where T : SaveInfo
		{
			FallbackArray<T> fallbackArray = new FallbackArray<T>();
			fallbackArray.AddRange(list);
			return fallbackArray;
		}

		internal static bool? IsEmptyArray(object obj)
		{
			IList list = obj as IList;
			if (list != null)
			{
				IFallbackArray fallbackArray = obj as IFallbackArray;
				if (fallbackArray != null && fallbackArray.UnknownNode.Count > 0)
				{
					return false;
				}
				return list.Count == 0;
			}
			return null;
		}
	}
}
