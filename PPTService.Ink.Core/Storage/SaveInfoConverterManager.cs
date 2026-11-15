using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class SaveInfoConverterManager
	{
		private readonly IDictionary<string, Lazy<ISaveInfoConverter>> _converters = new Dictionary<string, Lazy<ISaveInfoConverter>>();

		public void Load(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				SaveInfoConverterAttribute customAttribute = type.GetCustomAttribute<SaveInfoConverterAttribute>();
				if (customAttribute == null)
				{
					continue;
				}
				string[] names = customAttribute.Names;
				foreach (string key in names)
				{
					_converters.Add(key, new Lazy<ISaveInfoConverter>(() => (ISaveInfoConverter)Activator.CreateInstance(type)));
				}
			}
		}

		public bool TryGetConverter(string name, out ISaveInfoConverter converter)
		{
			Lazy<ISaveInfoConverter> value;
			if (_converters.TryGetValue(name, out value))
			{
				converter = value.Value;
				return true;
			}
			converter = null;
			return false;
		}
	}
}
