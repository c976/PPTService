using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Configurations
{
	internal static class ClassNameUtils
	{
		
		internal static string GetClassNameWithoutSuffix< T>(this T @this)
		{
			if (@this == null)
			{
				throw new ArgumentNullException("this");
			}
			string name = @this.GetType().Name;
			string name2 = typeof(T).Name;
			string text = (name.EndsWith(name2, StringComparison.Ordinal) ? name.Substring(0, name.Length - name2.Length) : name);
			if (!string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
			return name;
		}
	}
}
