using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Cache;

namespace PPTService.Ink.Core
{
	
	
	public class AppInfo : IAppInfo
	{
		private readonly Assembly _assembly;

		private readonly CachePool<Type, string> _assemblyAttributeCache;

		private readonly Lazy<List<AssemblyInfoAttribute>> _assemblyInfoCache;

		public string CopyRight => _assemblyAttributeCache[typeof(AssemblyCopyrightAttribute)];

		public string Version => _assemblyAttributeCache[typeof(AssemblyFileVersionAttribute)];

		public string ProductName => _assemblyAttributeCache[typeof(AssemblyProductAttribute)];

		public string Company => _assemblyAttributeCache[typeof(AssemblyCompanyAttribute)];

		public string this[string key]
		{
			get
			{
				string text = _assemblyInfoCache.Value.FirstOrDefault( (AssemblyInfoAttribute x) => x.Key == key)?.Value;
				if (text == null)
				{
					throw new KeyNotFoundException("Cannot find assembly info which contains the key " + key + ".");
				}
				return text;
			}
		}

		public AppInfo(Assembly assembly)
		{
			_assembly = assembly;
			_assemblyInfoCache = new Lazy<List<AssemblyInfoAttribute>>(() => _assembly.GetCustomAttributes(typeof(AssemblyInfoAttribute), false).OfType<AssemblyInfoAttribute>().ToList());
			_assemblyAttributeCache = new CachePoolValueMap<Type, string>((Type type) => (string)null)
			{
				[typeof(AssemblyCopyrightAttribute)] = (Type type) => GetEmbetedAttributeValue((AssemblyCopyrightAttribute x) => x.Copyright),
				[typeof(AssemblyFileVersionAttribute)] = (Type type) => GetEmbetedAttributeValue((AssemblyFileVersionAttribute x) => x.Version),
				[typeof(AssemblyProductAttribute)] = (Type type) => GetEmbetedAttributeValue((AssemblyProductAttribute x) => x.Product),
				[typeof(AssemblyCompanyAttribute)] = (Type type) => GetEmbetedAttributeValue((AssemblyCompanyAttribute x) => x.Company)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private string GetEmbetedAttributeValue< T>(Func<T, string> getValue) where T : Attribute
		{
			T val = _assembly.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
			if (val == null)
			{
				return null;
			}
			return getValue(val);
		}
	}
}
