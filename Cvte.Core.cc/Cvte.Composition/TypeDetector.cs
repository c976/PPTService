using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Cvte.Composition
{
	public class TypeDetector : ITypeDetector
	{
		private class MappingTypeCollection : ITypeCollection
		{
			private readonly IDictionary<Type, Lazy<object>> _mapping;

			public MappingTypeCollection(IDictionary<Type, Lazy<object>> mapping)
			{
				_mapping = mapping;
			}

			public IEnumerable<T> OfType<T>(Predicate<Type> typeFilter)
			{
				return GetInstances<T>(_mapping.Where((KeyValuePair<Type, Lazy<object>> x) => typeFilter(x.Key)).ToDictionary((KeyValuePair<Type, Lazy<object>> x) => x.Key, (KeyValuePair<Type, Lazy<object>> x) => x.Value));
			}
		}

		private readonly HashSet<Assembly> _processedAssemblies = new HashSet<Assembly>();

		private readonly IDictionary<Type, Lazy<object>> _mapping = new Dictionary<Type, Lazy<object>>();

		public event EventHandler<NewTypesDetectedEventArgs> NewTypesDetected;

		public void Detect(Assembly assembly)
		{
			if (_processedAssemblies.Contains(assembly))
			{
				return;
			}
			_processedAssemblies.Add(assembly);
			Dictionary<Type, Lazy<object>> dictionary = new Dictionary<Type, Lazy<object>>();
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (!_mapping.ContainsKey(type))
				{
					Type thisType = type;
					Lazy<object> value = new Lazy<object>(() => Create(thisType));
					_mapping.Add(thisType, value);
					dictionary.Add(thisType, value);
				}
			}
			OnNewTypesDetected(new NewTypesDetectedEventArgs(new MappingTypeCollection(dictionary), assembly));
		}

		public ReadOnlyCollection<T> GetDetectedObjects<T>()
		{
			return GetInstances<T>(_mapping);
		}

		private static ReadOnlyCollection<T> GetInstances<T>(IDictionary<Type, Lazy<object>> mapping)
		{
			return new ReadOnlyCollection<T>((from x in (from x in mapping.Where((KeyValuePair<Type, Lazy<object>> x) =>
					{
						Type key = x.Key;
						return typeof(T).IsAssignableFrom(x.Key) && !key.IsAbstract && key.GetConstructor(Type.EmptyTypes) != null && !key.ContainsGenericParameters;
					})
					select x.Value.Value).OfType<T>()
				where x != null
				select x).ToArray());
		}

		protected virtual object Create(Type type)
		{
			try
			{
				return Activator.CreateInstance(type);
			}
			catch
			{
				return null;
			}
		}

		protected virtual void OnNewTypesDetected(NewTypesDetectedEventArgs e)
		{
			NewTypesDetected?.Invoke(this, e);
		}
	}
}
