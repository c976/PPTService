using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cvte
{
	public class AppInfo
	{
		private readonly Assembly _assembly;

		public string CopyRight => GetAttribute<AssemblyCopyrightAttribute>().Copyright;

		public string Version => GetAttribute<AssemblyFileVersionAttribute>().Version;

		public string ProductName => GetAttribute<AssemblyProductAttribute>().Product;

		public string Company => GetAttribute<AssemblyCompanyAttribute>().Company;

		public string this[string key] => GetAttributes<AssemblyInfoAttribute>().First((AssemblyInfoAttribute x) => x.Key == key).Value;

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
