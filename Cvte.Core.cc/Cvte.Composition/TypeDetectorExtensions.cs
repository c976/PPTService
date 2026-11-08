using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cvte.Composition
{
	public static class TypeDetectorExtensions
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass1_0<SA_T0>
		{
			public Predicate<Type> typeFilter;

			public Action<SA_T0> typeProcessingMethod;

			internal void _003COnTypeDetected_003Eb__0(object sender, NewTypesDetectedEventArgs e)
			{
				using (IEnumerator<SA_T0> enumerator = e.GetTypeOf<SA_T0>(typeFilter).GetEnumerator())
				{
					if (6 != 0)
					{
						while (enumerator.MoveNext())
						{
							SA_T0 current = enumerator.Current;
							typeProcessingMethod(current);
						}
					}
				}
			}
		}

		public static void OnTypeDetected<T>(this ITypeDetector typeDetector, Action<T> typeProcessingMethod, bool processTypesAlreadyDetected = false)
		{
			if (4 != 0)
			{
				typeDetector.OnTypeDetected(typeProcessingMethod, (Type t) => true, processTypesAlreadyDetected);
			}
		}

		public static void OnTypeDetected<T>(this ITypeDetector typeDetector, Action<T> typeProcessingMethod, Predicate<Type> typeFilter, bool processTypesAlreadyDetected = false)
		{
			_003C_003Ec__DisplayClass1_0<T> obj = default(_003C_003Ec__DisplayClass1_0<T>);
			if (4 != 0)
			{
				obj = new _003C_003Ec__DisplayClass1_0<T>();
				if ((4 != 0) ? true : false)
				{
					obj.typeFilter = typeFilter;
					obj.typeProcessingMethod = typeProcessingMethod;
					if (processTypesAlreadyDetected)
					{
						goto IL_00b2;
					}
				}
				goto IL_0071;
			}
			goto IL_00b2;
			IL_0071:
			typeDetector.NewTypesDetected += obj._003COnTypeDetected_003Eb__0;
			return;
			IL_00b2:
			using (IEnumerator<T> enumerator = typeDetector.GetDetectedObjects<T>().GetEnumerator())
			{
				while (enumerator.MoveNext() && 5 != 0)
				{
					T current = enumerator.Current;
					obj.typeProcessingMethod(current);
				}
			}
			goto IL_0071;
		}
	}
}
