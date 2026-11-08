using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Composition
{
	public class MefContainerAdapter : IContainerAdapter
	{
		private readonly CompositionContainer _compositionContainer;

		public MefContainerAdapter([NotNull] CompositionContainer compositionContainer)
		{
			if (compositionContainer == null)
			{
				throw new ArgumentNullException("compositionContainer");
			}
			_compositionContainer = compositionContainer;
		}

		public object Get(Type type)
		{
			return Get(type, null);
		}

		public object Get(Type type, string contractName)
		{
			return Get(type, null, contractName);
		}

		public IEnumerable<object> GetAll(Type type)
		{
			return GetAll(type, null, null);
		}

		public TService Get<TService>()
		{
			return Get<TService>(null);
		}

		public TService Get<TService>(string contractName)
		{
			return (TService)Get(typeof(TService), contractName);
		}

		public IEnumerable<TService> GetAll<TService>()
		{
			return from x in GetAll(typeof(TService))
				select (TService)x;
		}

		public IEnumerable<object> GetAll(Type type, Type metadataType, string contractName)
		{
			return from export in _compositionContainer.GetExports(type, metadataType, contractName)
				select export.Value;
		}

		public object Get(Type type, Type metadataType, string contractName)
		{
			IEnumerable<Lazy<object, object>> exports = _compositionContainer.GetExports(type, metadataType, contractName);
			IList<Lazy<object, object>> source = (exports as IList<Lazy<object, object>>) ?? exports.ToList();
			if (!source.Any())
			{
				throw new CompositionException("Export not found.");
			}
			if (source.Count() > 1)
			{
				throw new CompositionException("More than one exports match the contract.");
			}
			return source.First().Value;
		}

		public void Set<T>(T value)
		{
			_compositionContainer.ComposeExportedValue(value);
		}

		public void Set<T>(string key, T value)
		{
			_compositionContainer.ComposeExportedValue(key, value);
		}
	}
}
