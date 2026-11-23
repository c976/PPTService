using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	public static class TypeExtension
	{
		
		public static bool IsNullable(this Type type)
		{
			return type.Name == typeof(Nullable<>).Name;
		}
	}
}
