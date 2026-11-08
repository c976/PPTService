using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cvte
{
	public class AppInfo
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass10_0
		{
			public string key;

			internal bool _003Cget_Item_003Eb__0(AssemblyInfoAttribute x)
			{
				return x.Key == key;
			}
		}

		private readonly Assembly _assembly;

		public string CopyRight => GetAttribute<AssemblyCopyrightAttribute>().Copyright;

		public string Version => GetAttribute<AssemblyFileVersionAttribute>().Version;

		public string ProductName => GetAttribute<AssemblyProductAttribute>().Product;

		public string Company => GetAttribute<AssemblyCompanyAttribute>().Company;

		public string this[string key]
		{
			get
			{
				_003C_003Ec__DisplayClass10_0 obj = new _003C_003Ec__DisplayClass10_0();
				_003C_003Ec__DisplayClass10_0 obj2 = default(_003C_003Ec__DisplayClass10_0);
				if (0 == 0)
				{
					obj2 = obj;
				}
				obj2.key = key;
				return GetAttributes<AssemblyInfoAttribute>().First(obj2._003Cget_Item_003Eb__0).Value;
			}
		}

		public AppInfo(Assembly assembly)
		{
			_assembly = assembly;
		}

		private T GetAttribute<T>() where T : Attribute
		{
			return (T)_assembly.GetCustomAttributes(typeof(T), false).Single();
		}

		private IEnumerable<T> GetAttributes<T>() where T : Attribute
		{
			return _assembly.GetCustomAttributes(typeof(T), false).OfType<T>();
		}
	}
}
