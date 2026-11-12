using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Cvte.Paint.Framework
{
	public static class Clone
	{
		private static Dictionary<Type, Delegate> CachedIl { get; set; } = new Dictionary<Type, Delegate>();

		public static void CloneObjectWithIL<T>(T source, T los)
		{
			if (CachedIl.ContainsKey(typeof(T)))
			{
				((Action<T, T>)CachedIl[typeof(T)])(source, los);
				return;
			}
			DynamicMethod dynamicMethod = new DynamicMethod("Clone", null, new Type[2]
			{
				typeof(T),
				typeof(T)
			});
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			foreach (PropertyInfo item in from temp in typeof(T).GetProperties()
				where temp.CanRead && temp.CanWrite
				select temp)
			{
				if (!item.GetAccessors(true)[0].IsStatic)
				{
					iLGenerator.Emit(OpCodes.Ldarg_1);
					iLGenerator.Emit(OpCodes.Ldarg_0);
					iLGenerator.Emit(OpCodes.Callvirt, item.GetMethod);
					iLGenerator.Emit(OpCodes.Callvirt, item.SetMethod);
				}
			}
			iLGenerator.Emit(OpCodes.Ret);
			Action<T, T> action = (Action<T, T>)dynamicMethod.CreateDelegate(typeof(Action<T, T>));
			CachedIl[typeof(T)] = action;
			action(source, los);
		}
	}
}
